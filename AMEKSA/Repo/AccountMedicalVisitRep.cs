using AMEKSA.AccountMedicalVisitModels;
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
    public class AccountMedicalVisitRep:IAccountMedicalVisitRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public AccountMedicalVisitRep(DbContainer db,ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public void AddPersonsToExistVisit(IEnumerable<AccountMedicalVisitPerson> list)
        {
            db.accountMedicalVisitPerson.AddRange(list);
            db.SaveChanges();
        }

        public void AddProductsToExistVisit(IEnumerable<AccountMedicalVisitProducts> list)
        {
            db.accountMedicalVisitProducts.AddRange(list);
            db.SaveChanges();
        }

        public void EditVisit(AccountMedicalVisit obj)
        {
            AccountMedicalVisit oldVisit = db.accountMedicalVisit.Find(obj.Id);

            oldVisit.VisitDate = obj.VisitDate;
            oldVisit.VisitTime = obj.VisitTime;
            oldVisit.AdditionalNotes = obj.AdditionalNotes;
            oldVisit.VisitNotes = obj.VisitNotes;
            db.SaveChanges();

            foreach (var item in obj.accountmedicalvisitperson)
            {
                AccountMedicalVisitPerson old = db.accountMedicalVisitPerson.Find(item.Id);
                old.PersonName = item.PersonName;
                old.PersonPosition = item.PersonPosition;
                db.SaveChanges();
            }

            foreach (var item in obj.accountmedicalvisitproducts)
            {
                AccountMedicalVisitProducts old = db.accountMedicalVisitProducts.Find(item.Id);
                old.ProductId = item.ProductId;
                db.SaveChanges();
            }
        }

        public CustomAccountMedicalVisit GetVisitById(int visitId)
        {
            var visitsids = db.accountMedicalVisit.Where(a => a.Id == visitId).Select(a => a.Id);

            List<CustomAccountMedicalVisitProducts> productlist = new List<CustomAccountMedicalVisitProducts>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visitsids)
            {
                var s = db.accountMedicalVisitProducts.Where(a => a.AccountMedicalVisitId == item).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    AccountMedicalVisitId = a.AccountMedicalVisitId,
                });
                
                foreach (var product in s)
                {
                    CustomAccountMedicalVisitProducts ad = new CustomAccountMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.AccountMedicalVisitId = product.AccountMedicalVisitId;
                    productlist.Add(ad);
                }

                var ss = db.accountMedicalVisitPerson.Where(a => a.AccountMedicalVisitId == item);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountMedicalVisitId;
                    personlist.Add(ab);
                }

            }




            CustomAccountMedicalVisit result = db.accountMedicalVisit.Where(n => n.Id == visitId).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                UserId = a.extendidentityuserid,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                Address = b.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = b.DistrictId,
                PhoneNumber = b.PhoneNumber,
                Email = b.Email,
                NumberOfDoctors = b.NumberOfDoctors,
                PurchaseTypeId = b.PurchaseTypeId,
                CategoryId = b.CategoryId
            }).Join(db.accountMedicalVisitProducts, a => a.Id, b => b.AccountMedicalVisitId, (a, b) => new
            {
                Id = a.Id,
                UserId = a.UserId,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                UserId = a.UserId,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                UserId = a.UserId,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                product = productlist.Where(x => x.AccountMedicalVisitId == a.Id),
                person = personlist.Where(c => c.KeyId == a.Id),
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AccountTypeName = b.AccountTypeName,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                UserId = a.UserId,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                product = productlist.Where(x => x.AccountMedicalVisitId == a.Id),
                person = personlist.Where(c => c.KeyId == a.Id),
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AccountTypeName = a.AccountTypeName,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId,
                CategoryName = b.CategoryName
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccountMedicalVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                UserId = a.UserId,
                AccountName = a.AccountName,
                product = productlist.Where(x => x.AccountMedicalVisitId == a.Id),
                person = personlist.Where(c => c.KeyId == a.Id),
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AccountTypeName = a.AccountTypeName,
                CategoryName = a.CategoryName,
                PurchaseTypeName = b.PurchaseTypeName
            }).DistinctBy(o => o.Id).SingleOrDefault();

          return result;

        }

        public IEnumerable<CustomAccountMedicalVisit> GetMyVisitsByDate(AccountSalesVisitByDateModel obj)
        {
            IEnumerable<AccountMedicalVisit> visits = db.accountMedicalVisit.Where(a => a.extendidentityuserid == obj.UserId && a.VisitDate >= obj.Start && a.VisitDate <= obj.End);
           
            var visitsids = db.accountMedicalVisit.Where(a => a.extendidentityuserid == obj.UserId && a.VisitDate >= obj.Start && a.VisitDate <= obj.End).Select(a => a.Id);

            List<CustomAccountMedicalVisitProducts> productlist = new List<CustomAccountMedicalVisitProducts>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visitsids)
            {
                var s = db.accountMedicalVisitProducts.Where(a => a.AccountMedicalVisitId == item).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    AccountMedicalVisitId = a.AccountMedicalVisitId
                });
                foreach (var product in s)
                {
                    CustomAccountMedicalVisitProducts ad = new CustomAccountMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.AccountMedicalVisitId = product.AccountMedicalVisitId;
                    productlist.Add(ad);
                }

                var ss = db.accountMedicalVisitPerson.Where(a => a.AccountMedicalVisitId == item);
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
                DistrictId = b.DistrictId
            }).Join(db.accountMedicalVisitProducts, a => a.Id, b => b.AccountMedicalVisitId, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                product = productlist.Where(x => x.AccountMedicalVisitId == a.Id),
                person = personlist.Where(c => c.KeyId == a.Id),
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                product = productlist.Where(x => x.AccountMedicalVisitId == a.Id),
                persons = personlist.Where(c => c.KeyId == a.Id),
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new CustomAccountMedicalVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                product = productlist.Where(x => x.AccountMedicalVisitId == a.Id),
                person = personlist.Where(c => c.KeyId == a.Id),
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate);

            List<CustomAccountMedicalVisit> res = new List<CustomAccountMedicalVisit>();
            foreach (var item in result)
            {
                RequestDeleteAccountMedical check = db.requestDeleteAccountMedical.Where(a => a.AccountMedicalVisitId == item.Id).FirstOrDefault();
                if (check == null)
                {
                    item.Requested = false;
                }
                else
                {
                    item.Requested = true;
                }
                res.Add(item);
            }


            return res;



         
        }

        public bool MakeVisit(AccountMedicalVisitModel obj)
        {

            DateTime datenow = ti.GetCurrentTime();
            int monthdays = DateTime.DaysInMonth(obj.VisitDate.Year, obj.VisitDate.Month);
            DateTime planstart = new DateTime(obj.VisitDate.Year, obj.VisitDate.Month, 1);
            DateTime planend = ti.GetCurrentTime().Date;

            AccountMedicalVisit visit = new AccountMedicalVisit();
            visit.VisitDate = obj.VisitDate;
            visit.VisitTime = obj.VisitTime;
            visit.SubmittingDate = datenow;
            visit.SubmittingTime = datenow;
            visit.AccountId = obj.AccountId;
            visit.extendidentityuserid = obj.extendidentityuserid;
            visit.VisitNotes = obj.VisitNotes;
            visit.AdditionalNotes = obj.AdditionalNotes;
            db.accountMedicalVisit.Add(visit);
            db.SaveChanges();

            foreach (var item in obj.ProductModel)
            {
                AccountMedicalVisitProducts products = new AccountMedicalVisitProducts();
                products.ProductId = item.ProductId;
                products.AccountMedicalVisitId = visit.Id;
                db.accountMedicalVisitProducts.Add(products);
                db.SaveChanges();
            }

            foreach (var item in obj.PersonModel)
            {
                AccountMedicalVisitPerson persons = new AccountMedicalVisitPerson();
                persons.PersonName = item.PersonName;
                persons.PersonPosition = item.PersonPosition;
                persons.AccountMedicalVisitId = visit.Id;
                db.accountMedicalVisitPerson.Add(persons);
                db.SaveChanges();
            }

            AccountMonthlyPlan todayplan = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == obj.extendidentityuserid && a.AccountId == obj.AccountId && a.Date == obj.VisitDate && a.Status == false).FirstOrDefault();
            if (todayplan != null)
            {
                todayplan.Status = true;
                todayplan.AccountMedicalVisitId = visit.Id;
                db.SaveChanges();
                return true;
            }
            else
            {
                AccountMonthlyPlan plan = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == obj.extendidentityuserid && a.AccountId == obj.AccountId && a.Date >= planstart && a.Date <= planend && a.Status == false).OrderBy(b => b.PlannedDate).FirstOrDefault();
                if (plan != null)
                {
                    plan.Status = true;
                    plan.AccountMedicalVisitId = visit.Id;
                    db.SaveChanges();
                }
                return true;
            }
        }

        public bool RequestDeleteAccountMedical(int VisitId)
        {
            RequestDeleteAccountMedical obj = new RequestDeleteAccountMedical();
            obj.AccountMedicalVisitId = VisitId;
            db.requestDeleteAccountMedical.Add(obj);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomAccountMedicalVisit> GetAMVDeleteRequests()
        {
            IEnumerable<CustomAccountMedicalVisit> results = db.requestDeleteAccountMedical.Join(db.accountMedicalVisit, a => a.AccountMedicalVisitId, b => b.Id, (a, b) => new
            {
                Id = b.Id,
                VisitDate = b.VisitDate,
                AccountId = b.AccountId,
                UserId = b.extendidentityuserid
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                VisitDate = a.VisitDate,
                AccountName = b.AccountName,
                UserId = a.UserId
            }).Join(db.Users, a => a.UserId, b => b.Id, (a, b) => new CustomAccountMedicalVisit
            {
                Id = a.Id,
                VisitDate = a.VisitDate,
                AccountName = a.AccountName,
                UserName = b.FullName
            });

            return results;
        }

        public bool ConfirmAMVDeleting(int visitid)
        {

            AccountMonthlyPlan plan = db.accountMonthlyPlan.Where(a => a.AccountMedicalVisitId == visitid).FirstOrDefault();

            if (plan == null)
            {

            }
            else
            {
                plan.AccountMedicalVisitId = null;
                plan.Status = false;
                db.SaveChanges();
            }

          IEnumerable<AccountMedicalVisitProducts> visitproducts =  db.accountMedicalVisitProducts.Where(a => a.AccountMedicalVisitId == visitid);

            foreach (var item in visitproducts)
            {
                db.accountMedicalVisitProducts.Remove(item);
            }
            IEnumerable<AccountMedicalVisitPerson> visitpersons = db.accountMedicalVisitPerson.Where(a => a.AccountMedicalVisitId == visitid);
            foreach (var item in visitpersons)
            {
                db.accountMedicalVisitPerson.Remove(item);
            }
            IEnumerable<RequestDeleteAccountMedical> request = db.requestDeleteAccountMedical.Where(a => a.AccountMedicalVisitId == visitid);

            foreach (var item in request)
            {
                db.requestDeleteAccountMedical.Remove(item);
            }

            AccountMedicalVisit visit = db.accountMedicalVisit.Find(visitid);
            db.accountMedicalVisit.Remove(visit);
            db.SaveChanges();
            return true;
        }

        public bool RejectAMVDeleting(int visitid)
        {
            IEnumerable<RequestDeleteAccountMedical> requests = db.requestDeleteAccountMedical.Where(a => a.AccountMedicalVisitId == visitid);
            foreach (var item in requests)
            {
                db.requestDeleteAccountMedical.Remove(item);
            }
            db.SaveChanges();
            return true;
        }
    }
}
