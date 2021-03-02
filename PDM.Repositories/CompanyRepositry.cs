using Microsoft.EntityFrameworkCore;
using PDM.Data.Entity.Models;
using PDM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDM.Repositories
{
    public class CompanyRepository : BaseRepository, ICompanyRepository
    {

        public CompanyRepository(PharmatiseContext pharmatiseContext) : base(pharmatiseContext)
        {

        }
        public bool SaveCompany(ProCompany proCompany)
        {
            if (proCompany.CompanyId == 0)
                dbContext.ProCompany.Add(proCompany);
            return dbContext.SaveChanges() > 0;
        }
        public List<ProCompany> GetCompanies() => dbContext.ProCompany.ToList();
        public ProCompany GetCompany(long companyId) => dbContext.ProCompany.Include(i => i.Address).AsQueryable().First(c => c.CompanyId == companyId);

        public bool IsExistCompany(Company modelData)
        {
                return dbContext.ProCompany.Any(c => c.CompanyId != modelData.CompanyId && c.CompanyName == modelData.CompanyName && c.Address.Address1 == modelData.Address.Address1
                        && c.Address.Address2 == modelData.Address.Address2 && c.Address.PostCode == modelData.Address.PostCode);
            
        }

    }
}
