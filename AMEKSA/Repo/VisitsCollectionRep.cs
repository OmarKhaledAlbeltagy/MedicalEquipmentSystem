using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
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
    public class VisitsCollectionRep:IVisitsCollectionRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;

        public VisitsCollectionRep(DbContainer db, UserManager<ExtendIdentityUser> userManager,ITimeRep ti)
        {
            this.db = db;
            this.userManager = userManager;
            this.ti = ti;
        }

        public IEnumerable<CollectionByBrandModel> CollectionByBrandFirstLineManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string ManagerId)
        {
           
            DateTime from = new DateTime(yearfrom,monthfrom,dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto); 
            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == ManagerId);
            IEnumerable<Brand> brands = db.brand.Select(a=>a);

            List<CollectionByBrandModel> res = new List<CollectionByBrandModel>();
            
            foreach (var brand in brands) 
            {
                float? plan = 0;
                float? act = 0;

                foreach (var rep in myteam)
                {


                    var planned = db.accountMonthlyPlan.Where(a => a.PlannedDate >= from && a.PlannedDate <= to).Join(db.accountMonthlyPlanCollection, a => a.Id, b => b.AccountMonthlyPlanId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        Collection = b.Collection,
                        UserId = a.ExtendIdentityUserId
                    }).Join(brands, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        PlannedCollection = a.Collection,
                        UserId = a.UserId

                    }).Where(a => a.BrandId == brand.Id && a.UserId == rep.Id);




                    float actual = db.accountSalesVisit.Where(a => a.VisitDate >= from && a.VisitDate <= to && a.extendidentityuserid == rep.Id).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                {
                    BrandId = b.BrandId,
                    ActualCollection = b.Collection
                }).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
                {
                    BrandId = a.BrandId,
                    BrandName = b.BrandName,
                    ActualCollection = a.ActualCollection
                }).Where(a => a.BrandId == brand.Id).Select(a => a.ActualCollection).Sum();

                    plan = plan + planned.Select(a=>a.PlannedCollection).Sum();
                    act = act + actual;


                }

                CollectionByBrandModel obj = new CollectionByBrandModel();
                obj.BrandName = brand.BrandName;
                obj.PlannedCollection = plan;
                obj.ActualCollection = act;
                res.Add(obj);
            }
            return res;

        }

        public IEnumerable<CollectionByRepModel> CollectionByRepFirstLineManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string ManagerId)
        {
       
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == ManagerId);
            IEnumerable<Brand> brands = db.brand.Select(a => a);

            List<CollectionByRepModel> res = new List<CollectionByRepModel>();

            foreach (var rep in myteam)
            {
                CollectionByRepModel obj = new CollectionByRepModel();

                float? plan = 0;
                float? act = 0;
                List<CollectionByBrandModel> listt = new List<CollectionByBrandModel>();
                foreach (var brand in brands)
                {
                    var planned = db.accountMonthlyPlan.Where(a => a.PlannedDate >= from && a.PlannedDate <= to).Join(db.accountMonthlyPlanCollection, a => a.Id, b => b.AccountMonthlyPlanId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        Collection = b.Collection,
                        UserId = a.ExtendIdentityUserId
                    }).Join(brands, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        PlannedCollection = a.Collection,
                        UserId = a.UserId

                    }).Where(a => a.BrandId == brand.Id && a.UserId == rep.Id);



                    float actual = db.accountSalesVisit.Where(a => a.VisitDate >= from && a.VisitDate <= to && a.extendidentityuserid == rep.Id).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        ActualCollection = b.Collection
                    }).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        ActualCollection = a.ActualCollection
                    }).Where(a => a.BrandId == brand.Id).Select(a => a.ActualCollection).Sum();


                    CollectionByBrandModel list = new CollectionByBrandModel();
                    list.BrandName = brand.BrandName;
                    list.PlannedCollection = planned.Select(a => a.PlannedCollection).Sum();
                    list.ActualCollection = actual;
                    listt.Add(list);


                    plan = plan + planned.Select(a => a.PlannedCollection).Sum();
                    act = act + actual;


                }

                obj.FullName = rep.FullName;
                obj.ActualTotal = act;
                obj.PlannedTotal = plan;
                obj.list = listt;
                obj.UserId = rep.Id;
                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);

        }

        public IEnumerable<BrandAccountCollectionModel> BrandAccountCollectionByRep(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string UserId)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            ExtendIdentityUser me = userManager.FindByIdAsync(UserId).Result;
            IEnumerable<Brand> brands = db.brand.Select(a => a);

            List<Account> accountsss = new List<Account>();


            IEnumerable<Account> acc = db.accountMonthlyPlan.Where(a=>a.ExtendIdentityUserId == UserId && a.PlannedDate >= from && a.PlannedDate <= to).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new 
            {
                MonthlyPlanId = a.Id,
                Id = b.Id,
                accountbrandpayment = b.accountbrandpayment,
                accountmedicalvisit = b.accountmedicalvisit,
                AccountName = b.AccountName,
                accountsalesvisit = b.accountsalesvisit,
                accounttype = b.accounttype,
                AccountTypeId = b.AccountTypeId,
                Address = b.Address,
                BestTimeFrom = b.BestTimeFrom,
                BestTimeTo = b.BestTimeTo,
                category = b.category,
                CategoryId = b.CategoryId,
                contact = b.contact,
                district = b.district,
                DistrictId = b.DistrictId,
                Email = b.Email,
                NumberOfDoctors = b.NumberOfDoctors,
                PaymentNote = b.PaymentNote,
                PhoneNumber = b.PhoneNumber,
                purchasetype = b.purchasetype,
                PurchaseTypeId = b.PurchaseTypeId,
                RelationshipNote = b.RelationshipNote,
                useraccount = b.useraccount
            }).Join(db.accountMonthlyPlanCollection,a=>a.MonthlyPlanId,b=>b.AccountMonthlyPlanId,(a,b)=>new Account 
            {
                Id = a.Id,
                accountbrandpayment = a.accountbrandpayment,
                accountmedicalvisit = a.accountmedicalvisit,
                AccountName = a.AccountName,
                accountsalesvisit = a.accountsalesvisit,
                accounttype = a.accounttype,
                AccountTypeId = a.AccountTypeId,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                category = a.category,
                CategoryId = a.CategoryId,
                contact = a.contact,
                district = a.district,
                DistrictId = a.DistrictId,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                PaymentNote = a.PaymentNote,
                PhoneNumber = a.PhoneNumber,
                purchasetype = a.purchasetype,
                PurchaseTypeId = a.PurchaseTypeId,
                RelationshipNote = a.RelationshipNote,
                useraccount = a.useraccount
            }).DistinctBy(a => a.Id);

            IEnumerable<Account> accc = db.accountSalesVisitCollection.Join(db.accountSalesVisit, a => a.AccountSalesVisitId, b => b.Id, (a, b) => new
            {
                AccountId = b.AccountId,
                VisitDate = b.VisitDate,
                UserId = b.extendidentityuserid
            }).Where(a => a.UserId == UserId && a.VisitDate >= from && a.VisitDate <= to).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new Account
            {
                Id = b.Id,
                accountbrandpayment = b.accountbrandpayment,
                accountmedicalvisit = b.accountmedicalvisit,
                AccountName = b.AccountName,
                accountsalesvisit = b.accountsalesvisit,
                accounttype = b.accounttype,
                AccountTypeId = b.AccountTypeId,
                Address = b.Address,
                BestTimeFrom = b.BestTimeFrom,
                BestTimeTo = b.BestTimeTo,
                category = b.category,
                CategoryId = b.CategoryId,
                contact = b.contact,
                district = b.district,
                DistrictId = b.DistrictId,
                Email = b.Email,
                NumberOfDoctors = b.NumberOfDoctors,
                PaymentNote = b.PaymentNote,
                PhoneNumber = b.PhoneNumber,
                purchasetype = b.purchasetype,
                PurchaseTypeId = b.PurchaseTypeId,
                RelationshipNote = b.RelationshipNote,
                useraccount = b.useraccount
            }).DistinctBy(a=>a.Id);

            foreach (var item in acc)
            {
                accountsss.Add(item);
            }
            foreach (var item in accc)
            {
                accountsss.Add(item);
            }

            IEnumerable<Account> accounts = accountsss.DistinctBy(a => a.Id);



            List<BrandAccountCollectionModel> res = new List<BrandAccountCollectionModel>();
            foreach (var account in accounts)
            {

              
                foreach (var brand in brands)
                {
              
                    float? planned = db.accountMonthlyPlan
                        .Where(a => a.PlannedDate >= from && a.PlannedDate <= to && a.ExtendIdentityUserId == UserId && a.AccountId == account.Id)
                        .Join(db.accountMonthlyPlanCollection, a => a.Id, b => b.AccountMonthlyPlanId, (a, b) => new
                        {
                            BrandId = b.BrandId,
                            PlannedCollection = b.Collection
                        }).Where(a => a.BrandId == brand.Id).Select(a => a.PlannedCollection).Sum();

                    float actual = db.accountSalesVisit.Where(a => a.VisitDate >= from && a.VisitDate <= to && a.extendidentityuserid == UserId && a.AccountId == account.Id).
                        Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                        {
                            BrandId = b.BrandId,
                            ActualCollection = b.Collection
                        }).Where(a => a.BrandId == brand.Id).Select(a => a.ActualCollection).Sum();

                    

                    BrandAccountCollectionModel obj = new BrandAccountCollectionModel();
                    obj.AccountName = account.AccountName;
                    obj.BrandName = brand.BrandName;
                    obj.PlannedCollection = planned;
                    obj.ActualCollection = actual;
                    res.Add(obj);
                }
               
            }
            return res.OrderBy(a => a.BrandName).OrderBy(a=>a.AccountName);
        }

        public IEnumerable<SalesCollection> GetMyTeamCollection(string ManagerId)
        {
            DateTime thismonth = ti.GetCurrentTime();
            DateTime pastmonth = thismonth.AddMonths(-1);
            int thismonthdays = DateTime.DaysInMonth(thismonth.Year, thismonth.Month);
            int pastmonthdays = DateTime.DaysInMonth(pastmonth.Year, pastmonth.Month);
            DateTime thismonthstart = new DateTime(thismonth.Year, thismonth.Month, 1);
            DateTime thismonthend = new DateTime(thismonth.Year, thismonth.Month, thismonthdays);
            DateTime pastmonthstart = new DateTime(pastmonth.Year, pastmonth.Month, 1);
            DateTime pastmonthend = new DateTime(pastmonth.Year, pastmonth.Month, pastmonthdays);


            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == ManagerId);
            List<SalesCollection> res = new List<SalesCollection>();

            foreach (var item in myteam)
            {
                SalesCollection x = new SalesCollection();
                x.FullName = item.FullName;
                x.Collectionthismonth = db.accountSalesVisitCollection.Join(db.accountSalesVisit, a => a.AccountSalesVisitId, b => b.Id, (a, b) => new

                {
                    Collection = a.Collection,
                    UserId = b.extendidentityuserid,
                    VisitDate = b.VisitDate

                }).Where(a => a.UserId == item.Id && a.VisitDate >= thismonthstart && a.VisitDate <= thismonthend).Select(c => c.Collection).Sum();

                x.CollectionPastMonth = db.accountSalesVisitCollection.Join(db.accountSalesVisit, a => a.AccountSalesVisitId, b => b.Id, (a, b) => new

                {
                    Collection = a.Collection,
                    UserId = b.extendidentityuserid,
                    VisitDate = b.VisitDate

                }).Where(a => a.UserId == item.Id && a.VisitDate >= pastmonthstart && a.VisitDate <= pastmonthend).Select(c => c.Collection).Sum();


                res.Add(x);
            }

            return res;
        }

        public IEnumerable<CollectionByBrandModel> CollectionByBrandTopManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Sales Representative").Result;
            IEnumerable<Brand> brands = db.brand.Select(a => a);

            List<CollectionByBrandModel> res = new List<CollectionByBrandModel>();

            foreach (var brand in brands)
            {
                float? plan = 0;
                float? act = 0;

                foreach (var rep in myteam)
                {


                    var planned = db.accountMonthlyPlan.Where(a => a.PlannedDate >= from && a.PlannedDate <= to).Join(db.accountMonthlyPlanCollection, a => a.Id, b => b.AccountMonthlyPlanId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        Collection = b.Collection,
                        UserId = a.ExtendIdentityUserId
                    }).Join(brands, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        PlannedCollection = a.Collection,
                        UserId = a.UserId

                    }).Where(a => a.BrandId == brand.Id && a.UserId == rep.Id);




                    float actual = db.accountSalesVisit.Where(a => a.VisitDate >= from && a.VisitDate <= to && a.extendidentityuserid == rep.Id).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        ActualCollection = b.Collection
                    }).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        ActualCollection = a.ActualCollection
                    }).Where(a => a.BrandId == brand.Id).Select(a => a.ActualCollection).Sum();

                    plan = plan + planned.Select(a => a.PlannedCollection).Sum();
                    act = act + actual;


                }

                CollectionByBrandModel obj = new CollectionByBrandModel();
                obj.BrandName = brand.BrandName;
                obj.PlannedCollection = plan;
                obj.ActualCollection = act;
                res.Add(obj);
            }
            return res;
        }

        public IEnumerable<CollectionByRepModel> CollectionByRepTopManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Sales Representative").Result;
            IEnumerable<Brand> brands = db.brand.Select(a=>a);

            List<CollectionByRepModel> res = new List<CollectionByRepModel>();

            foreach (var rep in myteam)
            {
                CollectionByRepModel obj = new CollectionByRepModel();

                float? plan = 0;
                float? act = 0;
                List<CollectionByBrandModel> listt = new List<CollectionByBrandModel>();
                foreach (var brand in brands)
                {
                    var planned = db.accountMonthlyPlan.Where(a => a.PlannedDate >= from && a.PlannedDate <= to).Join(db.accountMonthlyPlanCollection, a => a.Id, b => b.AccountMonthlyPlanId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        Collection = b.Collection,
                        UserId = a.ExtendIdentityUserId
                    }).Join(brands, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        PlannedCollection = a.Collection,
                        UserId = a.UserId

                    }).Where(a => a.BrandId == brand.Id && a.UserId == rep.Id);



                    float actual = db.accountSalesVisit.Where(a => a.VisitDate >= from && a.VisitDate <= to && a.extendidentityuserid == rep.Id).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        ActualCollection = b.Collection
                    }).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        ActualCollection = a.ActualCollection
                    }).Where(a => a.BrandId == brand.Id).Select(a => a.ActualCollection).Sum();


                    CollectionByBrandModel list = new CollectionByBrandModel();
                    list.BrandName = brand.BrandName;
                    list.PlannedCollection = planned.Select(a => a.PlannedCollection).Sum();
                    list.ActualCollection = actual;
                    listt.Add(list);


                    plan = plan + planned.Select(a => a.PlannedCollection).Sum();
                    act = act + actual;


                }

                obj.FullName = rep.FullName;
                obj.ActualTotal = act;
                obj.PlannedTotal = plan;
                obj.list = listt;
                obj.UserId = rep.Id;
                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);
        }

        public CollectionByRepModel CollectionByRep(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string userId)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            ExtendIdentityUser myteam = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.Id == userId).SingleOrDefault();
            IEnumerable<Brand> brands = db.brand.Select(a => a);

            CollectionByRepModel res = new CollectionByRepModel();

           

                float? plan = 0;
                float? act = 0;
                List<CollectionByBrandModel> listt = new List<CollectionByBrandModel>();
                foreach (var brand in brands)
                {
                    var planned = db.accountMonthlyPlan.Where(a => a.PlannedDate >= from && a.PlannedDate <= to).Join(db.accountMonthlyPlanCollection, a => a.Id, b => b.AccountMonthlyPlanId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        Collection = b.Collection,
                        UserId = a.ExtendIdentityUserId
                    }).Join(brands, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        PlannedCollection = a.Collection,
                        UserId = a.UserId

                    }).Where(a => a.BrandId == brand.Id && a.UserId == userId);



                    float actual = db.accountSalesVisit.Where(a => a.VisitDate >= from && a.VisitDate <= to && a.extendidentityuserid == userId).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        ActualCollection = b.Collection
                    }).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
                    {
                        BrandId = a.BrandId,
                        BrandName = b.BrandName,
                        ActualCollection = a.ActualCollection
                    }).Where(a => a.BrandId == brand.Id).Select(a => a.ActualCollection).Sum();


                    CollectionByBrandModel list = new CollectionByBrandModel();
                    list.BrandName = brand.BrandName;
                    list.PlannedCollection = planned.Select(a => a.PlannedCollection).Sum();
                    list.ActualCollection = actual;
                    listt.Add(list);


                    plan = plan + planned.Select(a => a.PlannedCollection).Sum();
                    act = act + actual;


                }

                res.FullName = myteam.FullName;
                res.ActualTotal = act;
                res.PlannedTotal = plan;
                res.list = listt;
                res.UserId = userId;
               
         
            return res;
        }

        public IEnumerable<AccountBalanceModel> AccountByBalanceFirstLineManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, SearchByWord s)
        {
            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);

            string normalized = s.Word.Normalize().ToUpper();


            List<int> AccountsIds = db.Users.Where(a => a.extendidentityuserid == s.ManagerId).Join(db.userAccount, a => a.Id, b => b.extendidentityuserid, (a, b) => new
            {
                AccountId = b.AccountId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new 
            {
            Id = a.AccountId,
            AccountName = b.AccountName
            }).Where(a=>a.AccountName.Contains(normalized)).Select(a => a.Id).ToList();


            if (AccountsIds == null)
            {
                return null;
            }
      

            List<AccountBalanceModel> res = new List<AccountBalanceModel>();
            foreach (var item in AccountsIds)
            {
                

                

                AccountBalanceModel obj = new AccountBalanceModel();
                obj.Id = item;
                obj.AccountName = db.account.Where(a => a.Id == item).Select(a => a.AccountName).FirstOrDefault();
                obj.TotalBalance = db.accountBalance.Where(a => a.AccountId == item).Select(a => a.Balance).Sum();
                obj.TotalCollection = db.accountSalesVisit.Where(a => a.AccountId == item && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                {
                    Collection = b.Collection
                }).Select(a => a.Collection).Sum();

                obj.CreditLimit = db.account.Where(a => a.Id == item).Select(a => a.CreditLimit).FirstOrDefault();
                obj.Residual = obj.CreditLimit - obj.TotalBalance;
               
                res.Add(obj);
            }

            return res.DistinctBy(a=>a.Id).OrderBy(a => a.AccountName);

        }

        public IEnumerable<AccountBalanceModel> AccountByBalanceTopManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, SearchByWord s)
        {
            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);
            string normalized = s.Word.Normalize().ToUpper();

            List<int> AccountsIds = db.account.Where(a=>a.AccountName.Contains(normalized)).Select(a => a.Id).ToList();
            if (AccountsIds == null)
            {
                return null;
            }
            List<AccountBalanceModel> res = new List<AccountBalanceModel>();
            foreach (var item in AccountsIds)
            {
               

                

                AccountBalanceModel obj = new AccountBalanceModel();
                obj.Id = item;
                obj.AccountName = db.account.Where(a => a.Id == item).Select(a => a.AccountName).FirstOrDefault();
                obj.TotalBalance = db.accountBalance.Where(a => a.AccountId == item).Select(a => a.Balance).Sum();
                obj.TotalCollection = db.accountSalesVisit.Where(a => a.AccountId == item && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                {
                    Collection = b.Collection
                }).Select(a => a.Collection).Sum();
                obj.CreditLimit = db.account.Where(a => a.Id == item).Select(a => a.CreditLimit).FirstOrDefault();
                obj.Residual = obj.CreditLimit - obj.TotalBalance;
              
                res.Add(obj);
            }

            return res.OrderBy(a => a.AccountName);
        }

        public IEnumerable<AccountBalanceModel> AccountByBalanceRep(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string UserId)
        {
            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);

            List<int> AccountsIds = db.userAccount.Where(a => a.extendidentityuserid == UserId).Select(a => a.AccountId).ToList();
            if (AccountsIds == null)
            {
                return null;
            }
            List<AccountBalanceModel> res = new List<AccountBalanceModel>();
            foreach (var item in AccountsIds)
            {
                

                AccountBalanceModel obj = new AccountBalanceModel();
                obj.Id = item;
                obj.AccountName = db.account.Where(a => a.Id == item).Select(a => a.AccountName).FirstOrDefault();
                obj.TotalBalance = db.accountBalance.Where(a => a.AccountId == item).Select(a => a.Balance).Sum();
                obj.TotalCollection = db.accountSalesVisit.Where(a => a.AccountId == item && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                {
                    Collection = b.Collection
                }).Select(a => a.Collection).Sum();
                obj.CreditLimit = db.account.Where(a => a.Id == item).Select(a => a.CreditLimit).FirstOrDefault();
                obj.Residual = obj.CreditLimit - obj.TotalBalance;
               
                res.Add(obj);
            }

            return res.OrderBy(a => a.AccountName);
        }

        public AccountBalanceModel AccountBalanceAccount(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, int AccountId)
        {
            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);

            AccountBalanceModel obj = new AccountBalanceModel();

            List<int> collectionbrands = db.accountSalesVisit.Where(a => a.AccountId == AccountId && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
            {
                BrandId = b.BrandId
            }).Select(a => a.BrandId).ToList();

            List<int> balancebrands = db.accountBalance.Where(a => a.AccountId == AccountId && a.Balance != 0).Select(a => a.BrandId).ToList();

            obj.Id = AccountId;
            obj.AccountName = db.account.Where(a => a.Id == AccountId).Select(a => a.AccountName).FirstOrDefault();
            obj.TotalBalance = db.accountBalance.Where(a => a.AccountId == AccountId).Select(a => a.Balance).Sum();
            obj.TotalCollection = db.accountSalesVisit.Where(a => a.AccountId == AccountId && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
            {
                Collection = b.Collection
            }).Select(a => a.Collection).Sum();
            obj.CreditLimit = db.account.Where(a => a.Id == AccountId).Select(a => a.CreditLimit).FirstOrDefault();
            obj.Residual = obj.CreditLimit - obj.TotalBalance;
            List<CollectionByBrandModel> br = new List<CollectionByBrandModel>();

            foreach (var id in collectionbrands)
            {
                CollectionByBrandModel h = db.accountSalesVisit.Where(a => a.AccountId == AccountId && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                {
                    BrandId = b.BrandId,
                    Collection = b.Collection
                }).Where(a => a.BrandId == id).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new CollectionByBrandModel
                {
                    BrandName = b.BrandName,
                    ActualCollection = a.Collection
                }).FirstOrDefault();
                br.Add(h);
            }

            List<BalanceByBrandModel> bc = new List<BalanceByBrandModel>();

            foreach (var id in balancebrands)
            {
                BalanceByBrandModel h = db.accountBalance.Where(a => a.AccountId == AccountId && a.BrandId == id).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new BalanceByBrandModel
                {
                    BrandName = b.BrandName,
                    Balance = a.Balance
                }).FirstOrDefault();
                bc.Add(h);
            }
            obj.BalanceByBrand = bc;
            obj.CollectionByBrand = br;

            return obj;
        }

        public IEnumerable<AccountBalanceModel> AccountByBalanceRepExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string UserId)
        {
            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);

            List<int> AccountsIds = db.userAccount.Where(a => a.extendidentityuserid == UserId).Select(a => a.AccountId).ToList();
           
            List<AccountBalanceModel> res = new List<AccountBalanceModel>();
            foreach (var item in AccountsIds)
            {
                List<int> collectionbrands = db.accountSalesVisit.Where(a => a.AccountId == item && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                {
                    BrandId = b.BrandId
                }).Select(a => a.BrandId).ToList();



                List<int> balancebrands = db.accountBalance.Where(a => a.AccountId == item && a.Balance != 0).Select(a => a.BrandId).ToList();

                AccountBalanceModel obj = new AccountBalanceModel();
                obj.Id = item;
                obj.AccountName = db.account.Where(a => a.Id == item).Select(a => a.AccountName).FirstOrDefault();
                obj.TotalBalance = db.accountBalance.Where(a => a.AccountId == item).Select(a => a.Balance).Sum();
                obj.TotalCollection = db.accountSalesVisit.Where(a => a.AccountId == item && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                {
                    Collection = b.Collection
                }).Select(a => a.Collection).Sum();
                obj.CreditLimit = db.account.Where(a => a.Id == item).Select(a => a.CreditLimit).FirstOrDefault();
                obj.Residual = obj.CreditLimit - obj.TotalBalance;
                List<CollectionByBrandModel> br = new List<CollectionByBrandModel>();

                foreach (var id in collectionbrands)
                {
                    CollectionByBrandModel h = db.accountSalesVisit.Where(a => a.AccountId == item && a.VisitDate >= start && a.VisitDate <= end).Join(db.accountSalesVisitCollection, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
                    {
                        BrandId = b.BrandId,
                        Collection = b.Collection
                    }).Where(a => a.BrandId == id).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new CollectionByBrandModel
                    {
                        BrandName = b.BrandName,
                        ActualCollection = a.Collection
                    }).FirstOrDefault();
                    br.Add(h);
                }

                List<BalanceByBrandModel> bc = new List<BalanceByBrandModel>();

                foreach (var id in balancebrands)
                {
                    BalanceByBrandModel h = db.accountBalance.Where(a => a.AccountId == item && a.BrandId == id).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new BalanceByBrandModel
                    {
                        BrandName = b.BrandName,
                        Balance = a.Balance
                    }).FirstOrDefault();
                    bc.Add(h);
                }
                obj.BalanceByBrand = bc;
                obj.CollectionByBrand = br;
                res.Add(obj);
            }

            return res.OrderBy(a => a.AccountName);
        }
    }
}
