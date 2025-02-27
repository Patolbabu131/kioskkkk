using kioskkkk.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace kioskkkk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly KioskkContext _context;

        public LoginController(KioskkContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User request)
             {
            // Validate input
            if (request.Id == 0 || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("User ID and Password are required");
            }

            // Check user in database
            var user = _context.Users.FirstOrDefault(u => u.Id == request.Id && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Return success response
            return Ok(new
            {
                message = "Login successful",
                user = user
            });
        }
    }


}