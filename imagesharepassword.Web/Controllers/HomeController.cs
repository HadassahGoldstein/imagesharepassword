using imagesharepassword.Data;
using imagesharepassword.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace imagesharepassword.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
        @"Data Source=.\sqlexpress; Initial Catalog=ShareImagesDB;Integrated Security=true;";

        private readonly IWebHostEnvironment _environment;
        public IActionResult Index()
        {
            return View();
        }
        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        [HttpPost]
        public IActionResult AddImage(IFormFile myImage,string password)
        {
            Guid guid = Guid.NewGuid();
            string fileName = $"{guid}-{myImage.FileName}";
            string finalFileName = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(finalFileName, FileMode.CreateNew);
            myImage.CopyTo(fs);
            var db = new ShareImagesDB(_connectionString);
            Image i = new(){ ImageName=fileName, Password=password};
            db.AddImage(i);
            ImageAddedViewModel vm = new()
            {
                ImageId =i.Id,
                Password=password
            };
            return View(vm);
        }
        public IActionResult View(int id)
        {
            var db = new ShareImagesDB(_connectionString);            
            var ids = HttpContext.Session.Get<List<int>>("Ids");
            if (ids == null)
            {
                ids = new List<int>();
            }            
            var vm = new ShowImageViewModel() { Id=id,Ids=ids};
            if (ids.Contains(id))
            {
                Image i = db.GetImage(id);
                db.UpdateViews(i);
                vm.Image = i;
                vm.ShowImage = true;
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult View(int id,string password)
        {           
            var db = new ShareImagesDB(_connectionString);                      
            Image i = db.GetImage(id);
            var ids = HttpContext.Session.Get<List<int>>("Ids");
            if (ids == null)
            {
                ids = new List<int>();
            }
            if (i.Password == password)
            {               
                ids.Add(id);
                HttpContext.Session.Set("Ids", ids);
                db.UpdateViews(i);
            }
            var vm = new ShowImageViewModel()
            {
                Id = id,
                Image=i,
                ShowImage=i.Password==password,
                FalsePassword=!(i.Password==password),
                Ids=ids
            };            
            return View(vm);
        }


    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
