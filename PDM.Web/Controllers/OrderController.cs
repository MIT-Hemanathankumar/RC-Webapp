using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using PDM.Helper;
using PDM.Model;
using PDM.Model.DataTableModel;
using PDM.Model.Parameter;
using PDM.Services;
using System.Text;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PDM.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IUserService userService;
        private readonly IConfiguration iConfiguration;
        
        public OrderController(IOrderService orderService, IUserService userService, IWebHostEnvironment hostingEnvironment, IConfiguration _iConfiguration)
        {
            this.orderService = orderService;
            this.userService = userService;
            this.hostingEnvironment = hostingEnvironment;
            this.iConfiguration = _iConfiguration;
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

        public IActionResult Create(long customerId)
        {
            ViewBag.Intervals = orderService.GetIntervals();
            ViewBag.PickTypes = orderService.GetPickupTypes();
            ViewBag.CaseTypes = orderService.GetCaseTypes(userService.GetLoggdInUser().CompanyId);
            ViewBag.OrderStatusList = orderService.GetOrderStatus();
            ViewBag.CustomerMedicines = orderService.GetCustomerOrdersMedicine(customerId);
            ViewBag.Routes = orderService.GetRoutes(userService.GetLoggdInUser().CompanyId);
            var modelData = orderService.CreateOrder(customerId);
            
            return View(modelData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order modelData, string Command)
        {
            ViewBag.Intervals = orderService.GetIntervals();
            ViewBag.PickTypes = orderService.GetPickupTypes();
            ViewBag.CaseTypes = orderService.GetCaseTypes(userService.GetLoggdInUser().CompanyId);
            ViewBag.Routes = orderService.GetRoutes(userService.GetLoggdInUser().CompanyId);
            if (modelData != null)
            {
                bool isSaved = orderService.SaveOrder(modelData, mailContent, surgeryOrderMailContent, FCMModelData);
            }

            return RedirectToAction("List");
        }

        public IActionResult BlankDetailRow()
        {
            var modelData = orderService.GetBlankOrderDetail();
            return PartialView("_OrderDetailPartial", modelData);
        }

        public IActionResult Detail(long orderId)
        {
            ViewBag.PickupTypes = orderService.GetPickupTypes();
            ViewBag.Intervals = orderService.GetIntervals();
            ViewBag.CaseTypes = orderService.GetCaseTypes(userService.GetLoggdInUser().CompanyId);
            var modelData = orderService.GetOrderDetail(orderId);
            ViewBag.CustomerMedicines = orderService.GetCustomerOrdersMedicine(modelData.CustomerId);
            ViewBag.OrderStatusList = orderService.GetOrderStatus();
            ViewBag.Routes = orderService.GetRoutes(userService.GetLoggdInUser().CompanyId);
            modelData.OldRecentDeliveryNote = modelData.RecentDeliveryNote;
            modelData.TempBranchNote = modelData.BranchNote;
            modelData.TempDeliveryNote = modelData.DeliveryNote;
            return View(modelData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detail(Order modelData, string Command)
        {
            if (modelData != null)
            {
                modelData.DeliveryNote = modelData.TempDeliveryNote;
                modelData.BranchNote = modelData.TempBranchNote;

                if (!string.IsNullOrEmpty(modelData.OldRecentDeliveryNote) && modelData.OldRecentDeliveryNote != modelData.RecentDeliveryNote)
                {
                    StringBuilder strdeliveryNote = new StringBuilder();
                    strdeliveryNote.Append(modelData.DeliveryNote);

                    if (string.IsNullOrEmpty(modelData.RecentDeliveryNote))
                    {
                        modelData.RecentDeliveryNote = modelData.OldRecentDeliveryNote;
                    }
                    else
                    {
                        strdeliveryNote.AppendLine(modelData.OldRecentDeliveryNote);
                    }

                    modelData.DeliveryNote = strdeliveryNote.ToString();

                }

                Model.FCMNotificationModel FCMModelData = new Model.FCMNotificationModel();
                var settingSection = iConfiguration.GetSection("Setting");
                if (settingSection != null)
                {
                    FCMModelData.ServerAPIKey = settingSection.GetSection("FCMServiceAPIKey").Value;
                    FCMModelData.SenderId = settingSection.GetSection("FCMSenderId").Value;
                }

                var registerUserTemplatePath = Path.Combine(hostingEnvironment.WebRootPath, "EmailTemplate", "OrderCreation.html");
                string mailContent = string.Empty;
                if (!string.IsNullOrEmpty(registerUserTemplatePath) && System.IO.File.Exists(registerUserTemplatePath))
                    mailContent = System.IO.File.ReadAllText(registerUserTemplatePath);

                var orderSurgeryTemplatePath = Path.Combine(hostingEnvironment.WebRootPath, "EmailTemplate", "OrderCreationWithSurgery.html");
                string surgeryOrderMailContent = string.Empty;
                if (!string.IsNullOrEmpty(orderSurgeryTemplatePath) && System.IO.File.Exists(orderSurgeryTemplatePath))
                {
                    surgeryOrderMailContent = System.IO.File.ReadAllText(orderSurgeryTemplatePath);
                }
                bool isSaved = orderService.UpdateOrder(modelData, mailContent, surgeryOrderMailContent, FCMModelData);
            }

            if (modelData.ActionType == 2)
            {
                ViewBag.PickupTypes = orderService.GetPickupTypes();
                ViewBag.Intervals = orderService.GetIntervals();
                ViewBag.CaseTypes = orderService.GetCaseTypes(userService.GetLoggdInUser().CompanyId);
                ViewBag.CustomerMedicines = orderService.GetCustomerOrdersMedicine(modelData.CustomerId);
                ViewBag.OrderStatusList = orderService.GetOrderStatus();
                ViewBag.Routes = orderService.GetRoutes(userService.GetLoggdInUser().CompanyId);
                modelData = orderService.GetOrderDetail(modelData.OrderId);
                modelData.OldRecentDeliveryNote = modelData.RecentDeliveryNote;
                ViewBag.ActionType = 2;
                return View(modelData);
            }
            else
                return RedirectToAction("List");
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            string downloadFile = Path.Combine(hostingEnvironment.WebRootPath, "Documents", "Customer", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(downloadFile, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(downloadFile), Path.GetFileName(downloadFile));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        public ActionResult PrintOrder(long orderId)
        {
            Order orderData = orderService.GetOrderDetail(orderId, false);
            return PartialView("_PrintOrderPartial", orderData);
        }

        public IActionResult GetOrderList(string startDate, string endDate, int orderStatusType)
        {
            List<DtOrderModel> orderList = new List<DtOrderModel>();
            ViewBag.FromDate = startDate;
            ViewBag.ToDate = endDate;
            ViewBag.OrderStatusType = orderStatusType;

            List<API.Model.Medicine> medicineList = orderService.GetMasterMedicine(-1, -1);
            
            return PartialView("OrderListPartial", orderList);
        }

        [HttpPost]
        public async Task<IActionResult> LoadListData([FromBody]DtParameters dtParameters)
        {
            var searchBy = dtParameters.Search?.Value;
            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "OrderId";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            InputParameter param = new InputParameter();
            param.SearchValue = searchBy;
            param.PageNo = (dtParameters.Start / dtParameters.Length) + 1;
            param.PageSize = dtParameters.Length;
            param.SortColumn = orderCriteria;
            param.SortOrder = dtParameters.Order[0].Dir.ToString().ToLower();
            param.FromDate = dtParameters.FromDate.HasValue ? dtParameters.FromDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            param.ToDate = dtParameters.ToDate.HasValue ? dtParameters.ToDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            param.OrderStatusType = dtParameters.Type.HasValue ? dtParameters.Type.Value : 1;
            List<DtOrderModel> orderList = orderService.GetOrderListBySP(param);
            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            int totalResultsCount = 0;
            int filteredResultsCount = 0;
            if (orderList.Count > 0)
            {
                totalResultsCount = orderList[0].TotalCount;
                filteredResultsCount = orderList[0].TotalCount;
            }
            return Json(new DtResult<DtOrderModel>
            {
                Draw = Converters.ConvertInt(dtParameters.Draw),
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = orderList
            });

        }

    }
}
