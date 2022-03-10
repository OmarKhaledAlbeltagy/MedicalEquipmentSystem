using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class ChangeCategoryRep:IChangeCategoryRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public ChangeCategoryRep(DbContainer db, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public bool ConfirmRequest(int RequestId)
        {
            RequestChangeContactCategory obj = db.requestChangeCategory.Find(RequestId);
            UserContact UC = db.userContact.Where(a => a.ContactId == obj.ContactId && a.extendidentityuserid == obj.ExtendIdentityUserId).FirstOrDefault();
            UC.CategoryId = obj.CategoryToId;
            obj.Confirmed = true;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<ChangeCategoryRequestsModel> GetAllRequests()
        {
            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Medical Representative").Result;

            IEnumerable<ChangeCategoryRequestsModel> res = myteam.Join(db.requestChangeCategory, a => a.Id, b => b.ExtendIdentityUserId, (a, b) => new
            {
                Id = b.Id,
                FullName = a.FullName,
                CategoryFromId = b.CategoryFromId,
                CategoryToId = b.CategoryToId,
                Rejected = b.Rejected,
                Confirmed = b.Confirmed,
                ContactId = b.ContactId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                CategoryFromId = a.CategoryFromId,
                CategoryToId = a.CategoryToId,
                Rejected = a.Rejected,
                Confirmed = a.Confirmed,
                ContactName = b.ContactName
            }).Join(db.category, a => a.CategoryFromId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                CategoryFromName = b.CategoryName,
                CategoryToId = a.CategoryToId,
                Rejected = a.Rejected,
                Confirmed = a.Confirmed,
                ContactName = a.ContactName
            }).Join(db.category, a => a.CategoryToId, b => b.Id, (a, b) => new ChangeCategoryRequestsModel
            {
                Id = a.Id,
                FullName = a.FullName,
                CategoryFromName = a.CategoryFromName,
                CategoryToName = b.CategoryName,
                Rejected = a.Rejected,
                Confirmed = a.Confirmed,
                ContactName = a.ContactName
            }).Where(a=>a.Confirmed == false);

            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<ChangeCategoryRequestsModel> GetMyTeamRequests(string ManagerId)
        {
            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.extendidentityuserid == ManagerId);

            IEnumerable<ChangeCategoryRequestsModel> res = myteam.Join(db.requestChangeCategory, a => a.Id, b => b.ExtendIdentityUserId, (a, b) => new
            {
                Id = b.Id,
                FullName = a.FullName,
                CategoryFromId = b.CategoryFromId,
                CategoryToId = b.CategoryToId,
                Rejected = b.Rejected,
                Confirmed = b.Confirmed,
                ContactId = b.ContactId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                CategoryFromId = a.CategoryFromId,
                CategoryToId = a.CategoryToId,
                Rejected = a.Rejected,
                Confirmed = a.Confirmed,
                ContactName = b.ContactName
            }).Join(db.category, a => a.CategoryFromId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                CategoryFromName = b.CategoryName,
                CategoryToId = a.CategoryToId,
                Rejected = a.Rejected,
                Confirmed = a.Confirmed,
                ContactName = a.ContactName
            }).Join(db.category, a => a.CategoryToId, b => b.Id, (a, b) => new ChangeCategoryRequestsModel
            {
                Id = a.Id,
                FullName = a.FullName,
                CategoryFromName = a.CategoryFromName,
                CategoryToName = b.CategoryName,
                Rejected = a.Rejected,
                Confirmed = a.Confirmed,
                ContactName = a.ContactName
            }).Where(a => a.Confirmed == false);

            return res.OrderBy(a=>a.FullName);
        }

        public bool MakeRequest(ChangeCategoryRequestModel obj)
        {
            int CategoryFromId = db.category.Where(a => a.CategoryName == obj.CurrentCategoryName).Select(a=>a.Id).FirstOrDefault();

            RequestChangeContactCategory req = new RequestChangeContactCategory();
            req.CategoryFromId = CategoryFromId;
            req.CategoryToId = obj.NewCategoryId;
            req.ExtendIdentityUserId = obj.UserId;
            req.ContactId = obj.ContactId;
            req.Rejected = false;
            req.Confirmed = false;
            db.requestChangeCategory.Add(req);
            db.SaveChanges();
            return true;
        }

        public bool RejectRequest(int RequestId)
        {
            RequestChangeContactCategory obj = db.requestChangeCategory.Find(RequestId);
            obj.Rejected = true;
            db.SaveChanges();
            return true;

        }
    }
}
