using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCore.Model;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace WebCllient.Controllers
{
    public class DepartmentsController : Controller
    {
        readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44372/api/")
        };
        public IActionResult Index()
        {
            return View("~/Views/Departments/Index.cshtml");
        }
        public JsonResult LoadDepart()
        {
            IEnumerable<Department> departments = null;
            var resTask = client.GetAsync("departments");
            resTask.Wait();
            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var read = result.Content.ReadAsAsync<IList<Department>>();
                read.Wait();
                departments = read.Result;
            }
            else
            {
                departments = Enumerable.Empty<Department>();
                ModelState.AddModelError(string.Empty, "Server error");
            }
            return Json(departments);
        }

        public JsonResult GetById(int id)
        {
            Department departments = null;
            var resTask = client.GetAsync("departments/" + id);
            resTask.Wait();
            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var getJson = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();
                departments = JsonConvert.DeserializeObject<Department>(getJson);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error try after sometimes.");
            }

            return Json(departments);
        }
        public JsonResult InsertOrUpdate(Department departments, int id)
        {
            try
            {
                var json = JsonConvert.SerializeObject(departments);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (departments.Id == 0)
                {
                    var result = client.PostAsync("departments", byteContent).Result;
                    return Json(result);
                }
                else if (departments.Id == id)
                {
                    var result = client.PutAsync("departments/" + id, byteContent).Result;
                    return Json(result);
                }

                return Json(404);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult Delete(int id)
        {
            var result = client.DeleteAsync("departments/" + id);
            result.Wait();
            var respon = result.Result;
            return Json(respon);
        }
    }
}
