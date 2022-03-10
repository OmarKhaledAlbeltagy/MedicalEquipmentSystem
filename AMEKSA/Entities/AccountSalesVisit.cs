﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMEKSA.Privilage;

namespace AMEKSA.Entities
{
    public class AccountSalesVisit
    {
        public int Id { get; set; }

        public string extendidentityuserid { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public int AccountId { get; set; }

        public Account account { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime VisitTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SubmittingDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime SubmittingTime { get; set; }

        [MaxLength(1000)]
        public string VisitNotes { get; set; }

        [MaxLength(1000)]
        public string PaymentNotes { get; set; }

        public ICollection<AccountSalesVisitCollection> accountSalesVisitCollection { get; set; }

        public ICollection<AccountSalesVisitBrand> accountsalesvisitbrand { get; set; }

        public ICollection<AccountSalesVisitPerson> accountsalesvisitperson { get; set; }
    }
}
