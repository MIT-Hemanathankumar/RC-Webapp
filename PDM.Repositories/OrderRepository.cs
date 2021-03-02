using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PDM.Data.Entity.Models;
using PDM.Helper;
using PDM.Model;
using PDM.Model.Parameter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace PDM.Repositories
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(PharmatiseContext pharmatiseContext) : base(pharmatiseContext)
        {

        }

        public ProOrder CreateOrder(long customerId)
        {
            throw new NotImplementedException();
        }

        public ProOrderDetail GetBlankOrderDetail()
        {
            throw new NotImplementedException();
        }

        public ProOrder GetOrder(long orderId) => dbContext.ProOrder.AsQueryable().First(c => c.OrderId == orderId);

        public List<ProOrder> GetOrders() => dbContext.ProOrder.AsQueryable().ToList();

        public List<ProOrder> GetCustomerOrders(long customerId) => dbContext.ProOrder.AsQueryable().Where(o => o.CustomerId == customerId).ToList();
        public bool SaveOrder(ProOrder orderData, Order modelData)
        {
            
            return dbContext.SaveChanges() > 0;
        }
        private string GetChangedDetail(ProOrder modelData)
        {
            string changes = "";
            var properties = typeof(ProOrder).GetProperties();
            var OrderTypes = dbContext.MasOrderType.ToList();
            var PickTypes = dbContext.MasPickupType.ToList();
            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = $"SELECT * FROM [ProOrder] WHERE OrderId={modelData.OrderId}";
                dbContext.Database.OpenConnection();
                string[] memoFields = { "OrderStatus", "DeliveryNote", "BranchNote", "PickupTypeId", "OrderTypeId", "DeliveryDate", "IsStorageFridge", "IsControlledDrugs", "RackNo" };
                using (var result = command.ExecuteReader())
                {
                    if (result.HasRows)
                    {
                        if (result.Read())
                        {
                            foreach (var item in properties)
                            {
                                if (memoFields.Contains(item.Name))
                                {
                                    var newValue = item.GetValue(modelData);
                                    var oldValue = result.GetFieldValue<object>(item.Name);
                                    if (!string.Equals(newValue, oldValue) && !(newValue == null && oldValue == DBNull.Value))
                                    {
                                        switch (item.Name)
                                        {
                                            case "OrderStatus":
                                                changes += $"<br /><b>Statuc</b> has been changed from {(OrderStatus)((int)oldValue)} to {(OrderStatus)((int)newValue)}";
                                                break;
                                            
                                            case "RackNo":
                                                changes += $"<br /><b>Location</b> has been changed from {oldValue} to {newValue}";
                                                break;
                                        }
                                    }
                                }

                            }
                        }
                    }

                }
            }

            return changes;
        }
        public Dashboard GetDashboardOrders(InputParameter param)
        {
            Dashboard dashboardData = null;
            DataSet dsDashboardData = GetData($"exec sp_phar_dashboard_count {param.CompanyId},{param.BranchId},{param.DateType}", "Dashboard");
            if (dsDashboardData != null && dsDashboardData.Tables.Count > 0)
            {
                var dashboardList = dsDashboardData.Tables[0].ToList<Dashboard>();
                if (dashboardList != null && dashboardList.Count > 0)
                    dashboardData = dashboardList[0];
            }
            return dashboardData;
        }

        public Dashboard GetDashboardDeliveries(InputParameter param)
        {
            Dashboard dashboardData = null;
            DataSet dsDashboardData = GetData($"exec sp_phar_dashboard_delivery_count {param.CompanyId},{param.BranchId},{param.DateType}", "Dashboard");
            if (dsDashboardData != null && dsDashboardData.Tables.Count > 0)
            {
                var dashboardList = dsDashboardData.Tables[0].ToList<Dashboard>();
                if (dashboardList != null && dashboardList.Count > 0)
                    dashboardData = dashboardList[0];
            }

            return dashboardData;
        }

        public List<OrderMedicine> GetCustomerOrdersMedicine(long customerId)
        {
            List<OrderMedicine> customerMedicines = new List<OrderMedicine>();
            DataSet dsCustomerMedicines = GetData($"exec sp_phar_customer_medicines {customerId}", "CustomerMedicines");
            if (dsCustomerMedicines?.Tables?.Count > 0)
                customerMedicines = dsCustomerMedicines.Tables[0].ToList<OrderMedicine>();

            return customerMedicines;
        }

        public List<OrderDetail> GetMasterMedicine(long companyId, long branchId, string medicineName)
        {
            List<OrderDetail> masterMedicines = new List<OrderDetail>();
            DataSet dsCustomerMedicines = GetData($"exec sp_phar_master_medicines {companyId},{branchId}", "MasterMedicines");
            if (dsCustomerMedicines?.Tables?.Count > 0)
                masterMedicines = dsCustomerMedicines.Tables[0].ToList<OrderDetail>();

            return masterMedicines;
        }

        public List<Model.Order> GetOrdersInline(long companyId, long branchId)
        {
            List<Model.Order> orderList = new List<Model.Order>();
            DataSet dsOrders = GetData($"select o.OrderId,o.OrderDate,o.OrderStatus,c.FirstName,c.LastName from ProOrder o inner join ProCustomer c  on o.CustomerId=c.CustomerId where o.CompanyId ={companyId} and o.BranchId ={branchId};", "Orders");
            if (dsOrders?.Tables?.Count > 0)
                orderList = dsOrders.Tables[0].ToList<Model.Order>();

            return orderList;
        }

        public bool IsOrderExist(long companyId, long branchId)
        => dbContext.ProOrder.Any(w => w.CompanyId == companyId && w.BranchId == branchId);

        public List<Model.DtOrderModel> GetOrderListBySP(InputParameter param)
        {
            List<DtOrderModel> orderList = new List<DtOrderModel>();
            DataSet dsOrderData = GetData($"exec sp_phar_order_list {param.CompanyId},{param.BranchId},{param.Customer},'{param.FromDate}','{param.ToDate}',{param.OrderStatusType},'{param.SearchValue}',{param.PageNo},{param.PageSize},'{param.SortColumn}','{param.SortOrder}'", "Order");
            if (dsOrderData != null && dsOrderData.Tables.Count > 0)
            {
                orderList = dsOrderData.Tables[0].ToList<DtOrderModel>();
            }
            return orderList;
        }
        public Dashboard GetDashboardOrderCount(InputParameter param)
        {
            Dashboard dashboardData = null;

            DataSet dsDashboardData = GetData($"exec sp_phar_dashboard_order_count {param.CompanyId},{param.BranchId},{param.Customer},'{param.FromDate}','{param.ToDate}'", "Dashboard");
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
