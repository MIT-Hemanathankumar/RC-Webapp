using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using PDM.Data.EntityMapper;
using PDM.Helper;
using PDM.Model;
using PDM.Model.DataTableModel;
using PDM.Model.Parameter;
using PDM.Services;
using PDM.Session;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PDM.Web.Controllers
{

    public class CustomerController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IUserService userService;
        private readonly IOrderService orderService;

        public CustomerController(ICustomerService customerService, IUserService _userService, IOrderService _orderService, IWebHostEnvironment hostingEnvironment)
        {
            this.customerService = customerService;
            this.userService = _userService;
            this.orderService = _orderService;
            this.hostingEnvironment = hostingEnvironment;
        }
        // GET: /<controller>/
        public IActionResult List()
        {
            return View();
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(int customerId)
        {
            ViewBag.PickupTypes = customerService.GetPickupTypes();
            ViewBag.Countries = customerService.GetCountries();
            ViewBag.Routes = customerService.GetRoutes(userService.GetLoggdInUser().CompanyId);
            ViewBag.Surgeries = customerService.GetDoctors();
            ViewBag.PaymentExemptions = customerService.GetPaymentExemptions();
            ViewBag.Surgeries = customerService.GetDoctors();
            ViewBag.PreferredContactTypes = customerService.GetPreferredContactTypes();
            ViewBag.CaseTypes = customerService.GetCaseTypes(userService.GetLoggdInUser().CompanyId);
            var customerData = customerService.GetCustomer(customerId);
            CustomerModel modelData = MapperConfig.Mapper.Map<CustomerModel>(customerData);
            modelData.OldEmail = modelData.Email;
            modelData.OldMobile = modelData.Mobile;
            return View(modelData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerModel modelData, string Command)
        {
            if (modelData != null)
            {
                bool isInvalidCustomer = (modelData.OldMobile != modelData.Mobile || modelData.OldEmail != modelData.Email) ? customerService.IsExistCustomer(modelData) : false;
                bool isInvalidCustomerEmail = (modelData.OldEmail != modelData.Email) ? customerService.IsExistEmail(modelData.Email) : false;
                bool isInvalidCustomerMobile = (modelData.OldMobile != modelData.Mobile) ? customerService.IsExistMobile(modelData.Mobile) : false;

                if (!isInvalidCustomer && !isInvalidCustomerEmail && !isInvalidCustomerMobile)
                {
                    
                    bool isSaved = customerService.SaveCustomer(modelData);
                }
                else
                {
                    ViewBag.PickupTypes = customerService.GetPickupTypes();
                    ViewBag.Countries = customerService.GetCountries();
                    ViewBag.Routes = customerService.GetRoutes(userService.GetLoggdInUser().CompanyId);
                    ViewBag.PaymentExemptions = customerService.GetPaymentExemptions();
                    ViewBag.Surgeries = customerService.GetDoctors();
                    ViewBag.PreferredContactTypes = customerService.GetPreferredContactTypes();
                    ViewBag.CaseTypes = customerService.GetCaseTypes(userService.GetLoggdInUser().CompanyId);
                    string errors = string.Empty;
                    if (isInvalidCustomer) errors += $"<br/>{PDM.Services.ResourceService.Resource.GetMessage("EmDuplicateCustomer")}";
                    if (isInvalidCustomerEmail) errors += $"<br/>{PDM.Services.ResourceService.Resource.GetMessage("EmDuplicateEmail")}";
                    if (isInvalidCustomerMobile) errors += $"<br/>{PDM.Services.ResourceService.Resource.GetMessage("EmDuplicateMobile")}";
                    modelData.ShowValidationMsg = true;
                    ViewBag.Message = "The following error(s) have occurred: " + errors;
                    return View(modelData);
                }
            }

            return RedirectToAction("List");
        }
        public IActionResult GetCustomer(int customerId)
        {
            var modelData = customerService.GetCustomer(customerId);
            return PartialView("_CustomerDetailPartial", modelData);
        }

        public IActionResult Approval()
        {
            List<Customer> modelList = customerService.GetNonApprovalCustomers();
            return View(modelList);
        }

        public IActionResult GetExistCustomers(long customerId)
        {
            var dataModel = customerService.GetCustomer(customerId);
            var listModel = customerService.GetMatchingCustomers(dataModel);
            return PartialView("_MatchingCustomerPartial", Tuple.Create(dataModel, listModel));
        }
        public IActionResult GetMatchCustomer(long sourceCustomerId, long matchingCustomerId)
        {
            var sourceCustomerModel = customerService.GetCustomer(sourceCustomerId);
            var matchingCustomerModel = customerService.GetCustomer(matchingCustomerId);
            return PartialView("_PrimaryMatchCustomerPartial", Tuple.Create(sourceCustomerModel, matchingCustomerModel));
        }

        public IActionResult ApproveCustomer(long sourceCustomerId, long matchingCustomerId)
        {
            var sourceCustomerModel = customerService.GetCustomer(sourceCustomerId);
            var matchingCustomerModel = customerService.GetCustomer(matchingCustomerId);
            var isUpdated = customerService.ApproveCustomer(sourceCustomerModel, matchingCustomerModel);

            return Json(JsonConvert.SerializeObject(new { Status = isUpdated }));
        }

        public IActionResult ApproveCustomerByDirect(long sourceCustomerId)
        {
            var sourceCustomerModel = customerService.GetCustomer(sourceCustomerId);
            var isUpdated = customerService.ApproveCustomerByDirect(sourceCustomerModel);
            return Json(JsonConvert.SerializeObject(new { Status = isUpdated }));
        }

        public IActionResult RefuseCustomerByDirect(long sourceCustomerId)
        {
            var sourceCustomerModel = customerService.GetCustomer(sourceCustomerId);
            var isUpdated = customerService.RefuseCustomerByDirect(sourceCustomerModel);
            return Json(JsonConvert.SerializeObject(new { Status = isUpdated }));
        }

        public IActionResult Refuse()
        {
            List<Customer> modelList = customerService.GetRefuseCustomers();
            return View(modelList);
        }
        [HttpPost]
        public async Task<IActionResult> LoadListData([FromBody]DtParameters dtParameters)
        {
            var searchBy = dtParameters.Search?.Value;
            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "CustomerId";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            InputParameter param = new InputParameter();
            param.CompanyId = userService.GetLoggdInUser().CompanyId;
            param.BranchId = userService.GetLoggdInUser().BranchId;
            param.SearchValue = searchBy;
            param.PageNo = (dtParameters.Start / dtParameters.Length) + 1;
            param.PageSize = dtParameters.Length;
            param.SortColumn = orderCriteria;
            param.SortOrder = dtParameters.Order[0].Dir.ToString().ToLower();
            List<DtCustomerModel> customerList = customerService.GetCustomerListBySP(param);
            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            int totalResultsCount = 0;
            int filteredResultsCount = 0;
            if (customerList.Count > 0)
            {
                totalResultsCount = customerList[0].TotalCount;
                filteredResultsCount = customerList[0].TotalCount;
            }
            return Json(new DtResult<DtCustomerModel>
            {
                Draw = Converters.ConvertInt(dtParameters.Draw),
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = customerList
            });

        }

        public IActionResult MedicineList()
        {
            List<API.Model.Medicine> medicineList = orderService.GetCustomerOrdersMedicine(userService.GetLoggdInUser().CustomerId.Value);
            return View(medicineList);
        }

        public IActionResult ShowCustomerNHSData(long customerId)
        {
            ViewBag.Surgeries = customerService.GetDoctors();
            var customerModel = customerService.GetCustomer(customerId);
            return PartialView("_UpdateNHSCustomerPartial", customerModel);
        }

        public IActionResult UpdateCustomerNHSId(long customerId, string NHSId, string surgeryId)
        {
            long? docterid = !string.IsNullOrEmpty(surgeryId) ? Converters.ConvertLong(surgeryId) : (long?)null;

            bool isUpdated = customerService.UpdateCustomerNHSByDirect(new Customer { CustomerId = customerId, NHSNumber = NHSId, DoctorId = docterid });
            return Json(JsonConvert.SerializeObject(new { Status = isUpdated }));
        }

    }
}
