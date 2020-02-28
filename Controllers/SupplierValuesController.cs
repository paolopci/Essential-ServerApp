using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using ServerApp.Models.BindingTargets;

namespace ServerApp.Controllers
{
    [Route("api/suppliers")]
    [ApiController]
    public class SupplierValuesController : Controller
    {
        private DataContext context;

        public SupplierValuesController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IEnumerable<Supplier> GetSupplier()
        {
            return context.Suppliers;
        }

        [HttpPost]
        public IActionResult CreateSupplier([FromBody] SupplierData sData)
        {
            if (ModelState.IsValid)
            {
                Supplier supp = sData.Supplier;
                context.Add(supp);
                context.SaveChanges();
                return Ok(supp.SupplierId);

            }

            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceSupplier(long id, [FromBody] SupplierData sData)
        {
            if (ModelState.IsValid)
            {
                Supplier supp = sData.Supplier;
                supp.SupplierId = id;
                context.Update(supp);
                context.SaveChanges();
                return Ok();
            }

            return BadRequest(ModelState);
        }

    [HttpDelete("{id}")]
    public void DeleteSupplier(long id)
    {
      context.Suppliers.Remove(new Supplier { SupplierId = id });
      context.SaveChanges();
    }
  }
}