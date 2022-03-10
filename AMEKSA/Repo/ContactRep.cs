﻿using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class ContactRep:IContactRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public ContactRep(DbContainer db, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public bool AddContact(Contact obj)
        {
            db.contact.Add(obj);
            db.SaveChanges();
            return true;
        }

        public bool DeleteContact(int id)
        {
            IEnumerable<ContactMedicalVisit> MV = db.contactMedicalVisit.Where(a => a.ContactId == id);
            foreach (var item in MV)
            {
                IEnumerable<ContactMedicalVisitProduct> MVP = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == item.Id);
                IEnumerable<ContactSalesAid> MVPs = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == item.Id);

                foreach (var product in MVP)
                {
                    db.contactMedicalVisitProduct.Remove(product);
                    ;
                }
                foreach (var aid in MVPs)
                {
                    db.contactSalesAid.Remove(aid);

                }
                db.contactMedicalVisit.Remove(item);

            }
           

            IEnumerable<UserContact> UA = db.userContact.Where(a => a.ContactId == id);
            foreach (var item in UA)
            {
                db.userContact.Remove(item);

            }
          
      
            Contact acc = db.contact.Find(id);

            db.contact.Remove(acc);
            db.SaveChanges();

            return true;
        }

        public bool EditContactContactinfo(ContactContactInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.MobileNumber = obj.MobileNumber;
            old.LandLineNumber = obj.LandLineNumber;
            old.Email = obj.Email;
            db.SaveChanges();
            return true;
        }

        public bool EditContactGeneralInfo(ContactGeneralInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.Gender = obj.Gender;
            old.ContactName = obj.ContactName;
            old.ContactTypeId = obj.ContactTypeId;
            old.PurchaseTypeId = obj.PurchaseTypeId;
            old.AccountId = obj.AccountId;
            db.SaveChanges();
            return true;
        }

        public bool EditContactLocationInfo(ContactLocationInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.DistrictId = obj.DistrictId;
            old.Address = obj.Address;
            db.SaveChanges();
            return true;
        }

        public bool EditContactNotesInfo(ContactNoteInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.PaymentNotes = obj.PaymentNotes;
            old.RelationshipNote = obj.RelationshipNote;
            db.SaveChanges();
            return true;
        }

        public bool EditContactTimeInfo(ContactTimeInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.BestTimeFrom = obj.BestTimeFrom;
            old.BestTimeTo = obj.BestTimeTo;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomContact> GetAllContacts()
        {
 
            return db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictId = b.Id,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       Address = a.Address,
                       LandLineNumber = a.LandLineNumber,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       ContactTypeId = a.ContactTypeId,
                       PaymentNotes = a.PaymentNotes,
                       RelationshipNotes = a.RelationshipNote,
                       BestTimeFrom = a.BestTimeFrom,
                       BestTimeTo = a.BestTimeTo,
                       PurchaseTypeId = a.PurchaseTypeId,
                       AccountId = a.AccountId,
                       Gender = a.Gender
                   }).Join(db.city,
                   a => a.CityId,
                   b => b.Id,
                   (a, b) => new
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       CityName = b.CityName,
                       Address = a.Address,
                       LandLineNumber = a.LandLineNumber,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       ContactTypeId = a.ContactTypeId,
                       PaymentNotes = a.PaymentNotes,
                       RelationshipNotes = a.RelationshipNotes,
                       BestTimeFrom = a.BestTimeFrom,
                       BestTimeTo = a.BestTimeTo,
                       PurchaseTypeId = a.PurchaseTypeId,
                       AccountId = a.AccountId,
                       Gender = a.Gender
                   }).Join(db.contactType,
                   a => a.ContactTypeId,
                   b => b.Id,
                   (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       CityName = a.CityName,
                       Address = a.Address,
                       LandLineNumber = a.LandLineNumber,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       ContactTypeId = a.ContactTypeId,
                       ContactTypeName = b.ContactTypeName,
                       PaymentNotes = a.PaymentNotes,
                       RelationshipNotes = a.RelationshipNotes,
                       BestTimeFrom = a.BestTimeFrom,
                       BestTimeTo = a.BestTimeTo,
                       PurchaseTypeId = a.PurchaseTypeId,
                       AccountId = a.AccountId,
                       Gender = a.Gender
                   }
                   ).Join(db.purchaseType,
                   a => a.PurchaseTypeId,
                   b => b.Id,
                   (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       CityName = a.CityName,
                       Address = a.Address,
                       LandLineNumber = a.LandLineNumber,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       ContactTypeId = a.ContactTypeId,
                       ContactTypeName = a.ContactTypeName,
                       PaymentNotes = a.PaymentNotes,
                       RelationshipNote = a.RelationshipNotes,
                       BestTimeFrom = a.BestTimeFrom,
                       BestTimeTo = a.BestTimeTo,
                       PurchaseTypeId = a.PurchaseTypeId,
                       PurchaseTypeName = b.PurchaseTypeName,
                       AccountId = a.AccountId,
                       Gender = a.Gender
                   });


            
        }

        public IEnumerable<CustomContact> GetAllContactsFiltered(FilteringContactsModel obj)
        {

            if (obj.DistrictId == 0 && obj.ContactTypeId == 0)
            {
                List<CustomContact> cus = new List<CustomContact>();
                IEnumerable<CustomContact> one = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId
                   }).Join(db.account,a=>a.AccountId,b=>b.Id,(a,b)=>new CustomContact
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName
                   }).Where(a => a.CityId == obj.CityId);

               
                IEnumerable<CustomContact> two = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new CustomContact
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId
                   }).Where(a => a.CityId == obj.CityId && a.AccountId == null);

                foreach (var item in one)
                {
                    cus.Add(item);
                }
                foreach (var item in two)
                {
                    cus.Add(item);
                }

                return cus.DistinctBy(a=>a.Id).OrderBy(a=>a.ContactName);
            }
            else
            {

                if (obj.DistrictId == 0)
                {
                    List<CustomContact> cus = new List<CustomContact>();

                    IEnumerable<CustomContact> one = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId
                   }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName
                   }).Where(a => a.CityId == obj.CityId && a.ContactTypeId == obj.ContactTypeId);

                    IEnumerable<CustomContact> two = db.contact
               .Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new CustomContact
                  {
                      Id = a.Id,
                      ContactTypeId = a.ContactTypeId,
                      ContactName = a.ContactName,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      AccountId = a.AccountId
                  }).Where(a => a.CityId == obj.CityId && a.ContactTypeId == obj.ContactTypeId && a.AccountId == null);

                    foreach (var item in one)
                    {
                        cus.Add(item);
                    }
                    foreach (var item in two)
                    {
                        cus.Add(item);
                    }
                    return cus.DistinctBy(a => a.Id).OrderBy(a => a.ContactName);
                }

                else
                {
                    if (obj.ContactTypeId == 0)
                    {
                        List<CustomContact> cus = new List<CustomContact>();

                        IEnumerable<CustomContact> one = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId
                   }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       DistrictId = a.DistrictId,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName
                   }).Where(a => a.DistrictId == obj.DistrictId);

                        IEnumerable<CustomContact> two = db.contact
               .Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new CustomContact
                  {
                      Id = a.Id,
                      ContactTypeId = a.ContactTypeId,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      AccountId = a.AccountId
                  }).Where(a => a.DistrictId == obj.DistrictId && a.AccountId == null) ;

                        foreach (var item in one)
                        {
                            cus.Add(item);
                        }
                        foreach (var item in two)
                        {
                            cus.Add(item);
                        }

                        return cus.DistinctBy(a => a.Id).OrderBy(a => a.ContactName);
                    }
                    else
                    {
                        List<CustomContact> cus = new List<CustomContact>();

                        IEnumerable<CustomContact> one = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       DistrictId = a.DistrictId,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId
                   }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName
                   }).Where(a => a.DistrictId == obj.DistrictId && a.ContactTypeId == obj.ContactTypeId);

                        IEnumerable<CustomContact> two = db.contact
               .Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new CustomContact
                  {
                      Id = a.Id,
                      ContactTypeId = a.ContactTypeId,
                      DistrictId = a.DistrictId,
                      ContactName = a.ContactName,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      AccountId = a.AccountId
                  }).Where(a => a.DistrictId == obj.DistrictId && a.ContactTypeId == obj.ContactTypeId && a.AccountId == null);

                        foreach (var item in one)
                        {
                            cus.Add(item);
                        }
                        foreach (var item in two)
                        {
                            cus.Add(item);
                        }

                        return cus.DistinctBy(a => a.Id).OrderBy(a => a.ContactName);
                    }
                }
            }








            
        }

        public CustomContact GetContactByIdWithAccount(int id)
        {
            Contact con = db.contact.Find(id);
            int CityId = db.district.Where(a => a.Id == con.DistrictId).Select(a => a.CityId).First();
            int medicalvisits = db.contactMedicalVisit.Where(a => a.ContactId == id).Count();
            IEnumerable<string> firstlinenames = db.userContact.Where(a => a.ContactId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new
            {
                ManagerId = b.extendidentityuserid
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Name = b.FullName
            }).Select(a => a.Name);
            IEnumerable<string> medicalids = db.userContact.Where(a => a.ContactId == id).Select(a => a.extendidentityuserid);
            List<string> medicalnames = new List<string>();
            foreach (var item in medicalids)
            {
                string username = db.Users.Where(a => a.Id == item).Select(a => a.FullName).SingleOrDefault();
                medicalnames.Add(username);
            }

          


            CustomContact result = db.contact
               .Where(x => x.Id == id).Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = b.Id,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNote,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      AccountId = a.AccountId,
                      Gender = a.Gender
                  }).Join(db.city,
                  a => a.CityId,
                  b => b.Id,
                  (a, b) => new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = b.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      AccountId = a.AccountId,
                      Gender = a.Gender
                  }).Join(db.contactType,
                  a => a.ContactTypeId,
                  b => b.Id,
                  (a, b) =>
                  new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = b.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      AccountId = a.AccountId,
                      Gender = a.Gender
                  }
                  ).Join(db.purchaseType,
                  a => a.PurchaseTypeId,
                  b => b.Id,
                  (a, b) => new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = a.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      PurchaseTypeName = b.PurchaseTypeName,
                      AccountId = a.AccountId,
                      Gender = a.Gender
                  }).Join(db.account,
                  a => a.AccountId,
                  b => b.Id,
                  (a, b) => new CustomContact
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = a.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNote = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      PurchaseTypeName = a.PurchaseTypeName,
                      AccountId = a.AccountId,
                      AccountName = b.AccountName,
                      Gender = a.Gender,
                      NumberOfMedicalVisits = medicalvisits,
                      FirstLineNames = firstlinenames,
                      MedicalNames = medicalnames
                  }).SingleOrDefault();

            return result;
        }

        public CustomContact GetContactByIdWithoutAccount(int id)
        {
            Contact con = db.contact.Find(id);
            int CityId = db.district.Where(a => a.Id == con.DistrictId).Select(a => a.CityId).First();
            int medicalvisits = db.contactMedicalVisit.Where(a => a.ContactId == id).Count();
            IEnumerable<string> firstlinenames = db.userContact.Where(a => a.ContactId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new
            {
                ManagerId = b.extendidentityuserid
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Name = b.FullName
            }).Select(a => a.Name);
            IEnumerable<string> medicalids = db.contactMedicalVisit.Where(a => a.ContactId == id).Select(a => a.extendidentityuserid);
            List<string> medicalnames = new List<string>();
            foreach (var item in medicalids)
            {
                string username = db.Users.Where(a => a.Id == item).Select(a => a.FullName).SingleOrDefault();
                medicalnames.Add(username);
            }




            CustomContact result = db.contact
               .Where(x => x.Id == id).Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = b.Id,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNote,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      Gender = a.Gender
                  }).Join(db.city,
                  a => a.CityId,
                  b => b.Id,
                  (a, b) => new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = b.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      Gender = a.Gender
                  }).Join(db.contactType,
                  a => a.ContactTypeId,
                  b => b.Id,
                  (a, b) =>
                  new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = b.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      Gender = a.Gender
                  }
                  ).Join(db.purchaseType,
                  a => a.PurchaseTypeId,
                  b => b.Id,
                  (a, b) => new CustomContact
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = a.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNote = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      PurchaseTypeName = b.PurchaseTypeName,
                      Gender = a.Gender,
                      NumberOfMedicalVisits = medicalvisits,
                      FirstLineNames = firstlinenames,
                      MedicalNames = medicalnames
                  }).SingleOrDefault();

            return result;
        }

        public IEnumerable<CustomContact> SearchContact(SearchByWord contactName)
        {
            string normalized = contactName.Word.Normalize().ToUpper();
            List<CustomContact> res = new List<CustomContact>();
            IEnumerable<CustomContact> one = db.contact.Where(a => a.ContactName.Contains(normalized))
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       DistrictId = a.DistrictId,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId
                   }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName
                   });
            IEnumerable<CustomContact> two = db.contact.Where(a => a.ContactName.Contains(normalized))
               .Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new CustomContact
                  {
                      Id = a.Id,
                      ContactTypeId = a.ContactTypeId,
                      DistrictId = a.DistrictId,
                      ContactName = a.ContactName,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      AccountId = a.AccountId
                  });
            foreach (var item in one)
            {
                res.Add(item);
            }
            foreach (var item in two)
            {
                res.Add(item);
            }
            return res.DistinctBy(a=>a.Id).OrderBy(a=>a.AccountName);
        }
    }
}
