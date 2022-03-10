using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.FirstManagerModels;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class TopManagerRep : ITopManagerRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;

        public TopManagerRep(DbContainer db, UserManager<ExtendIdentityUser> userManager,ITimeRep ti)
        {
            this.db = db;
            this.userManager = userManager;
            this.ti = ti;
        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> AccountpastMonthPercentage()
        {
            

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();

           




            int APlusAccounts = db.account.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AAccounts = db.account.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BAccounts = db.account.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CAccounts = db.account.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();





           
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.AddMonths(-1).Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int AMVAPlus = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVAPlus = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsAPlus = AMVAPlus + ASVAPlus;

            int AMVA = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
               a => a.AccountId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   AccountId = b.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVA = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == AId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsA = AMVA + ASVA;

            int AMVB = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId).DistinctBy(z => z.AccountId).Select(c => c.Id).Count();

            int ASVB = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == BId).DistinctBy(z => z.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsB = AMVB + ASVB;

            int AMVC = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVC = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == CId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsC = AMVC + ASVC;


            List<AccountVisitsPercentageByCategoryModel> result = new List<AccountVisitsPercentageByCategoryModel>();
            AccountVisitsPercentageByCategoryModel APlusResult = new AccountVisitsPercentageByCategoryModel();
            if (APlusAccounts != 0)
            {
                APlusResult.CategoryName = "A+";
                float rateAPlus = (float)AccountsVisitsAPlus / (float)APlusAccounts;
                APlusResult.percentage = rateAPlus * 100;
                result.Add(APlusResult);
            }
            else
            {
                APlusResult.CategoryName = "A+";
                APlusResult.percentage = 0;
                result.Add(APlusResult);
            }

            AccountVisitsPercentageByCategoryModel AResult = new AccountVisitsPercentageByCategoryModel();
            if (AAccounts != 0)
            {
                AResult.CategoryName = "A";
                float rateA = (float)AccountsVisitsA / (float)AAccounts;
                AResult.percentage = rateA * 100;
                result.Add(AResult);
            }
            else
            {
                AResult.CategoryName = "A";
                AResult.percentage = 0;
                result.Add(AResult);
            }

            AccountVisitsPercentageByCategoryModel BResult = new AccountVisitsPercentageByCategoryModel();
            if (BAccounts != 0)
            {
                BResult.CategoryName = "B";
                float rateB = (float)AccountsVisitsB / (float)BAccounts;
                BResult.percentage = rateB * 100;
                result.Add(BResult);
            }
            else
            {
                BResult.CategoryName = "B";
                BResult.percentage = 0;
                result.Add(BResult);
            }

            AccountVisitsPercentageByCategoryModel CResult = new AccountVisitsPercentageByCategoryModel();
            if (CAccounts != 0)
            {
                CResult.CategoryName = "C";
                float rateC = (float)AccountsVisitsC / (float)CAccounts;
                CResult.percentage = rateC * 100;
                result.Add(CResult);
            }

            else
            {
                CResult.CategoryName = "C";
                CResult.percentage = 0;
                result.Add(CResult);
            }

            return result;
        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> AccountThisMonthPercentage()
        {

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();

            




            int APlusAccounts = db.account.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AAccounts = db.account.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BAccounts = db.account.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CAccounts = db.account.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();





           
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int AMVAPlus = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVAPlus = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsAPlus = AMVAPlus + ASVAPlus;

            int AMVA = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
               a => a.AccountId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   AccountId = b.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVA = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == AId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsA = AMVA + ASVA;

            int AMVB = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId).DistinctBy(z => z.AccountId).Select(c => c.Id).Count();

            int ASVB = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == BId).DistinctBy(z => z.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsB = AMVB + ASVB;

            int AMVC = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVC = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == CId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsC = AMVC + ASVC;


            List<AccountVisitsPercentageByCategoryModel> result = new List<AccountVisitsPercentageByCategoryModel>();
            AccountVisitsPercentageByCategoryModel APlusResult = new AccountVisitsPercentageByCategoryModel();
            if (APlusAccounts != 0)
            {
                APlusResult.CategoryName = "A+";
                float rateAPlus = (float)AccountsVisitsAPlus / (float)APlusAccounts;
                APlusResult.percentage = rateAPlus * 100;
                result.Add(APlusResult);
            }
            else
            {
                APlusResult.CategoryName = "A+";
                APlusResult.percentage = 0;
                result.Add(APlusResult);
            }

            AccountVisitsPercentageByCategoryModel AResult = new AccountVisitsPercentageByCategoryModel();
            if (AAccounts != 0)
            {
                AResult.CategoryName = "A";
                float rateA = (float)AccountsVisitsA / (float)AAccounts;
                AResult.percentage = rateA * 100;
                result.Add(AResult);
            }
            else
            {
                AResult.CategoryName = "A";
                AResult.percentage = 0;
                result.Add(AResult);
            }

            AccountVisitsPercentageByCategoryModel BResult = new AccountVisitsPercentageByCategoryModel();
            if (BAccounts != 0)
            {
                BResult.CategoryName = "B";
                float rateB = (float)AccountsVisitsB / (float)BAccounts;
                BResult.percentage = rateB * 100;
                result.Add(BResult);
            }
            else
            {
                BResult.CategoryName = "B";
                BResult.percentage = 0;
                result.Add(BResult);
            }

            AccountVisitsPercentageByCategoryModel CResult = new AccountVisitsPercentageByCategoryModel();
            if (CAccounts != 0)
            {
                CResult.CategoryName = "C";
                float rateC = (float)AccountsVisitsC / (float)CAccounts;
                CResult.percentage = rateC * 100;
                result.Add(CResult);
            }

            else
            {
                CResult.CategoryName = "C";
                CResult.percentage = 0;
                result.Add(CResult);
            }

            return result;
        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> ContactPastMonthPercentage()
        {
            

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();



           


            int APlusContacts = db.userContact.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AContacts = db.userContact.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BContacts = db.userContact.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CContacts = db.userContact.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();

     
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.AddMonths(-1).Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int CMVAPlus = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
                a => a.ContactId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = b.Id
                }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new 
                {
                    Id = a.Id,
                    ContactId = a.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.ContactId).Select(c => c.Id).Count();


        


            int CMVA = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
               a => a.ContactId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = b.Id
               }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = a.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId).DistinctBy(a => a.ContactId).Select(c => c.Id).Count();




            int CMVB = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id
              }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = a.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId).DistinctBy(z => z.ContactId).Select(c => c.Id).Count();
            ;

            int CMVC = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id
              }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = a.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId).DistinctBy(a => a.ContactId).Select(c => c.Id).Count();



            List<AccountVisitsPercentageByCategoryModel> result = new List<AccountVisitsPercentageByCategoryModel>();
            AccountVisitsPercentageByCategoryModel APlusResult = new AccountVisitsPercentageByCategoryModel();
            if (APlusContacts != 0)
            {
                APlusResult.CategoryName = "A+";
                float rateAPlus = (float)CMVAPlus / (float)APlusContacts;
                APlusResult.percentage = rateAPlus * 100;
                result.Add(APlusResult);
            }
            else
            {
                APlusResult.CategoryName = "A+";
                APlusResult.percentage = 0;
                result.Add(APlusResult);
            }

            AccountVisitsPercentageByCategoryModel AResult = new AccountVisitsPercentageByCategoryModel();
            if (AContacts != 0)
            {
                AResult.CategoryName = "A";
                float rateA = (float)CMVA / (float)AContacts;
                AResult.percentage = rateA * 100;
                result.Add(AResult);
            }
            else
            {
                AResult.CategoryName = "A";
                AResult.percentage = 0;
                result.Add(AResult);
            }

            AccountVisitsPercentageByCategoryModel BResult = new AccountVisitsPercentageByCategoryModel();
            if (BContacts != 0)
            {
                BResult.CategoryName = "B";
                float rateB = (float)CMVB / (float)BContacts;
                BResult.percentage = rateB * 100;
                result.Add(BResult);
            }
            else
            {
                BResult.CategoryName = "B";
                BResult.percentage = 0;
                result.Add(BResult);
            }

            AccountVisitsPercentageByCategoryModel CResult = new AccountVisitsPercentageByCategoryModel();
            if (CContacts != 0)
            {
                CResult.CategoryName = "C";
                float rateC = (float)CMVC / (float)CContacts;
                CResult.percentage = rateC * 100;
                result.Add(CResult);
            }

            else
            {
                CResult.CategoryName = "C";
                CResult.percentage = 0;
                result.Add(CResult);
            }

            return result;
        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> ContactThisMonthPercentage()
        {
            

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();



            


            int APlusContacts = db.userContact.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AContacts = db.userContact.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BContacts = db.userContact.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CContacts = db.userContact.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();

            
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int CMVAPlus = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
                a => a.ContactId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = b.Id
                }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = a.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.ContactId).Select(c => c.Id).Count();


            int CMVA = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
               a => a.ContactId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = b.Id
               }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = a.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId).DistinctBy(a => a.ContactId).Select(c => c.Id).Count();




            int CMVB = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id
              }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = a.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId).DistinctBy(z => z.ContactId).Select(c => c.Id).Count();
            ;

            int CMVC = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id
              }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = a.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId).DistinctBy(a => a.ContactId).Select(c => c.Id).Count();



            List<AccountVisitsPercentageByCategoryModel> result = new List<AccountVisitsPercentageByCategoryModel>();
            AccountVisitsPercentageByCategoryModel APlusResult = new AccountVisitsPercentageByCategoryModel();
            if (APlusContacts != 0)
            {
                APlusResult.CategoryName = "A+";
                float rateAPlus = (float)CMVAPlus / (float)APlusContacts;
                APlusResult.percentage = rateAPlus * 100;
                result.Add(APlusResult);
            }
            else
            {
                APlusResult.CategoryName = "A+";
                APlusResult.percentage = 0;
                result.Add(APlusResult);
            }

            AccountVisitsPercentageByCategoryModel AResult = new AccountVisitsPercentageByCategoryModel();
            if (AContacts != 0)
            {
                AResult.CategoryName = "A";
                float rateA = (float)CMVA / (float)AContacts;
                AResult.percentage = rateA * 100;
                result.Add(AResult);
            }
            else
            {
                AResult.CategoryName = "A";
                AResult.percentage = 0;
                result.Add(AResult);
            }

            AccountVisitsPercentageByCategoryModel BResult = new AccountVisitsPercentageByCategoryModel();
            if (BContacts != 0)
            {
                BResult.CategoryName = "B";
                float rateB = (float)CMVB / (float)BContacts;
                BResult.percentage = rateB * 100;
                result.Add(BResult);
            }
            else
            {
                BResult.CategoryName = "B";
                BResult.percentage = 0;
                result.Add(BResult);
            }

            AccountVisitsPercentageByCategoryModel CResult = new AccountVisitsPercentageByCategoryModel();
            if (CContacts != 0)
            {
                CResult.CategoryName = "C";
                float rateC = (float)CMVC / (float)CContacts;
                CResult.percentage = rateC * 100;
                result.Add(CResult);
            }

            else
            {
                CResult.CategoryName = "C";
                CResult.percentage = 0;
                result.Add(CResult);
            }

            return result;
        }

        public IEnumerable<CustomAccountSalesVisit> GetAccountSalesVisitsByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExtendIdentityUser> GetAllFirstManagers()
        {
            List<ExtendIdentityUser> res = new List<ExtendIdentityUser>();
            IEnumerable<ExtendIdentityUser> FirstManagers = userManager.GetUsersInRoleAsync("First Line Manager").Result;
            IEnumerable<ExtendIdentityUser> TopManagers = userManager.GetUsersInRoleAsync("Top Line Manager").Result;
            foreach (var f in FirstManagers)
            {
                res.Add(f);
            }

            foreach (var t in TopManagers)
            {
                res.Add(t);
            }
          
            return res.OrderBy(a=>a.FullName);
        }

        public IEnumerable<CustomAccountMedicalVisit> GetDetailedAMV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);

            List<ExtendIdentityUser> team = new List<ExtendIdentityUser>();
            if (userid == "0")
            {
                team = userManager.GetUsersInRoleAsync("Medical Representative").Result.ToList();
            }
            else
            {
                team = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.extendidentityuserid == userid).ToList();
            }
            List<AccountMedicalVisit> visits = new List<AccountMedicalVisit>();
            foreach (var item in team)
            {
                IEnumerable<AccountMedicalVisit> v = db.accountMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= from && a.VisitDate <= to);
                foreach (var vis in v)
                {
                    visits.Add(vis);
                }
            }

            List<CustomAccountMedicalVisitProducts> productlist = new List<CustomAccountMedicalVisitProducts>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visits)
            {
                var s = db.accountMedicalVisitProducts.Where(a => a.AccountMedicalVisitId == item.Id).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    KeyId = a.AccountMedicalVisitId
                });
                foreach (var product in s)
                {
                    CustomAccountMedicalVisitProducts ad = new CustomAccountMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.AccountMedicalVisitId = product.KeyId;
                    productlist.Add(ad);
                }

                var ss = db.accountMedicalVisitPerson.Where(a => a.AccountMedicalVisitId == item.Id);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountMedicalVisitId;
                    personlist.Add(ab);
                }

            }




            IEnumerable<CustomAccountMedicalVisit> result = visits.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = b.DistrictId,
                CategoryId = b.CategoryId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.extendidentityuserid
            }).Join(db.accountMedicalVisitProducts, a => a.Id, b => b.AccountMedicalVisitId, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new {

                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                DistrictName = a.DistrictName,
                CategoryName = b.CategoryName,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId
            }).Join(db.Users,a=>a.UserId,b=>b.Id,(a,b)=>new CustomAccountMedicalVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                product = productlist.Where(x => x.AccountMedicalVisitId == a.Id),
                person = personlist.Where(c => c.KeyId == a.Id),
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = a.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                UserName = b.FullName
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName);



            return result;
        }

        public IEnumerable<CustomAccountSalesVisit> GetDetailedASV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);

            List<ExtendIdentityUser> team = new List<ExtendIdentityUser>();
            if (userid == "0")
            {
                team = userManager.GetUsersInRoleAsync("Sales Representative").Result.ToList();
            }
            else
            {
                team = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == userid).ToList();
            }

            List<AccountSalesVisit> visits = new List<AccountSalesVisit>();
            foreach (var item in team)
            {
                IEnumerable<AccountSalesVisit> v = db.accountSalesVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= from && a.VisitDate <= to);
                foreach (var vis in v)
                {
                    visits.Add(vis);
                }
            }

            //var visitsids = db.accountSalesVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Select(a => a.Id);

            List<CustomVisitBrand> brandlist = new List<CustomVisitBrand>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visits)
            {
                var s = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == item.Id).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
                {
                    BrandName = b.BrandName,
                    KeyId = a.AccountSalesVisitId
                });
                foreach (var brand in s)
                {
                    CustomVisitBrand ad = new CustomVisitBrand();
                    ad.BrandName = brand.BrandName;
                    ad.KeyId = brand.KeyId;
                    brandlist.Add(ad);
                }

                var ss = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == item.Id);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountSalesVisitId;
                    personlist.Add(ab);
                }

            }




            IEnumerable<CustomAccountSalesVisit> result = visits.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = b.DistrictId,
                CategoryId = b.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.extendidentityuserid
            }).Join(db.accountSalesVisitBrand, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new {

                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                DistrictName = a.DistrictName,
                CategoryName = b.CategoryName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new 
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                brands = brandlist.Where(x => x.KeyId == a.Id),
                persons = personlist.Where(c => c.KeyId == a.Id),
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.Users, a => a.UserId, b => b.Id, (a, b) => new CustomAccountSalesVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                brands = brandlist.Where(x => x.KeyId == a.Id),
                persons = personlist.Where(c => c.KeyId == a.Id),
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = a.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserName = b.FullName
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName);



            return result;
        }

        public IEnumerable<CustomContactMedicalVisit> GetDetailedCMV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            List<ExtendIdentityUser> team = new List<ExtendIdentityUser>();
            if (userid == "0")
            {
                team = userManager.GetUsersInRoleAsync("Medical Representative").Result.ToList();
            }
            else
            {
                team = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.extendidentityuserid == userid).ToList();
            }
            

            List<ContactMedicalVisit> visits = new List<ContactMedicalVisit>();
            foreach (var item in team)
            {
                IEnumerable<ContactMedicalVisit> v = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= from && a.VisitDate <= to);
                foreach (var vis in v)
                {
                    visits.Add(vis);
                }
            }


            

            List<CustomContactMedicalVisitProducts> productlist = new List<CustomContactMedicalVisitProducts>();
            List<CustomContactSalesAid> salesaidslist = new List<CustomContactSalesAid>();
            foreach (var item in visits)
            {
                var s = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == item.Id).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    ProductShare = a.ProductShare,
                    KeyId = a.ContactMedicalVisitId
                });
                foreach (var product in s)
                {
                    CustomContactMedicalVisitProducts ad = new CustomContactMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.ProductShare = product.ProductShare;
                    ad.ContactMedicalVisitId = product.KeyId;
                    productlist.Add(ad);
                }

                var ss = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == item.Id).Join(db.salesAid, a => a.SalesAidId, b => b.Id, (a, b) => new
                {
                    SalesAidName = b.SalesAidName,
                    ContactMedicalVisitId = a.ContactMedicalVisitId
                });
                foreach (var aid in ss)
                {
                    CustomContactSalesAid ab = new CustomContactSalesAid();
                    ab.SalesAidName = aid.SalesAidName;
                    ab.ContactMedicalVisitId = aid.ContactMedicalVisitId;
                    salesaidslist.Add(ab);
                }

            }



            var xxx = visits.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = b.DistrictId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.extendidentityuserid,
                customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == a.Id),
                customcontactsalesaid = salesaidslist.Where(c => c.ContactMedicalVisitId == a.Id)
            }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid,
                CategoryId = b.CategoryId
            });





            var result = visits.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Email = b.Email,
                AccountId = b.AccountId,
                PhoneNumber = b.MobileNumber,
                LandLineNumber = b.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = b.DistrictId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.extendidentityuserid,
                customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == a.Id),
                customcontactsalesaid = salesaidslist.Where(c => c.ContactMedicalVisitId == a.Id)
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = b.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid
            }).Join(db.Users, a => a.UserId, b => b.Id, (a, b) => new 
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                UserName = b.FullName,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid
            }).Join(db.userContact,a=>a.ContactId,b=>b.ContactId,(a,b)=>new
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                UserName = a.UserName,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid,
                CategoryId = b.CategoryId
            }).Join(db.category,a=>a.CategoryId,b=>b.Id,(a,b)=>new 
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                UserName = a.UserName,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid,
                CategoryId = a.CategoryId,
                CategoryName = b.CategoryName
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContactMedicalVisit
            {
                Id = a.Id,
                Email = a.Email,
                AccountName = b.AccountName,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                UserName = a.UserName,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid,
                CategoryId = a.CategoryId,
                CategoryName = a.CategoryName,
                Requested = false
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName);



            return result;
        }

        public IEnumerable<CustomTarget> GetTarget(SearchTargetModel obj)
        {
            if (obj.CategoryId == 0)
            {
                int days = DateTime.DaysInMonth(obj.year, obj.Month);
                DateTime start = new DateTime(obj.year, obj.Month, 1);
                DateTime end = new DateTime(obj.year, obj.Month, days);

                var x = db.userContact.Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new
                {
                    UserId = b.Id,
                    ContactId = a.ContactId,
                    FullName = b.FullName,
                    ManagerId = b.extendidentityuserid,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId
                }).Where(u => u.ManagerId == obj.ManagerId).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
                {
                    FullName = a.FullName,
                    ContactName = b.ContactName,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId,
                    ContactId = b.Id,
                    UserId = a.UserId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    FullName = a.FullName,
                    ContactName = a.ContactName,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId,
                    ContactId = a.ContactId,
                    UserId = a.UserId,
                    CategoryName = b.CategoryName
                });

                List<CustomTarget> result = new List<CustomTarget>();
                foreach (var item in x)
                {
                    CustomTarget t = new CustomTarget();
                    int count = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end && a.extendidentityuserid == item.UserId && a.ContactId == item.ContactId).Select(a => a.Id).Count();
                    t.ContactName = item.ContactName;
                    t.FullName = item.FullName;
                    t.CurrentVisits = count;
                    t.MonthlyTarget = item.MonthlyTarget;
                    t.CategoryName = item.CategoryName;
                    result.Add(t);
                }

                return result;
            }

            else
            {
                int days = DateTime.DaysInMonth(obj.year, obj.Month);
                DateTime start = new DateTime(obj.year, obj.Month, 1);
                DateTime end = new DateTime(obj.year, obj.Month, days);

                var x = db.userContact.Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new
                {
                    UserId = b.Id,
                    ContactId = a.ContactId,
                    FullName = b.FullName,
                    ManagerId = b.extendidentityuserid,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId
                }).Where(u => u.ManagerId == obj.ManagerId).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
                {
                    FullName = a.FullName,
                    ContactName = b.ContactName,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId,
                    ContactId = b.Id,
                    UserId = a.UserId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    FullName = a.FullName,
                    ContactName = a.ContactName,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId,
                    ContactId = a.ContactId,
                    UserId = a.UserId,
                    CategoryName = b.CategoryName
                });

                List<CustomTarget> result = new List<CustomTarget>();
                foreach (var item in x)
                {
                    CustomTarget t = new CustomTarget();
                    int count = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end && a.extendidentityuserid == item.UserId && a.ContactId == item.ContactId).Select(a => a.Id).Count();
                    t.ContactName = item.ContactName;
                    t.FullName = item.FullName;
                    t.CurrentVisits = count;
                    t.MonthlyTarget = item.MonthlyTarget;
                    t.CategoryName = item.CategoryName;
                    result.Add(t);
                }

                return result;
            }
            
        }

        public IEnumerable<MyTeamModel> GetTeamMedical()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MyTeamModel> GetTeamSales()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountDamamByMonth(int year, int month)
        {
          
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            double accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

           

            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            double contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

           


            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            double accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountJeddahByMonth(int year, int month)
        {
            
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            double accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();

     



            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            double contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();
   
            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;

            result.Add(conmed);

            double accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate < end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountReyadByMonth(int year, int month)
        {
            
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            double accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();

     

            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            double contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();


            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            double accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountThisMonthDamam()
        {

            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            int accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a=>a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();


            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            int contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();
 

            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            int accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountThisMonthJeddah()
        {
            
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            int accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();
           

            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            int contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();


            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            int accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate < end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountThisMonthReyad()
        {
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            int accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,a=>a.AccountId,b=>b.Id,(a,b)=>new
            {
            Id = a.Id,
            DistrictId = b.DistrictId
            }).Join(db.district,a=>a.DistrictId,b=>b.Id,(a,b)=>new
            {
            Id = a.Id,
            CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x=>x.CityId == 1).Select(a => a.Id).Count();


            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            int contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,a=>a.ContactId,b=>b.Id,(a,b)=>new
            {
            Id = a.Id,
            DistrictId= b.DistrictId
            }).Join(db.district,a=>a.DistrictId,b=>b.Id,(a,b)=>new
            {
            Id = a.Id,
            CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x=>x.CityId == 1).Select(a => a.Id).Count();

       

            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            int accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            return result;

        }

        public IEnumerable<TopManagerMorrisLine> MorrisLine()
        {
           DateTime now = ti.GetCurrentTime();
           int month = now.Month;
            List<TopManagerMorrisLine> res = new List<TopManagerMorrisLine>();
            for (int i = 1; i <= month; i++)
            {
                int days = DateTime.DaysInMonth(now.Year, i);
                DateTime start = new DateTime(now.Year, i, 1);
                DateTime end = new DateTime(now.Year, i, days);
                int medicalcontact = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Count();
                int salesaccount = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Count();
                string yearmonth = start.ToString("yyyy-MM");
                TopManagerMorrisLine obj = new TopManagerMorrisLine();
                obj.y = yearmonth;
                obj.a = salesaccount;
                obj.b = medicalcontact;
                res.Add(obj);
            }
            return res;

        }
    }
}
