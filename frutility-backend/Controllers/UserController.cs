using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using frutility_backend.Data;
using frutility_backend.Data.Model;
using frutility_backend.Data.ViewModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace frutility_backend.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/usercontroller")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DataContext _context;
        public UserController(UserManager<ApplicationUser> usermanager,
            SignInManager<ApplicationUser> signInManager, DataContext context)
        {
            _userManager = usermanager;
            _signInManager = signInManager;
            _context = context;
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(ApplicationUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return Ok();
                }
            }
            return NoContent();
        }
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
                    string id = _userManager.GetUserId(User);
                    ApplicationUser user = await _context.Users.FindAsync(id);
                    return Ok(new {
                        user.Id,
                        user.UserName
                    });
                }
            }
            return NoContent();
        }

        [Route("checkuser")]
        [HttpGet]
        public ActionResult<IEnumerable<ApplicationUser>> CheckLogin()
        {
            var result = User.Identity.IsAuthenticated;
            if (result)
            {
                return Ok(User.Identity.Name);
            }
            return Ok(false);
        }

        [Route("signout")]
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok(true);
        }


    }
}
