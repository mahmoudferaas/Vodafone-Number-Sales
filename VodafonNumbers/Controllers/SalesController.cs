using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Vodafon.Models;
using VodafonNumbers.Models;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml;

namespace VodafonNumbers.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private VodafonEntities db = new VodafonEntities();
        private ApplicationDbContext DefualtDB = new ApplicationDbContext();

        // GET: Sales
        public ActionResult Index()
        {
             ViewBag.State = "Pending";
            List<SalesViewModel> sales = new List<SalesViewModel>();
            List<Sale> salesDB = new List<Sale>();
            string userId = User.Identity.GetUserId();
            var roles = DefualtDB.Roles.Where(a => a.Users.Any(x => x.UserId == userId)).ToList();
            if(roles.Count > 0)
            {
                Session["UserRole"] = roles[0].Name;
                //ViewBag.UserRole = roles[0].Name;

                if (roles[0].Name == "Agent")
                {
                    salesDB = db.Sales.ToList().Where(a => a.UserID == userId && a.StatusID == 4).OrderByDescending(s => s.ID).ToList();
                    foreach (var item in salesDB)
                    {
                        sales.Add(SalesViewModel.GetVMFromDB(item));
                    }

                }


                else
                {
                    salesDB = db.Sales.Where(a=> a.StatusID == 4).OrderByDescending(s => s.ID).ToList();
                    foreach (var item in salesDB)
                    {
                        sales.Add(SalesViewModel.GetVMFromDB(item));
                    }
                }
                          
            }

            var data = new[]{
                 new SelectListItem{ Value="1",Text="Serial"},
                 new SelectListItem{ Value="2",Text="ChoosenNo"},
                 new SelectListItem{ Value="3",Text="CustomerName"},
             };

            ViewBag.SearchTypes = data;

            return View(sales);
        }

        public ActionResult ActiveIndex()
        {
            ViewBag.State = "Active";
            List<Sale> salesDB = new List<Sale>();
            List<SalesViewModel> sales = new List<SalesViewModel>();
            string userId = User.Identity.GetUserId();
            var user = (new ApplicationDbContext()).Users.FirstOrDefault(s => s.Id == userId);
            Session["UserName"] = user.UserName;
            Session["UserId"] = user.Id;
            var roles = DefualtDB.Roles.Where(a => a.Users.Any(x => x.UserId == userId)).ToList();
            if (roles.Count > 0)
            {
                if (roles[0].Name == "Agent")
                {
                    salesDB = db.Sales.Where(a => a.UserID == userId && a.StatusID == 2).OrderByDescending(s => s.ID).ToList();
                    foreach (var item in salesDB)
                    {
                        sales.Add(SalesViewModel.GetVMFromDB(item));
                    }

                }


                else
                {
                    salesDB = db.Sales.Where(a => a.StatusID == 2).OrderByDescending(s => s.ID).ToList();
                    foreach (var item in salesDB)
                    {
                        sales.Add(SalesViewModel.GetVMFromDB(item));
                    }
                }
            }

            var data = new[]{
                 new SelectListItem{ Value="1",Text="Serial"},
                 new SelectListItem{ Value="2",Text="ChoosenNo"},
                 new SelectListItem{ Value="3",Text="CustomerName"},
             };

            ViewBag.SearchTypes = data;

            return View("Index" , sales);
        }

        public ActionResult FollowIndex()
        {
            ViewBag.State = "Follow";

            List<Sale> salesDB = new List<Sale>();
            List<SalesViewModel> sales = new List<SalesViewModel>();
            string userId = User.Identity.GetUserId();
            var roles = DefualtDB.Roles.Where(a => a.Users.Any(x => x.UserId == userId)).ToList();
            if (roles.Count > 0)
            {
                Session["UserRole"] = roles[0].Name;

                if (roles[0].Name == "Agent")
                {
                    salesDB = db.Sales.ToList().Where(a => a.UserID == userId && a.StatusID == 1).OrderByDescending(s => s.ID).ToList();
                    foreach (var item in salesDB)
                    {
                        sales.Add(SalesViewModel.GetVMFromDB(item));
                    }

                }


                else
                {
                    salesDB = db.Sales.Where(a => a.StatusID == 1).OrderByDescending(s => s.ID).ToList();
                    foreach (var item in salesDB)
                    {
                        sales.Add(SalesViewModel.GetVMFromDB(item));
                    }
                }
            }

            var data = new[]{
                 new SelectListItem{ Value="1",Text="Serial"},
                 new SelectListItem{ Value="2",Text="ChoosenNo"},
                 new SelectListItem{ Value="3",Text="CustomerName"},
             };

            ViewBag.SearchTypes = data;

            return View("Index", sales);
        }

        public ActionResult RejectIndex()
        {
            ViewBag.State = "Reject";

            List<Sale> salesDB = new List<Sale>();
            List<SalesViewModel> sales = new List<SalesViewModel>();
            string userId = User.Identity.GetUserId();
            var user = (new ApplicationDbContext()).Users.FirstOrDefault(s => s.Id == userId);
            Session["UserName"] = user.UserName;
            Session["UserId"] = user.Id;
            var roles = DefualtDB.Roles.Where(a => a.Users.Any(x => x.UserId == userId)).ToList();
            if (roles[0].Name == "Agent")
            {
                salesDB = db.Sales.ToList().Where(a => a.UserID == userId && a.StatusID == 3).OrderByDescending(s => s.ID).ToList();
                foreach (var item in salesDB)
                {
                    sales.Add(SalesViewModel.GetVMFromDB(item));
                }

            }


            else
            {
                salesDB = db.Sales.Where(a =>a.StatusID == 3).OrderByDescending(s => s.ID).ToList();
                foreach (var item in salesDB)
                {
                    sales.Add(SalesViewModel.GetVMFromDB(item));
                }
            }

            var data = new[]{
                 new SelectListItem{ Value="1",Text="Serial"},
                 new SelectListItem{ Value="2",Text="ChoosenNo"},
                 new SelectListItem{ Value="3",Text="CustomerName"},
             };

            ViewBag.SearchTypes = data;

            return View("Index", sales);
        }

        // GET: Sales/Create
        public ActionResult Create(int? numberID)
        {
            var Number = db.Numbers.Where(a => a.ID == numberID).FirstOrDefault();

            SalesViewModel _model = new SalesViewModel();

            List<Sale> SalesList = db.Sales.ToList();
            if (SalesList.Count == 0)
                _model.Serial = 1;

            else
                _model.Serial = SalesList.Max(m => m.Serial) + 1;
            _model.ChoosenNo = numberID;

            var ChoosenNumbers = db.Numbers.Where(a => a.ID == numberID).ToList();

            ViewBag.ChoosenNumbers = db.Numbers.Where(a=>a.IsChoosen != true && a.IsPrivate != true).Select(m => new SelectListItem
            {
                Value = m.ID.ToString(),
                Text = m.No,
            }).ToList();


            ViewBag.Packages = db.Packages.Select(m => new SelectListItem
            {
                Value = m.ID.ToString(),
                Text = m.PackageName,
            }).ToList();

            ViewBag.Status = db.Status.Select(m => new SelectListItem
            {
                Value = m.ID.ToString(),
                Text = m.Name,
            }).ToList();


            string userId = User.Identity.GetUserId();
            if (Number != null)
            {
                Number.IsChoosen = true;
                Number.UserID = userId;
                db.SaveChanges();
                //_model.ChoosenNo = Number.No;
            }

            return View(_model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesViewModel sale)
        {
            //for number
            var No = db.Numbers.Where(a => a.ID == sale.ChoosenNo).FirstOrDefault();
            if(No != null)
            {
                No.IsChoosen = true;
                db.SaveChanges();
            }
           
            string userId = User.Identity.GetUserId();
            var user = (new ApplicationDbContext()).Users.FirstOrDefault(s => s.Id == userId);
            Session["UserName"] = user.UserName;
            Session["UserId"] = user.Id;

            if (ModelState.IsValid)
            {
                Sale obj = new Sale();

                List<Sale> SalesList = db.Sales.ToList();
                if (SalesList.Count == 0)
                    obj.Serial = 1;

                else
                    obj.Serial = SalesList.Max(m => m.Serial) + 1;

                //obj.Serial = sale.Serial;
                obj.Address = sale.Address;
                obj.ChoosenNo = sale.ChoosenNo;
                obj.City = sale.City;
                obj.CreationDate = DateTime.Now;
                obj.CustomerID = sale.CustomerID;
                obj.CustomerName = sale.CustomerName;
                obj.District = sale.District;
                obj.Mobile1 = sale.Mobile1;
                obj.Mobile2 = sale.Mobile2;
                obj.Mobile2 = sale.Mobile2;
                obj.Package = sale.Package;
                obj.StatusID = sale.StatusID;
                obj.UserID = Session["UserId"].ToString();
                obj.UserName = Session["UserName"].ToString();
                obj.Comment = sale.Comment;
                obj.VisitDate = sale.VisitDate;

                if (sale.StatusID == 3)
                {
                    var Number = db.Numbers.Where(a => a.ID == sale.ChoosenNo).FirstOrDefault();
                    Number.IsChoosen = false;
                    obj.ChoosenNo = null;
                    obj.RejectDate = DateTime.Now;
                }

                if (sale.StatusID == 2)
                {
                    obj.ActivationDate = DateTime.Now;
                }

                if (obj.StatusID == null)
                    obj.StatusID = 4;

                db.Sales.Add(obj);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

           
            return View(sale);
        }

        // GET: Sales/Edit/5
        public ActionResult Edit(int? id)
        {
            string userId = User.Identity.GetUserId();
            var roles = DefualtDB.Roles.Where(a => a.Users.Any(x => x.UserId == userId)).ToList();
            if (roles.Count > 0)
            {
                ViewBag.UserRole = roles[0].Name;
                if(roles[0].Name == "Agent")
                {
                    ViewBag.Status = db.Status.Where(a=>a.ID == 1 || a.ID == 4).Select(m => new SelectListItem
                    {
                        Value = m.ID.ToString(),
                        Text = m.Name,
                    }).ToList();
                }
                else
                {
                    ViewBag.Status = db.Status.Select(m => new SelectListItem
                    {
                        Value = m.ID.ToString(),
                        Text = m.Name,
                    }).ToList();
                }
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sale saleDB = db.Sales.Where(a => a.ID == id).FirstOrDefault();

            SalesViewModel sale = SalesViewModel.GetVMFromDB(saleDB);

            if (sale == null)
            {
                return HttpNotFound();
            }

            ViewBag.ChoosenNumbers = db.Numbers.Where(a => a.IsChoosen != true && a.IsPrivate != true || a.ID == sale.ChoosenNo).Select(m => new SelectListItem
            {
                Value = m.ID.ToString(),
                Text = m.No,
            }).ToList();            

            ViewBag.Packages = db.Packages.Select(m => new SelectListItem
            {
                Value = m.ID.ToString(),
                Text = m.PackageName,
            }).ToList();

           

            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SalesViewModel sale)
        {
            if (ModelState.IsValid)
            {
                Sale obj = db.Sales.Where(a => a.ID == sale.ID).FirstOrDefault();
                obj.Serial = sale.Serial;
                obj.Address = sale.Address;
                obj.ChoosenNo = sale.ChoosenNo;
                obj.City = sale.City;
               // obj.CreationDate = DateTime.Now;
                obj.CustomerName = sale.CustomerName;
                obj.CustomerID = sale.CustomerID;
                obj.District = sale.District;
                obj.Mobile1 = sale.Mobile1;
                obj.Mobile2 = sale.Mobile2;
                obj.Mobile2 = sale.Mobile2;
                obj.Package = sale.Package;
                obj.StatusID = sale.StatusID;
                obj.Comment = sale.Comment;
                obj.VisitDate = sale.VisitDate;

                if (sale.StatusID == 3)
                {
                    var No = db.Numbers.Where(a => a.ID == sale.ChoosenNo).FirstOrDefault();
                    No.IsChoosen = false;
                    obj.ChoosenNo = null;
                    obj.RejectDate = DateTime.Now;
                }

                if (sale.StatusID == 2)
                {
                    obj.ActivationDate = DateTime.Now;
                }

                if (obj.StatusID == null)
                    obj.StatusID = 4;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sale);
        }

        // GET: Sales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sale sale = db.Sales.Find(id);
            db.Sales.Remove(sale);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public void ExportToExcel(List<SalesViewModel> sales)
        {
           
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Sr";
            ws.Cells["B1"].Value = "Req Date";
            ws.Cells["C1"].Value = "Red Number";
            ws.Cells["D1"].Value = "Red Plan";
            ws.Cells["E1"].Value = "Contact Number";
            ws.Cells["F1"].Value = "CustomerName";
            ws.Cells["G1"].Value = "District";
            ws.Cells["H1"].Value = "Address";
            ws.Cells["I1"].Value = "Gov";
            ws.Cells["J1"].Value = "#ID";
            ws.Cells["K1"].Value = "Visit Date";
            //ws.Cells["L1"].Value = "CustomerID";
            //ws.Cells["M1"].Value = "Comment";

            ws.Row(1).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);

            int rowStart = 2;
            foreach (var item in sales)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.Serial;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.CreationDateString;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.ChoosenNumber;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.PackageName;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Mobile1;
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.CustomerName;
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.Address;
                ws.Cells[string.Format("H{0}", rowStart)].Value = item.District;
                ws.Cells[string.Format("I{0}", rowStart)].Value = item.City;
                ws.Cells[string.Format("J{0}", rowStart)].Value = item.CustomerID;
               // ws.Cells[string.Format("J{0}", rowStart)].Value = item.StatusName;
                ws.Cells[string.Format("K{0}", rowStart)].Value = item.VisitDateString;
             //   ws.Cells[string.Format("L{0}", rowStart)].Value = item.CustomerID;
               // ws.Cells[string.Format("M{0}", rowStart)].Value = item.Comment;

                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }

        public ActionResult CheckSerialIsValid()
        {
            long validSerial = 0;

            List<Sale> SalesList = db.Sales.ToList();

            if (SalesList.Count == 0)
                validSerial = 1;

            else
                validSerial = SalesList.Max(m => m.Serial) + 1;

           
            return Json(new { validSerial = validSerial }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterData(int? SearchType, string SearchValue, string FromDateString, string ToDateString, int? StatusID, string ActivationFromDateString, string ActivationToDateString)
        {
            DateTime? FromDate = null, ToDate = null, ActivationFromDate = null, ActivationToDate = null;
            if (FromDateString != "")
                FromDate = Convert.ToDateTime(FromDateString);

            if (ToDateString != "")
            {
                ToDate = Convert.ToDateTime(ToDateString);
                ToDate = ToDate.Value.AddDays(1);
            }

            if (!string.IsNullOrEmpty(ActivationFromDateString) )
                ActivationFromDate = Convert.ToDateTime(ActivationFromDateString);

            if ( !string.IsNullOrEmpty(ActivationToDateString))
            {
                ActivationToDate = Convert.ToDateTime(ActivationToDateString);
                ActivationToDate = ActivationToDate.Value.AddDays(1);
            }

            List<SalesViewModel> sales = new List<SalesViewModel>();
            List<Sale> SalesDB = new List<Sale>();

            if (SearchType == 2)
            {
                SearchValue = GetNumberId(SearchValue).ToString();
            }

            string userId = User.Identity.GetUserId();
            var roles = DefualtDB.Roles.Where(a => a.Users.Any(x => x.UserId == userId)).ToList();
            if (roles.Count > 0)
            {
                if (roles[0].Name == "Agent")
                {
                    if (SearchType == 1)
                        SalesDB = db.Sales.ToList().Where(a => a.StatusID == StatusID && a.UserID == userId && ((a.Serial == long.Parse(SearchValue) || (SearchValue == null)) && ((a.CreationDate >= FromDate || (FromDate == null)) && (a.CreationDate <= ToDate || (ToDate == null))) && ((a.ActivationDate >= ActivationFromDate || (ActivationFromDate == null)) && (a.ActivationDate <= ActivationToDate || (ActivationToDate == null))) )).ToList();
                    if (SearchType == 2)
                        SalesDB = db.Sales.ToList().Where(a => a.StatusID == StatusID && a.UserID == userId && ((a.ChoosenNo == int.Parse(SearchValue) || (SearchValue == null)) && ((a.CreationDate >= FromDate || (FromDate == null)) && (a.CreationDate <= ToDate || (ToDate == null))) && ((a.ActivationDate >= ActivationFromDate || (ActivationFromDate == null)) && (a.ActivationDate <= ActivationToDate || (ActivationToDate == null))) )).ToList();
                    if (SearchType == 3)
                        SalesDB = db.Sales.ToList().Where(a => a.StatusID == StatusID && a.UserID == userId && (a.CustomerName != null)  && ( (a.CustomerName.Contains(SearchValue) || (SearchValue == null)) && ((a.CreationDate >= FromDate || (FromDate == null)) && (a.CreationDate <= ToDate || (ToDate == null))) && ((a.ActivationDate >= ActivationFromDate || (ActivationFromDate == null)) && (a.ActivationDate <= ActivationToDate || (ActivationToDate == null))) )).ToList();
                    if(SearchType == null)
                        SalesDB = db.Sales.ToList().Where(a => a.StatusID == StatusID && a.UserID == userId && ((a.CreationDate >= FromDate || (FromDate == null) && (a.CreationDate <= ToDate || (ToDate == null))) && ((a.ActivationDate >= ActivationFromDate || (ActivationFromDate == null)) && (a.ActivationDate <= ActivationToDate || (ActivationToDate == null))) )).ToList();


                }
                else
                {
                    if (SearchType == 1)
                        SalesDB = db.Sales.ToList().Where(a => a.StatusID == StatusID && ((a.Serial == long.Parse(SearchValue) || (SearchValue == null)) && ((a.CreationDate >= FromDate || (FromDate == null)) && (a.CreationDate <= ToDate || (ToDate == null))) && ((a.ActivationDate >= ActivationFromDate || (ActivationFromDate == null)) && (a.ActivationDate <= ActivationToDate || (ActivationToDate == null))))).ToList();
                    if (SearchType == 2)
                        SalesDB = db.Sales.ToList().Where(a => a.StatusID == StatusID && ((a.ChoosenNo == int.Parse(SearchValue) || (SearchValue == null)) && ((a.CreationDate >= FromDate || (FromDate == null)) && (a.CreationDate <= ToDate || (ToDate == null))) && ((a.ActivationDate >= ActivationFromDate || (ActivationFromDate == null)) && (a.ActivationDate <= ActivationToDate || (ActivationToDate == null))))).ToList();
                    if (SearchType == 3)
                        SalesDB = db.Sales.ToList().Where(a => a.StatusID == StatusID && (a.CustomerName != null)  && ( (a.CustomerName.Contains(SearchValue) || (SearchValue == null)) && ((a.CreationDate >= FromDate || (FromDate == null)) && (a.CreationDate <= ToDate || (ToDate == null))) && ((a.ActivationDate >= ActivationFromDate || (ActivationFromDate == null)) && (a.ActivationDate <= ActivationToDate || (ActivationToDate == null)))) ).ToList();
                    if(SearchType == null)
                        SalesDB = db.Sales.ToList().Where(a => a.StatusID == StatusID && ((a.CreationDate >= FromDate || (FromDate == null)) && (a.CreationDate <= ToDate || (ToDate == null)) && ((a.ActivationDate >= ActivationFromDate || (ActivationFromDate == null)) && (a.ActivationDate <= ActivationToDate || (ActivationToDate == null))))).ToList();

                }


                foreach (var item in SalesDB)
                {
                    sales.Add(SalesViewModel.GetVMFromDB(item));
                }
                return PartialView("_index", sales);
            }
            else
                return null;
        }
        public int? GetNumberId(string number)
        {
            var Number = db.Numbers.Where(a => a.No.Contains(number)).FirstOrDefault();
            if (Number != null)
                return Number.ID;

            return null;
        }

    }
}
