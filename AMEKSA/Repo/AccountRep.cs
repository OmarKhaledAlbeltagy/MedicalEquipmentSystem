using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;

namespace AMEKSA.Repo
{
    public class AccountRep: IAccountRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public AccountRep(DbContainer db, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public bool AddAccount(AccountModel obj)
        {


            Account res = new Account();
            res.AccountName = obj.AccountName;
            res.AccountTypeId = obj.AccountTypeId;
            res.Address = obj.Address;
            res.PhoneNumber = obj.PhoneNumber;
            res.DistrictId = obj.DistrictId;
            res.Email = obj.Email;
            res.NumberOfDoctors = obj.NumberOfDoctors;
            res.PurchaseTypeId = obj.PurchaseTypeId;
            res.PaymentNote = obj.PaymentNote;
            res.BestTimeFrom = obj.BestTimeFrom;
            res.BestTimeTo  = obj.BestTimeTo ;
            res.RelationshipNote = obj.RelationshipNote;
            res.CategoryId = obj.CategoryId;
            res.CreditLimit = 0;
            db.account.Add(res);
            List<int> brandids = db.brand.Select(a => a.Id).ToList();
            foreach (var item in brandids)
            {
                AccountBalance o = new AccountBalance();
                o.AccountId = res.Id;
                o.BrandId = item;
                o.Balance = 0;
                db.accountBalance.Add(o);
            }
            db.SaveChanges();
            return true;
        }

        public bool DeleteAccount(int id)
        {
            IEnumerable<AccountMedicalVisit> MV = db.accountMedicalVisit.Where(a => a.AccountId == id);
            foreach (var item in MV)
            {
                IEnumerable<AccountMedicalVisitPerson> MVP = db.accountMedicalVisitPerson.Where(a => a.AccountMedicalVisitId == item.Id);
                IEnumerable<AccountMedicalVisitProducts> MVPs = db.accountMedicalVisitProducts.Where(a => a.AccountMedicalVisitId == item.Id);

                foreach (var person in MVP)
                {
                    db.accountMedicalVisitPerson.Remove(person);
                    ;
                }
                foreach (var product in MVPs)
                {
                    db.accountMedicalVisitProducts.Remove(product);
                  
                }
                db.accountMedicalVisit.Remove(item);
               
            }
            IEnumerable<AccountSalesVisit> SV = db.accountSalesVisit.Where(a => a.AccountId == id);
            foreach (var item in SV)
            {
               IEnumerable<AccountSalesVisitBrand> SVB = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == item.Id);
                foreach (var brand in SVB)
                {
                    db.accountSalesVisitBrand.Remove(brand);
                   
                }
                IEnumerable<AccountSalesVisitPerson> SVP = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == item.Id);
                foreach (var person in SVP)
                {
                    db.accountSalesVisitPerson.Remove(person);
                    ;
                }
                db.accountSalesVisit.Remove(item);
                
            }

            IEnumerable<UserAccount> UA = db.userAccount.Where(a => a.AccountId == id);
            foreach (var item in UA)
            {
                db.userAccount.Remove(item);
               
            }
            IEnumerable<AccountBrandPayment> ABP = db.accountBrandPayment.Where(a => a.AccountId == id);
            foreach (var payment in ABP)
            {
                db.accountBrandPayment.Remove(payment);
               
            }
            IEnumerable<Contact> contacts = db.contact.Where(a => a.AccountId == id);
            foreach (var item in contacts)
            {
                item.AccountId = null;
            }
           Account acc = db.account.Find(id);

            db.account.Remove(acc);
            db.SaveChanges();

            return true;
        }

        public CustomAccount GetAccountById(int id)
        {
            Account acc = db.account.Find(id);
            int CityId = db.district.Where(a => a.Id == acc.DistrictId).Select(a => a.CityId).First();
            int medicalvisits = db.accountMedicalVisit.Where(a => a.AccountId == id).Count();
            int salesvisits = db.accountSalesVisit.Where(a => a.AccountId == id).Count();
            IEnumerable<string> firstlinenames = db.userAccount.Where(a => a.AccountId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new
            {
                ManagerId = b.extendidentityuserid
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Name = b.FullName
            }).Select(a => a.Name);
            IEnumerable<string> medicals = userManager.GetUsersInRoleAsync("Medical Representative").Result.Select(a=>a.Id);
            List<string> medicalids = new List<string>();
            foreach (var item in medicals)
            {
                string m = db.userAccount.Where(a => a.AccountId == id && a.extendidentityuserid == item).Select(a => a.extendidentityuserid).FirstOrDefault();
                if (m == "" || m == null)
                {

                }
                else
                {
                    medicalids.Add(m);
                }
                
            }
            
            List<string> medicalnames = new List<string>();
            foreach (var item in medicalids)
            {
                string username = db.Users.Where(a => a.Id == item).Select(a => a.FullName).SingleOrDefault();
                medicalnames.Add(username);
            }
            IEnumerable<string> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.Select(a => a.Id);
            List<string> salesids = new List<string>();
            foreach (var item in sales)
            {
                string s = db.userAccount.Where(a => a.AccountId == id && a.extendidentityuserid == item).Select(a => a.extendidentityuserid).FirstOrDefault();
                if (s == null || s == "")
                {

                }
                else
                {
                    salesids.Add(s);
                }
                
            }
            List<string> salesnames = new List<string>();
            foreach (var item in salesids)
            {
                string username = db.Users.Where(a => a.Id == item).Select(a => a.FullName).SingleOrDefault();
                salesnames.Add(username);
            }

            IEnumerable<string> contacts = db.contact.Where(a => a.AccountId == id).Select(a => a.ContactName);

            return db.account.Where(a => a.Id == id)
                .Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) =>
                new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    NumberOfDoctors = a.NumberOfDoctors,
                    PaymentNote = a.PaymentNote,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    RelationshipNote = a.RelationshipNote,
                    AccountTypeId = b.Id,
                    AccountTypeName = b.AccountTypeName,
                    DistrictId = a.DistrictId,
                    PurchaseTypeId = a.PurchaseTypeId,
                    CategoryId = a.CategoryId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) =>
                       new
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           Address = a.Address,
                           PhoneNumber = a.PhoneNumber,
                           Email = a.Email,
                           NumberOfDoctors = a.NumberOfDoctors,
                           PaymentNote = a.PaymentNote,
                           BestTimeFrom = a.BestTimeFrom,
                           BestTimeTo = a.BestTimeTo,
                           RelationshipNote = a.RelationshipNote,
                           AccountTypeId = a.AccountTypeId,
                           AccountTypeName = a.AccountTypeName,
                           DistrictId = a.DistrictId,
                           DistrictName = b.DistrictName,
                           CityId = b.CityId,
                           PurchaseTypeId = a.PurchaseTypeId,
                           CategoryId = a.CategoryId
                       }).Join(db.city, a => a.CityId, b => b.Id, (a, b) =>
                       new
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           Address = a.Address,
                           PhoneNumber = a.PhoneNumber,
                           Email = a.Email,
                           NumberOfDoctors = a.NumberOfDoctors,
                           PaymentNote = a.PaymentNote,
                           BestTimeFrom = a.BestTimeFrom,
                           BestTimeTo = a.BestTimeTo,
                           RelationshipNote = a.RelationshipNote,
                           AccountTypeId = a.AccountTypeId,
                           AccountTypeName = a.AccountTypeName,
                           DistrictId = a.DistrictId,
                           DistrictName = a.DistrictName,
                           CityId = a.CityId,
                           CityName = b.CityName,
                           PurchaseTypeId = a.PurchaseTypeId,
                           CategoryId = a.CategoryId
                       }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) =>
                       new
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           Address = a.Address,
                           PhoneNumber = a.PhoneNumber,
                           Email = a.Email,
                           NumberOfDoctors = a.NumberOfDoctors,
                           PaymentNote = a.PaymentNote,
                           BestTimeFrom = a.BestTimeFrom,
                           BestTimeTo = a.BestTimeTo,
                           RelationshipNote = a.RelationshipNote,
                           AccountTypeId = a.AccountTypeId,
                           AccountTypeName = a.AccountTypeName,
                           DistrictId = a.DistrictId,
                           DistrictName = a.DistrictName,
                           CityId = a.CityId,
                           CityName = a.CityName,
                           PurchaseTypeId = a.PurchaseTypeId,
                           PurchaseTypeName = b.PurchaseTypeName,
                           CategoryId = a.CategoryId
                       }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) =>
                       new CustomAccount
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           Address = a.Address,
                           PhoneNumber = a.PhoneNumber,
                           Email = a.Email,
                           NumberOfDoctors = a.NumberOfDoctors,
                           PaymentNote = a.PaymentNote,
                           BestTimeFrom = a.BestTimeFrom,
                           BestTimeTo = a.BestTimeTo,
                           RelationshipNote = a.RelationshipNote,
                           AccountTypeId = a.AccountTypeId,
                           AccountTypeName = a.AccountTypeName,
                           DistrictId = a.DistrictId,
                           DistrictName = a.DistrictName,
                           CityId = a.CityId,
                           CityName = a.CityName,
                           PurchaseTypeId = a.PurchaseTypeId,
                           PurchaseTypeName = a.PurchaseTypeName,
                           CategoryId = a.CategoryId,
                           CategoryName = b.CategoryName,
                           NumberOfMedicalVisits = medicalvisits,
                           NumberOfSalesVisits = salesvisits,
                           FirstLineNames = firstlinenames,
                           MedicalsNames = medicalnames,
                           SalesNames = salesnames,
                           ContactsNames = contacts
                       }).SingleOrDefault();

            
        }

        public IEnumerable<CustomAccount> GetAllAccounts()
        {
            IEnumerable<CustomAccount> accounts = db.account
                .Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) =>
                new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    NumberOfDoctors = a.NumberOfDoctors,
                    PaymentNote = a.PaymentNote,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    RelationshipNote = a.RelationshipNote,
                    AccountTypeId = b.Id,
                    AccountTypeName = b.AccountTypeName,
                    DistrictId = a.DistrictId,
                    PurchaseTypeId = a.PurchaseTypeId,
                    CategoryId = a.CategoryId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) =>
                       new
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           Address = a.Address,
                           PhoneNumber = a.PhoneNumber,
                           Email = a.Email,
                           NumberOfDoctors = a.NumberOfDoctors,
                           PaymentNote = a.PaymentNote,
                           BestTimeFrom = a.BestTimeFrom,
                           BestTimeTo = a.BestTimeTo,
                           RelationshipNote = a.RelationshipNote,
                           AccountTypeId = a.AccountTypeId,
                           AccountTypeName = a.AccountTypeName,
                           DistrictId = a.DistrictId,
                           DistrictName = b.DistrictName,
                           CityId = b.CityId,
                           PurchaseTypeId = a.PurchaseTypeId,
                           CategoryId = a.CategoryId
                       }).Join(db.city, a => a.CityId, b => b.Id, (a, b) =>
                       new
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           Address = a.Address,
                           PhoneNumber = a.PhoneNumber,
                           Email = a.Email,
                           NumberOfDoctors = a.NumberOfDoctors,
                           PaymentNote = a.PaymentNote,
                           BestTimeFrom = a.BestTimeFrom,
                           BestTimeTo = a.BestTimeTo,
                           RelationshipNote = a.RelationshipNote,
                           AccountTypeId = a.AccountTypeId,
                           AccountTypeName = a.AccountTypeName,
                           DistrictId = a.DistrictId,
                           DistrictName = a.DistrictName,
                           CityId = a.CityId,
                           CityName = b.CityName,
                           PurchaseTypeId = a.PurchaseTypeId,
                           CategoryId = a.CategoryId
                       }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) =>
                       new
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           Address = a.Address,
                           PhoneNumber = a.PhoneNumber,
                           Email = a.Email,
                           NumberOfDoctors = a.NumberOfDoctors,
                           PaymentNote = a.PaymentNote,
                           BestTimeFrom = a.BestTimeFrom,
                           BestTimeTo = a.BestTimeTo,
                           RelationshipNote = a.RelationshipNote,
                           AccountTypeId = a.AccountTypeId,
                           AccountTypeName = a.AccountTypeName,
                           DistrictId = a.DistrictId,
                           DistrictName = a.DistrictName,
                           CityId = a.CityId,
                           CityName = a.CityName,
                           PurchaseTypeId = a.PurchaseTypeId,
                           PurchaseTypeName = b.PurchaseTypeName,
                           CategoryId = a.CategoryId
                       }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) =>
                       new CustomAccount
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           Address = a.Address,
                           PhoneNumber = a.PhoneNumber,
                           Email = a.Email,
                           NumberOfDoctors = a.NumberOfDoctors,
                           PaymentNote = a.PaymentNote,
                           BestTimeFrom = a.BestTimeFrom,
                           BestTimeTo = a.BestTimeTo,
                           RelationshipNote = a.RelationshipNote,
                           AccountTypeId = a.AccountTypeId,
                           AccountTypeName = a.AccountTypeName,
                           DistrictId = a.DistrictId,
                           DistrictName = a.DistrictName,
                           CityId = a.CityId,
                           CityName = a.CityName,
                           PurchaseTypeId = a.PurchaseTypeId,
                           PurchaseTypeName = a.PurchaseTypeName,
                           CategoryId = a.CategoryId,
                           CategoryName = b.CategoryName
                       }).OrderBy(a => a.AccountName);


            return accounts;
        }

        public bool SetAccountOpenning(NewOpenning op)
        {
            AccountBrandPayment check = db.accountBrandPayment
                .Where(
                a => a.AccountId == op.AccountId
                &&
                a.BrandId == op.BrandId)
                .SingleOrDefault();
            if (check == null)
            {
                AccountBrandPayment obj = new AccountBrandPayment();
                obj.AccountId = op.AccountId;
                obj.BrandId = op.BrandId;
                obj.Openning = op.Openning;
                obj.Collection = 0;
                obj.Balance = op.Openning - obj.Collection;
                db.accountBrandPayment.Add(obj);
                db.SaveChanges();
                return true;
            }

            else {
                return false;
            }
            
        }

        public bool EditAccountOpenning(int openningId, int openning)
        {
            AccountBrandPayment obj = db.accountBrandPayment.Find(openningId);
           decimal op = obj.Openning + openning;
           decimal ba = op - obj.Collection;

            obj.Openning = op;
            obj.Balance = ba;
            db.SaveChanges();
            return true;
        }

        public bool EditAccountGeneralInfo(AccountGeneralInfo obj)
        {
            Account old = db.account.Find(obj.Id);
            old.AccountName = obj.AccountName;
            old.AccountTypeId = obj.AccountTypeId;
            old.PurchaseTypeId = obj.PurchaseTypeId;
            old.CategoryId = obj.CategoryId;
            old.NumberOfDoctors = obj.NumberOfDoctors;
            db.SaveChanges();
            return true;

        }

        public bool EditAccountLocationInfo(AccountLocationInfo obj)
        {
            Account old = db.account.Find(obj.Id);

            old.DistrictId = obj.DistrictId;
            old.Address = obj.Address;
            db.SaveChanges();
            return true;

        }

        public bool EditAccountContactinfo(AccountContactInfo obj)
        {
            Account old = db.account.Find(obj.Id);
            old.PhoneNumber = obj.PhoneNumber;
            old.Email = obj.Email;
            db.SaveChanges();
            return true;
        }

        public bool EditAccountTimeInfo(AccountTimeInfo obj)
        {
            Account old = db.account.Find(obj.Id);
            old.BestTimeFrom = obj.BestTimeFrom;
            old.BestTimeTo = obj.BestTimeTo;
            db.SaveChanges();
            return true;
        }

        public bool EditAccountNotesInfo(AccountNoteInfo obj)
        {
            Account old = db.account.Find(obj.Id);
            old.RelationshipNote = obj.RelationshipNote;
            old.PaymentNote = obj.PaymentNote;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<Brand> GetAvailableBrandsForOppening(int accoundId)
        {
            List<int> currentbarndsids = db.accountBrandPayment
                .Where(a => a.AccountId == accoundId)
                .Select(a => a.BrandId).ToList();
            List<Brand> result = db.brand.Select(a => a).ToList();

            if (currentbarndsids != null)
            {
                foreach (var item in currentbarndsids)
                {
                    List<Brand> d = result.Where(a => a.Id == item).ToList();

                    foreach (var del in d)
                    {
                        result.Remove(del);
                    }
                }
            }
             

            return result;
            
        }

        public IEnumerable<CustomAccountOpenning> GetAccountOpenningsByAccountId(int accountId)
        {
            IEnumerable<CustomAccountOpenning> result = db.accountBrandPayment.Where(a => a.AccountId == accountId)
                .Join(db.account, a => a.AccountId, b => b.Id, (a, b) =>
                       new
                       {
                           Id = a.Id,
                           AccountName = b.AccountName,
                           BrandId = a.BrandId,
                           Openning = a.Openning,
                           Collection = a.Collection,
                           Balance = a.Balance
                       }).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new CustomAccountOpenning
                       {
                           Id = a.Id,
                           AccountName = a.AccountName,
                           BrandName = b.BrandName,
                           Openning = a.Openning,
                           Collection = a.Collection,
                           Balance = a.Balance
                       });

            return result;
        }

        public IEnumerable<CustomAccountName> GetAccountNames()
        {
            IEnumerable<Account> accounts = db.account.Select(a => a);

            List<CustomAccountName> result = new List<CustomAccountName>();

            foreach (var item in accounts)
            {
                CustomAccountName x = new CustomAccountName();
                x.Id = item.Id;
                x.AccountName = item.AccountName;
                result.Add(x);
            }
            return result;

        }

        public CustomAccountOpenning GetOpenningById(int openningId)
        {
            return db.accountBrandPayment.Where(a => a.Id == openningId).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                BrandId = a.BrandId,
                Openning = a.Openning,
                Collection = a.Collection,
                Balance = a.Balance
            }).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new CustomAccountOpenning
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                BrandId = a.BrandId,
                BrandName = b.BrandName,
                Openning = a.Openning,
                Collection = a.Collection,
                Balance = a.Balance
            }).SingleOrDefault();
        }

        public IEnumerable<CustomAccount> GetAllAccountsfiltered(FilteringAccounts obj)
        {

            if (obj.AccountTypeId == 0 && obj.DistrictId == 0)
            {

                IEnumerable<CustomAccount> accounts = db.account
                 .Join(db.district, a => a.DistrictId, b => b.Id, (a, b) =>
                        new CustomAccount
                        {
                            Id = a.Id,
                            CityId = b.CityId,
                            AccountName = a.AccountName,
                            Address = a.Address,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            DistrictName = b.DistrictName
                        }).Where(x => x.CityId == obj.CityId).OrderBy(a => a.AccountName);
                return accounts;
            }
            else
            {
                if (obj.AccountTypeId == 0)
                {
                    IEnumerable<CustomAccount> accounts = db.account
                 .Join(db.district, a => a.DistrictId, b => b.Id, (a, b) =>
                        new CustomAccount
                        {
                            Id = a.Id,
                            DistrictId = a.DistrictId,
                            AccountName = a.AccountName,                    
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            DistrictName = b.DistrictName
                        }).Where(x=>x.DistrictId == obj.DistrictId).OrderBy(a => a.AccountName);
                    return accounts;
                }



                else
                {
                    if (obj.DistrictId == 0)
                    {
                        IEnumerable<CustomAccount> accounts = db.account
                 .Join(db.district, a => a.DistrictId, b => b.Id, (a, b) =>
                        new CustomAccount
                        {
                            Id = a.Id,
                            AccountName = a.AccountName,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            AccountTypeId = a.AccountTypeId,
                            DistrictName = b.DistrictName,
                            CityId = b.CityId
                        }).Where(x => x.AccountTypeId == obj.AccountTypeId && x.CityId == obj.CityId).OrderBy(a => a.AccountName); ;
                        return accounts;
                    }
                    else
                    {
                        IEnumerable<CustomAccount> accounts = db.account
                 .Join(db.district, a => a.DistrictId, b => b.Id, (a, b) =>
                        new CustomAccount
                        {
                            Id = a.Id,
                            AccountName = a.AccountName,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            AccountTypeId = a.AccountTypeId,
                            DistrictId = a.DistrictId,
                            DistrictName = b.DistrictName
                        }).Where(x => x.AccountTypeId == obj.AccountTypeId && x.DistrictId == obj.DistrictId).OrderBy(a => a.AccountName); ;
                        return accounts;
                    }
                }
            }

            
            ;
        }

        public IEnumerable<CustomAccount> GetAllAccountsfilteredWithouttype(FilteringAccounts obj)
        {
            IEnumerable<CustomAccount> accounts = db.account
                 .Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) =>
                 new
                 {
                     Id = a.Id,
                     AccountName = a.AccountName,
                     Address = a.Address,
                     PhoneNumber = a.PhoneNumber,
                     Email = a.Email,
                     NumberOfDoctors = a.NumberOfDoctors,
                     PaymentNote = a.PaymentNote,
                     BestTimeFrom = a.BestTimeFrom,
                     BestTimeTo = a.BestTimeTo,
                     RelationshipNote = a.RelationshipNote,
                     AccountTypeId = b.Id,
                     AccountTypeName = b.AccountTypeName,
                     DistrictId = a.DistrictId,
                     PurchaseTypeId = a.PurchaseTypeId,
                     CategoryId = a.CategoryId
                 }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) =>
                        new
                        {
                            Id = a.Id,
                            AccountName = a.AccountName,
                            Address = a.Address,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            NumberOfDoctors = a.NumberOfDoctors,
                            PaymentNote = a.PaymentNote,
                            BestTimeFrom = a.BestTimeFrom,
                            BestTimeTo = a.BestTimeTo,
                            RelationshipNote = a.RelationshipNote,
                            AccountTypeId = a.AccountTypeId,
                            AccountTypeName = a.AccountTypeName,
                            DistrictId = a.DistrictId,
                            DistrictName = b.DistrictName,
                            CityId = b.CityId,
                            PurchaseTypeId = a.PurchaseTypeId,
                            CategoryId = a.CategoryId
                        }).Join(db.city, a => a.CityId, b => b.Id, (a, b) =>
                        new
                        {
                            Id = a.Id,
                            AccountName = a.AccountName,
                            Address = a.Address,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            NumberOfDoctors = a.NumberOfDoctors,
                            PaymentNote = a.PaymentNote,
                            BestTimeFrom = a.BestTimeFrom,
                            BestTimeTo = a.BestTimeTo,
                            RelationshipNote = a.RelationshipNote,
                            AccountTypeId = a.AccountTypeId,
                            AccountTypeName = a.AccountTypeName,
                            DistrictId = a.DistrictId,
                            DistrictName = a.DistrictName,
                            CityId = a.CityId,
                            CityName = b.CityName,
                            PurchaseTypeId = a.PurchaseTypeId,
                            CategoryId = a.CategoryId
                        }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) =>
                        new
                        {
                            Id = a.Id,
                            AccountName = a.AccountName,
                            Address = a.Address,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            NumberOfDoctors = a.NumberOfDoctors,
                            PaymentNote = a.PaymentNote,
                            BestTimeFrom = a.BestTimeFrom,
                            BestTimeTo = a.BestTimeTo,
                            RelationshipNote = a.RelationshipNote,
                            AccountTypeId = a.AccountTypeId,
                            AccountTypeName = a.AccountTypeName,
                            DistrictId = a.DistrictId,
                            DistrictName = a.DistrictName,
                            CityId = a.CityId,
                            CityName = a.CityName,
                            PurchaseTypeId = a.PurchaseTypeId,
                            PurchaseTypeName = b.PurchaseTypeName,
                            CategoryId = a.CategoryId
                        }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) =>
                        new CustomAccount
                        {
                            Id = a.Id,
                            AccountName = a.AccountName,
                            Address = a.Address,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            NumberOfDoctors = a.NumberOfDoctors,
                            PaymentNote = a.PaymentNote,
                            BestTimeFrom = a.BestTimeFrom,
                            BestTimeTo = a.BestTimeTo,
                            RelationshipNote = a.RelationshipNote,
                            AccountTypeId = a.AccountTypeId,
                            AccountTypeName = a.AccountTypeName,
                            DistrictId = a.DistrictId,
                            DistrictName = a.DistrictName,
                            CityId = a.CityId,
                            CityName = a.CityName,
                            PurchaseTypeId = a.PurchaseTypeId,
                            PurchaseTypeName = a.PurchaseTypeName,
                            CategoryId = a.CategoryId,
                            CategoryName = b.CategoryName
                        }).Where(x=> x.DistrictId == obj.DistrictId).OrderBy(a => a.AccountName); ;

            return accounts;
        }

        public IEnumerable<Account> GetFLMAccounts(string userId)
        {
            ExtendIdentityUser me = db.Users.Find(userId);
            int? mycity = me.CityId;

            IEnumerable<Account> result = db.account.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountName = a.AccountName,
                DistrictId = a.DistrictId,
                CityId = b.CityId
            }).Where(x => x.CityId == mycity).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new Account
            {
                Id = a.Id,
                AccountName = a.AccountName
            }).OrderBy(c => c.AccountName);

            return result;
        }

        public IEnumerable<CustomAccount> SearchAccount(SearchByWord accountName)
        {
            string normalized = accountName.Word.Normalize().ToUpper();
            IEnumerable<CustomAccount> res = db.account.Where(a=>a.AccountName.Contains(normalized)).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) =>
                        new CustomAccount
                        {
                            Id = a.Id,
                            AccountName = a.AccountName,
                            Address = a.Address,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            DistrictName = b.DistrictName
                        }).OrderBy(a => a.AccountName);

            return res;
        }

        public float GetAccountCreditLimit(int id)
        {
            Account obj = db.account.Find(id);
            return obj.CreditLimit;
        }

        public bool EditAccountCreditLimit(int id, float creditlimit)
        {
            Account obj = db.account.Find(id);
            obj.CreditLimit = creditlimit;
            db.SaveChanges();
            return true;
        }

        public float GetAccountResidualCreditLimit(int id)
        {
            float CreditLimit = db.account.Where(a => a.Id == id).Select(a => a.CreditLimit).FirstOrDefault();
            float totalbalance = db.accountBalance.Where(a => a.AccountId == id).Select(a => a.Balance).Sum();
            float Residual = CreditLimit - totalbalance;
            return Residual;
        }
    }
}
