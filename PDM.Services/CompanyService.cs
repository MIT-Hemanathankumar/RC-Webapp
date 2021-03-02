using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PDM.Data.Entity;
using PDM.Data.Entity.Models;
using PDM.Data.EntityMapper;
using PDM.Helper;

namespace PDM.Services
{
    public sealed class CompanyService : BaseService, ICompanyService
    {
        private readonly ICompanyRepository companyRepositry;
        private readonly IUserRepository userRepository;
        private readonly IMasterRepository masterRepository;
        private readonly IMasterService masterService;
        private readonly IUserService userService;
        private readonly ILogger<CompanyService> logger;

        public CompanyService(ICompanyRepository companyRepositry, IUserRepository userRepository, IMasterRepository masterRepository, IMasterService masterService, IUserService userService, ILogger<CompanyService> logger) : base(companyRepositry)
        {
            this.companyRepositry = companyRepositry;
            this.userRepository = userRepository;
            this.masterRepository = masterRepository;
            this.masterService = masterService;
            this.userService = userService;
            this.logger = logger;
        }

        private Company MapCompanyData(long companyId)
        {
            Company companyData = new Company();
            try
            {
                var proCompany = companyRepositry.GetCompany(companyId);
                companyData = MapperConfig.Mapper.Map<Company>(proCompany);
                companyData.Address = MapperConfig.Mapper.Map<Address>(proCompany.Address);

                proCompany.Address.ProContact.ToList().ForEach(f =>
                {
                    if (f.ContactTypeId == (int)ContactTypes.Email)
                    {
                        companyData.Email = f.Value;
                    }
                    else if (f.ContactTypeId == (int)ContactTypes.Mobile)
                    {
                        companyData.Mobile = f.Value;
                    }
                    else if (f.ContactTypeId == (int)ContactTypes.ContactPerson)
                    {
                        companyData.ContactPerson = f.Value;
                    }
                });
                if (Converters.ConvertInt(proCompany.SubscriptionId) > 0)
                    companyData.Subscription = masterRepository.GetSubscription(Converters.ConvertInt(proCompany.SubscriptionId)).Subscription;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return companyData;
        }

        public List<Company> GetCompanies()
        {
            List<Company> companyList = new List<Company>();
            try
            {
                companyRepositry.GetCompanies().ForEach(f =>
                {
                    companyList.Add(MapCompanyData(f.CompanyId));
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return companyList;
        }

        public Company GetCompany(long companyId)
        {
            var companyData = new Company();
            try
            {
                List<WeekDay> weekDayList = GetWeekDays();
                if (companyId > 0)
                {
                    companyData = MapCompanyData(companyId);
                    companyData.OldCompanyName = companyData.CompanyName;
                    companyData.Address.OldAddress1 = companyData.Address.Address1;
                    companyData.Address.OldAddress2 = companyData.Address.Address2;
                    companyData.Address.OldPostCode = companyData.Address.PostCode;
                    companyData.OldMobile = companyData.Mobile;

                    
                }
                else
                {
                    companyData.Address = companyData.Address ?? new Address();
                    companyData.Address.Contacts = companyData.Address.Contacts ?? new List<Contact>();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return companyData;
        }
        public bool SaveCompany(Company modelData, string confirmationUrl, string mailContent)
        {
            bool IsNew = false;
            bool bSuccess = false;
            try
            {
                ProCompany companyData;
                if (modelData.CompanyId > 0)
                {
                    companyData = companyRepositry.GetCompany(modelData.CompanyId);
                    modelData.ModifiedOn = DateTime.Now;
                    modelData.ModifiedBy = userService.GetLoggdInUser().UserId;
                }
                else
                {
                    companyData = new ProCompany();
                    modelData.CreatedOn = DateTime.Now;
                    modelData.CreatedBy = userService.GetLoggdInUser().UserId;
                    modelData.IsActive = 1;
                    IsNew = true;
                }
                

                bSuccess = companyRepositry.SaveCompany(companyData);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return bSuccess;
        }

        public bool IsExistCompany(Company modelData)
        => companyRepositry.IsExistCompany(modelData);

    }
}
