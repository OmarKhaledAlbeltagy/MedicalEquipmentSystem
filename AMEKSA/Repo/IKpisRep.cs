using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IKpisRep
    {
        IEnumerable<Properties> GetAllProperties();

        bool EditProperty(int id, int value);

        SalesKpiModel GetSalesKpi(int year, int month, string userId);

        IEnumerable<SalesKpiModel> GetAllSalesKpi(int year, int month);

        IEnumerable<SalesKpiModel> GetTeamSalesKpi(int year, int month, string managerId);



        MedicalKpiModel GetMedicalKpi(int year, int month, string userId);

        IEnumerable<MedicalKpiModel> GetAllMedicalKpi(int year, int month);

        IEnumerable<MedicalKpiModel> GetTeamMedicalKpi(int year, int month, string managerId);

        int GetTimeOffDiff(string id, int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto);



    }
}
