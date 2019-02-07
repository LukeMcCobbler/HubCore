using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HubCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace HubCore.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            IDbConnection cn = new SqlConnection();
            var myStuff = cn.Query<Product>(sql: "SELECT * FROM a");

            List<string> results = new List<string>();
            foreach (Product p in Product.GetProducts())
            {
                string name = p?.Name;
                decimal? price = p?.Price;
                results.Add(string.Format("Name: {0}, Price: {1}", name, price));
            }
            return View(results);
        }
    }
}