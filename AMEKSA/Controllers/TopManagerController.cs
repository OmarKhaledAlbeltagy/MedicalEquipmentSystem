using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Privilage;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AMEKSA.CustomEntities;
using ClosedXML.Excel;
using MoreLinq;
using System.IO;
using AMEKSA.Models;
namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class TopManagerController : ControllerBase
    {
        private readonly ITopManagerRep topManagerRep;
        private readonly IFirstManagerRep firstManagerRep;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public TopManagerController(ITopManagerRep topManagerRep,IFirstManagerRep firstManagerRep, UserManager<ExtendIdentityUser> userManager)
        {
            this.topManagerRep = topManagerRep;
            this.firstManagerRep = firstManagerRep;
            this.userManager = userManager;
        }



        //MorrisLine
        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult MorrisLine()
        {
            return Ok(topManagerRep.MorrisLine());
        }

        [Route("[controller]/[Action]/{year}/{month}")]
        [HttpGet]
        public IActionResult GetVisitsCountReyadByMonth(int year,int month)
        {

            return Ok(topManagerRep.GetVisitsCountReyadByMonth(year,month));
        }

        [Route("[controller]/[Action]/{year}/{month}")]
        [HttpGet]
        public IActionResult GetVisitsCountDamamByMonth(int year, int month)
        {

            return Ok(topManagerRep.GetVisitsCountDamamByMonth(year, month));
        }

        [Route("[controller]/[Action]/{year}/{month}")]
        [HttpGet]
        public IActionResult GetVisitsCountJeddahByMonth(int year, int month)
        {

            return Ok(topManagerRep.GetVisitsCountJeddahByMonth(year, month));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetVisitsCountThisMonthReyad()
        {

            return Ok(topManagerRep.GetVisitsCountThisMonthReyad());
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetTarget(SearchTargetModel obj)
        {

            return Ok(topManagerRep.GetTarget(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetVisitsCountThisMonthJeddah()
        {

            return Ok(topManagerRep.GetVisitsCountThisMonthJeddah());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetVisitsCountThisMonthDamam()
        {

            return Ok(topManagerRep.GetVisitsCountThisMonthDamam());
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult AccountThisMonthPercentage()
        {

            return Ok(topManagerRep.AccountThisMonthPercentage());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult ContactThisMonthPercentage()
        {
            return Ok(topManagerRep.ContactThisMonthPercentage());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult AccountpastMonthPercentage()
        {
            return Ok(topManagerRep.AccountpastMonthPercentage());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult ContactPastMonthPercentage()
        {
            return Ok(topManagerRep.ContactPastMonthPercentage());
        }
        
        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllFirstManagers()
        {
            return Ok(topManagerRep.GetAllFirstManagers());
        }

        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountMedicalVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = "";
            if (userid == "0")
            {
                name = "AME";
            }
            else
            {
                name = userManager.FindByIdAsync(userid).Result.FullName;
            }
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomAccountMedicalVisit> vis = topManagerRep.GetDetailedAMV(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Account";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Category";
            worksheet.Cell(currentRow, 5).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 7).Value = "Visit Notes";
            worksheet.Cell(currentRow, 8).Value = "Additional Notes";
            worksheet.Cell(currentRow, 9).Value = "Products";
            worksheet.Cell(currentRow, 10).Value = "Persons Met";


            currentRow = 2;
            foreach (var viss in vis)
            {

                IEnumerable<CustomAccountMedicalVisitProducts> productss = viss.product.DistinctBy(a => a.ProductName);
                IEnumerable<CustomVisitPerson> personss = viss.person;
                worksheet.Cell(currentRow, 1).Value = viss.UserName;
                worksheet.Cell(currentRow, 2).Value = viss.AccountName;
                worksheet.Cell(currentRow, 3).Value = viss.AccountTypeName;
                worksheet.Cell(currentRow, 4).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 5).Value = date + " " + time;

                DateTime sub = viss.SubmittingDate;
                worksheet.Cell(currentRow, 6).Value = sub;

                worksheet.Cell(currentRow, 7).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 8).Value = viss.AdditionalNotes;

                string productstring = "";

                foreach (var item in productss)
                {
                    productstring = productstring+ item.ProductName + " - ";
                }

                if (productstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = productstring.Remove(productstring.Length - 3);
                }
                string personstring = "";
                foreach (var item in personss)
                {
                    personstring = personstring + "( " + item.PersonName + " - " + item.PersonPosition + " ) | ";
                }

                if (personstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 10).Value = personstring.Remove(personstring.Length - 3);
                }
                currentRow++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s team visits to accounts from " + startdate + " to " + enddate + ".xlsx");




        }

        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountSalesVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = "";
            if (userid == "0")
            {
                name = "AME";
            }
            else
            {
                name = userManager.FindByIdAsync(userid).Result.FullName;
            }
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomAccountSalesVisit> vis = topManagerRep.GetDetailedASV(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Account";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Category";
            worksheet.Cell(currentRow, 5).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 7).Value = "Visit Notes";
            worksheet.Cell(currentRow, 8).Value = "Payment Notes";
            worksheet.Cell(currentRow, 9).Value = "Brand";
            worksheet.Cell(currentRow, 10).Value = "Persons Met";


            currentRow = 2;
            foreach (var viss in vis)
            {

                IEnumerable<CustomVisitBrand> brandss = viss.brands.DistinctBy(a => a.BrandName);
                IEnumerable<CustomVisitPerson> personss = viss.persons;
                worksheet.Cell(currentRow, 1).Value = viss.UserName;
                worksheet.Cell(currentRow, 2).Value = viss.AccountName;
                worksheet.Cell(currentRow, 3).Value = viss.AccountTypeName;
                worksheet.Cell(currentRow, 4).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 5).Value = date + " " + time;

                DateTime subdate = viss.SubmittingDate;




                worksheet.Cell(currentRow, 6).Value = subdate;


                worksheet.Cell(currentRow, 7).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 8).Value = viss.PaymentNotes;

                string brandstring = "";

                foreach (var item in brandss)
                {
                    brandstring = brandstring + item.BrandName + " - ";
                }

                if (brandstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = brandstring.Remove(brandstring.Length - 3);
                }
                string personstring = "";
                foreach (var item in personss)
                {
                    personstring = personstring + "( " + item.PersonName + " - " + item.PersonPosition + " ) | ";
                }

                if (personstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 10).Value = personstring.Remove(personstring.Length - 3);
                }
                currentRow++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s team visits to accounts from " + startdate + " to " + enddate + ".xlsx");




        }

        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult ContactMedicalVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = "";
            if (userid == "0")
            {
                name = "AME";
            }
            else
            {
                name = userManager.FindByIdAsync(userid).Result.FullName;
            }
            
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomContactMedicalVisit> vis = topManagerRep.GetDetailedCMV(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Contact";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Category";
            worksheet.Cell(currentRow, 5).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 7).Value = "Visit Notes";
            worksheet.Cell(currentRow, 8).Value = "Requests";
            worksheet.Cell(currentRow, 9).Value = "Products";
            worksheet.Cell(currentRow, 10).Value = "Sales Aids";


            currentRow = 2;
            foreach (var viss in vis)
            {
                IEnumerable<CustomContactMedicalVisitProducts> productss = viss.customcontactmedicalvisitproduct.DistinctBy(a => a.ProductName);
                IEnumerable<CustomContactSalesAid> salesaids = viss.customcontactsalesaid;
                worksheet.Cell(currentRow, 1).Value = viss.UserName;
                worksheet.Cell(currentRow, 2).Value = viss.ContactName;
                worksheet.Cell(currentRow, 3).Value = viss.ContactTypeName;
                worksheet.Cell(currentRow, 4).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 5).Value = date + " " + time;
                DateTime sub = viss.SubmittingDate;

                worksheet.Cell(currentRow, 6).Value = sub;
                worksheet.Cell(currentRow, 7).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 8).Value = viss.Requests;

                string productstring = "";

                foreach (var item in productss)
                {
                    productstring = productstring + "( Product: " + item.ProductName + " - Product Share: " + item.ProductShare + " ) | ";
                }
                if (productstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = productstring.Remove(productstring.Length - 3);
                }


                string salesaidsstring = "";
                foreach (var item in salesaids)
                {
                    salesaidsstring = salesaidsstring + "( " + item.SalesAidName + " ) | ";
                }

                if (salesaidsstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 10).Value = salesaidsstring.Remove(salesaidsstring.Length - 3);
                }
                currentRow++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s team visits to contacts from " + startdate + " to " + enddate + ".xlsx");




        }




        [Route("[controller]/[Action]/{ManagerId}/{CategoryId}/{Month}/{Year}")]
        [HttpGet]
        public IActionResult GetTargetExcel(string ManagerId,int CategoryId,int Month,int Year)
        {
            string name = userManager.FindByIdAsync(ManagerId).Result.FullName;
            string month = new DateTime(Year, Month, 1).ToString("MMMM");
            
            SearchTargetModel obj = new SearchTargetModel();
            obj.CategoryId = CategoryId;
            obj.ManagerId = ManagerId;
            obj.Month = Month;
            obj.year = Year;
            IEnumerable<CustomTarget> vis = topManagerRep.GetTarget(obj);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Target");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Contact";
            worksheet.Cell(currentRow, 3).Value = "Category";
            worksheet.Cell(currentRow, 4).Value = "Current Visits";
            worksheet.Cell(currentRow, 5).Value = "Monthly Target";
        


            currentRow = 2;
            foreach (var viss in vis)
            {
          
                worksheet.Cell(currentRow, 1).Value = viss.FullName;
                worksheet.Cell(currentRow, 2).Value = viss.ContactName;
                worksheet.Cell(currentRow, 3).Value = viss.CategoryName;
                worksheet.Cell(currentRow, 4).Value = viss.CurrentVisits;
                if (viss.MonthlyTarget == null)
                {
                    viss.MonthlyTarget = 0;
                }
                worksheet.Cell(currentRow, 5).Value = viss.MonthlyTarget;

              

                if (viss.CurrentVisits == 0 || viss.CurrentVisits < viss.MonthlyTarget)
                {
                    worksheet.Cell(currentRow, 4).Style.Font.FontColor = XLColor.Red;
                }
                else
                {
                    if (viss.CurrentVisits >= viss.MonthlyTarget)
                    {
                        worksheet.Cell(currentRow, 4).Style.Font.FontColor = XLColor.Green;
                    }
                }

                currentRow++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s team Target Reprot on " + month + ".xlsx");




        }


    }
}
