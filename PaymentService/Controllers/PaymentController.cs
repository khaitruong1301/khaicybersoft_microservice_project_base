using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
//using PaymentService.Models;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(PaymentServiceContext _context) : ControllerBase
    {
      

        [HttpGet("GetAllPayment")]
        public async Task<IActionResult> GetAllPayment()
        {

            return Ok(_context.PaymentMethods);
        }

  
    }
}