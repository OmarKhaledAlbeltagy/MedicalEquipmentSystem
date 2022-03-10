using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AMEKSA.FirstManagerModels;
using ClosedXML.Excel;
using System.IO;
using AMEKSA.CustomEntities;
using MoreLinq;
using Microsoft.AspNetCore.Identity;
using AMEKSA.Privilage;
using AMEKSA.Models;
using ClosedXML.Attributes;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class FirstManagerController : ControllerBase
    {
        private readonly IFirstManagerRep rep;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITopManagerRep topManagerRep;
        private readonly IVisitsCollectionRep colRep;

        public FirstManagerController(IFirstManagerRep rep, UserManager<ExtendIdentityUser> userManager,ITopManagerRep topManagerRep, IVisitsCollectionRep colRep)
        {
            this.rep = rep;
            this.userManager = userManager;
            this.topManagerRep = topManagerRep;
            this.colRep = colRep;
        }

        [Route("[controller]/[Action]/{year}/{month}/{myId}")]
        [HttpGet]
        public IActionResult GetVisitsCountByMonth(int year, int month, string myId)
        {
            return Ok(rep.GetVisitsCountByMonth(year,month,myId));
        }



   

        [Route("[controller]/[Action]/{myId}")]
        [HttpGet]
        public IActionResult AccountThisMonthPercentage(string myId)
        {

            return Ok(rep.AccountThisMonthPercentage(myId));
        }

        [Route("[controller]/[Action]/{myId}")]
        [HttpGet]
        public IActionResult ContactThisMonthPercentage(string myId)
        {

            return Ok(rep.ContactThisMonthPercentage(myId));
        }

        [Route("[controller]/[Action]/{myId}")]
        [HttpGet]
        public IActionResult AccountpastMonthPercentage(string myId)
        {
            return Ok(rep.AccountpastMonthPercentage(myId));
        }

        [Route("[controller]/[Action]/{myId}")]
        [HttpGet]
        public IActionResult ContactPastMonthPercentage(string myId)
        {
            return Ok(rep.ContactPastMonthPercentage(myId));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetMyTeamAMVisits(VisitsSearchModel obj)
        {

            return Ok(rep.GetMyTeamAMVisits(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetMyTeamASVisits(VisitsSearchModel obj)
        {

            return Ok(rep.GetMyTeamASVisits(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetMyTeamCMVisits(VisitsSearchModel obj)
        {

            return Ok(rep.GetMyTeamCMVisits(obj));
        }

        [Route("[controller]/[Action]/{managerid}")]
        [HttpGet]
        public IActionResult GetMyTeamMedical(string managerid)
        {

            return Ok(rep.GetMyTeamMedical(managerid));
        }

        [Route("[controller]/[Action]/{managerid}")]
        [HttpGet]
        public IActionResult GetMyTeamSales(string managerid)
        {

            return Ok(rep.GetMyTeamSales(managerid));
        }

        [Route("[controller]/[Action]/{managerid}")]
        [HttpGet]
        public IActionResult GetMyTeamRequests(string managerid)
        {
            return Ok(rep.GetMyTeamRequests(managerid));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeclineRequest(int id)
        {
            return Ok(rep.DeclineRequest(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult AcceptRequest(int id)
        {
            return Ok(rep.AcceptRequest(id));
        }

        
        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetAccountSalesVisitsByUserId(string id)
        {
            return Ok(rep.GetAccountSalesVisitsByUserId(id));
        }





        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountSalesVisitExcel(string userid,int dayfrom,int monthfrom, int yearfrom, int dayto, int monthto,int yearto)
        {
            string name = userManager.FindByIdAsync(userid).Result.FullName;
            string startdate = new DateTime(yearfrom,monthfrom,dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomAccountSalesVisit> vis = rep.GetASDetailedVisits(userid,dayfrom,monthfrom,yearfrom,dayto,monthto,yearto);

            XLWorkbook workbook = new XLWorkbook();


            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account";
            worksheet.Cell(currentRow, 2).Value = "Type";
            worksheet.Cell(currentRow, 3).Value = "Category";
            worksheet.Cell(currentRow, 4).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 5).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Visit Notes";
            worksheet.Cell(currentRow, 7).Value = "Payment Notes";
            worksheet.Cell(currentRow, 8).Value = "Brand";
            worksheet.Cell(currentRow, 9).Value = "Persons Met";
         

            currentRow = 2;
            foreach (var viss in vis)
            {

                IEnumerable<CustomVisitBrand> brandss = viss.brands.DistinctBy(a => a.BrandName);
                IEnumerable<CustomVisitPerson> personss = viss.persons;

                worksheet.Cell(currentRow, 1).Value = viss.AccountName;
                worksheet.Cell(currentRow, 2).Value = viss.AccountTypeName;
                worksheet.Cell(currentRow, 3).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 4).Value = date + " " + time;

                DateTime subdate = viss.SubmittingDate;
                



                worksheet.Cell(currentRow, 5).Value = subdate;
           

                worksheet.Cell(currentRow, 6).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 7).Value = viss.PaymentNotes;

                string brandstring = "";

                foreach (var item in brandss)
                {
                    brandstring = brandstring  + item.BrandName + " - ";
                }
                
                if (brandstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 8).Value = brandstring.Remove(brandstring.Length - 3);
                }
                string personstring = "";
                foreach (var item in personss)
                {
                    personstring = personstring + "( " + item.PersonName + " - " + item.PersonPosition + " ) | ";
                }
                
                if (personstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = personstring.Remove(personstring.Length - 3);
                }
                currentRow++;
            }
       
            
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + " visits to accounts from " + startdate + " to "+enddate+".xlsx");




        }

      

        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountMedicalVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = userManager.FindByIdAsync(userid).Result.FullName;
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomAccountMedicalVisit> vis = rep.GetAMDetailedVisits(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Accounts");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account";
            worksheet.Cell(currentRow, 2).Value = "Type";
            worksheet.Cell(currentRow, 3).Value = "Category";
            worksheet.Cell(currentRow, 4).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 5).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Visit Notes";
            worksheet.Cell(currentRow, 7).Value = "Additional Notes";
            worksheet.Cell(currentRow, 8).Value = "Products";
            worksheet.Cell(currentRow, 9).Value = "Persons Met";


            currentRow = 2;
            foreach (var viss in vis)
            {

                IEnumerable<CustomAccountMedicalVisitProducts> productss = viss.product.DistinctBy(a => a.ProductName);
                IEnumerable<CustomVisitPerson> personss = viss.person;

                worksheet.Cell(currentRow, 1).Value = viss.AccountName;
                worksheet.Cell(currentRow, 2).Value = viss.AccountTypeName;
                worksheet.Cell(currentRow, 3).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 4).Value = date + " " + time;

                DateTime sub = viss.SubmittingDate;
                worksheet.Cell(currentRow, 5).Value = sub;

               worksheet.Cell(currentRow, 6).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 7).Value = viss.AdditionalNotes;

                string productstring = "";

                foreach (var item in productss)
                {
                    productstring = productstring + item.ProductName + " - ";
                }
                
                if (productstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 8).Value = productstring.Remove(productstring.Length - 3);
                }
                string personstring = "";
                foreach (var item in personss)
                {
                    personstring = personstring + "( " + item.PersonName + " - " + item.PersonPosition + " ) | ";
                }
                
                if (personstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = personstring.Remove(personstring.Length - 3);
                }
                currentRow++;
            }

            IEnumerable<CustomContactMedicalVisit> visits = rep.GetCMDetailedVisits(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            var worksheettwo = workbook.Worksheets.Add("Contacts");
            var twocurrentRow = 1;

            worksheettwo.Cell(twocurrentRow, 1).Value = "Contact";
            worksheettwo.Cell(twocurrentRow, 2).Value = "Account Affiliation";
            worksheettwo.Cell(twocurrentRow, 3).Value = "Type";
            worksheettwo.Cell(twocurrentRow, 4).Value = "Category";
            worksheettwo.Cell(twocurrentRow, 5).Value = "Visit Date & Time";
            worksheettwo.Cell(twocurrentRow, 6).Value = "Submitting Date & Time";
            worksheettwo.Cell(twocurrentRow, 7).Value = "Visit Notes";
            worksheettwo.Cell(twocurrentRow, 8).Value = "Requests";
            worksheettwo.Cell(twocurrentRow, 9).Value = "Products";
            worksheettwo.Cell(twocurrentRow, 10).Value = "Sales Aids";

            twocurrentRow = 2;

            foreach (var visi in visits)
            {

                IEnumerable<CustomContactMedicalVisitProducts> productss = visi.customcontactmedicalvisitproduct.DistinctBy(a => a.ProductName);
                IEnumerable<CustomContactSalesAid> salesaids = visi.customcontactsalesaid;

                worksheettwo.Cell(twocurrentRow, 1).Value = visi.ContactName;
                worksheettwo.Cell(twocurrentRow, 2).Value = visi.AccountName;
                worksheettwo.Cell(twocurrentRow, 3).Value = visi.ContactTypeName;
                worksheettwo.Cell(twocurrentRow, 4).Value = visi.CategoryName;
                string date = visi.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = visi.VisitTime.TimeOfDay;
                worksheettwo.Cell(twocurrentRow, 5).Value = date + " " + time;
                DateTime sub = visi.SubmittingDate;

                worksheettwo.Cell(twocurrentRow, 6).Value = sub;
                worksheettwo.Cell(twocurrentRow, 7).Value = visi.VisitNotes;
                worksheettwo.Cell(twocurrentRow, 8).Value = visi.Requests;

                string productstring = "";

                foreach (var item in productss)
                {
                    productstring = productstring + "( Product: " + item.ProductName + " - Product Share: " + item.ProductShare + " ) | ";
                }
                if (productstring.Length > 3)
                {
                    worksheettwo.Cell(twocurrentRow, 9).Value = productstring.Remove(productstring.Length - 3);
                }


                string salesaidsstring = "";
                foreach (var item in salesaids)
                {
                    salesaidsstring = salesaidsstring + "( " + item.SalesAidName + " ) | ";
                }

                if (salesaidsstring.Length > 3)
                {
                    worksheettwo.Cell(twocurrentRow, 10).Value = salesaidsstring.Remove(salesaidsstring.Length - 3);
                }
                twocurrentRow++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name+" visits from " + startdate + " to "+enddate+".xlsx");

        }

       
        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult ContactMedicalVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = userManager.FindByIdAsync(userid).Result.FullName;
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomContactMedicalVisit> vis = rep.GetCMDetailedVisits(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Contact";
            worksheet.Cell(currentRow, 2).Value = "Type";
            worksheet.Cell(currentRow, 3).Value = "Category";
            worksheet.Cell(currentRow, 4).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 5).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Visit Notes";
            worksheet.Cell(currentRow, 7).Value = "Requests";
            worksheet.Cell(currentRow, 8).Value = "Products";
            worksheet.Cell(currentRow, 9).Value = "Sales Aids";


            currentRow = 2;
            foreach (var viss in vis)
            {

                IEnumerable<CustomContactMedicalVisitProducts> productss = viss.customcontactmedicalvisitproduct.DistinctBy(a => a.ProductName);
                IEnumerable<CustomContactSalesAid> salesaids = viss.customcontactsalesaid;

                worksheet.Cell(currentRow, 1).Value = viss.ContactName;
                worksheet.Cell(currentRow, 2).Value = viss.ContactTypeName;
                worksheet.Cell(currentRow, 3).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 4).Value = date+" "+time;
                DateTime sub = viss.SubmittingDate;

                worksheet.Cell(currentRow, 5).Value = sub;
                worksheet.Cell(currentRow, 6).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 7).Value = viss.Requests;

                string productstring = "";

                foreach (var item in productss)
                {
                    productstring = productstring + "( Product: " + item.ProductName + " - Product Share: " + item.ProductShare + " ) | ";
                }
                if (productstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 8).Value = productstring.Remove(productstring.Length - 3);
                }
                

                string salesaidsstring = "";
                foreach (var item in salesaids)
                {
                    salesaidsstring = salesaidsstring + "( " + item.SalesAidName + " ) | ";
                }
                
                if (salesaidsstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = salesaidsstring.Remove(salesaidsstring.Length - 3);
                }
                currentRow++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + " visits to contacts from " + startdate + " to "+enddate+".xlsx");




        }




        [Route("[controller]/[Action]/{myId}")]
        [HttpGet]
        public IActionResult GetMyTeamNames(string myId)
        {

            return Ok(rep.GetMyTeamNames(myId));
        }


        [Route("[controller]/[Action]/{myId}")]
        [HttpGet]
        public IActionResult GetMyTeamCollection(string myId)
        {
            return Ok(colRep.GetMyTeamCollection(myId));
        }

        [Route("[controller]/[Action]/{year}/{month}/{managerid}")]
        [HttpGet]
        public IActionResult GetVisitsThisMonthByCity(int year,int month,string managerid)
        {
            int? CityId = userManager.FindByIdAsync(managerid).Result.CityId;
            if (CityId == 1)
            {
                return Ok(topManagerRep.GetVisitsCountReyadByMonth(year,month));
            }
            else
            {
                if (CityId == 2)
                {
                    return Ok(topManagerRep.GetVisitsCountDamamByMonth(year, month));
                }
                else
                {
                    return Ok(topManagerRep.GetVisitsCountJeddahByMonth(year, month));
                }
            }

            
        }





    }
}
