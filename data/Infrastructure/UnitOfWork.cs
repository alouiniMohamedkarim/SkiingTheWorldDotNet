﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mehdi.data;
using data;
using domaine.entities;

namespace Mehdi.data.Infrastructure
{
	public class UnitOfWork : IUnitOfWork
	{

        private SWModel dataContext;

		IDatabaseFactory dbFactory;
		public UnitOfWork(IDatabaseFactory dbFactory)
		{
			this.dbFactory = dbFactory;
			dataContext = dbFactory.DataContext;
		}

		public void Commit()
		{
			dataContext.SaveChanges();
		}
		public void Dispose()
		{
			dataContext.Dispose();
		}
		public IRepository<T> GetRepository<T>() where T : class
		{
			IRepository<T> repo = new RepositoryBase<T>(dbFactory);
			return repo;
		}



	}
}
