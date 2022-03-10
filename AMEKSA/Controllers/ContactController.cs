using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class ContactController : ControllerBase
    {
        private readonly IContactRep contactRep;
        private readonly IContactTypeRep contactTypeRep;
        private readonly DbContainer db;

        public ContactController(IContactRep contactRep, IContactTypeRep contactTypeRep, DbContainer db)
        {
            this.contactRep = contactRep;
            this.contactTypeRep = contactTypeRep;
            this.db = db;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SearchContact(SearchByWord contactName)
        {
            return Ok(contactRep.SearchContact(contactName));
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllContactTypes()
        {
            return Ok(contactTypeRep.GetAllContactTypes());
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactType(ContactType obj)
        {
            return Ok(contactTypeRep.EditContactType(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddNewContactType(ContactType obj)
        {
            return Ok(contactTypeRep.AddContactType(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllContacts()
        {
            return Ok(contactRep.GetAllContacts());
        }


        [Route("[controller]/[Action]/{contactId}")]
        [HttpGet("{contactId}")]
        public IActionResult GetContactById(int contactId)
        {

            int? accountid = db.contact.Where(a => a.Id == contactId).Select(a => a.AccountId).SingleOrDefault();

            CustomContact result;

            if (accountid == null)
            {
                result = contactRep.GetContactByIdWithoutAccount(contactId);
            }
            else
            {
                result = contactRep.GetContactByIdWithAccount(contactId);
            }

            return Ok(result);
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddContact(AddContact obj)
        {
            if (obj.AccountName == null)
            {
                Contact contact = new Contact();
                contact.ContactName = obj.ContactName;
                contact.Gender = obj.Gender;
                contact.DistrictId = obj.DistrictId;
                contact.Address = obj.Address;
                contact.LandLineNumber = obj.LandLineNumber;
                contact.MobileNumber = obj.MobileNumber;
                contact.Email = obj.Email;
                contact.ContactTypeId = obj.ContactTypeId;
                contact.PaymentNotes = obj.PaymentNotes;
                contact.RelationshipNote = obj.RelationshipNote;
                contact.BestTimeFrom = obj.BestTimeFrom;
                contact.BestTimeTo = obj.BestTimeTo;
                contact.PurchaseTypeId = obj.PurchaseTypeId;
                contact.AccountId = null;
                return Ok(contactRep.AddContact(contact));
            }
            else
            {
                Account account = db.account.Where(a => a.AccountName == obj.AccountName).FirstOrDefault();

                if (account == null)
                {
                    return Ok(false);
                }
                else
                {
                    Contact contact = new Contact();
                    contact.ContactName = obj.ContactName;
                    contact.Gender = obj.Gender;
                    contact.DistrictId = obj.DistrictId;
                    contact.Address = obj.Address;
                    contact.LandLineNumber = obj.LandLineNumber;
                    contact.MobileNumber = obj.MobileNumber;
                    contact.Email = obj.Email;
                    contact.ContactTypeId = obj.ContactTypeId;
                    contact.PaymentNotes = obj.PaymentNotes;
                    contact.RelationshipNote = obj.RelationshipNote;
                    contact.BestTimeFrom = obj.BestTimeFrom;
                    contact.BestTimeTo = obj.BestTimeTo;
                    contact.PurchaseTypeId = obj.PurchaseTypeId;
                    contact.AccountId = account.Id;
                    return Ok(contactRep.AddContact(contact));
                }
            }

            
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactGeneralInfo(ContactGeneralInfo obj)
        {
            if (obj.AccountName == null)
            {
                return Ok(contactRep.EditContactGeneralInfo(obj));
            }

            else
            {
                Account account = db.account.Where(a => a.AccountName == obj.AccountName).FirstOrDefault();

                if (account == null)
                {
                    return Ok(false);
                }
                else
                {
                    obj.AccountId = account.Id;
                    return Ok(contactRep.EditContactGeneralInfo(obj));
                }
            }
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactLocationInfo(ContactLocationInfo obj)
        {
            return Ok(contactRep.EditContactLocationInfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactContactInfo(ContactContactInfo obj)
        {
            return Ok(contactRep.EditContactContactinfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactTimeInfo(ContactTimeInfo obj)
        {
            return Ok(contactRep.EditContactTimeInfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactNoteInfo(ContactNoteInfo obj)
        {
            return Ok(contactRep.EditContactNotesInfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetAllContactsFiltered(FilteringContactsModel obj)
        {
            return Ok(contactRep.GetAllContactsFiltered(obj));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteContact(int id)
        {
            return Ok(contactRep.DeleteContact(id));
        }

   

    }
}
