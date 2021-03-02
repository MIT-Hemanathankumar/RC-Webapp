using Microsoft.EntityFrameworkCore;
using PDM.Data.Entity.Models;
using PDM.Helper;
using PDM.Model;
using PDM.Model.Parameter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PDM.Repositories
{
    public class DeliveryRepository : BaseRepository, IDeliveryRepository
    {
        public DeliveryRepository(PharmatiseContext pharmatiseContext) : base(pharmatiseContext)
        {

        }

        public ProDelivery GetDelivery(long deliveryId) => dbContext.ProDelivery.AsQueryable().First(c => c.DeliveryId == deliveryId);

        public ProOrderDetail GetBlankOrderDetail()
        {
            throw new NotImplementedException();
        }

        public List<ProDelivery> GetDeliveries() => dbContext.ProDelivery.Where(d => d.DeliveryStatus != (int)DeliveryStatus.Cancelled).ToList();

        public ProOrder GetOrder(int OrderId) => dbContext.ProOrder.AsQueryable().Include(a => a.Customer).First(c => c.OrderId == OrderId);

        public ProDelivery GetOrderDelivery(long orderId) => dbContext.ProDelivery.FirstOrDefault(d => d.OrderId == orderId && d.DeliveryStatus != (int)DeliveryStatus.Cancelled);

        public bool SaveDelivery(ProDelivery modelData)
        {
            if (modelData.DeliveryId == 0)
                dbContext.ProDelivery.Add(modelData);
            return dbContext.SaveChanges() > 0;
        }

        public List<ProDelivery> GetDeliveries(int Route) => dbContext.ProDelivery.Include(a => a.Order).Where(w => w.DeliveryStatus != (int)DeliveryStatus.Cancelled && w.RouteId == Route).ToList();

        public List<ProDelivery> GetDashboardDeliveries(long companyId) => dbContext.ProDelivery.Include(a => a.Order).Where(w => w.DeliveryStatus != (int)DeliveryStatus.Cancelled && w.Order.CompanyId == companyId).ToList();
        public List<Dashboard> GetDriverWiseSummary(InputParameter param)
        {
            List<Dashboard> driverWiseSummary = null;

            DataSet dsDashboardData = GetData($"exec sp_phar_dashboard_driver_summary {param.CompanyId},{param.BranchId},{param.UserId}", "DriverList");
            if (dsDashboardData != null && dsDashboardData.Tables.Count > 0)
            {
                var dashboardList = dsDashboardData.Tables[0].ToList<Dashboard>();
                if (dashboardList != null && dashboardList.Count > 0)
                    driverWiseSummary = dashboardList;
            }

            return driverWiseSummary;
        }

        public List<Model.DtOrderModel> GetDeliveryListBySP(InputParameter param)
        {
            List<DtOrderModel> deliveryList = new List<DtOrderModel>();
            DataSet dsDeliveryData = GetData($"exec sp_phar_delivery_list {param.CompanyId},{param.BranchId},{param.Customer},'{param.FromDate}','{param.ToDate}',{param.DeliveryStatusType},'{param.SearchValue}',{param.PageNo},{param.PageSize},'{param.SortColumn}','{param.SortOrder}'", "Delivery");
            if (dsDeliveryData != null && dsDeliveryData.Tables.Count > 0)
            {
                deliveryList = dsDeliveryData.Tables[0].ToList<DtOrderModel>();
            }
            return deliveryList;
        }

        public Dashboard GetDashboardDeliveryCount(InputParameter param)
        {
            Dashboard dashboardData = null;

            DataSet dsDashboardData = GetData($"exec sp_phar_dashboard_delivery_count {param.CompanyId},{param.BranchId},{param.Customer},'{param.FromDate}','{param.ToDate}'", "Dashboard");
            if (dsDashboardData != null && dsDashboardData.Tables.Count > 0)
            {
                var dashboardList = dsDashboardData.Tables[0].ToList<Dashboard>();
                if (dashboardList != null && dashboardList.Count > 0)
                    dashboardData = dashboardList[0];
            }
            return dashboardData;
        }

    }
}
