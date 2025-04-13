using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Kafka;
using OrderService.Models;
//using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController (OrderServiceContext _context, IKafkaProducer _producer): ControllerBase
    {
       
        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> GetAllOrder()
        {

            var res = await _context.Orders.ToListAsync();

            return Ok(res);
        }

        [HttpGet("AddOrder")]
        public async Task<IActionResult> AddOrder(Order order)
        {
            await _context.Database.BeginTransactionAsync();
            try {
                await _context.Orders.AddAsync(order);
                _context.SaveChanges();

                await _producer.ProduceAsync("order-topic", new Message<string,string>(){
                    Key = order.Id.ToString(),
                    Value= JsonSerializer.Serialize(order)
                });
                await _context.Database.CommitTransactionAsync();
            }catch(Exception err) {
               await _context.Database.RollbackTransactionAsync();
            }
            return Ok("Successfully");
        }
    }
}

