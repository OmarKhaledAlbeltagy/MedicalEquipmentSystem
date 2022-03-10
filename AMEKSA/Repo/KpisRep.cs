using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class KpisRep:IKpisRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;


        public KpisRep(DbContainer db, UserManager<ExtendIdentityUser> userManager,ITimeRep ti)
        {
            this.db = db;
            this.userManager = userManager;
            this.ti = ti;
        }

        public bool EditProperty(int id, int value)
        {
            Properties obj = db.properties.Find(id);
            obj.Value = value;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<MedicalKpiModel> GetAllMedicalKpi(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, days, 23, 59, 59);
            
            int? workingdaysdata = db.workingDays.Where(a => a.Month == month && a.Year == year).Select(a => a.NumberOfWorkingDays).FirstOrDefault();
            IEnumerable<ExtendIdentityUser> users = userManager.GetUsersInRoleAsync("Medical Representative").Result;
            List<MedicalKpiModel> ress = new List<MedicalKpiModel>();
            foreach (var item in users)
            {
                int? timeoffdays = GetTimeOffDiff(item.Id, year, month,1,year,month,days);
                   

             
               
                IEnumerable<ContactWithCategory> UserAPlusAndA = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 1 || a.CategoryId == 2);

                IEnumerable<ContactWithCategory> UserB = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 3);

                IEnumerable<ContactWithCategory> UserC = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 4);

                int AverageVisitPerDayKpiTarget = db.properties.Where(a => a.Id == 7).Select(a => a.Value).FirstOrDefault();
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 8).Select(a => a.Value).FirstOrDefault();
                int VisitsTargerAchievmentWeight = db.properties.Where(a => a.Id == 9).Select(a => a.Value).FirstOrDefault();
                int AAndAPlusWegiht = db.properties.Where(a => a.Id == 10).Select(a => a.Value).FirstOrDefault();
                int BWeight = db.properties.Where(a => a.Id == 11).Select(a => a.Value).FirstOrDefault();
                int CWeight = db.properties.Where(a => a.Id == 12).Select(a => a.Value).FirstOrDefault();
                int SellingDaysInTheFieldWegiht = db.properties.Where(a => a.Id == 13).Select(a => a.Value).FirstOrDefault();

                MedicalKpiModel res = new MedicalKpiModel();
                res.FullName = item.FullName;
                res.CityName = db.city.Where(a => a.Id == item.CityId).Select(a => a.CityName).FirstOrDefault();
                res.RoleName = "Medical Representative";
                res.Month = start.ToString("MMMM - yyyy");
                res.ActualTotalNumberOfVisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
                res.AplusAndAListed = UserAPlusAndA.Select(a => a.Id).Count();
                res.BListed = UserB.Select(a => a.Id).Count();
                res.CListed = UserC.Select(a => a.Id).Count();
                int APLusAndAVisited = 0;
                int BVisited = 0;
                int CVisited = 0;
                foreach (var aa in UserAPlusAndA)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == aa.Id).FirstOrDefault();
                    if (x != null)
                    {
                        APLusAndAVisited++;
                    }
                }
                foreach (var bb in UserB)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == bb.Id).FirstOrDefault();
                    if (x != null)
                    {
                        BVisited++;
                    }
                }
                foreach (var cc in UserC)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == cc.Id).FirstOrDefault();
                    if (x != null)
                    {
                        CVisited++;
                    }
                }
                res.APlusAndAVisited = APLusAndAVisited;
                res.BVisited = BVisited;
                res.CVisited = CVisited;
                res.SellingDaysInTheField = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Day).Select(a => a.Id).Count();
                res.WorkingDays = workingdaysdata - timeoffdays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitPerDayKpiTarget;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargerAchievmentWeight;
                res.APlusAndAWeight = AAndAPlusWegiht;
                res.BWeight = BWeight;
                res.CWeight = CWeight;
                res.SellingDaysInTheFieldWeight = SellingDaysInTheFieldWegiht;
                res.TimeOffDays = timeoffdays;
                ress.Add(res);
            }
            return ress.OrderBy(a => a.FullName);
        }

        public IEnumerable<Properties> GetAllProperties()
        {
            IEnumerable<Properties> res = db.properties.Select(a => a);
            return res;
        }

        public IEnumerable<SalesKpiModel> GetAllSalesKpi(int year,int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, days, 23, 59, 59);
            
            int? WorkingDaysData = db.workingDays.Where(a => a.Month == month && a.Year == year).Select(a => a.NumberOfWorkingDays).FirstOrDefault();
            IEnumerable<ExtendIdentityUser> list = userManager.GetUsersInRoleAsync("Sales Representative").Result;
            List<SalesKpiModel> result = new List<SalesKpiModel>();

            foreach (var obj in list)
            {
                int? timeoffdays = GetTimeOffDiff(obj.Id, year, month, 1, year, month, days);
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
                int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
                int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
                int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

                int AverageVisitsPerDayKpiTarget = db.properties.Where(a => a.Id == 1).Select(a => a.Value).SingleOrDefault();

                double? WorkingDays = WorkingDaysData - timeoffdays;

                string FullName = obj.FullName;

                string office = db.city.Where(a => a.Id == obj.CityId).Select(a => a.CityName).FirstOrDefault();

                string date = start.ToString("MMMM - yyyy");

                double ActualTotalNumberOfVisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();

                double ListedAccounts = db.userAccount.Where(a => a.extendidentityuserid == obj.Id).DistinctBy(x => x.AccountId).Count();

                double SellingDaysInTheField = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Day).Select(a => a.Id).Count();

                //double? TargetedNumberOfVisits = WorkingDays * AverageVisitsPerDayKpiTarget;

                double VisitedAccounts = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.AccountId).Select(a => a.Id).Count();

                //double AverageVisitsPerDayActual = (double)ActualTotalNumberOfVisits / (double)SellingDaysInTheField;

                SalesKpiModel res = new SalesKpiModel();
                res.FullName = FullName;
                res.Month = date;
                res.RoleName = "Sales Representative";

                res.CityName = office;
                res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
                res.ListedAccounts = ListedAccounts;
                res.SellingDaysInTheField = SellingDaysInTheField;
                //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
                res.VisitedAccounts = VisitedAccounts;
                res.WorkingDays = WorkingDays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
               //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
                //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
                //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
                //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
                //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
                res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
                res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
                res.TimeOffDays = timeoffdays;
                //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
                //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
                //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
                //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
                //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
                result.Add(res);
            }
            return result.OrderBy(a => a.FullName);
        }

        public MedicalKpiModel GetMedicalKpi(int year, int month, string userId)
        {
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, days, 23, 59, 59);
            
            int? WorkingDaysData = db.workingDays.Where(a => a.Month == month && a.Year == year).Select(a => a.NumberOfWorkingDays).FirstOrDefault();
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;

            int? timeoffdays = GetTimeOffDiff(userId, year, month, 1, year, month, days);

            IEnumerable<ContactWithCategory> UserAPlusAndA = db.userContact.Where(a=>a.extendidentityuserid == user.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender,
                Address = b.Address,
                LandLineNumber = b.LandLineNumber,
                MobileNumber = b.MobileNumber,
                Email = b.Email,
                PaymentNotes = b.PaymentNotes,
                RelationshipNote = b.RelationshipNote,
                AccountId = b.AccountId,
                CategoryId = a.CategoryId
            }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 1 || a.CategoryId == 2);

            IEnumerable<ContactWithCategory> UserB = db.userContact.Where(a => a.extendidentityuserid == user.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender,
                Address = b.Address,
                LandLineNumber = b.LandLineNumber,
                MobileNumber = b.MobileNumber,
                Email = b.Email,
                PaymentNotes = b.PaymentNotes,
                RelationshipNote = b.RelationshipNote,
                AccountId = b.AccountId,
                CategoryId = a.CategoryId
            }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 3);

            IEnumerable<ContactWithCategory> UserC = db.userContact.Where(a => a.extendidentityuserid == user.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender,
                Address = b.Address,
                LandLineNumber = b.LandLineNumber,
                MobileNumber = b.MobileNumber,
                Email = b.Email,
                PaymentNotes = b.PaymentNotes,
                RelationshipNote = b.RelationshipNote,
                AccountId = b.AccountId,
                CategoryId = a.CategoryId
            }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 4);

            int AverageVisitPerDayKpiTarget = db.properties.Where(a => a.Id == 7).Select(a => a.Value).FirstOrDefault();
            int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 8).Select(a => a.Value).FirstOrDefault();
            int VisitsTargerAchievmentWeight = db.properties.Where(a => a.Id == 9).Select(a => a.Value).FirstOrDefault();
            int AAndAPlusWegiht = db.properties.Where(a => a.Id == 10).Select(a => a.Value).FirstOrDefault();
            int BWeight = db.properties.Where(a => a.Id == 11).Select(a => a.Value).FirstOrDefault();
            int CWeight = db.properties.Where(a => a.Id == 12).Select(a => a.Value).FirstOrDefault();
            int SellingDaysInTheFieldWegiht = db.properties.Where(a => a.Id == 13).Select(a => a.Value).FirstOrDefault();

            MedicalKpiModel res = new MedicalKpiModel();
            res.FullName = user.FullName;
            res.CityName = db.city.Where(a => a.Id == user.CityId).Select(a => a.CityName).FirstOrDefault();
            res.RoleName = "Medical Representative";
            res.Month = start.ToString("MMMM - yyyy");
            res.ActualTotalNumberOfVisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a=>a.Id).Count();
            res.AplusAndAListed = UserAPlusAndA.Select(a => a.Id).Count();
            res.BListed = UserB.Select(a => a.Id).Count();
            res.CListed = UserC.Select(a => a.Id).Count();
            int APLusAndAVisited = 0;
            int BVisited = 0;
            int CVisited = 0;
            foreach (var item in UserAPlusAndA)
            {
                ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == item.Id).FirstOrDefault();
                if (x != null)
                {
                    APLusAndAVisited++;
                }
            }
            foreach (var item in UserB)
            {
                ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == item.Id).FirstOrDefault();
                if (x != null)
                {
                    BVisited++;
                }
            }
            foreach (var item in UserC)
            {
                ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == item.Id).FirstOrDefault();
                if (x != null)
                {
                    CVisited++;
                }
            }
            res.APlusAndAVisited = APLusAndAVisited;
            res.BVisited = BVisited;
            res.CVisited = CVisited;
            res.SellingDaysInTheField = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Day).Select(a=>a.Id).Count();
            res.WorkingDays = WorkingDaysData - timeoffdays;
            res.AverageVisitsPerDayKpiTarget = AverageVisitPerDayKpiTarget;
            res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
            res.VisitsTargetAchievmentWeight = VisitsTargerAchievmentWeight;
            res.APlusAndAWeight = AAndAPlusWegiht;
            res.BWeight = BWeight;
            res.CWeight = CWeight;
            res.SellingDaysInTheFieldWeight = SellingDaysInTheFieldWegiht;
            res.TimeOffDays = timeoffdays;
            return res;
        }

        public SalesKpiModel GetSalesKpi(int year, int month, string userId)
        {
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, days, 23, 59, 59);
           
            int? WorkingDaysData = db.workingDays.Where(a => a.Month == month && a.Year == year).Select(a => a.NumberOfWorkingDays).FirstOrDefault();
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            int? timeoffdays = GetTimeOffDiff(userId, year, month, 1, year, month, days);
            int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
            int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
            int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
            int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

            int AverageVisitsPerDayKpiTarget = db.properties.Where(a=>a.Id == 1).Select(a=>a.Value).SingleOrDefault();

           

            string FullName = user.FullName;

            string office = db.city.Where(a => a.Id == user.CityId).Select(a => a.CityName).FirstOrDefault();

            string date = start.ToString("MMMM - yyyy");

            double ActualTotalNumberOfVisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).Select(a=>a.Id).Count();

            double ListedAccounts = db.userAccount.Where(a => a.extendidentityuserid == userId).DistinctBy(x => x.AccountId).Count();

            double SellingDaysInTheField = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Day).Select(a => a.Id).Count();

            //double? TargetedNumberOfVisits = WorkingDays * AverageVisitsPerDayKpiTarget;

            double VisitedAccounts = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.AccountId).Select(a => a.Id).Count();

           //double AverageVisitsPerDayActual =(double)ActualTotalNumberOfVisits / (double)SellingDaysInTheField;

            SalesKpiModel res = new SalesKpiModel();
            res.FullName = FullName;
            res.Month = date;
            res.RoleName = "Sales Representative";

            res.CityName = office;
            res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
            res.ListedAccounts = ListedAccounts;
            res.SellingDaysInTheField = SellingDaysInTheField;
            //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
            res.VisitedAccounts = VisitedAccounts;
            res.WorkingDays = WorkingDaysData - timeoffdays;
            res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
            //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
            //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
            //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
            //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
            //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
            res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
            res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
            res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
            res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
            res.TimeOffDays = timeoffdays;
            //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
            //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
            //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
            //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
            //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
            return res;
        }

        public IEnumerable<MedicalKpiModel> GetTeamMedicalKpi(int year, int month, string managerId)
        {
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, days, 23, 59, 59);
            
            int? WorkingDaysdata = db.workingDays.Where(a => a.Month == month && a.Year == year).Select(a => a.NumberOfWorkingDays).FirstOrDefault();
            List<ExtendIdentityUser> users = new List<ExtendIdentityUser>();


            IEnumerable<ExtendIdentityUser> u = userManager.Users.Where(a=>a.extendidentityuserid == managerId);
            foreach (var item in u)
            {
              var x =  userManager.IsInRoleAsync(item, "Medical Representative").Result;
                if (x)
                {
                    users.Add(item);
                }
            }
            List<MedicalKpiModel> ress = new List<MedicalKpiModel>();
            foreach (var item in users)
            {
                int? timeoffdays = GetTimeOffDiff(item.Id, year, month, 1, year, month, days);
                IEnumerable<ContactWithCategory> UserAPlusAndA = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 1 || a.CategoryId == 2);

                IEnumerable<ContactWithCategory> UserB = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 3);

                IEnumerable<ContactWithCategory> UserC = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 4);

                int AverageVisitPerDayKpiTarget = db.properties.Where(a => a.Id == 7).Select(a => a.Value).FirstOrDefault();
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 8).Select(a => a.Value).FirstOrDefault();
                int VisitsTargerAchievmentWeight = db.properties.Where(a => a.Id == 9).Select(a => a.Value).FirstOrDefault();
                int AAndAPlusWegiht = db.properties.Where(a => a.Id == 10).Select(a => a.Value).FirstOrDefault();
                int BWeight = db.properties.Where(a => a.Id == 11).Select(a => a.Value).FirstOrDefault();
                int CWeight = db.properties.Where(a => a.Id == 12).Select(a => a.Value).FirstOrDefault();
                int SellingDaysInTheFieldWegiht = db.properties.Where(a => a.Id == 13).Select(a => a.Value).FirstOrDefault();

                MedicalKpiModel res = new MedicalKpiModel();
                res.FullName = item.FullName;
                res.CityName = db.city.Where(a => a.Id == item.CityId).Select(a => a.CityName).FirstOrDefault();
                res.RoleName = "Medical Representative";
                res.Month = start.ToString("MMMM - yyyy");
                res.ActualTotalNumberOfVisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
                res.AplusAndAListed = UserAPlusAndA.Select(a => a.Id).Count();
                res.BListed = UserB.Select(a => a.Id).Count();
                res.CListed = UserC.Select(a => a.Id).Count();
                int APLusAndAVisited = 0;
                int BVisited = 0;
                int CVisited = 0;
                foreach (var aa in UserAPlusAndA)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == aa.Id).FirstOrDefault();
                    if (x != null)
                    {
                        APLusAndAVisited++;
                    }
                }
                foreach (var bb in UserB)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == bb.Id).FirstOrDefault();
                    if (x != null)
                    {
                        BVisited++;
                    }
                }
                foreach (var cc in UserC)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == cc.Id).FirstOrDefault();
                    if (x != null)
                    {
                        CVisited++;
                    }
                }
                res.APlusAndAVisited = APLusAndAVisited;
                res.BVisited = BVisited;
                res.CVisited = CVisited;
                res.SellingDaysInTheField = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Day).Select(a => a.Id).Count();
                res.WorkingDays = WorkingDaysdata - timeoffdays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitPerDayKpiTarget;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargerAchievmentWeight;
                res.APlusAndAWeight = AAndAPlusWegiht;
                res.BWeight = BWeight;
                res.CWeight = CWeight;
                res.SellingDaysInTheFieldWeight = SellingDaysInTheFieldWegiht;
                res.TimeOffDays = timeoffdays;
                ress.Add(res);
            }
            return ress.OrderBy(a => a.FullName);
        }

        public IEnumerable<SalesKpiModel> GetTeamSalesKpi(int year, int month, string managerId)
        {
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, days, 23, 59, 59);
            
            int? WorkingDaysData = db.workingDays.Where(a => a.Month == month && a.Year == year).Select(a => a.NumberOfWorkingDays).FirstOrDefault();
            List<ExtendIdentityUser> list = new List<ExtendIdentityUser>();


            IEnumerable<ExtendIdentityUser> u = userManager.Users.Where(a => a.extendidentityuserid == managerId);
            foreach (var item in u)
            {
                var x = userManager.IsInRoleAsync(item, "Sales Representative").Result;
                if (x)
                {
                    list.Add(item);
                }
            }
            List<SalesKpiModel> result = new List<SalesKpiModel>();

            foreach (var ress in list)
            {
                int? timeoffdays = GetTimeOffDiff(ress.Id, year, month, 1, year, month, days);
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
                int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
                int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
                int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

                int AverageVisitsPerDayKpiTarget = db.properties.Where(a => a.Id == 1).Select(a => a.Value).SingleOrDefault();

                

                string FullName = ress.FullName;

                string office = db.city.Where(a => a.Id == ress.CityId).Select(a => a.CityName).FirstOrDefault();

                string date = start.ToString("MMMM - yyyy");

                double ActualTotalNumberOfVisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();

                double ListedAccounts = db.userAccount.Where(a => a.extendidentityuserid == ress.Id).DistinctBy(x => x.AccountId).Count();

                double SellingDaysInTheField = db.accountSalesVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Day).Select(a => a.Id).Count();

                //double? TargetedNumberOfVisits = WorkingDays * AverageVisitsPerDayKpiTarget;

                double VisitedAccounts = db.accountSalesVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.AccountId).Select(a => a.Id).Count();

                //double AverageVisitsPerDayActual = (double)ActualTotalNumberOfVisits / (double)SellingDaysInTheField;

                SalesKpiModel res = new SalesKpiModel();
                res.FullName = FullName;
                res.Month = date;
                res.RoleName = "Sales Representative";

                res.CityName = office;
                res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
                res.ListedAccounts = ListedAccounts;
                res.SellingDaysInTheField = SellingDaysInTheField;
                //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
                res.VisitedAccounts = VisitedAccounts;
                res.WorkingDays = WorkingDaysData - timeoffdays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
                //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
                //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
                //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
                //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
                //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
                res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
                res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
                res.TimeOffDays = timeoffdays;
                //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
                //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
                //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
                //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
                //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
                result.Add(res);
            }
            return result.OrderBy(a => a.FullName);
        }

        public int GetTimeOffDiff(string id,int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
           
            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);
            var s = end - start;
            int days = s.Days;
            TimeSpan diff = new TimeSpan(0, 5, 0, 0, 0);
            int timeoffdays = db.userTimeOff.Where(a => a.ExtendIdentityUserId == id && a.DateTimeFrom >= start && a.DateTimeFrom <= end && a.DateTimeTo - a.DateTimeFrom >= diff && a.Accepted == true).DistinctBy(a=>a.DateTimeFrom.Date).Select(a => a.Id).Count();
   
            for (int i = 0; i <= days; i++)
            {
                DateTime d = start.AddDays(i).Date;
                IEnumerable<UserTimeOff> t = db.userTimeOff.Where(a => a.ExtendIdentityUserId == id && a.DateTimeFrom - a.DateTimeTo < diff && a.DateTimeFrom.Date == d.Date && a.Accepted == true).DistinctBy(a=>a.DateTimeFrom.Date);
                var tt = t.Count();
                if (tt > 1)
                {
                    
                    TimeSpan x = new TimeSpan(0, 0, 0, 0, 0);
                    foreach (var itemm in t)
                    {
                        x = x + (itemm.DateTimeTo - itemm.DateTimeFrom);
                    }
                    if (x >= diff)
                    {
                        timeoffdays++;
                    }


                }
            }
      
            return timeoffdays;
        }
    }
}
