using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VodafonNumbers.Models;

namespace Vodafon.Models
{
    public class SalesViewModel
    {
        public int ID { get; set; }
        public long Serial { get; set; }
        public string CustomerName { get; set; }
       // [Required]
        public Nullable<int> ChoosenNo { get; set; }
        public string ChoosenNumber { get; set; }
       // [Required]
        public Nullable<int> Package { get; set; }
        public string PackageName { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string StatusName { get; set; }
        public int? StatusID { get; set; }
        public string CustomerID { get; set; }
        public string Comment { get; set; }

        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> VisitDate { get; set; }
        public string CreationDateString { get; set; }
        public string VisitDateString { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public string ActivationDateString { get; set; }

        public static SalesViewModel GetVMFromDB(Sale _obj)
        {
            SalesViewModel model = new SalesViewModel();
            if (_obj != null)
            {
                model.ID = _obj.ID;
                model.Serial = _obj.Serial;
                model.CustomerName = _obj.CustomerName;
                model.CustomerID = _obj.CustomerID;
                model.ChoosenNo = _obj.ChoosenNo;
                model.ChoosenNumber = _obj.Number != null ? _obj.Number.No : "";;
                model.Package = _obj.Package;
                model.PackageName = _obj.Package != null ? _obj.Package1.PackageName : "";
                model.Mobile1 = _obj.Mobile1;
                model.Mobile2 = _obj.Mobile2;
                model.Address = _obj.Address;
                model.District = _obj.District;
                model.City = _obj.City;
                model.StatusID = _obj.StatusID;
                model.UserID = _obj.UserID;
                model.StatusName = _obj.Status != null ? _obj.Status.Name : "";
                model.UserName = _obj.UserName;
                model.Comment = _obj.Comment;
                model.CreationDate = _obj.CreationDate;
                model.CreationDateString = _obj.CreationDate != null ? _obj.CreationDate.Value.ToString("dd/MM/yyyy") : "";
                model.VisitDate = _obj.VisitDate;
                model.VisitDateString = _obj.VisitDate != null ? _obj.VisitDate.Value.ToString("dd/MM/yyyy") : "";
                model.ActivationDate = _obj.ActivationDate;
                model.ActivationDateString = _obj.ActivationDate != null ? _obj.ActivationDate.Value.ToString("dd/MM/yyyy") : "";
            }
               
            return model;
        }

        public static Sale GetDBFromVM(SalesViewModel sale)
        {
            Sale obj = new Sale();
            if (sale != null)
            {
                obj.Serial = sale.Serial;
                obj.Address = sale.Address;
                obj.ChoosenNo = sale.ChoosenNo;
                obj.City = sale.City;
                obj.CreationDate = DateTime.Now;
                obj.CustomerName = sale.CustomerName;
                obj.CustomerID = sale.CustomerID;
                obj.District = sale.District;
                obj.Mobile1 = sale.Mobile1;
                obj.Mobile2 = sale.Mobile2;
                obj.Mobile2 = sale.Mobile2;
                obj.Package = sale.Package;
                obj.StatusID = sale.StatusID;
                obj.Comment = sale.Comment;
                obj.ActivationDate = sale.ActivationDate;
            }           

            return obj;
        }

    }
}