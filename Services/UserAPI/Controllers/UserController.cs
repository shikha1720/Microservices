using JWTAuthenticationManager;
using JWTAuthenticationManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        public UserController(JwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenHandler = jwtTokenHandler;
        }

        private static List<UserModel> users = new List<UserModel>
        {
        new UserModel {
            Id = 1,
            Username = "admin",
            Password = "admin@123",
            Role = "admin"
        },
        new UserModel {
            Id = 2,
            Username = "user",
            Password = "user@123",
            Role = "user"
        }
        };

        [HttpPost("authenticate")]
        public ActionResult<AuthenticationResponse?> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            var authresponse = _jwtTokenHandler.GenerateJetToken(authenticationRequest);
            if (authresponse == null) return Unauthorized();
            return authresponse;
        }

        [HttpGet("allUser")]
        
        public ActionResult<IEnumerable<UserModel>> GetUsers()
        {
            try
            {
                if (users.Count == 0)
                {
                    return BadRequest("No user");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("register")]
        //public ActionResult Register(UserModel newUser)
        //{
        //    try
        //    {
        //        if (newUser == null || newUser.Username == null || newUser.Password == null)
        //        {
        //            return BadRequest("Invalid registration data");
        //        }

        //        if (users.Any(u => u.Username == newUser.Username))
        //        {
        //            return Conflict("Username is already taken");
        //        }

        //        var user = new UserModel
        //        {
        //            Id = users.Count + 1,
        //            Username = newUser.Username,
        //            Password = newUser.Password,
        //            Role = newUser.Role == "" ? "User" : "Admin"
        //        };

        //        users.Add(user);

        //        return Ok("Registered Succesfully!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost("login")]
        //public ActionResult<string> Login(string userName, string password)
        //{
        //    try
        //    {
        //        if (userName == null || password == null)
        //        {
        //            return BadRequest("Invalid login credentials");
        //        }

        //        var user = users.FirstOrDefault(u => u.Username == userName && u.Password == password);

        //        if (user == null)
        //        {
        //            return Unauthorized("Invalid username or password");
        //        }

        //        return Ok("Login successful");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
