using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaWebApp.ViewModels.Account;

namespace WebAppRelation.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleManager<IdentityRole> RoleManager { get; }

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = new AppUser()
            {
                UserName = registerVM.Username,
                Email = registerVM.Email,
                Name = registerVM.Name,
                Surname = registerVM.Surname
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);

                }
                return View();
            }
            await _signInManager.SignInAsync(user, false);
            await _userManager.AddToRoleAsync(user,UserRole.Member.ToString());
            return RedirectToAction("Home", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
                if(user == null)
                {
                    ModelState.AddModelError("", "UserName/Email Or Password is Wrong");
                    return View();                
                }
            }



            var result = _signInManager.CheckPasswordSignInAsync(user, loginVM.Password, true).Result;
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Wait Please");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName/Email Or Password is Wrong");
                return View();
            }

            await _signInManager.SignInAsync(user, loginVM.RememberMe);
            //if(ReturnUrl != null && !ReturnUrl.Contains("Login"));
            //{
            //    return RedirectToAction(ReturnUrl);
            //}
            return RedirectToAction(nameof(Index), "Home");
            
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CreateRole()
        {
            foreach (UserRole item in Enum.GetValues(typeof(UserRole)))
            {
                if(await _roleManager.FindByNameAsync(item.ToString()) != null)
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = item.ToString(),
                    });
                }
            }

            return RedirectToAction(nameof(Index), "Home");
        }


    }
}