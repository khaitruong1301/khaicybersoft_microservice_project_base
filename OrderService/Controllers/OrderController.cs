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
using OrderService.Models.ViewModel;
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

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder(OrderRequest order)
        {
            await _context.Database.BeginTransactionAsync();
            try {
                Order model = new Order();
                model.UserId = order.UserId;
                foreach(OrderItemRequest item in order.OrderItems){
                    OrderItem itemOrder = new OrderItem();
                    itemOrder.Id = item.Id;
                    itemOrder.OrderId = model.Id;
                    itemOrder.ProductId = item.ProductId;
                    itemOrder.Quantity = item.Quantity;

                }


                await _context.Orders.AddAsync(model);
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

