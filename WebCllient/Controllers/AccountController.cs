using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.ViewModel;
using Newtonsoft.Json;
using BC = BCrypt.Net.BCrypt;

namespace WebCllient.Controllers
{
    public class AccountController : Controller
    {
        readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44372/api/")
        };

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }
        [Route("verify")]
        public IActionResult Verify()
        {
            return View();
        }

        [Route("validate")]
        public IActionResult Validate(UserVm userVm)
        {
            if (userVm.Username == null)
            {
                var jsonUserVM = JsonConvert.SerializeObject(userVm);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonUserVM);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var resTask = client.PostAsync("account/login/", byteContent);
                resTask.Wait();
                var result = resTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync().Result;
                    if (data != "")
                    {
                        var json = JsonConvert.DeserializeObject(data).ToString();
                        var account = JsonConvert.DeserializeObject<UserVm>(json);
                        //if (BC.Verify(userVM.Password, account.Password) && (account.RoleName == "Admin" || account.RoleName == "Sales"))
                        if (account.VerifyCode != null)
                        {
                            if (userVm.VerifyCode != account.VerifyCode)
                            {
                                return Json(new { status = true, msg = "Check your Code" });
                            }
                        }
                        else if (account.RoleName == "HR" || account.RoleName == "Sales")
                        {
                            HttpContext.Session.SetString("id", account.Id);
                            HttpContext.Session.SetString("uname", account.Username);
                            HttpContext.Session.SetString("email", account.Email);
                            HttpContext.Session.SetString("lvl", account.RoleName);
                            if (account.RoleName == "HR")
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "HR" });
                            }
                            else
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Sales" });
                            }
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Invalid Username or Password!" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Username Not Found!" });
                    }
                }
                else
                {
                    //return RedirectToAction("Login","Auth");
                    return Json(new { status = false, msg = "Something Wrong!" });
                }
            }
            else if (userVm.Username != null)
            {
                var json = JsonConvert.SerializeObject(userVm);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = client.PostAsync("account/register/", byteContent).Result;
                if (result.IsSuccessStatusCode)
                {
                    return Json(new { status = true, code = result, msg = "Register Success! " });
                }
                else
                {
                    return Json(new { status = false, msg = "Something Wrong!" });
                }
            }
            return Redirect("/login");
        }

        [Route("verifCode")]
        public IActionResult VerifCode(UserVm userVm)
        {
            if (userVm.VerifyCode != null)
            {
                var jsonUserVM = JsonConvert.SerializeObject(userVm);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonUserVM);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = client.PostAsync("account/code/", byteContent).Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync().Result;
                    if (data != "")
                    {
                        var json = JsonConvert.DeserializeObject(data).ToString();
                        var account = JsonConvert.DeserializeObject<UserVm>(json);
                        if (account.RoleName == "HR" || account.RoleName == "Sales")
                        {
                            HttpContext.Session.SetString("id", account.Id);
                            HttpContext.Session.SetString("uname", account.Username);
                            HttpContext.Session.SetString("email", account.Email);
                            HttpContext.Session.SetString("lvl", account.RoleName);
                            if (account.RoleName == "HR")
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "HR" });
                            }
                            else
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Sales" });
                            }
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Invalid Username or Password!" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Username Not Found!" });
                    }
                    //var data = result.Content.ReadAsStringAsync().Result;
                    //var json = JsonConvert.DeserializeObject(data).ToString();
                    //var account = JsonConvert.DeserializeObject<UserVM>(json);
                    //var dataLogin = new UserVM()
                    //{
                    //    Email = account.Email,
                    //    Password = account.Password
                    //};
                    //this.Validate(dataLogin);
                    //return Json(new { status = true, code = result, msg = "Login Success! " });
                }
                else
                {
                    return Json(new { status = false, msg = "Your Code is Wrong!" });
                }
            }
            else
            {
                return Json(new { status = false, msg = "Something Wrong!" });
            }
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/login");
        }
    }
}