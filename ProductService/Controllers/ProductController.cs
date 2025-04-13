using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ProductServiceContext _context) : ControllerBase
    {
  
        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct()
        {
            return Ok(_context.Products);
        }

        [HttpGet("/product/{id}")]
        public async Task<IActionResult> Product([FromRoute] int id)
        {
        

            return Ok(_context.Products.SingleOrDefault(n => n.Id == id));
        }

   
    }
}