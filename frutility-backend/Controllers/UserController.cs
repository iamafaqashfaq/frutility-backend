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
        private readonly IConfiguration _config;

        public UserController(UserManager<ApplicationUser> usermanager,
            SignInManager<ApplicationUser> signInManager, IConfiguration config, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = usermanager;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
        }


        [AllowAnonymous]
        [Route("userregister")]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(ApplicationUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
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
                    var token = GenerateToken(userId);
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
                    var token = GenerateToken(user);
                    return Ok(token);
                }
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
                bool check = checkRole.Result.Contains("Admin");
                if (check)
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
