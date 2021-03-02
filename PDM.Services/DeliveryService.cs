using AutoMapper;
using Microsoft.Extensions.Logging;
using PDM.API.Model;
using PDM.Data.Entity;
using PDM.Data.Entity.Models;
using PDM.Data.EntityMapper;
using PDM.Helper;

namespace PDM.Services
{
    public sealed class DeliveryService : BaseService, IDeliveryService
    {
        private readonly IDeliveryRepository deliveryRepository;
        private readonly IUserService userService;
        private readonly IOrderService orderService;
        private readonly ICustomerService customerService;
        private readonly ICustomerRepository customerRepositry;
        private readonly ILogger<DeliveryService> logger;
        

        public DeliveryService(IDeliveryRepository deliveryRepository, IUserService userService, IOrderService orderService, ICustomerService customerService,
            ICustomerRepository customerRepositry, ILogger<DeliveryService> logger, IMasterRepository masterRepository, IOrderRepository orderRepository, IUserRepository _userRepository,
            IBranchRepository _branchRepository, IFCMService _FCMService, IMessageTemplateService messageTemplateService) : base(deliveryRepository)
        {
            this.deliveryRepository = deliveryRepository;
            this.userService = userService;
            this.orderService = orderService;
            this.customerService = customerService;
            this.customerRepositry = customerRepositry;
            this.masterRepository = masterRepository;
            
        }


        private Delivery MapDeliveryData(long deliveryId)
        {
            Delivery deliveryData = new Delivery();
            try
            {
                var proDelivery = deliveryRepository.GetDelivery(deliveryId);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return deliveryData;
        }

        public Delivery CreateDelivery(long orderId)
        {
            var deliveryData = new Delivery();
            try
            {
                if (orderId > 0)
                {
                    var orderData = orderService.GetOrderDetail(orderId, false);
                    deliveryData.OrderId = orderData.OrderId;
                    if (orderData.CustomerId > 0)
                        deliveryData.Customer = customerService.GetCustomer(orderData.CustomerId);
                    deliveryData.CustomerId = orderData.CustomerId;
                }
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return deliveryData;
        }

        public bool SaveDelivery(Delivery modelData, FCMNotificationModel FCMModelData)
        {
            bool bSuccess = false;
            try
            {
                ProDelivery deliveryData;
                if (modelData.DeliveryId > 0)
                {
                    deliveryData = deliveryRepository.GetDelivery(modelData.DeliveryId);
                    modelData.ModifiedOn = DateTime.Now;
                    modelData.ModifiedBy = userService.GetLoggdInUser().UserId;
                }
                else
                {
                    deliveryData = new ProDelivery();
                    modelData.CreatedOn = DateTime.Now;
                    modelData.CreatedBy = userService.GetLoggdInUser().UserId;
                    modelData.CompanyId = userService.GetLoggdInUser().CompanyId;
                    modelData.BranchId = userService.GetLoggdInUser().BranchId; ;
                    modelData.DeliveryStatus = (int)DeliveryStatus.NotReady;
                }
                if (modelData.InputDeliveryDate.HasValue)
                {
                    modelData.DeliveryDate = modelData.InputDeliveryDate.Value;
                }
                deliveryData = MapperConfig.Mapper.Map<Delivery, ProDelivery>(modelData, deliveryData);
                bSuccess = deliveryRepository.SaveDelivery(deliveryData);
                if (bSuccess)
                {
                    ProOrder orderData = orderRepository.GetOrder(modelData.OrderId);
                    orderData.ModifiedOn = DateTime.Now;
                    orderData.ModifiedBy = userService.GetLoggdInUser().UserId;
                    Model.Order orderModelData = new Model.Order();
                    int? orderOldStatus = orderData.OrderStatus;
                    orderData.OrderStatus = (int)GetOrderStatusForDeliveryStatus((DeliveryStatus)modelData.DeliveryStatus);
                    orderModelData.Remarks = $"Changed status to {GetOrderStatusForDeliveryStatus((DeliveryStatus)modelData.DeliveryStatus).ToString()}";
                    bSuccess = orderRepository.SaveOrder(orderData, orderModelData);

                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return bSuccess;
        }

        public List<Delivery> GetDeliveries()
        {
            List<Delivery> deliveryList = new List<Delivery>();
            try
            {
                deliveryRepository.GetDeliveries().ForEach(f =>
                {
                    var deliveryData = MapperConfig.Mapper.Map<Delivery>(f);
                    if (deliveryData.DeliveryStatus > 0)
                        deliveryData.DeliveryStatusDesc = ResourceService.Resource.GetCaption(((DeliveryStatus)deliveryData.DeliveryStatus).ToString());
                    if (deliveryData.RouteId != null)
                        deliveryData.RouteName = masterRepository.GetRoute(deliveryData.RouteId.Value)?.RouteName;
                    
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return deliveryList;
        }

        public Delivery GetDelivery(long deliveryId)
        {
            var deliveryData = new Delivery();
            try
            {
                if (deliveryId > 0)
                {
                    deliveryData = MapDeliveryData(deliveryId);
                    deliveryData.InputDeliveryDate = deliveryData.DeliveryDate;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return deliveryData;
        }

        public List<DeliveryDetails> GetDeliveries(int Route)
        {
            List<DeliveryDetails> deliveryList = new List<DeliveryDetails>();
            try
            {
                var proDeliveryList = deliveryRepository.GetDeliveries(Route).ToList();

                DeliveryDetails deliveryDetailData = null;
                Address addressData = null;
                foreach (var item in proDeliveryList)
                {
                    deliveryDetailData = new DeliveryDetails
                    {
                        CustomerName = $"{item.Order.Customer.FirstName} {item.Order.Customer.LastName}",
                        DeliveryDate = item.DeliveryDate.Value.ToString(PDM.Helper.ConfigurationKeys.APIDateFormat),
                        DeliveryId = item.DeliveryId,
                        DeliveryStatus = item.DeliveryStatus,
                        DeliveryStatusDesc = ((DeliveryStatus)item.DeliveryStatus).ToString(),
                        OrderId = item.OrderId.Value,
                        DeliveryNote = item.Order.DeliveryNote,
                        RecentDeliveryNote = item.Order.RecentDeliveryNote
                    };
                    

                    deliveryList.Add(deliveryDetailData);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return deliveryList;
        }

        public List<Delivery> GetDashboardDeliveries()
        {
            List<Delivery> deliveryList = new List<Delivery>();
            try
            {
                deliveryRepository.GetDashboardDeliveries(userService.GetLoggdInUser().CompanyId).OrderByDescending(d => d.DeliveryId).Take(3).ToList().ForEach(f =>
                  {
                      var deliveryData = MapperConfig.Mapper.Map<Delivery>(f);
                      if (deliveryData.DeliveryStatus > 0)
                          deliveryData.DeliveryStatusDesc = ResourceService.Resource.GetCaption(((DeliveryStatus)deliveryData.DeliveryStatus).ToString());
                      if (deliveryData.RouteId != null)
                          deliveryData.RouteName = masterRepository.GetRoute(deliveryData.RouteId.Value).RouteName;
                      
                  });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return deliveryList;
        }

        public List<Dashboard> GetDriveryWiseDeliveries()
        {

            InputParameter param = new InputParameter
            {
                CompanyId = userService.GetLoggdInUser().CompanyId,
                BranchId = userService.GetLoggdInUser().BranchId,
                UserId = (userService.GetLoggdInUser().Role.ToLower().Equals("driver")) ? userService.GetLoggdInUser().UserId : -1
            };

            List<Dashboard> driverWiseSummary = deliveryRepository.GetDriverWiseSummary(param);
            return (driverWiseSummary != null) ? driverWiseSummary : new List<Dashboard>();
        }

        public bool UpdateAPIDelivery(Delivery modelData, FCMNotificationModel FCMModelData)
        {
            bool bSuccess = false;
            try
            {
                if (modelData.DeliveryId > 0)
                {
                    ProDelivery deliveryData = deliveryRepository.GetDelivery(modelData.DeliveryId);
                    deliveryData.ModifiedOn = DateTime.Now;
                    deliveryData.ModifiedBy = modelData.DriverId;
                    deliveryData.DeliveryStatus = modelData.DeliveryStatus;
                    deliveryData.DeliveredTo = modelData.DeliveredTo;
                    deliveryData.DeliveryRemarks = modelData.DeliveryRemarks;
                    bSuccess = deliveryRepository.SaveDelivery(deliveryData);
                    if (bSuccess)
                    {
                        ProOrder orderData = orderRepository.GetOrder(deliveryData.OrderId.Value);
                        orderData.ModifiedOn = DateTime.Now;
                        orderData.ModifiedBy = modelData.DriverId;
                        Model.Order orderModelData = new Model.Order();
                        int? orderOldStatus = orderData.OrderStatus;
                        orderData.OrderStatus = (int)GetOrderStatusForDeliveryStatus((DeliveryStatus)deliveryData.DeliveryStatus);
                        orderModelData.Remarks = $"Changed status to {GetOrderStatusForDeliveryStatus((DeliveryStatus)deliveryData.DeliveryStatus).ToString()}";
                        bSuccess = orderRepository.SaveOrder(orderData, orderModelData);
                        
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return bSuccess;
        }

        public bool UpdateAPIDeliveryAcknowledgement(Delivery modelData)
        {
            bool bSuccess = false;
            try
            {

                if (modelData.DeliveryId > 0)
                {
                    ProDelivery deliveryData = deliveryRepository.GetDelivery(modelData.DeliveryId);
                    deliveryData.ModifiedOn = DateTime.Now;
                    deliveryData.ModifiedBy = modelData.DriverId;
                    deliveryData.BaseSignature = modelData.BaseSignature;
                    bSuccess = deliveryRepository.SaveDelivery(deliveryData);

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return bSuccess;
        }

        private OrderStatus GetOrderStatusForDeliveryStatus(DeliveryStatus deliveryStatus)
        {
            switch (deliveryStatus)
            {
                case DeliveryStatus.NotReady:
                    return OrderStatus.Received;
                case DeliveryStatus.ReadyForDelivery:
                    return OrderStatus.Ready;
                case DeliveryStatus.OutForDelivery:
                    return OrderStatus.OutForDelivery;
                case DeliveryStatus.Completed:
                    return OrderStatus.Completed;
                case DeliveryStatus.Failed:
                case DeliveryStatus.Returned:
                case DeliveryStatus.Cancelled:
                    return OrderStatus.Failed;
                default:
                    return OrderStatus.Requested;
            }
        }

        public List<Model.DtOrderModel> GetDeliveryListBySP(InputParameter param)
        {
            List<DtOrderModel> deliveryList = new List<DtOrderModel>();
            param.CompanyId = userService.GetLoggdInUser().CompanyId;
            param.BranchId = userService.GetLoggdInUser().BranchId;
            param.Customer = "-1";
            if (userService.GetLoggdInUser().Role == "Customer")
            {
                param.Customer = userService.GetLoggdInUser().CustomerId.Value.ToString();
            }
            deliveryList = deliveryRepository.GetDeliveryListBySP(param);

            return deliveryList;
        }

        public Dashboard GetDashboardDeliveryCount(string startDate, string endDate)
        {
            InputParameter param = new InputParameter
            {
                CompanyId = userService.GetLoggdInUser().CompanyId,
                BranchId = userService.GetLoggdInUser().BranchId,
                FromDate = startDate,
                ToDate = endDate
            };
            param.Customer = "-1";
            if (userService.GetLoggdInUser().Role == "Customer")
            {
                param.Customer = userService.GetLoggdInUser().CustomerId.Value.ToString();
            }
            var dashboardData = deliveryRepository.GetDashboardDeliveryCount(param);
            return (dashboardData != null) ? dashboardData : new Dashboard();
        }

    }
}
