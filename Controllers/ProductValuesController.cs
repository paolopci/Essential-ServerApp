using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductValuesController : Controller
    {
        private DataContext context;

        public ProductValuesController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet("{id}")]
        public Product GetProduct(long id)
        {
            System.Threading.Thread.Sleep(5000);
            return context.Products.Find(id);
        }
    }
}