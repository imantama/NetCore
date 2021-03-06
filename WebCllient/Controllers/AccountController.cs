﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
            return View("~/Views/Account/Login.cshtml");
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

        [Route("notfound")]
        public IActionResult Notfound()
        {
            return View();
        }

        [Route("validate")]
        public IActionResult Validate(UserVm userVm)
        {
            if (userVm.Username == null)
            { // Login
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
                    if (data != null)
                    {
                        HttpContext.Session.SetString("token", "Bearer " + data);
                        var handler = new JwtSecurityTokenHandler();
                        var tokenS = handler.ReadJwtToken(data);

                        //var user = new UserVM();
                        //user.Id = tokenS.Claims.First(claim => claim.Type == "Id").Value;
                        //user.Username = tokenS.Claims.First(claim => claim.Type == "Username").Value;
                        //user.Email = tokenS.Claims.First(claim => claim.Type == "Email").Value;
                        //user.RoleName = tokenS.Claims.First(claim => claim.Type == "RoleName").Value;
                        //user.VerifyCode = tokenS.Claims.First(claim => claim.Type == "VerifyCode").Value;

                        var jwtPayloadSer = JsonConvert.SerializeObject(tokenS.Payload.ToDictionary(x => x.Key, x => x.Value));
                        var jwtPayloadDes = JsonConvert.DeserializeObject(jwtPayloadSer).ToString();
                        var account = JsonConvert.DeserializeObject<UserVm>(jwtPayloadSer);

                        //var json = JsonConvert.DeserializeObject(data).ToString();
                        //var account = JsonConvert.DeserializeObject<UserVM>(json);
                        //if (BC.Verify(userVM.Password, account.Password) && (account.RoleName == "Admin" || account.RoleName == "Sales"))
                        if (!account.VerifyCode.Equals(""))
                        {
                            if (userVm.VerifyCode != account.VerifyCode)
                            {
                                return Json(new { status = true, msg = "Check your Code" });
                            }
                        }
                        else if (account.RoleName == "Admin" || account.RoleName == "Sales" || account.RoleName == "HR")
                        {
                            HttpContext.Session.SetString("id", account.Id);
                            HttpContext.Session.SetString("uname", account.Username);
                            HttpContext.Session.SetString("email", account.Email);
                            HttpContext.Session.SetString("lvl", account.RoleName);
                            if (account.RoleName == "Admin")
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Admin" });
                            }
                            else if (account.RoleName == "Sales")
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Sales" });
                            }
                            else
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "HR" });
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
            { // Register
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
                        if (account.RoleName == "HR" || account.RoleName == "Sales" || account.RoleName == "Admin")
                        {
                            HttpContext.Session.SetString("id", account.Id);
                            HttpContext.Session.SetString("uname", account.Username);
                            HttpContext.Session.SetString("email", account.Email);
                            HttpContext.Session.SetString("lvl", account.RoleName);
                            if (account.RoleName == "Admin")
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Admin" });
                            }
                            else if (account.RoleName == "Sales")
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Sales" });
                            }
                            else
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "HR" });
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
            //var jwtauthmanager = new IJwtAuthManager();
            //var userName = User.Identity.Name;
            //_jwtAuthManager.RemoveRefreshTokenByUserName(userName); // can be more specific to ip, user agent, device name, etc.
            //_logger.LogInformation($"User [{userName}] logged out the system.");

            //HttpContext.Session.Remove("lvl");
            HttpContext.Session.Clear();


            //HttpContext.Session.Abandon();
            //HttpContext.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            return Redirect("/login");
        }

        [Route("getjwt")]
        public IActionResult GetName()
        {
            var stream = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6ImRiM2VhZmIxLTkyMWUtNDdmYS1hOGFiLTIwNDYxMzkxM2FlMCIsIlVzZXJuYW1lIjoiUmlmcXkiLCJFbWFpbCI6Im11aGFtbWFkcmlmcWkwQGdtYWlsLmNvbSIsIlJvbGVOYW1lIjoiU2FsZXMiLCJleHAiOjE1OTk1NDY0MTYsImlzcyI6IkludmVudG9yeUF1dGhlbnRpY2F0aW9uU2VydmVyIiwiYXVkIjoiSW52ZW50b3J5c2VydmljZVBvc3RtYW50Q2xpZW50In0.ziIjgvqJdH17w4HwHGzvXyZTUz41S06i0xHWGxAnY2M";
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadJwtToken(stream);
            //var cek = tokenS.Payload;
            //cek.u

            //var jsonToken = handler.ReadToken(stream);
            //var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

            //var id = tokenS.Claims.First(claim => claim.Type == "Id").Value;
            //var uname = tokenS.Claims.First(claim => claim.Type == "Username").Value;
            //var mail = tokenS.Claims.First(claim => claim.Type == "Email").Value;
            //var role = tokenS.Claims.First(claim => claim.Type == "RoleName").Value;

            var user = new UserVm()
            {
                Id = tokenS.Claims.First(claim => claim.Type == "Id").Value,
                Username = tokenS.Claims.First(claim => claim.Type == "Username").Value,
                Email = tokenS.Claims.First(claim => claim.Type == "Email").Value,
                RoleName = tokenS.Claims.First(claim => claim.Type == "RoleName").Value,
            };

            var usrVm = new UserVm();
            //return Json(user);
            return Json(tokenS.Payload);
        }
    }
}