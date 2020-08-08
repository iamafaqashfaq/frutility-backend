using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using frutility_backend.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using SQLitePCL;
using Microsoft.Extensions.Primitives;

namespace frutility_backend.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/usercontroller")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public UserController(UserManager<ApplicationUser> usermanager,
            SignInManager<ApplicationUser> signInManager, IConfiguration config,
            RoleManager<IdentityRole> roleManager, DataContext context)
        {
            _userManager = usermanager;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        [Route("customerslist")]
        public async Task<ActionResult<ApplicationUser>> GetUsersList()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                string[] token = Request.Headers.GetCommaSeparatedValues("Authorization");
                string strtoken = String.Concat(token);
                strtoken = strtoken.Replace("Bearer ", "");
                var user = await _userManager.FindByIdAsync(User.Identity.Name);
                var result = await _userManager.GetRolesAsync(user);
                if (result.Contains("Admin"))
                {
                    var users = await _userManager.GetUsersInRoleAsync("User");
                    return Ok(users);
                }
            }
            return NoContent();
        }


        [AllowAnonymous]
        [Route("userregister")]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(UserSignupVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = model.username,
                    Email = model.email,
                    FirstName = model.fname,
                    LastName = model.lname,
                    ShippingAddress = model.sAddress,
                    ShippingState = model.sState,
                    ShippingCity = model.sCity,
                    BillingAddress = model.bAddress,
                    BillingState = model.bState,
                    BillingCity = model.bCity,
                    PhoneNumber = model.phone
                };
                var result = await _userManager.CreateAsync(user, model.password);
                var userRole = new IdentityRole("User");
                var roleresult = await _roleManager.RoleExistsAsync(userRole.Name);
                if (!roleresult)
                {
                    await _roleManager.CreateAsync(userRole);
                }
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, userRole.Name);
                    await _signInManager.SignInAsync(user, false);
                    var userId = await _userManager.FindByNameAsync(user.UserName);
                    Token token = new Token
                    {
                        entoken = GenerateToken(userId),
                        UserName = userId.UserName
                    };
                    return Ok(token);
                }
            }
            return Ok(false);
        }


        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LoginUser(AppLogin model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password
                    , false, false);

                if (result.Succeeded)
                {
                    ApplicationUser user = await _userManager.FindByNameAsync(model.UserName);
                    Token token = new Token
                    {
                        entoken = GenerateToken(user),
                        UserName = user.UserName
                    };
                    return Ok(token);
                }
            }
            return Ok(false);
        }

        [Authorize]
        [Route("changepassword")]
        [HttpPut]
        public async Task<ActionResult> ChangePassword(ChangePasswordVM model)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.Name);
            var result = await _userManager.ChangePasswordAsync(user, model.Oldpassword, model.Newpassword);
            if (result.Succeeded)
            {
                return Ok(true);
            }
            return Ok(false);
        }
        [Authorize]
        [Route("changeaddress")]
        [HttpPut]
        public async Task<ActionResult> ChangeAddress(ChangeAddressVM model)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.Name);
            if(!string.IsNullOrEmpty(model.ShippingAddress) 
                && !string.IsNullOrEmpty(model.ShippingState)
                && !string.IsNullOrEmpty(model.ShippingCity))
            {
                user.ShippingAddress = model.ShippingAddress;
                user.ShippingState = model.ShippingState;
                user.ShippingCity = model.ShippingCity;
            }
            if (!string.IsNullOrEmpty(model.BillingAddress)
                && !string.IsNullOrEmpty(model.BillingState)
                && !string.IsNullOrEmpty(model.BillingCity))
            {
                user.BillingAddress = model.BillingAddress;
                user.BillingState = model.BillingState;
                user.BillingCity = model.BillingCity;
            }
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(true);
            }
            return Ok(false);
        }


        [AllowAnonymous]
        [Route("adminlogin")]
        [HttpPost]
        public async Task<IActionResult> LoginAdmin(AppLogin model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(model.UserName);
                var checkRole = _userManager.GetRolesAsync(user);
                if (checkRole.Result.Contains("Admin"))
                {
                    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password
                        ,false, false);
                    if (result.Succeeded)
                    {
                        Token token = new Token
                        {
                            entoken = GenerateToken(user),
                            UserName = user.UserName
                        };
                        return Ok(token);
                    }
                }

            }
            return Ok(false);
        }

        //Get User Data to show user Account
        [Authorize]
        [Route("getmydata")]
        [HttpGet]
        public async Task<ActionResult> GetMyData()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.Name);
            return Ok(user);
        }


        public string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        [Authorize]
        [Route("checkuser")]
        [HttpPost]
        public async Task<IActionResult> CheckLogin(Token entoken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokens = handler.ReadToken(entoken.entoken) as JwtSecurityToken;
            var decoded = tokens.Claims.FirstOrDefault(t => t.Type == "unique_name");
            var user = await _userManager.FindByIdAsync(decoded.Value);
            var result = _userManager.GetRolesAsync(user);
            if (result != null)
            {
                return Ok(result.Result);
            }
            return Ok(decoded);
        }


        [Authorize]
        [Route("signout")]
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            var result = User.Identity.IsAuthenticated;
            if (result)
            {
                await _signInManager.SignOutAsync();
                return Ok(true);
            }
            return Ok(false);
        }


    }
}
