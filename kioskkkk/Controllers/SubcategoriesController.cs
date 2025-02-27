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
    public class SubcategoriesController : ControllerBase
    {
        private readonly KioskkContext _context;

        public SubcategoriesController(KioskkContext context)
        {
            _context = context;
        }

        // GET: api/Subcategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subcategory>>> GetSubcategories([FromQuery] int? categoryId)
        {
            var filteredSubcategories = await _context.Subcategories
            .Where(s => s.CategorieId == categoryId)
            .ToListAsync();

            return Ok(filteredSubcategories);
        }

        // GET: api/Subcategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subcategory>> GetSubcategory(int id)
        {
            var subcategory = await _context.Subcategories.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            return subcategory;
        }

        // PUT: api/Subcategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubcategory(int id, Subcategory subcategory)
        {
            if (id != subcategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(subcategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubcategoryExists(id))
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

        // POST: api/Subcategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Subcategory>> CreateSubcategory(CreateSubcategoryDto subcategoryDto)
        {
            // Validate incoming data
            if (subcategoryDto == null ||
                string.IsNullOrEmpty(subcategoryDto.Name) ||
                string.IsNullOrEmpty(subcategoryDto.Status) ||
                string.IsNullOrEmpty(subcategoryDto.Image) ||
                subcategoryDto.CategoryId <= 0)
            {
                return BadRequest("Invalid data. Name, Status, Image, and CategoryId are required.");
            }

            // Convert base64 string to byte array
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(subcategoryDto.Image);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid base64 image data.");
            }

            string path = Directory.GetCurrentDirectory();

            // Generate a unique file name
            string fileName = Guid.NewGuid().ToString() + ".jpg";
            string filePath = Path.Combine(path, "uploads", "subcategories", "images", fileName);

            // Save the image to the server
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            // Create a new Subcategory object
            var subcategory = new Subcategory
            {
                Name = subcategoryDto.Name,
                Image = "/subcategories/images/" + fileName, // Save image path
                Description = subcategoryDto.Description,
                Price = subcategoryDto.Price,
                Status = subcategoryDto.Status,
                CategorieId = subcategoryDto.CategoryId, // Map CategoryId to CategorieId
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add subcategory to database
            _context.Subcategories.Add(subcategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubcategory), new { id = subcategory.Id }, subcategory);
        }


        public class CreateSubcategoryDto
        {
            public string Name { get; set; }
            public string Image { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
            public int Price { get; set; }
            public int CategoryId { get; set; } // Maps to CategorieId in Subcategory
        }


        // DELETE: api/Subcategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubcategory(int id)
        {
            var subcategory = await _context.Subcategories.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }

            _context.Subcategories.Remove(subcategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }
      
        private bool SubcategoryExists(int id)
        {
            return _context.Subcategories.Any(e => e.Id == id);
        }
    }
}
