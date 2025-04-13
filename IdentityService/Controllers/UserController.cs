using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;
//using IdentityService.Models;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserServiceContext _context) : ControllerBase
    {
       
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
        
            return Ok(_context.Users);
        }
    }
}