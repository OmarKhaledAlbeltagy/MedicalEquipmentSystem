using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using AMEKSA.Context;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AMEKSA.Repo;
using AMEKSA.Models;
using AMEKSA.CustomEntities;
using System.Net.Mail;
using System.Net;

namespace AMEKSA.Controllers
{

    [ApiController]
    [EnableCors("allow")]
    public class UsersController : ControllerBase
    {

        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly SignInManager<ExtendIdentityUser> signInManager;
        private readonly RoleManager<ExtendIdentityRole> roleManager;
        private readonly IUserRep userRep;

        public UsersController(DbContainer db, UserManager<ExtendIdentityUser> userManager, SignInManager<ExtendIdentityUser> signInManager, RoleManager<ExtendIdentityRole> roleManager,IUserRep userRep)
        {

            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.userRep = userRep;
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult ForgotPasswordEmail(ForgotPasswordEmailModel obj)
        {
            ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
            if (user == null)
            {
                return Ok(false);
            }
            else
            {
                string token = userManager.GeneratePasswordResetTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "You Requested to reset your password \n\nClick Here\n https://amereport.com/forgot-password.html?"+token+"&"+obj.Email;

                smtp.Send(EmailModel.EmailAddress, obj.Email, obj.Subject, body);
                return Ok(true);
            }
            
        }

        [Route("[controller]/[Action]")]
        [AcceptVerbs("Post", "Get")]
        public IActionResult ForgotPassword(ForgotPasswordModel obj)
        {
           ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
           IdentityResult newpassword = userManager.ResetPasswordAsync(user, obj.token, obj.NewPassword).Result;
            if (newpassword.Succeeded)
            {
                return Ok(true);
            }
            else
            {
                return Ok(newpassword.Errors);
            }
        }

        [Route("[controller]/[Action]")]
        [AcceptVerbs("Post", "Get")]
        public IActionResult ResetPassword(ResetPasswordModel obj)
        {
           ExtendIdentityUser user = userManager.FindByIdAsync(obj.UserId).Result;

           IdentityResult newpassword = userManager.ChangePasswordAsync(user, obj.OldPassword, obj.NewPassword).Result;

         

            if (newpassword.Succeeded)
            {
                return Ok(true);
            }

            else
            {
                return BadRequest(false);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult ConfirmEmail(ConfirmEmailModel obj)
        {
           ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
           IdentityResult res = userManager.ConfirmEmailAsync(user, obj.token).Result;

            if (res.Succeeded)
            {
                return Ok(true);
            }

            else
            {
                return Ok(false);
            }
           
        }

        [Route("[controller]/[Action]/{email}")]
        [HttpGet]
        public IActionResult ResendEmailConfirmation(string email)
        {
            ExtendIdentityUser user = userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                return Ok(false);
            }

            else
            {
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by ame reporting tool system admin with your email address \n\nClick Here to verify your account\n https://amereport.com/emailconfirmed.html?" + token + "&" + email;
                smtp.Send(EmailModel.EmailAddress, email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
        }

        [Route("[controller]/[Action]")]
        [AcceptVerbs("Post","Get")]
        public IActionResult ResetEmail(ResetEmailModel obj)
        {
          ExtendIdentityUser user = userManager.FindByEmailAsync(obj.OldEmail).Result;

          string emailtoken = userManager.GenerateChangeEmailTokenAsync(user, obj.NewEmail).Result;

          IdentityResult emailresult = userManager.ChangeEmailAsync(user, obj.NewEmail, emailtoken).Result;

           IdentityResult usernameresult = userManager.SetUserNameAsync(user, obj.NewEmail).Result;
           Task x = userManager.UpdateNormalizedUserNameAsync(user);

            if (emailresult.Succeeded && usernameresult.Succeeded)
            {
                return Ok(true);
            }

            else
            {
                return Ok(false);
            }

        }
        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditEmailSetting(ResetEmailModel obj)
        {
            ExtendIdentityUser userr = db.Users.Where(a => a.Id == obj.Id && a.Email == obj.OldEmail).SingleOrDefault();

            if (userr == null)
            {
                return Ok(false);
            }
            else
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.OldEmail).Result;

                string emailtoken = userManager.GenerateChangeEmailTokenAsync(user, obj.NewEmail).Result;

                IdentityResult emailresult = userManager.ChangeEmailAsync(user, obj.NewEmail, emailtoken).Result;

                IdentityResult usernameresult = userManager.SetUserNameAsync(user, obj.NewEmail).Result;
                Task x = userManager.UpdateNormalizedUserNameAsync(user);

                if (emailresult.Succeeded && usernameresult.Succeeded)
                {
                    return Ok(true);
                }

                else
                {
                    return Ok(false);
                }
            }

        }



        [Route("[controller]/[Action]/{name}")]
        [HttpGet("{name}")]
        public IActionResult AddRole(string name)
        {
            ExtendIdentityRole r = new ExtendIdentityRole();
            r.Name = name;
            var res = roleManager.CreateAsync(r).Result;
            return Ok(true);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult Login(LoginModel obj)
        {
            ExtendIdentityUser usercheck = userManager.FindByEmailAsync(obj.Email).Result;
            if (usercheck == null)
            {
                return Ok("not found");
            }
            else
            {
                if (usercheck.Active == false)
                {
                    return Ok("deactivated");
                }
                else
                {
                    var res = signInManager.PasswordSignInAsync(obj.Email, obj.Password, obj.RememberMe, false).Result;

                    if (res.IsNotAllowed)
                    {
                        return Ok("not allowed");
                    }
                    else
                    {
                        if (res.Succeeded)
                        {
                            ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                            CustomUserRole userrole = new CustomUserRole();
                            userrole.FullName = user.FullName;
                            userrole.UserEmail = user.Email;
                            userrole.UserId = user.Id;
                            userrole.RoleName = role;
                            return Ok(userrole);
                        }
                        else
                        {
                            return Ok("wrong password");
                        }
                    }
                }
            }
            

        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult LogOut()
        {
            Task x = signInManager.SignOutAsync();

            if (x.IsCompletedSuccessfully)
            {
                return Ok(true);
            }

            else
            {
                return BadRequest(x.Exception.Message);
            }
                
        }


        [Route("[controller]/[action]/{id?}")]
        [HttpGet("{id?}")]
        public IActionResult GetUserById(string id)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(id).Result;

            return Ok(user);

        }



        [Route("[controller]/[action]/{Email?}")]
        [HttpGet("{Email?}")]
        public IActionResult CheckEmail(string Email)
        {
            ExtendIdentityUser res = userManager.FindByEmailAsync(Email).Result;


            if (res == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [Route("[controller]/[action]/{Phone?}")]
        [HttpGet("{Phone?}")]
        public IActionResult CheckPhone(string Phone)
        {
            ExtendIdentityUser res = userManager.Users.Where(a=>a.PhoneNumber == Phone).FirstOrDefault();


            if (res == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetRoles()
        {
            IEnumerable<string> roles = roleManager.Roles.Select(a=>a.Name);
            return Ok(roles);
        }

        [Route("[controller]/[Action]/{cityid}")]
        [HttpGet("{cityid}")]
        public IActionResult GetManagerByCityId(int cityid)
        {

            ExtendIdentityRole role = roleManager.FindByNameAsync("First Line Manager").Result;
            string roleid = role.Id;

            var managers = userManager.Users.Join(db.UserRoles, a => a.Id, b => b.UserId, (a, b) => new
            {
                Id = a.Id,
                ManagerName = a.FullName,
                RoleId = b.RoleId,
                CityId = a.CityId
            }).Where(x => x.RoleId == roleid && x.CityId == cityid);

            return Ok(managers);

        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditUser(ExtendIdentityUser obj)
        {
            bool res = userRep.EditUser(obj);

            return Ok(res);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditUserGeneralSetting(UserGeneralSetting obj)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(obj.Id).Result;
            
           string phonetoken = userManager.GenerateChangePhoneNumberTokenAsync(user, obj.PhoneNumber).Result;
           var phonechange = userManager.ChangePhoneNumberAsync(user, obj.PhoneNumber, phonetoken).Result;
            if (phonechange.Succeeded)
            {
                user.FullName = obj.FullName;
                db.SaveChanges();
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [Route("[controller]/[Action]/{userid}/{newpassword}")]
        [HttpPost]
        public IActionResult resetpassworddeveopment(string userid,string newpassword)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(userid).Result;
            string token = userManager.GeneratePasswordResetTokenAsync(user).Result;

            IdentityResult x = userManager.ResetPasswordAsync(user, token, newpassword).Result;

            if (x.Succeeded)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
           
           
        }
    }
}
