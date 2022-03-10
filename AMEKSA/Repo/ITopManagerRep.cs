using AMEKSA.CustomEntities;
using AMEKSA.FirstManagerModels;
using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Models;
namespace AMEKSA.Repo
{
   public interface ITopManagerRep
    {
        IEnumerable<VisitsCountModel> GetVisitsCountThisMonthReyad();

        IEnumerable<VisitsCountModel> GetVisitsCountThisMonthDamam();

        IEnumerable<VisitsCountModel> GetVisitsCountThisMonthJeddah();

        IEnumerable<VisitsCountModel> GetVisitsCountReyadByMonth(int year,int month);

        IEnumerable<VisitsCountModel> GetVisitsCountDamamByMonth(int year, int month);

        IEnumerable<VisitsCountModel> GetVisitsCountJeddahByMonth(int year, int month);

        IEnumerable<AccountVisitsPercentageByCategoryModel> AccountThisMonthPercentage();

        IEnumerable<AccountVisitsPercentageByCategoryModel> AccountpastMonthPercentage();

        IEnumerable<AccountVisitsPercentageByCategoryModel> ContactThisMonthPercentage();

        IEnumerable<AccountVisitsPercentageByCategoryModel> ContactPastMonthPercentage();

     

        IEnumerable<MyTeamModel> GetTeamMedical();

        IEnumerable<MyTeamModel> GetTeamSales();

        IEnumerable<CustomAccountMedicalVisit> GetDetailedAMV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<CustomContactMedicalVisit> GetDetailedCMV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<CustomAccountSalesVisit> GetDetailedASV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<CustomAccountSalesVisit> GetAccountSalesVisitsByUserId(string userId);

        IEnumerable<ExtendIdentityUser> GetAllFirstManagers();

        IEnumerable<CustomTarget> GetTarget(SearchTargetModel obj);

        IEnumerable<TopManagerMorrisLine> MorrisLine();

        //IEnumerable<CustomAccountSalesVisit> GetASDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        //IEnumerable<CustomAccountMedicalVisit> GetAMDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        //IEnumerable<CustomContactMedicalVisit> GetCMDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

    }
}
