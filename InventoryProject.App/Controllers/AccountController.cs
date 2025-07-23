using AutoMapper;
using InventoryProject.App.ViewModels;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.Models.Authentication;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Persistence.Repositories.UserRepo;
using InventoryProject.DataAccess.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace InventoryProject.App.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly DataAccess.Services.Interface.IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IUserRepository userRepo,
            DataAccess.Services.Interface.IAuthenticationService authenticationService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _userRepo = userRepo;
            _authenticationService = authenticationService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToLocal(returnUrl);

            var viewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View("Login", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                // Verification.
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest("Invalid Request!");
                //}
                //var company = await _companyRepo.GetByIdAsync(model.CompanyId);

                AuthRequest authRequest = new()
                {
                    Username = model.Username,
                    Password = model.Password,
                    //CompanyCode = company.Code,
                };

                var result = await _authenticationService.Authenticate(authRequest);
                model.Id = result.Id;

                await SignInUserAsync(model);

                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserModel model)
        {
            try
            {
                model.IsUpdate = false;
                //if (!ModelState.IsValid)
                //    return BadRequest(ModelState);
                var data = await _userRepo.SaveAsync(model, userId: 1);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Update(UserModel model)
        {
            try
            {
                model.IsUpdate = true;

                // Verification.
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest("Invalid Request!");
                //}
                //var company = await _companyRepo.GetByIdAsync(model.CompanyId);

                AuthRequest authRequest = new()
                {
                    Username = model.Username,
                    Password = model.Password,
                    IsUpdate = model.IsUpdate,
                };

                var result = await _authenticationService.Authenticate(authRequest);

                if (result == null)
                    return BadRequest("Invalid Username/Password");

                var data = await _userRepo.SaveAsync(model, userId: 1);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task SignInUserAsync(LoginViewModel model)
        {
            try
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Id.ToString()),
                new Claim(ClaimTypes.Role, model.Id.ToString()),
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    ExpiresUtc = DateTimeOffset.Now.AddMinutes(15),
                    // The time at which the authentication ticket expires. A
                    // value set here overrides the ExpireTimeSpan option of
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = model.RememberMe
                    // Whether the authentication session is persisted across
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http
                    // redirect response value.
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogOff()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public async Task<IActionResult> IsAuthenticated()
        {
            try
            {
                int userId = int.Parse(User.Identity.Name ?? "0");
                int companyId = int.Parse(User.FindFirstValue("Company") ?? "0");

                if (userId != 0)
                {
                    var user = await _userRepo.GetUserByIdAsync(userId);
                    if (user is not null)
                        return RedirectToAction("LogOff");
                }

                bool _IsAuthorized = User.Identity.IsAuthenticated;
                return Ok(_IsAuthorized);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        public async Task<IActionResult> UserProfile()
        {
            try
            {
                int userId = int.Parse(User.Identity.Name ?? "0");
                var user = await _userRepo.GetUserByIdAsync(userId);
                if (user is null)
                    return RedirectToAction("Login");

                var _model = _mapper.Map<UserModel>(user);
                if (!_model.IsAdmin)
                    _model.IsAdmin = false;
                var model = new UserViewModel
                {
                    User = _model
                };

                return View(model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUser(UserModel model)
        {
            try
            {
                model.IsUpdate = true;
                //if (!ModelState.IsValid)
                //    return Conflict(ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors }));

                var userId = int.Parse(User.Identity.Name);
                //var companyId = int.Parse(User.FindFirstValue("Company"));
                var rootFolder = _webHostEnvironment.WebRootPath;

                string? profilePicture = null;

                if (model.ProfilePictureFile != null)
                {
                    var filename = model.Username + Path.GetExtension(model.ProfilePictureFile.FileName);
                    var path = Path.Combine(rootFolder, "Files", "Images", "UserPics", model.Username, filename);
                    Directory.CreateDirectory(Path.Combine(rootFolder, "Files", "Images", "UserPics", model.Username));
                    using var image = Image.Load(model.ProfilePictureFile.OpenReadStream());
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(200, 200),
                        Mode = ResizeMode.Crop
                    }));
                    image.Save(path);

                    profilePicture = "/" + Path.Combine("Files", "Images", "UserPics", model.Username, filename).Replace("\\", "/");
                }

                model.ProfilePicture = profilePicture;

                var data = await _userRepo.SaveAsync(model, userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            return Ok(user);
        }
    }
}
