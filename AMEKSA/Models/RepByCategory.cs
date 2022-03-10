using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class RepByCategory
    {
        public string RepName { get; set; }

        public int TotalContacts { get; set; }

        public int APlusContacts { get; set; }

        public int AContacts { get; set; }

        public int BContacts { get; set; }

        public int CContacts { get; set; }

        public float APlusContactsPercentage { get; set; }

        public float AContactsPercentage { get; set; }

        public float BContactsPercentage { get; set; }

        public float CContactsPercentage { get; set; }
    }
}
