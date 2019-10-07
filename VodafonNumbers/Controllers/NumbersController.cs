using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Vodafon.Models;
using VodafonNumbers.Hubs;
using VodafonNumbers.Models;

namespace Vodafon.Controllers
{
    [Authorize]
    public class NumbersController : Controller
    {
        private VodafonEntities db = new VodafonEntities();
        private ApplicationDbContext DefualtDB = new ApplicationDbContext();

        //[Authorize]
        // GET: Numbers
        public ActionResult ImportNumbers()
        {

            return View("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {

            var Numsads = db.Numbers.Where(a => a.IsChoosen == true && a.Sales.Count == 0).ToList();
            for (int i = 0; i < Numsads.Count; i++)
            {
                Numsads[i].IsChoosen = false;
            }
            if (Numsads.Count > 0)
                db.SaveChanges();



            var Nums = db.Numbers.Where(a => a.IsChoosen != true && a.Sales.Count > 0).ToList();
            foreach (var item in Nums)
            {
                if (item.Sales.FirstOrDefault().StatusID != 3)
                    item.IsChoosen = true;
            }

            if (Nums.Count > 0)
                db.SaveChanges();

            //for zeros
            var Numers = db.Numbers.ToList();
            foreach (var item in Numers)
            {
                string num = item.No;
                if(num[0] != '0')
                {
                    item.No = "0" + item.No;
                }
            }
            db.SaveChanges();

            NumbersHub.Show();
            var Numbers = db.Numbers.Where(a => a.IsChoosen != true && a.IsPrivate != true).ToList();

            string userId = User.Identity.GetUserId();
            var user = (new ApplicationDbContext()).Users.FirstOrDefault(s => s.Id == userId);
            var roles = DefualtDB.Roles.Where(a => a.Users.Any(x => x.UserId == userId)).ToList();
            if (roles.Count > 0)
            {
                ViewBag.UserRole = roles[0].Name;
            }
            return View(Numbers);
        }

        public ActionResult PrivateNumbers()
        {
            var Numbers = db.Numbers.Select(a => new NumbersViewModel
            {
                ID = a.ID,
                IsChoosen = a.IsChoosen ?? false,
                IsPrivate = a.IsPrivate ?? false,
                No = a.No
            }).ToList();

            // _model.Data = Numbers;

            return View(Numbers);
        }

        public int? GetNumberId(string number)
        {
            var Number = db.Numbers.Where(a => a.No.Contains(number)).FirstOrDefault();
            if (Number != null)
                return Number.ID;

            return null;
        }



        [HttpPost]
        public ActionResult PrivateNumbers(List<NumbersViewModel> modelList)
        {
            foreach (var item in modelList)
            {
                Number number = db.Numbers.Where(a => a.ID == item.ID).FirstOrDefault();
                number.IsChoosen = item.IsChoosen;
                number.IsPrivate = item.IsPrivate;
            }
            db.SaveChanges();

            return RedirectToAction("Create");
        }

        public ActionResult Choose(int Id)
        {
            string userId = User.Identity.GetUserId();
            var No = db.Numbers.Where(a => a.ID == Id).FirstOrDefault();
            if (No != null)
            {
                No.IsChoosen = true;
                No.UserID = userId;
                db.SaveChanges();
            }
            return RedirectToAction("Create", "Sales", new { numberID = No.ID });
        }

        public ActionResult UnChoose(int Id)
        {
            var No = db.Numbers.Where(a => a.ID == Id).FirstOrDefault();
            No.IsChoosen = false;
            db.SaveChanges();

            //NumbersHub.Show();
            //var Numbers = db.Numbers.Where(a => a.IsChoosen != true && a.IsPrivate != true).ToList();
            return View();
        }

        public ActionResult RemoveAll()
        {
            var No = db.Numbers.Where(a => a.Sales.Count == 0).ToList();
            db.Numbers.RemoveRange(No);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        public JsonResult Get()
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["VodafonConnection"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [ID],[No],[IsChoosen] FROM [dbo].[Numbers] WHERE [IsChoosen] <> 1 and [IsPrivate] <> 1", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    var listCus = reader.Cast<IDataRecord>()
                            .Select(x => new
                            {
                                Id = (int)x["ID"],
                                No = (string)x["No"],
                                IsChoosen = (bool)x["IsChoosen"],
                            }).ToList();

                    return Json(new { listCus = listCus }, JsonRequestBehavior.AllowGet);

                }
            }
        }


        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            NumbersHub.Show();
        }


        public ActionResult Upload(FormCollection formCollection)
        {
            var numberList = new List<Number>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
                        {
                            var number = new Number();
                            number.No = workSheet.Cells[rowIterator, 1].Value.ToString();
                            number.IsChoosen = false;
                            number.IsPrivate = false;
                            numberList.Add(number);
                        }
                    }
                }
            }

            foreach (var item in numberList)
            {
                if(item.No != "Number")
                {
                    var num = db.Numbers.Where(a => a.No.Contains(item.No)).FirstOrDefault();
                    if (num == null)
                        db.Numbers.Add(item);
                }
               
            }

            db.SaveChanges();

            return RedirectToAction("Create");
        }


        public ActionResult CheckNumberIsValid(int? NumberId)
        {
            bool valid = true;
            string userId = User.Identity.GetUserId();
            var No = db.Numbers.Where(a => a.ID == NumberId).FirstOrDefault();
            if (No != null)
            {
                if (No.UserID != userId)
                    valid = !(No.IsChoosen) ?? true;
            }
            return Json(new { valid = valid }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportNumbers()
        {
            var Numbers = db.Numbers.Select(a => new NumbersViewModel
            {
                ID = a.ID,
                IsChoosen = a.IsChoosen ?? false,
                IsPrivate = a.IsPrivate ?? false,
                No = a.No
            }).ToList();

            return View("ExportPrivateNumbers", Numbers);
        }
        public ActionResult GetPrivateOnly(bool? privateOnly, bool? publicOnly)
        {
            List<NumbersViewModel> Numbers = new List<NumbersViewModel>();

            if (privateOnly == true)
            {
                Numbers = db.Numbers.Where(a => a.IsPrivate == true).Select(a => new NumbersViewModel
                {
                    ID = a.ID,
                    IsChoosen = a.IsChoosen ?? false,
                    IsPrivate = a.IsPrivate ?? false,
                    No = a.No
                }).ToList();
            }

            else if (publicOnly == true)
            {
                Numbers = db.Numbers.Where(a => a.IsPrivate != true && a.IsChoosen != true).Select(a => new NumbersViewModel
                {
                    ID = a.ID,
                    IsChoosen = a.IsChoosen ?? false,
                    IsPrivate = a.IsPrivate ?? false,
                    No = a.No
                }).ToList();
            }

            else
            {
                Numbers = db.Numbers.Select(a => new NumbersViewModel
                {
                    ID = a.ID,
                    IsChoosen = a.IsChoosen ?? false,
                    IsPrivate = a.IsPrivate ?? false,
                    No = a.No
                }).ToList();
            }

            return PartialView("_ExportPrivateNumbers", Numbers);
        }

        public void ExportToExcel(List<NumbersViewModel> Numbers)
        {

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Number";

            ws.Row(1).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);

            int rowStart = 2;
            foreach (var item in Numbers)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.No;
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }

    }
}
