using System.Security.Claims;
using InsideOutSellingSolutions.DTOs.AccountDTO;
using InsideOutSellingSolutions.Repository;
using InsideOutSellingSolutions.Repository.IRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsideOutSellingSolutions.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _iaccountrepository;
        private readonly ICommonRepository _icommonRepository;
        public AccountController(IAccountRepository iaccountRepository, ICommonRepository icommonRepository)
        {
            _iaccountrepository = iaccountRepository;
            _icommonRepository = icommonRepository;
        }
        public IActionResult UserIndex()
        {
            return View(_iaccountrepository.GetAllUsers());
        }

        [HttpGet]
        public IActionResult Register()
        {
            var registerDTOobj = new RegisterDTO();

            registerDTOobj.Roles = _iaccountrepository.GetAllRoles();

            return View(registerDTOobj);
        }

        [HttpPost]
        public IActionResult Register(RegisterDTO registerDTOobj)
        {
            try
            {
                var username = _iaccountrepository.GetAllUsers().Find(un => un.UserName == registerDTOobj.UserName);
                var email = _iaccountrepository.GetAllUsers().Find(e => e.EmailId == registerDTOobj.EmailId);

                if (username != null && email != null)
                {
                    TempData["ErrorMessage"] = "User Name & Email Already Exists...";
                }
                else if (username != null)
                {
                    TempData["ErrorMessage"] = "User Name Already Exists...";
                }
                else if (email != null)
                {
                    TempData["ErrorMessage"] = "Email Already Exists...";
                }
                else
                {
                    if (registerDTOobj != null && !string.IsNullOrWhiteSpace(registerDTOobj.FullName))
                    {
                        _iaccountrepository.Registration(registerDTOobj);                        
                        return RedirectToAction("UserIndex");
                    }
                }
                registerDTOobj.Roles = _iaccountrepository.GetAllRoles();
                return View(registerDTOobj);
            }
            catch (Exception ex)
            {
                registerDTOobj.Roles = _iaccountrepository.GetAllRoles();
                return View(registerDTOobj);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO loginDTOobj)
        {
            if (string.IsNullOrWhiteSpace(loginDTOobj.LoginId) || string.IsNullOrWhiteSpace(loginDTOobj.Password))
            {
                return View();
            }

            LoginResponseDTO result = _iaccountrepository.Login(loginDTOobj);

            if (result.Message == "Login successful")
            {
                // 1. Create claims
                var claims = new List<Claim>
        {
            new Claim("UserName", result.UserName),
            new Claim("FullName", result.FullName),
            new Claim("RoleName", result.Role),
            new Claim(ClaimTypes.Role, result.RoleId.ToString()), // ✅ Correct Claim
        };

                // 2. Create identity and principal
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                // 3. Sign in using cookie authentication
                await HttpContext.SignInAsync("MyCookieAuth", principal);

                // 4. Optional: Set session values if still used elsewhere
                HttpContext.Session.SetString("FullName", result.FullName);
                HttpContext.Session.SetString("Role", result.Role);
                HttpContext.Session.SetString("UserName", result.UserName);
                // Newly Added Part
                if (result.RoleId.HasValue)
                {
                    HttpContext.Session.SetInt32("RoleId", result.RoleId.Value);
                }
                else
                {
                    HttpContext.Session.Remove("RoleId"); // Or don't set anything
                }


                //_notyf.Success("Login Successfull..!!");
                return RedirectToAction("HomeIndex", "Home");
            }
            else
            {
                ViewBag.Message = result.Message;
                return View();
            }
        }
    }
}
