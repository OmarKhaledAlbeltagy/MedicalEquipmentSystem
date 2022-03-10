using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class RepRep:IRepRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public RepRep(DbContainer db,ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public IEnumerable<RepAccountNotVisitsReport> AccountMedicalNotVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom, 0, 0, 0);
            DateTime to = new DateTime(yearto, monthto, dayto, 23, 59, 59);

            IEnumerable<UserAccount> myaccounts = db.userAccount.Where(a => a.extendidentityuserid == userId);
            IEnumerable<AccountMedicalVisit> myvisits = db.accountMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= from && a.VisitDate <= to);
            IEnumerable<AccountMedicalVisit> myvisitsdistinct = myvisits.DistinctBy(x => x.AccountId);




            List<RepAccountNotVisitsReport> notvisited = db.account.Join(db.userAccount, a => a.Id, b => b.AccountId, (a, b) => new
            {
                AccountId = a.Id,
                AccountName = a.AccountName,
                CategoryId = a.CategoryId,
                UserId = b.extendidentityuserid

            }).Where(a => a.UserId == userId).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new RepAccountNotVisitsReport
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = b.CategoryName
            }).ToList();

            foreach (var item in myvisitsdistinct)
            {
                List<RepAccountNotVisitsReport> list = notvisited.Where(a => a.AccountId == item.AccountId).ToList();

                foreach (var obj in list)
                {
                    notvisited.Remove(obj);
                }

                
            }
          
            return notvisited;
        }

        public IEnumerable<RepAccountVisitsReport> AccountMedicalVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom, 0, 0, 0);
            DateTime to = new DateTime(yearto, monthto, dayto, 23, 59, 59);

            IEnumerable<UserAccount> myaccounts = db.userAccount.Where(a => a.extendidentityuserid == userId);
            IEnumerable<AccountMedicalVisit> myvisits = db.accountMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= from && a.VisitDate <= to);
            IEnumerable<AccountMedicalVisit> myvisitsdistinct = myvisits.DistinctBy(x => x.AccountId);

            IEnumerable<RepAccountVisitsReport> res = myvisitsdistinct.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                AccountId = b.Id,
                AccountName = b.AccountName,
                CategoryId = b.CategoryId,
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = b.CategoryName
            }).Join(myaccounts, a => a.AccountId, b => b.AccountId, (a, b) => new RepAccountVisitsReport
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = a.CategoryName
            });

            List<RepAccountVisitsReport> ress = new List<RepAccountVisitsReport>();

            foreach (var item in res)
            {
                IList<AccountMedicalVisit> vis = myvisits.Where(a => a.AccountId == item.AccountId).OrderByDescending(a => a.VisitDate).ToList(); ;
                int visitcount = vis.Select(a => a.Id).Count();
                DateTime LastVisit = new DateTime(1,1,0001);
                DateTime BeforeLastVisit = new DateTime(1, 1, 0001);
                DateTime BeforeBeforeLastVisit = new DateTime(1, 1, 0001);
                DateTime BeforeBeforeBeforeLastVisit = new DateTime(1, 1, 0001);

                if (visitcount == 0)
                {
                    
                }
                else
                {
                    if (visitcount == 1)
                    {
                         LastVisit = vis[0].VisitDate;
                       
                    }
                    else
                    {
                        if (visitcount == 2)
                        {
                             LastVisit = vis[0].VisitDate;
                             BeforeLastVisit = vis[1].VisitDate;
                            
                        }
                        else
                        {
                            if (visitcount ==3)
                            {
                                 LastVisit = vis[0].VisitDate;
                                 BeforeLastVisit = vis[1].VisitDate;
                                 BeforeBeforeLastVisit = vis[2].VisitDate;
                            }
                            else
                            {
                                if (visitcount >= 4)
                                {
                                    LastVisit = vis[0].VisitDate;
                                    BeforeLastVisit = vis[1].VisitDate;
                                    BeforeBeforeLastVisit = vis[2].VisitDate;
                                    BeforeBeforeBeforeLastVisit = vis[3].VisitDate;
                                }
                            }
                        }
                    }
                }

                RepAccountVisitsReport obj = new RepAccountVisitsReport();

                obj.AccountId = item.AccountId;
                obj.AccountName = item.AccountName;
                obj.CategoryName = item.CategoryName;
                obj.VisitsCount = visitcount;
                obj.LastVisit = LastVisit;
                obj.BeforeLastVisit = BeforeLastVisit;
                obj.BeforeBeforeLastVisit = BeforeBeforeLastVisit;
                obj.BeforeBeforeBeforeLastVisit = BeforeBeforeBeforeLastVisit;
                ress.Add(obj);
            }

            return ress;
        }

        public IEnumerable<RepAccountNotVisitsReport> AccountSalesNotVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom, 0, 0, 0);
            DateTime to = new DateTime(yearto, monthto, dayto, 23, 59, 59);

            IEnumerable<UserAccount> myaccounts = db.userAccount.Where(a => a.extendidentityuserid == userId);
            IEnumerable<AccountSalesVisit> myvisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= from && a.VisitDate <= to);
            IEnumerable<AccountSalesVisit> myvisitsdistinct = myvisits.DistinctBy(x => x.AccountId);

            List<RepAccountNotVisitsReport> notvisited = db.account.Join(db.userAccount, a => a.Id, b => b.AccountId, (a, b) => new
            {
                AccountId = a.Id,
                AccountName = a.AccountName,
                CategoryId = a.CategoryId,
                UserId = b.extendidentityuserid

            }).Where(a => a.UserId == userId).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new RepAccountNotVisitsReport
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = b.CategoryName
            }).ToList();

            foreach (var item in myvisitsdistinct)
            {
                List<RepAccountNotVisitsReport> list = notvisited.Where(a => a.AccountId == item.AccountId).ToList();

                foreach (var obj in list)
                {
                    notvisited.Remove(obj);
                }
            }

            return notvisited;
        }

        public IEnumerable<RepAccountVisitsReport> AccountSalesVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom, 0, 0, 0);
            DateTime to = new DateTime(yearto, monthto, dayto, 23, 59, 59);

            IEnumerable<UserAccount> myaccounts = db.userAccount.Where(a => a.extendidentityuserid == userId);
            IEnumerable<AccountSalesVisit> myvisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= from && a.VisitDate <= to);
            IEnumerable<AccountSalesVisit> myvisitsdistinct = myvisits.DistinctBy(x => x.AccountId);

            IEnumerable<RepAccountVisitsReport> res = myvisitsdistinct.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                AccountId = b.Id,
                AccountName = b.AccountName,
                CategoryId = b.CategoryId,
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = b.CategoryName
            }).Join(myaccounts, a => a.AccountId, b => b.AccountId, (a, b) => new RepAccountVisitsReport
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = a.CategoryName
            });

            List<RepAccountVisitsReport> ress = new List<RepAccountVisitsReport>();

            foreach (var item in res)
            {
                IList<AccountSalesVisit> vis = myvisits.Where(a => a.AccountId == item.AccountId).OrderByDescending(a => a.VisitDate).ToList(); ;
                int visitcount = vis.Select(a=>a.Id).Count();
                DateTime LastVisit = new DateTime(1, 1, 0001);
                DateTime BeforeLastVisit = new DateTime(1, 1, 0001);
                DateTime BeforeBeforeLastVisit = new DateTime(1, 1, 0001);
                DateTime BeforeBeforeBeforeLastVisit = new DateTime(1, 1, 0001);
                if (visitcount == 0)
                {

                }
                else
                {
                    if (visitcount == 1)
                    {
                        LastVisit = vis[0].VisitDate;

                    }
                    else
                    {
                        if (visitcount == 2)
                        {
                            LastVisit = vis[0].VisitDate;
                            BeforeLastVisit = vis[1].VisitDate;

                        }
                        else
                        {
                            if (visitcount == 3)
                            {
                                LastVisit = vis[0].VisitDate;
                                BeforeLastVisit = vis[1].VisitDate;
                                BeforeBeforeLastVisit = vis[2].VisitDate;
                            }
                            else
                            {
                                if (visitcount >= 4)
                                {
                                    LastVisit = vis[0].VisitDate;
                                    BeforeLastVisit = vis[1].VisitDate;
                                    BeforeBeforeLastVisit = vis[2].VisitDate;
                                    BeforeBeforeBeforeLastVisit = vis[3].VisitDate;
                                }
                            }
                        }
                    }
                }

                RepAccountVisitsReport obj = new RepAccountVisitsReport();

                obj.AccountId = item.AccountId;
                obj.AccountName = item.AccountName;
                obj.CategoryName = item.CategoryName;
                obj.VisitsCount = visitcount;
                obj.LastVisit = LastVisit;
                obj.BeforeLastVisit = BeforeLastVisit;
                obj.BeforeBeforeLastVisit = BeforeBeforeLastVisit;
                obj.BeforeBeforeBeforeLastVisit = BeforeBeforeBeforeLastVisit;
                ress.Add(obj);
            }

            return ress;
        }

        public IEnumerable<RepContactNotVisitsReport> ContactMedicalNotVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom, 0, 0, 0);
            DateTime to = new DateTime(yearto, monthto, dayto, 23, 59, 59);

            IEnumerable<UserContact> mycontacts = db.userContact.Where(a => a.extendidentityuserid == userId);
            IEnumerable<ContactMedicalVisit> myvisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= from && a.VisitDate <= to);
            IEnumerable<ContactMedicalVisit> myvisitsdistinct = myvisits.DistinctBy(x => x.ContactId);

            List<RepContactNotVisitsReport> notvisited = db.contact.Join(db.userContact, a => a.Id, b => b.ContactId, (a, b) => new
            {
                ContactId = a.Id,
                ContactName = a.ContactName,
                AccountId = a.AccountId,
                CategoryId = b.CategoryId,
                UserId = b.extendidentityuserid,
                MonthlyTarget = b.MonthlyTarget
            }).Where(a => a.UserId == userId).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                AccountId = a.AccountId,
                CategoryName = b.CategoryName,
                MonthlyTarget = a.MonthlyTarget
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new RepContactNotVisitsReport
            {
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                CategoryName = a.CategoryName,
                AccountName = b.AccountName,
                MonthlyTarget = a.MonthlyTarget
            }).ToList();


            foreach (var item in myvisitsdistinct)
            {
                List<RepContactNotVisitsReport> list = notvisited.Where(a => a.ContactId == item.ContactId).ToList();

                foreach (var obj in list)
                {
                    notvisited.Remove(obj);
                }
            }

            return notvisited;
        }

        public IEnumerable<RepContactVisitsReport> ContactMedicalVisitReport(string userId,int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom,0,0,0);
            DateTime to = new DateTime(yearto,monthto,dayto,23,59,59);

            IEnumerable<UserContact> mycontacts = db.userContact.Where(a => a.extendidentityuserid == userId);
            IEnumerable<ContactMedicalVisit> myvisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= from && a.VisitDate <= to);
            IEnumerable<ContactMedicalVisit> myvisitsdistinct = myvisits.DistinctBy(x => x.ContactId);

            IEnumerable<RepContactVisitsReport> res = myvisitsdistinct.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                ContactId = b.Id,
                ContactName = b.ContactName,
                AccountId = b.AccountId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                AccountName = b.AccountName
            }).Join(mycontacts, a => a.ContactId, b => b.ContactId, (a, b) => new RepContactVisitsReport
            {
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                AccountName = a.AccountName,
                MonthlyTarget = b.MonthlyTarget
            });

            List<RepContactVisitsReport> ress = new List<RepContactVisitsReport>();

            foreach (var item in res)
            {
                IList<ContactMedicalVisit> vis = myvisits.Where(a => a.ContactId == item.ContactId).OrderByDescending(a => a.VisitDate).ToList(); ;
                int visitcount = vis.Select(a=>a.Id).Count();

                DateTime LastVisit = new DateTime(1, 1, 0001);
                DateTime BeforeLastVisit = new DateTime(1, 1, 0001);
                DateTime BeforeBeforeLastVisit = new DateTime(1, 1, 0001);
                DateTime BeforeBeforeBeforeLastVisit = new DateTime(1, 1, 0001);
                if (visitcount == 0)
                {

                }
                else
                {
                    if (visitcount == 1)
                    {
                        LastVisit = vis[0].VisitDate;

                    }
                    else
                    {
                        if (visitcount == 2)
                        {
                            LastVisit = vis[0].VisitDate;
                            BeforeLastVisit = vis[1].VisitDate;

                        }
                        else
                        {
                            if (visitcount == 3)
                            {
                                LastVisit = vis[0].VisitDate;
                                BeforeLastVisit = vis[1].VisitDate;
                                BeforeBeforeLastVisit = vis[2].VisitDate;
                            }
                            else
                            {
                                if (visitcount >= 4)
                                {
                                    LastVisit = vis[0].VisitDate;
                                    BeforeLastVisit = vis[1].VisitDate;
                                    BeforeBeforeLastVisit = vis[2].VisitDate;
                                    BeforeBeforeBeforeLastVisit = vis[3].VisitDate;
                                }
                            }
                        }
                    }
                }

                RepContactVisitsReport obj = new RepContactVisitsReport();

                obj.ContactId = item.ContactId;
                obj.ContactName = item.ContactName;
                obj.CategoryName = item.CategoryName;
                obj.AccountName = item.AccountName;
                obj.MonthlyTarget = item.MonthlyTarget;
                obj.VisitsCount = visitcount;
                obj.LastVisit = LastVisit;
                obj.BeforeLastVisit = BeforeLastVisit;
                obj.BeforeBeforeLastVisit = BeforeBeforeLastVisit;
                obj.BeforeBeforeBeforeLastVisit = BeforeBeforeBeforeLastVisit;
                ress.Add(obj);


            }

            return ress;

        }

        public DateTime GetVisitsDateLimit()
        {
            Properties x = db.properties.Find(14);
            DateTime l = ti.GetCurrentTime().AddDays(x.Value * -1);
            return (l);
            
        }

        public RepVisitedContacts VisitedAccountsByMonthSales(int year, int month, string userId)
        {
            
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, days, 23, 59, 59);
            int myaccounts = db.userAccount.Where(a => a.extendidentityuserid == userId).DistinctBy(b => b.AccountId).Select(c => c.Id).Count();
            int visited = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.AccountId).Select(b => b.Id).Count();
            int notvisited = myaccounts - visited;

            RepVisitedContacts res = new RepVisitedContacts();

            res.Visited = visited;
            res.NotVisited = notvisited;
            res.Total = myaccounts;

            return res;
        }

        public RepVisitedContacts VisitedContactsByMonth(int year, int month, string userId)
        {
          
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, days, 23, 59, 59);
            int mycontacts = db.userContact.Where(a => a.extendidentityuserid == userId).DistinctBy(b => b.ContactId).Select(c => c.Id).Count();
            int visited = db.contactMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.ContactId).Select(b => b.Id).Count();
            int notvisited = mycontacts - visited;

            RepVisitedContacts res = new RepVisitedContacts();

            res.Visited = visited;
            res.NotVisited = notvisited;
            res.Total = mycontacts;

            return res;
        }

        public IEnumerable<RepVisitedContactsByCategory> VisitedContactsThisMonthByCategory(string userId)
        {
            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).FirstOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).FirstOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).FirstOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).FirstOrDefault();
            int NOId = db.category.Where(a => a.CategoryName == "NO").Select(a => a.Id).FirstOrDefault();
            int thisyear = ti.GetCurrentTime().Year;
            int thismonth = ti.GetCurrentTime().Month;
            int thisdays = DateTime.DaysInMonth(thisyear, thismonth);
            DateTime start = new DateTime(thisyear, thismonth, 1, 0, 0, 0);
            DateTime end = new DateTime(thisyear, thismonth, thisdays, 0, 0, 0);

            var MyAPlusContacts = db.userContact.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                UserId = a.extendidentityuserid,
                ContactId = b.Id,
                CategoryId = a.CategoryId
            }).Where(a => a.UserId == userId && a.CategoryId == APlusId).DistinctBy(c => c.ContactId);

            int MyAPlusContactsCount = MyAPlusContacts.Select(d => d.ContactId).Count();
            /////
            var MyAContacts = db.userContact.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                UserId = a.extendidentityuserid,
                ContactId = b.Id,
                CategoryId = a.CategoryId
            }).Where(a => a.UserId == userId && a.CategoryId == AId).DistinctBy(c => c.ContactId);

            
            int MyAContactsCount = MyAContacts.Select(d => d.ContactId).Count();

            /////
            var MyBContacts = db.userContact.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                UserId = a.extendidentityuserid,
                ContactId = b.Id,
                CategoryId = a.CategoryId
            }).Where(a => a.UserId == userId && a.CategoryId == BId).DistinctBy(c => c.ContactId);

            int MyBContactsCount = MyBContacts.Select(d => d.ContactId).Count();

            //////
            var MyCContacts = db.userContact.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                UserId = a.extendidentityuserid,
                ContactId = b.Id,
                CategoryId = a.CategoryId
            }).Where(a => a.UserId == userId && a.CategoryId == CId).DistinctBy(c => c.ContactId);

            int MyCContactsCount = MyCContacts.Select(d => d.ContactId).Count();

            ///////

            var MyNOContacts = db.userContact.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                UserId = a.extendidentityuserid,
                ContactId = b.Id,
                CategoryId = a.CategoryId
            }).Where(a => a.UserId == userId && a.CategoryId == NOId).DistinctBy(c => c.ContactId);

            int MyNOContactsCount = MyNOContacts.Select(d => d.ContactId).Count();




            int MyAPlusVisited = MyAPlusContacts.Join(db.contactMedicalVisit, a => a.ContactId, b => b.ContactId, (a, b) => new
            {
                Id = b.Id,
                UserId = b.extendidentityuserid,
                VisitDate = b.VisitDate,
                ContactId = b.ContactId
            }).Where(a => a.UserId == userId && a.VisitDate >= start && a.VisitDate <= end).Select(c => c.Id).Count();

            int MyAVisited = MyAContacts.Join(db.contactMedicalVisit, a => a.ContactId, b => b.ContactId, (a, b) => new
            {
                Id = b.Id,
                UserId = b.extendidentityuserid,
                VisitDate = b.VisitDate,
                ContactId = b.ContactId
            }).Where(a => a.UserId == userId && a.VisitDate >= start && a.VisitDate <= end).Select(c => c.Id).Count();

            int MyBVisited = MyBContacts.Join(db.contactMedicalVisit, a => a.ContactId, b => b.ContactId, (a, b) => new
            {
                Id = b.Id,
                UserId = b.extendidentityuserid,
                VisitDate = b.VisitDate,
                ContactId = b.ContactId
            }).Where(a => a.UserId == userId && a.VisitDate >= start && a.VisitDate <= end).Select(c => c.Id).Count();

            int MyCVisited = MyCContacts.Join(db.contactMedicalVisit, a => a.ContactId, b => b.ContactId, (a, b) => new
            {
                Id = b.Id,
                UserId = b.extendidentityuserid,
                VisitDate = b.VisitDate,
                ContactId = b.ContactId
            }).Where(a => a.UserId == userId && a.VisitDate >= start && a.VisitDate <= end).Select(c => c.Id).Count();

            int MyNOVisited = MyNOContacts.Join(db.contactMedicalVisit, a => a.ContactId, b => b.ContactId, (a, b) => new
            {
                Id = b.Id,
                UserId = b.extendidentityuserid,
                VisitDate = b.VisitDate,
                ContactId = b.ContactId
            }).Where(a => a.UserId == userId && a.VisitDate >= start && a.VisitDate <= end).Select(c => c.Id).Count();

            int APlusNotVisited = MyAPlusContactsCount - MyAPlusVisited;
            int ANotVisited = MyAContactsCount - MyAVisited;
            int BNotVisited = MyBContactsCount - MyBVisited;
            int CNotVisited = MyCContactsCount - MyCVisited;
            int NONotVisited = MyNOContactsCount - MyNOVisited;


            List<RepVisitedContactsByCategory> res = new List<RepVisitedContactsByCategory>();

            RepVisitedContactsByCategory APlus = new RepVisitedContactsByCategory();
            APlus.Category = "A+";
            APlus.Visited = MyAPlusVisited;
            APlus.NotVisited = APlusNotVisited;
            APlus.Total = MyAPlusContactsCount;
            res.Add(APlus);

            RepVisitedContactsByCategory A = new RepVisitedContactsByCategory();
            A.Category = "A";
            A.Visited = MyAVisited;
            A.NotVisited = ANotVisited;
            A.Total = MyAContactsCount;
            res.Add(A);

            RepVisitedContactsByCategory B = new RepVisitedContactsByCategory();
            B.Category = "B";
            B.Visited = MyBVisited;
            B.NotVisited = BNotVisited;
            B.Total = MyBContactsCount;
            res.Add(B);

            RepVisitedContactsByCategory C = new RepVisitedContactsByCategory();
            C.Category = "C";
            C.Visited = MyCVisited;
            C.NotVisited = CNotVisited;
            C.Total = MyCContactsCount;
            res.Add(C);

            RepVisitedContactsByCategory NO = new RepVisitedContactsByCategory();
            NO.Category = "No Category";
            NO.Visited = MyNOVisited;
            NO.NotVisited = NONotVisited;
            NO.Total = MyNOContactsCount;
            res.Add(NO);

            return res;
        }

       
    }
}
