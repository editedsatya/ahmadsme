using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRS.Extensions;
using TRS.Models;

namespace TRS.Controllers
{

    [Authorize]
    public class CustomerController : Controller
    {
        private ApplicationDbContext dbContext;
        

        public CustomerController()
        {

            dbContext = new ApplicationDbContext();

        }



        // GET: Customer
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Index()
        {
            return View();
        }
       

        public ActionResult GetData()
        {
            
                List<Customer> customerList = dbContext.Customers.ToList<Customer>();
                return Json(new { data = customerList }, JsonRequestBehavior.AllowGet);
          
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Customer());
            else
            {
                return View(dbContext.Customers.Where(x => x.Id == id).FirstOrDefault<Customer>());
            }
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult AddOrEdit(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }
            else
            {
                if (customer.Id == 0)
                {
                    dbContext.Customers.Add(customer);
                    dbContext.SaveChanges();
                    //return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    dbContext.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                   // return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index", "Customer");
                }
            }
            
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(int id)
        {

            Customer customer = dbContext.Customers.Where(x => x.Id == id).FirstOrDefault<Customer>();
            dbContext.Customers.Remove(customer);
            dbContext.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }

        

        [Authorize(Roles = "Admin")]
        public ActionResult CustomerStructure()
        {
            var CustomerId = User.Identity.GetCustomerId();


                var plist = dbContext.CustomerStructures.Where(p => p.IsTID ==false && p.CustomerId == CustomerId).Select(a => new
                {
                    a.Id,
                    a.Name
                }).ToList();

                ViewBag.plist = plist;
         


            GetHierarchy();
            return View();
        }


        public JsonResult GetHierarchy()
        {
            List<CustomerStructure> hdList;
            List<HierarchyViewModel> records;

            var CustomerId = User.Identity.GetCustomerId();

            hdList = dbContext.CustomerStructures.Where(s => s.CustomerId == CustomerId).ToList();

                records = hdList.Where(l => l.PerentId == null)
                    .Select(l => new HierarchyViewModel
                    {
                        Id = l.Id,
                        text = l.Name,
                        perentId = l.PerentId,
                        children = GetChildren(hdList, l.Id)
                    }).ToList();
            

            return this.Json(records, JsonRequestBehavior.AllowGet);
            // return View();
        }

        private List<HierarchyViewModel> GetChildren(List<CustomerStructure> hdList, int parentId)
        {
            return hdList.Where(l => l.PerentId == parentId)
                .Select(l => new HierarchyViewModel
                {
                    Id = l.Id,
                    text = l.Name,
                    perentId = l.PerentId,
                    children = GetChildren(hdList, l.Id)
                }).ToList();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewNode(CustomerStructure model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var CustomerId = User.Identity.GetCustomerId();

                    CustomerStructure customerStructure = new CustomerStructure()
                    {
                        Id =( dbContext.CustomerStructures.Where(u=>u.CustomerId== CustomerId).Max(u => (int?) u.Id) ?? 0)+1,
                        CustomerId= CustomerId,
                        Name = model.Name,
                        PerentId = model.PerentId,
                        SeqNo=model.SeqNo,
                        IsTID=model.IsTID
                        
                    };

                        dbContext.CustomerStructures.Add(customerStructure);
                        dbContext.SaveChanges();
                
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DeleteNode(string values)
        {
            try
            {
                var CustomerId = User.Identity.GetCustomerId();

                //var id = values.Split(',');

                var idToDelete = values.Split(',');

                var itemsToDelete = (from item in dbContext.CustomerStructures
                                     where idToDelete.Contains(item.Id.ToString()) && item.CustomerId==CustomerId select item).ToList();

              
              
                  //  int ID = int.Parse(item);
                    dbContext.CustomerStructures.RemoveRange(itemsToDelete);
                    dbContext.SaveChanges();
          

             
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
                // return Json(new { success = false,Exception= ex }, JsonRequestBehavior.AllowGet);
            }

        }



    }
}