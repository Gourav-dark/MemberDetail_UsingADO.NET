using MemberDetail_UsingADO.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace MemberDetail_UsingADO.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private SqlConnection _conn;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private void Connection()
        {
            string constr = _config.GetConnectionString("ConnString");
            _conn = new SqlConnection(constr);
        }
        public HomeController(ILogger<HomeController> logger, IConfiguration config, IWebHostEnvironment env)
        {
            _logger = logger;
            _config = config;
            _env = env;
        }

        public IActionResult Index()
        {
            List<User> userlist = new List<User>();
            Connection();
            SqlCommand com = new SqlCommand("sp_GetAllUsers", _conn);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            _conn.Open();
            int i=da.Fill(dt);
            _conn.Close();
            foreach (DataRow dr in dt.Rows)
            {
                User user = new User();
                user.Id = Convert.ToInt32(dr["id"]);
                user.Name = Convert.ToString(dr["Name"]);
                user.PhoneNumber = Convert.ToString(dr["PhoneNumber"]);
                user.Gender = Convert.ToString(dr["Gender"]);
                user.Address = Convert.ToString(dr["Address"]);
                user.ImageUrl = Convert.ToString(dr["Image"]);
                userlist.Add(user);
            }
            return View(userlist);
        }

        public IActionResult AddUser()
        {
            return View("User");
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(User obj)
        {

            obj.ImageUrl=await ImageUrlGenerate(obj.Image, obj.PhoneNumber);
            Connection();
            SqlCommand com = new SqlCommand("sp_InsertUser", _conn);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Name", obj.Name);
            com.Parameters.AddWithValue("@PhoneNumber", obj.PhoneNumber);
            com.Parameters.AddWithValue("@Gender", obj.Gender);
            com.Parameters.AddWithValue("@Address", obj.Address);
            com.Parameters.AddWithValue("@Image", obj.ImageUrl);
            _conn.Open();
            com.ExecuteNonQuery();
            _conn.Close();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteMember(int Id)
        {
            Connection();
            SqlCommand com = new SqlCommand("sp_DeleteUser", _conn);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("Id", Id);
            _conn.Open();
            com.ExecuteNonQuery();
            _conn.Close();
            return RedirectToAction("Index");
        }
        public IActionResult EditMember(int Id)
        {
            User user=new User();
            Connection();
            SqlCommand com = new SqlCommand("sp_GetUserById", _conn);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("Id", Id);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            _conn.Open();
            da.Fill(dt);
            _conn.Close();
            foreach (DataRow dr in dt.Rows)
            {
                user.Id = Convert.ToInt32(dr["id"]);
                user.Name = Convert.ToString(dr["Name"]);
                user.PhoneNumber = Convert.ToString(dr["PhoneNumber"]);
                user.Gender = Convert.ToString(dr["Gender"]);
                user.Address = Convert.ToString(dr["Address"]);
                user.ImageUrl = Convert.ToString(dr["Image"]);
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditMember(User obj){
            obj.ImageUrl =await ImageUrlGenerate(obj.Image, obj.PhoneNumber);
            Connection();
            SqlCommand com = new SqlCommand("sp_UpdateUser", _conn);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Id", obj.Id);
            com.Parameters.AddWithValue("@Name", obj.Name);
            com.Parameters.AddWithValue("@PhoneNumber", obj.PhoneNumber);
            com.Parameters.AddWithValue("@Gender", obj.Gender);
            com.Parameters.AddWithValue("@Address", obj.Address);
            com.Parameters.AddWithValue("@Image", obj.ImageUrl);
            _conn.Open();
            com.ExecuteNonQuery();
            _conn.Close();
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<string> ImageUrlGenerate(IFormFile image,string PhoneNumber)
        {
            string filename = PhoneNumber + "_photo.jpg";
            string imagepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Photos", filename);

            if (System.IO.File.Exists(imagepath))
            {
                System.IO.File.Delete(imagepath);
            }

            using (var stream = new FileStream(imagepath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            string ImageUrl = $"{Request.Scheme}://{Request.Host}/Photos/{filename}";
            
            return ImageUrl;
        }
    }
}