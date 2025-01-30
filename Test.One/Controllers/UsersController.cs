using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.One.Data;
using Test.One.Models;
using Test.One.Services;

namespace Test.One.Controllers
{
    public class UsersController : Controller
    {
        private readonly UsersServices _usersServices;
        private readonly ApplicationDbContext _dbContext;

        public UsersController(UsersServices usersServices, ApplicationDbContext dbContext)
        {
            _usersServices = usersServices;
            _dbContext = dbContext;
        }



        public IActionResult Login()
        {
            return View();
        }

        public IActionResult GoogleLogin()
        {
            // Redirect to Google authentication
            var properties = new AuthenticationProperties
            {
                // Leave the RedirectUri as default handled by Google authentication
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }



        public async Task<IActionResult> GoogleCallback()
        {
            // Get the authentication result from Google
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                TempData["ErrorMessage"] = "Google authentication failed.";
                return RedirectToAction("Login");
            }

            // Retrieve the user information
            var email = authenticateResult.Principal.FindFirst(claim => claim.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(claim => claim.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

            if (email == null)
            {
                TempData["ErrorMessage"] = "Unable to retrieve email from Google.";
                return RedirectToAction("Login");
            }

            // Check if the user exists in your database and log them in or register them
            // Example: _userService.FindOrCreateUser(email, name);

            TempData["SuccessMessage"] = $"Welcome, {name}!";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var data = await _usersServices.GetUsersList();

                if (data == null)
                {
                    return View();
                }

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "A problem has occurred while submitting your data.";
                return View();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Users objUsers)
        {
            try
            {
                var data = await _usersServices.AddUser(objUsers, 0);

                if (data < 1)
                {
                    TempData["ErrorMessage"] = "A problem has occurred while submitting your data.";
                    return View();
                }
                TempData["SuccessMessage"] = "Your data inserted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "A problem has occurred while submitting your data.";
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var data = await _dbContext.Users.Where(m => m.UserId == id).FirstOrDefaultAsync();

                if (data == null)
                {
                    TempData["ErrorMessage"] = "A problem has occurred while fetching your data.";
                    return RedirectToAction(nameof(Index));
                }
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "A problem has occurred while fetching your data.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Users objUsers)
        {
            try
            {
                var data = await _usersServices.UpdateUser(objUsers, 0);

                if (data < 1)
                {
                    TempData["ErrorMessage"] = "A problem has occurred while updating your data.";
                    return View();
                }
                TempData["SuccessMessage"] = "Your data updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "A problem has occurred while updating your data.";
                return View();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var data = await _dbContext.Users.Where(m => m.UserId == id).FirstOrDefaultAsync();

                if (data == null)
                {
                    TempData["ErrorMessage"] = "A problem has occurred while fetching your data.";
                    return RedirectToAction(nameof(Index));
                }
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "A problem has occurred while fetching your data.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(Users objUsers)
        {
            try
            {
                var data = await _usersServices.DeleteUser(objUsers.UserId);

                if (data < 0)
                {
                    TempData["ErrorMessage"] = "A problem has occurred while deleting your data.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["SuccessMessage"] = "Your data deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "A problem has occurred while deleting your data.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Verify(int id)
        {
            try
            {
                var data = await _dbContext.Users.Where(m => m.UserId == id).FirstOrDefaultAsync();

                if (data == null)
                {
                    return Json(data);
                }
                return Json(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "A problem has occurred while fetching your data.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> VerifyPost(int id)
        {
            try
            {
                var data = await _usersServices.VerifyUser(id);

                if (data <= 0) // Adjusted to check for failure
                {
                    return Json(new { success = false, message = "A problem has occurred while verifying your data." });
                }
                TempData["SuccessMessage"] = "Your data verified successfully.";
                return Json(new { success = true, message = "Your data has been verified successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception details if needed
                return Json(new { success = false, message = "A problem has occurred while verifying your data." });
            }
        }


    }
}
