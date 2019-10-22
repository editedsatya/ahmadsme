using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TRS.Models;
using TRS.Extensions;

namespace TRS.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : BaseController
    {
        Models.DataAccessLayer db = new Models.DataAccessLayer();
        private ApplicationDbContext _context;


        public AdminController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }



        // GET: Admin

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult GetData()
        {
            List<ApplicationUser> userList = null;


            if (User.IsInRole(RoleName.SuperAdmin))
            {
                userList = _context.Users.ToList();
            }
            else
            {
                var CustomerId = User.Identity.GetCustomerId();
                userList = _context.Users.Where(x => x.CustomerId == CustomerId).ToList();
            }


          



            return Json(new { data = userList }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public async Task<ActionResult> AddOrEdit(string id = null)
        {

            var CustomerId = User.Identity.GetCustomerId();

            

            IEnumerable<Customer> Customers = null;

            if (User.IsInRole(RoleName.SuperAdmin))
                Customers = _context.Customers.ToList();
            else
                Customers = _context.Customers.Where(x => x.Id == CustomerId).ToList();



            var viewModel = new RegisterViewModel();

            if (string.IsNullOrEmpty(id))
            {

                viewModel.Customers = Customers;


                if (User.IsInRole(RoleName.SuperAdmin))
                {
                    ViewBag.Roles = _context.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Name
                    });
                }
                else
                {
                    ViewBag.Roles = _context.Roles.Where(x => x.Name.Equals(RoleName.SuperAdmin) == false).ToList().Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Name
                    });
                }

            }
            else
            {

                var userInDb = _context.Users.SingleOrDefault(c => c.Id == id);
                if (userInDb == null)
                    return HttpNotFound();

                viewModel = new RegisterViewModel
                {

                    Customers = Customers,
                    Id = userInDb.Id,
                    NameIdentifier= userInDb.NameIdentifier,
                    Email = userInDb.Email,
                    CustomerId = userInDb.CustomerId,
                    PhoneNumber = userInDb.PhoneNumber,
                    Password = null
                };





                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var userRoles = await UserManager.GetRolesAsync(id);

                if (User.IsInRole(RoleName.SuperAdmin))
                {
                    ViewBag.Roles = _context.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    });
                }
                else
                {
                    ViewBag.Roles = _context.Roles.Where(x => x.Name.Equals(RoleName.SuperAdmin) == false).ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    });
                }


            }



            return View(viewModel);

        }

        [HttpPost]
        public async Task<ActionResult> AddOrEdit(RegisterViewModel viewModel, params string[] selectedRole)
        {
            IdentityResult result = null;

            if (!ModelState.IsValid)
            {

                var CustomerId = User.Identity.GetCustomerId();


                IEnumerable<Customer> Customers = null;

                if (User.IsInRole(RoleName.SuperAdmin))
                    Customers = _context.Customers.ToList();
                else
                    Customers = _context.Customers.Where(x => x.Id == CustomerId).ToList();

                viewModel.Customers = Customers;


                if (User.IsInRole(RoleName.SuperAdmin))
                {
                    ViewBag.Roles = _context.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = (selectedRole == null ? false : selectedRole.Contains(x.Name)),
                        Text = x.Name,
                        Value = x.Name
                    });
                }
                else
                {
                    ViewBag.Roles = _context.Roles.Where(x => x.Name.Equals(RoleName.SuperAdmin) == false).ToList().Select(x => new SelectListItem()
                    {
                        Selected = (selectedRole == null ? false : selectedRole.Contains(x.Name)),
                        Text = x.Name,
                        Value = x.Name
                    });
                }

                return View(viewModel);
            }

            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            if (string.IsNullOrEmpty(viewModel.Id))
            {
                var user = new ApplicationUser { UserName = viewModel.Email, Email = viewModel.Email, NameIdentifier = viewModel.NameIdentifier, CustomerId = viewModel.CustomerId, PhoneNumber = viewModel.PhoneNumber };
                result = await UserManager.CreateAsync(user, viewModel.Password);
                if (selectedRole != null && result.Succeeded)
                {
                    result = await UserManager.AddToRolesAsync(user.Id, selectedRole.ToArray<string>());
                }

            }
            else
            {
                var currentUser = UserManager.FindByEmail(viewModel.Email);
                var userRoles = await UserManager.GetRolesAsync(currentUser.Id);



                //update user information 
                currentUser.CustomerId = viewModel.CustomerId;
                currentUser.PhoneNumber = viewModel.PhoneNumber;
                result = await UserManager.UpdateAsync(currentUser);
                if (!result.Succeeded)
                    goto error;


                //change user Password
                string code = await UserManager.GeneratePasswordResetTokenAsync(currentUser.Id);
                result = await UserManager.ResetPasswordAsync(currentUser.Id, code, viewModel.Password);
                if (!result.Succeeded)
                    goto error;

                //update user roles

                result = await UserManager.AddToRolesAsync(currentUser.Id, selectedRole.Except(userRoles).ToArray<string>());
                result = await UserManager.RemoveFromRolesAsync(currentUser.Id, userRoles.Except(selectedRole).ToArray<string>());

            }




        error:

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", string.Join(",", result.Errors));

                var CustomerId = User.Identity.GetCustomerId();


                IEnumerable<Customer> Customers = null;

                if (User.IsInRole(RoleName.SuperAdmin))
                    Customers = _context.Customers.ToList();
                else
                    Customers = _context.Customers.Where(x => x.Id == CustomerId).ToList();

                viewModel.Customers = Customers;


                if (User.IsInRole(RoleName.SuperAdmin))
                {
                    ViewBag.Roles = _context.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = (selectedRole == null ? false : selectedRole.Contains(x.Name)),
                        Text = x.Name,
                        Value = x.Name
                    });
                }
                else
                {
                    ViewBag.Roles = _context.Roles.Where(x => x.Name.Equals(RoleName.SuperAdmin) == false).ToList().Select(x => new SelectListItem()
                    {
                        Selected = (selectedRole == null ? false : selectedRole.Contains(x.Name)),
                        Text = x.Name,
                        Value = x.Name
                    });
                }

                return View(viewModel);

            }


            return RedirectToAction("Index", "Admin");

        }



        [HttpPost]
        public ActionResult Delete(string id)
        {

            var UserInDb = _context.Users.SingleOrDefault(c => c.Id == id);

            if (UserInDb == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);

            _context.Users.Remove(UserInDb);
            _context.SaveChanges();

            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }
        

        [HttpGet]
        public ActionResult AssignCustomerStructureToUser(string id = null)
        {
            var userInDb = _context.Users.SingleOrDefault(c => c.Id == id);


            ViewBag.UserId = userInDb.Id;
            ViewBag.UserName = userInDb.UserName;


            return View();
            //return PartialView("AssignCustomerStructureToUserPartial");


        }


        public JsonResult LoadCustomerStructure(string UserId = null)
        {

            List<HierarchyViewModel> records;

            List<SqlParameter> sqlParams = new List<SqlParameter>();

            var userInDb = _context.Users.SingleOrDefault(c => c.Id == UserId);


            sqlParams.Add(new SqlParameter("@CustomerId", userInDb.CustomerId));
            sqlParams.Add(new SqlParameter("@UserId", userInDb.Id));

            DataTable objDt = db.populate("Select A.*,CONVERT(BIT, CASE WHEN NodeId IS NULL THEN 0 ELSE 1 END) as chk  from CustomerStructures A left join AssignCustomerStructureToUser B on A.CustomerId = B.CustomerId and A.Id = B.NodeId and UserId=@UserId where A.CustomerId=@CustomerId ", sqlParams.ToArray());
            objDt.TableName = "myTable";


            records = objDt.AsEnumerable().Where(l => l["PerentId"] == System.DBNull.Value)
                  .Select(l => new HierarchyViewModel
                  {
                      Id = l.Field<System.Int32>("Id"),
                      text = l.Field<System.String>("Name"),
                      perentId = l.Field<System.Int32?>("PerentId"),
                      @checked = l.Field<System.Boolean>("chk"),
                      children = GetChildren(objDt, l.Field<System.Int32>("Id"))
                  }).ToList();





            return this.Json(records, JsonRequestBehavior.AllowGet);
            // return View();
        }

        private List<HierarchyViewModel> GetChildren(DataTable objDt, int parentId)
        {
            return objDt.AsEnumerable().Where(l => l.Field<System.Int32?>("PerentId") == parentId)
                .Select(l => new HierarchyViewModel
                {
                    Id = l.Field<System.Int32>("Id"),
                    text = l.Field<System.String>("Name"),
                    perentId = l.Field<System.Int32>("PerentId"),
                    @checked = l.Field<System.Boolean>("chk"),
                    children = GetChildren(objDt, l.Field<System.Int32>("Id"))
                }).ToList();

        }

        [HttpPost]
        public JsonResult AssignCustomerStructureToUser(List<int> checkedIds, string UserId)
        {
            if (checkedIds == null)
            {
                checkedIds = new List<int>();
            }
            var userInDb = _context.Users.SingleOrDefault(c => c.Id == UserId);


            _context.Database.ExecuteSqlCommand("delete from AssignCustomerStructureToUser where UserId=N'" + userInDb.Id + "' and customerId=N'" + userInDb.CustomerId + "' ");
            _context.Database.ExecuteSqlCommand("insert into AssignCustomerStructureToUser (UserId,customerId,NodeId) SELECT '" + userInDb.Id + "' as UserId ,'" + userInDb.CustomerId + "' as CustomerId,[value]  As NodeId FROM STRING_SPLIT('" + string.Join(",", checkedIds.ToArray()) + "', ',') ");





            return this.Json(true);
        }



    }
}