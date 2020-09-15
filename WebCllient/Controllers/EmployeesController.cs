using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCore.Model;
using NetCore.ViewModel;
using Newtonsoft.Json;

namespace WebCllient.Controllers
{
    public class EmployeesController : Controller
    {
        readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44372/api/")
        };
        public IActionResult Index()
        {
            return View("~/Views/Employees/Index.cshtml");
        }
        public JsonResult LoadEmploy()

        {
            IEnumerable<EmployeeVm> emp = null;
            //var token = HttpContext.Session.GetString("token");
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var resTask = client.GetAsync("employees");
            resTask.Wait();

            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<List<EmployeeVm>>();
                readTask.Wait();
                emp = readTask.Result;
            }
            else
            {
                emp = Enumerable.Empty<EmployeeVm>();
                ModelState.AddModelError(string.Empty, "Server Error try after sometimes.");
            }
            return Json(emp);
        }

            public IActionResult GetById(string id)
        {
            EmployeeVm emp = null;
            //var token = HttpContext.Session.GetString("token");
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var resTask = client.GetAsync("employees/" + id);
            resTask.Wait();

            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();
                emp = JsonConvert.DeserializeObject<EmployeeVm>(json);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error.");
            }
            return Json(emp);
        }

        public IActionResult Delete(string id)
        {
            //var token = HttpContext.Session.GetString("token");
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var result = client.DeleteAsync("employees/" + id).Result;
            return Json(result);
        }

    }
}