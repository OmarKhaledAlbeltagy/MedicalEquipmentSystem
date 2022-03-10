using AMEKSA.Context;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class HrRep:IHrRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public HrRep(DbContainer db, ITimeRep ti, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.ti = ti;
            this.userManager = userManager;
        }

        public IEnumerable<DateTime> GetActualWorkingDaysMedical(string userId)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime yearstart = now.AddYears(-1);
            int days = DateTime.DaysInMonth(now.Year, now.Month);
            DateTime start = new DateTime(yearstart.Year, yearstart.Month, 1, 0, 0, 0);
            DateTime end = new DateTime(now.Year, now.Month, days, 23, 59, 59);
            IEnumerable<DateTime> res = db.contactMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.DayOfYear).Select(a => a.VisitDate);
            return res;
        }

        public IEnumerable<DateTime> GetActualWorkingDaysSales(string userId)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime yearstart = now.AddYears(-1);
            int days = DateTime.DaysInMonth(now.Year, now.Month);
            DateTime start = new DateTime(yearstart.Year, yearstart.Month, 1, 0, 0, 0);
            DateTime end = new DateTime(now.Year, now.Month, days, 23, 59, 59);
            IEnumerable<DateTime> res = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.DayOfYear).Select(a => a.VisitDate);
            return res;
        }

        public IEnumerable<ExtendIdentityUser> GetAllMedicalRep()
        {
            IEnumerable<ExtendIdentityUser> res = userManager.GetUsersInRoleAsync("Medical Representative").Result;

            return res.OrderBy(a=>a.FullName);
        }

        public IEnumerable<ExtendIdentityUser> GetAllSalesRep()
        {
            IEnumerable<ExtendIdentityUser> res = userManager.GetUsersInRoleAsync("Sales Representative").Result;

            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<DateTime> GetTimeOffDays(string userId)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime yearstart = now.AddYears(-1);
            int days = DateTime.DaysInMonth(now.Year, now.Month);
            DateTime start = new DateTime(yearstart.Year, yearstart.Month, 1, 0, 0, 0);
            DateTime end = new DateTime(now.Year, now.Month, days, 23, 59, 59);
            
            IEnumerable<DateTime> res = db.userTimeOff.Where(a => a.ExtendIdentityUserId == userId && a.DateTimeFrom >= start && a.DateTimeFrom <= end).Select(a => a.DateTimeFrom);

            return res;
        }
    }
}
