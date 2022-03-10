﻿using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IBrandRep
    {
        Brand AddBrand(Brand obj);

        bool DeleteBrand(int id);

        bool EditBrand(Brand obj);

        IEnumerable<Brand> GetAllBrands();

        Brand GetBrandById(int id);

        IEnumerable<Brand> GetMyBrands(string userId);


        bool AccountBalanceSet();
   
    }
}
