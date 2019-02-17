using Bidster.Entities.Tenants;
using Bidster.Entities.Users;
using Bidster.Models;
using Bidster.Models.Tenants;
using Bidster.Services.Tenants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Controllers.Api
{
    [Authorize]
    [Route("api/tenants/{tenantId}/users")]
    [ApiController]
    public class TenantUsersController : ControllerBase
    {
        private readonly ITenantUserService _tenantUserService;
        private readonly UserManager<User> _userManager;
        private readonly ISystemClock _systemClock;
        private readonly ILogger<TenantUsersController> _logger;

        public TenantUsersController(ITenantUserService tenantUserService,
            UserManager<User> userManager,
            ISystemClock systemClock,
            ILogger<TenantUsersController> logger)
        {
            _tenantUserService = tenantUserService;
            _userManager = userManager;
            _systemClock = systemClock;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TenantUserModel>> GetById(int tenantId, int id)
        {
            var tenantUser = await _tenantUserService.GetByIdAsync(id);
            if (tenantUser == null || tenantUser.TenantId != tenantId)
            {
                return NotFound();
            }

            var model = tenantUser.ToModel();
            return Ok(model);
        }

        [HttpGet("")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<TenantUserModel>>> GetAll(int tenantId, bool? isAdmin = null)
        {
            var tenantUsers = await _tenantUserService.GetByTenantIdAsync(tenantId);
            if (isAdmin.HasValue)
            {
                // TODO: move to the service
                tenantUsers = tenantUsers.Where(x => x.IsAdmin == isAdmin).ToList();
            }

            var result = tenantUsers.Select(x => x.ToModel());

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TenantUserModel>> Create([FromRoute]int tenantId, [FromBody] CreateTenantUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                ModelState.AddModelError("UserId", "Invalid user ID");
            }

            if (await _tenantUserService.IsUserInTenantAsync(model.UserId, tenantId))
            {
                ModelState.AddModelError("UserId", "User is already a member of this tenant");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tenantUser = new TenantUser
                {
                    TenantId = tenantId,
                    UserId = model.UserId,
                    AddedOn = _systemClock.UtcNow.Date,
                    IsAdmin = model.IsAdmin
                };
                await _tenantUserService.CreateAsync(tenantUser);

                tenantUser = await _tenantUserService.GetByIdAsync(tenantUser.Id);
                var result = tenantUser.ToModel();

                return CreatedAtAction(nameof(GetById), new { tenantId, id = tenantUser.Id }, result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error adding user to tenant");
                return StatusCode(500);
            }
        }
    }

    public class CreateTenantUserModel
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public int UserId { get; set; }

        public DateTime AddedOn { get; set; }

        public bool IsAdmin { get; set; }

    }
}