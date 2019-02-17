using Bidster.Entities.Users;
using Bidster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Controllers.Api
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var model = user.ToUserModel();
            return Ok(model);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll(string email = null)
        {
            var users = await _userManager.Users
                .Where(x => string.IsNullOrEmpty(email) || x.Email.Contains(email))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => x.ToUserModel())
                .ToListAsync();

            return Ok(users);
        }
    }
}