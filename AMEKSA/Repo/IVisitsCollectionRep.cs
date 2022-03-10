﻿using AMEKSA.CustomEntities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IVisitsCollectionRep
    {
        IEnumerable<SalesCollection> GetMyTeamCollection(string ManagerId);

        IEnumerable<BrandAccountCollectionModel> BrandAccountCollectionByRep(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string UserId);

        IEnumerable<CollectionByBrandModel> CollectionByBrandFirstLineManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string ManagerId);

        IEnumerable<CollectionByRepModel> CollectionByRepFirstLineManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string ManagerId);

        IEnumerable<CollectionByBrandModel> CollectionByBrandTopManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto);

        IEnumerable<CollectionByRepModel> CollectionByRepTopManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto);

        CollectionByRepModel CollectionByRep(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string userId);

        IEnumerable<AccountBalanceModel> AccountByBalanceFirstLineManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, SearchByWord s);

        IEnumerable<AccountBalanceModel> AccountByBalanceTopManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, SearchByWord s);

        IEnumerable<AccountBalanceModel> AccountByBalanceRep(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string UserId);

        AccountBalanceModel AccountBalanceAccount(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, int AccountId);


        IEnumerable<AccountBalanceModel> AccountByBalanceRepExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string UserId);

        

    }
}
