using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kioskkkk.Models;

namespace kioskkkk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly KioskkContext _context;

        public OrdersController(KioskkContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        public class OrderDetailsResponse
        {
            public int Id { get; set; }
            public decimal Amount { get; set; }
            public DateTime CreatedAt { get; set; }
            public List<Orderdetail> OrderDetails { get; set; }
            public List<Payment> Payments { get; set; }
        }
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<OrderDetailsResponse>>> GetOrderDetails()
        {
            var orders = await _context.Orders
                .Include(o => o.Orderdetails) // Include Orderdetails
                .Include(o => o.Payments)    // Include Payments
                .Select(o => new OrderDetailsResponse
                {
                    Id = o.Id,
                    Amount = o.Amount,
                    CreatedAt = o.CreatedAt,
                    OrderDetails = o.Orderdetails.Select(od => new Orderdetail
                    {
                        Id = od.Id,
                        ItemId = od.ItemId,
                        Quantity = od.Quantity,
                        Price = od.Price
                    }).ToList(),
                    Payments = o.Payments.Select(p => new Payment
                    {
                        Id = p.Id,
                        PaymentType = p.PaymentType,
                        Amount = p.Amount,
                        Status = p.Status,
                        CreatedAt = p.CreatedAt
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }
    


[HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrder([FromBody] OrderRequest request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Step 1: Create the Order
                    var order = new Order
                    {
                        Amount = Convert.ToInt32(request.OrderDetails.Sum(od => od.Quantity * od.Price)),
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Step 2: Create Order Details
                    foreach (var detail in request.OrderDetails)
                    {
                        var orderDetail = new Orderdetail
                        {
                            OrderId = order.Id,
                            ItemId = detail.ItemId,
                            Quantity = detail.Quantity,
                            Price = detail.Price
                        };

                        _context.Orderdetails.Add(orderDetail);
                    }

                    await _context.SaveChangesAsync();

                    // Step 3: Create Payment
                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        PaymentType = request.PaymentType,
                        Amount = order.Amount,
                        Status = "Pending", // or any default status
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { OrderId = order.Id, PaymentId = payment.Id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
        public class OrderRequest
        {
            public string PaymentType { get; set; } 
            public List<OrderDetailRequest> OrderDetails { get; set; }
        }

        public class OrderDetailRequest
        {
            public int ItemId { get; set; }
            public int Quantity { get; set; }
            public int Price { get; set; }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
