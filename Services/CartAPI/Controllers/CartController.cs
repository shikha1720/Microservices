using CartAPI.Models;
using CartAPI.RabbitmqService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
using System.Text;

namespace CartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private static List<CartModel> cartItems = new List<CartModel>();
        private static readonly HttpClient _httpClient = new HttpClient();
        private static ReceiveService _receiveService;
        private readonly ILogger<CartController> _logger;



        public CartController(ReceiveService receiveService, ILogger<CartController> logger)
        {
            _logger = logger;
            _receiveService = receiveService;
            //_receiveService.ReceiveProduct();

        }

        [HttpGet("getCartItems")]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var receivedData = await _receiveService.ReceiveProduct();
                if(!string.IsNullOrEmpty(receivedData))
                {
                    return Ok(receivedData);
                }
                return BadRequest("Product not received");
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                _logger.LogError(ex, "An error occurred while processing the request in get cart items.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }

        [HttpGet("getCartItem/{userId}")]
        public ActionResult GetUserCart(int userId)
        {
            try
            {
                var userCart = cartItems.FindAll(item => item.UserId == userId);
                if (userCart != null)
                {
                    return Ok(userCart);
                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in get cart by user.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }

        [HttpPost("addToCart")]
        public ActionResult AddToCart(CartModel cartItem)
        {
            try
            {
                if (cartItem == null || cartItem.ProductId <= 0 || cartItem.Quantity <= 0)
                {
                    return BadRequest("Invalid cart item");
                }

                var existingItem = cartItems.FirstOrDefault(item => item.UserId == cartItem.UserId && item.ProductId == cartItem.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += cartItem.Quantity;
                }
                else
                {
                    cartItem.Id = cartItems.Count + 1;
                    cartItems.Add(cartItem);
                }

                return Ok("Product Added To Cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in deleting cart items.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }

        [HttpDelete("removeCartItem/{itemId}")]
        public ActionResult RemoveFromCart(int itemId)
        {
            try
            {
                var itemToRemove = cartItems.Find(item => item.Id == itemId);
                if (itemToRemove != null)
                {
                    if (itemToRemove.Quantity > 1)
                    {
                        itemToRemove.Quantity -= 1;
                    }
                    else
                    {
                        cartItems.Remove(itemToRemove);
                    }
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in adding to cart.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred. Please try again later." });
            }
        }

    }
}

