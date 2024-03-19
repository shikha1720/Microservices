using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;
using Newtonsoft.Json;
using ProductAPI.RabbitmqService;
using Microsoft.Extensions.Logging;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private static List<ProductModel> products = new List<ProductModel>
        {
            new ProductModel{
                Id = 1,
                ProductName="Puma T-Shirt",
                Price=1539,
                Size="Medium",
                Design="Striped Regular Fit Tshirt with Short Sleeve and Band Collar" 
            },
            new ProductModel{
                Id = 2,
                ProductName="ADRO T-Shirt",
                Price=499,
                Size="Medium",
                Design="Printed Regular Fit T-shirt with Half Sleeve Round Collar"
            }
        };

        private readonly IMessageProducer _messageProducer;

        private readonly ILogger<ProductController> _logger;
        
        public ProductController(IMessageProducer messageProducer, ILogger<ProductController> logger)
        {
            _messageProducer = messageProducer;
            _logger = logger;

        }

        [HttpGet("allProducts")]
        public ActionResult<IEnumerable<ProductModel>> GetProducts()
        {
            try
            {
                if (products.Count != 0)
                {
                    return Ok(products);
                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in get all product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }

        [HttpPost("addProduct")]
        public ActionResult AddProduct(ProductModel product)
        {
            try
            {
                if (product == null || product.ProductName == null || product.Price == 0 || product.Design == null || product.Size == null)
                {
                    return BadRequest("all fields are required");
                }

                var newProduct = new ProductModel
                {
                    Id = products.Count + 1,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Design = product.Design,
                    Size = product.Size
                };
                products.Add(newProduct);
                return Ok("Product Added Successfull!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in adding productS.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }

        [HttpDelete("deleteProduct/{productId}")]
        public ActionResult RemoveProduct(int productId)
        {
            try
            {
                var productToRemove = products.Find(p => p.Id == productId);
                if (productToRemove != null)
                {
                    products.Remove(productToRemove);
                    return Ok("Product Removed");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in deleting product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }

        [HttpGet("productById/{productId}")]
        public ActionResult GetProductById(int productId)
        {
            try
            {
                var product = products.Find(p => p.Id == productId);
                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in geting product by id.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }

        //private async Task ConsumeProductQueue()
        //{
        //    var factory = new ConnectionFactory() { HostName = "rabbitmq" };

        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.QueueDeclare(queue: "product_queue",
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //        var consumer = new AsyncEventingBasicConsumer(channel);
        //        consumer.Received += async (model, ea) =>
        //        {
        //            var body = ea.Body;
        //            var message = Encoding.UTF8.GetString(body.Span);
        //            Console.WriteLine($"Received message from Cart Service: {message}");

        //            string productId = message.Split(':')[1];

        //            var productData = FetchProductDetails(Convert.ToInt32(productId));

        //            var responseMessage = $"ProductDetails:{productData}";
        //            var responseBody = Encoding.UTF8.GetBytes(responseMessage);

        //            await Task.Run(() =>
        //            {
        //                // Publish the response
        //                channel.BasicPublish(exchange: "",
        //                                     routingKey: ea.BasicProperties.ReplyTo,
        //                                     basicProperties: CreateBasicProperties(ea.BasicProperties),
        //                                     body: responseBody);

        //                // Acknowledge the original message
        //                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        //            });
        //        };

        //        channel.BasicConsume(queue: "product_queue",
        //                             autoAck: false,
        //                             consumer: consumer);

        //        Console.WriteLine("Waiting for messages...");
        //        Console.ReadLine();  // To keep the service running
        //    }
        //}

        //private IBasicProperties CreateBasicProperties(IBasicProperties originalProperties)
        //{
        //    var properties = originalProperties;

        //    // Copy other properties as needed

        //    return properties;
        //}
        [HttpPost("addToCart")]
        public IActionResult AddToCart([FromBody] AddToCartModel request)
        {
            try
            {
                if (request.Id < 0)
                {
                    return BadRequest();
                }
                var product = products.FirstOrDefault(x => x.Id == request.Id);
                if (product == null)
                {
                    return NotFound();
                }
                _messageProducer.SendingMessage<ProductModel>(product);

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in adding to cart.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }
    }
}
