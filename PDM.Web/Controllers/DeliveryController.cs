using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PDM.Helper;
using PDM.Model;
using PDM.Model.DataTableModel;
using PDM.Model.Parameter;
using PDM.Services;
using PDM.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PDM.Web.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly IDeliveryService deliveryService;
        private readonly IUserService userService;
        private readonly IConfiguration iConfiguration;

        public DeliveryController(IDeliveryService deliveryService, IUserService _userService, IConfiguration _iConfiguration)
        {
            this.deliveryService = deliveryService;
            this.userService = _userService;
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
        public IActionResult Create(int orderId)
        {
            var modelData = deliveryService.CreateDelivery(orderId);
            return View(modelData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Delivery modelData, string Command)
        {
            if (modelData != null)
            {
                Model.FCMNotificationModel FCMModelData = new Model.FCMNotificationModel();
                var settingSection = iConfiguration.GetSection("Setting");
                if (settingSection != null)
                {
                    FCMModelData.ServerAPIKey = settingSection.GetSection("FCMServiceAPIKey").Value;
                    FCMModelData.SenderId = settingSection.GetSection("FCMSenderId").Value;
                }
                bool isSaved = deliveryService.SaveDelivery(modelData, FCMModelData);
            }

            return RedirectToAction("List");
        }
        public IActionResult Detail(int deliveryId)
        {
            ViewBag.PickupTypes = deliveryService.GetPickupTypes();
            ViewBag.DeliveryStatusList = deliveryService.GetDeliveryStatus();
            ViewBag.Routes = deliveryService.GetRoutes(userService.GetLoggdInUser().CompanyId);
            var modelData = deliveryService.GetDelivery(deliveryId);
            return View(modelData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detail(Delivery modelData, string Command)
        {
            if (modelData != null)
            {
                Model.FCMNotificationModel FCMModelData = new Model.FCMNotificationModel();
                var settingSection = iConfiguration.GetSection("Setting");
                if (settingSection != null)
                {
                    FCMModelData.ServerAPIKey = settingSection.GetSection("FCMServiceAPIKey").Value;
                    FCMModelData.SenderId = settingSection.GetSection("FCMSenderId").Value;
                }

                bool isSaved = deliveryService.SaveDelivery(modelData, FCMModelData);
            }

            return RedirectToAction("List");
        }

        public IActionResult GetDeliveryList(string startDate, string endDate, int deliveryStatusType)
        {
            List<DtOrderModel> deliveryList = new List<DtOrderModel>();
            ViewBag.FromDate = startDate;
            ViewBag.ToDate = endDate;
            ViewBag.DeliveryStatusType = deliveryStatusType;
            return PartialView("DeliveryListPartial", deliveryList);
        }

        [HttpPost]
        public async Task<IActionResult> LoadListData([FromBody]DtParameters dtParameters)
        {
            var searchBy = dtParameters.Search?.Value;
            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "DeliveryId";
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
            param.DeliveryStatusType = dtParameters.Type.HasValue ? dtParameters.Type.Value : 1;
            List<DtOrderModel> deliveryList = deliveryService.GetDeliveryListBySP(param);
            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            int totalResultsCount = 0;
            int filteredResultsCount = 0;
            if (deliveryList.Count > 0)
            {
                totalResultsCount = deliveryList[0].TotalCount;
                filteredResultsCount = deliveryList[0].TotalCount;
            }
            return Json(new DtResult<DtOrderModel>
            {
                Draw = Converters.ConvertInt(dtParameters.Draw),
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = deliveryList
            });

        }

    }
}
