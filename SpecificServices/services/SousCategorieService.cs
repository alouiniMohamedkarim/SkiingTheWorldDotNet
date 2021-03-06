﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mehdi.data.Infrastructure;
using Mehdi.Services;
using SkiingTheWorld_PI.Domaine.Entities;


namespace SpecificServices.services
{
    public class SousCategorieService:Service<souscategorie>
    {
        private static IDatabaseFactory dbf = new DatabaseFactory();
        private static IUnitOfWork uow = new UnitOfWork(dbf);
        public SousCategorieService() : base(uow)
        {

        }


        public IEnumerable<souscategorie>getByCategory(souscategorie.CategorieEnum cat)
        {
            return dbf.DataContext.souscategories.Where(sc => sc.MarquecategorieProd == cat);
        }

    }
}
