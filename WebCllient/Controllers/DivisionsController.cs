using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCore.Model;
using Newtonsoft.Json;

namespace WebCllient.Controllers
{
    public class DivisionsController : Controller
    {
        readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44372/api/")
        };
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult LoadDiv()
        {
            IEnumerable<Division> divisions = null;
            var resTask = client.GetAsync("divisions");
            resTask.Wait();
            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Division>>();
                readTask.Wait();
                divisions = readTask.Result;
            }
            else
            {
                divisions = Enumerable.Empty<Division>();
                ModelState.AddModelError(string.Empty, "Server Error try after sometimes.");
            }

            return Json(divisions);
        }

        public JsonResult GetById(int id)
        {
            Division divisions = null;
            var resTask = client.GetAsync("divisions/" + id);
            resTask.Wait();
            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var getJson = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();
                divisions = JsonConvert.DeserializeObject<Division>(getJson);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error try after sometimes.");
            }

            return Json(divisions);
        }
        public JsonResult InsertOrUpdate(Division divisions, int id)
        {
            try
            {
                var json = JsonConvert.SerializeObject(divisions);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (divisions.Id == 0)
                {
                    var result = client.PostAsync("divisions", byteContent).Result;
                    return Json(result);
                }
                else if (divisions.Id != 0)
                {
                    var result = client.PutAsync("divisions/" + id, byteContent).Result;
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
            var result = client.DeleteAsync("divisions/" + id).Result;
            return Json(result);
        }
    }
}