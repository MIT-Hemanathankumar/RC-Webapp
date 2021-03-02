using AutoMapper;
using EmailService;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Bcpg.OpenPgp;


namespace PDM.Services
{
    public sealed class OrderService : BaseService, IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly ICustomerService customerService;
        private readonly ICustomerRepository customerRepository;
        private readonly IDeliveryRepository deliveryRepository;
        private readonly IUserService userService;
        


        public OrderService(IOrderRepository orderRepository, ICustomerService customerService, ICustomerRepository customerRepository,
            IDeliveryRepository deliveryRepository, IUserService userService, IEmailService emailService, ILogger<OrderService> logger, IMasterService _masterService,
            IFCMService _FCMService, IUserRepository _userRepository, IBranchRepository _branchRepositry, IMemoryCache _memoryCache, IMessageTemplateService messageTemplateService) : base(orderRepository)
        {
            this.orderRepository = orderRepository;
            this.customerService = customerService;
            this.customerRepository = customerRepository;
            this.deliveryRepository = deliveryRepository;
            
        }
        private Model.Order MapOrderData(long orderId)
        {
            Model.Order orderData = new Model.Order();
            try
            {
                var proOrder = orderRepository.GetOrder(orderId);
                orderData = MapperConfig.Mapper.Map<Model.Order>(proOrder);
                if (orderData.CustomerId > 0)
                    orderData.Customer = customerService.GetCustomer(orderData.CustomerId);
                if (orderData.BranchId > 0)
                    orderData.BranchName = branchRepositry.GetBranch(orderData.BranchId).BranchName;
                
               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return orderData;
        }

        public List<Model.Order> GetOrders()
        {
            List<Model.Order> orderList = new List<Model.Order>();
            try
            {
                orderRepository.GetOrders().Where(c => c.CompanyId == userService.GetLoggdInUser().CompanyId && c.BranchId == userService.GetLoggdInUser().BranchId && c.OrderStatus != (int)OrderStatus.Completed).OrderByDescending(o => o.OrderStatus).ToList();
                     
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return orderList.OrderByDescending(o => o.OrderId).ToList();
        }

        public Model.Order CreateOrder(long customerId)
        {
            var orderData = new Model.Order();
            try
            {
                if (customerId > 0)
                {
                    
                    orderData.CustomerId = customerId;
                    orderData.BranchNote = orderData.Customer.BranchNote;
                    orderData.DeliveryNote = orderData.Customer.DeliveryNote;
                    orderData.SurgeryNote = orderData.Customer.SurgeryNote;
                    orderData.TempBranchNote = orderData.Customer.BranchNote;
                    orderData.TempDeliveryNote = orderData.Customer.DeliveryNote;
                    
                }
                

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return orderData;
        }
        public bool SaveOrder(Model.Order modelData, string mailContent, string surgeryOrderMailContent, FCMNotificationModel FCMModelData)
        {
            bool bSuccess = false;
            try
            {
                ProOrder orderData;
                if (modelData.OrderId > 0)
                {
                    orderData = orderRepository.GetOrder(modelData.OrderId);
                    modelData.ModifiedOn = DateTime.Now;
                    modelData.ModifiedBy = userService.GetLoggdInUser().UserId;
                }
                
                //modelData.SurgeryNote = modelData.SurgeryNote;
                orderData = MapperConfig.Mapper.Map<Model.Order, ProOrder>(modelData, orderData);
                orderData.Customer = customerRepository.GetCustomer(modelData.CustomerId);
                
                bSuccess = orderRepository.SaveOrder(orderData, modelData);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return bSuccess;
        }

        public Model.OrderDetail GetBlankOrderDetail()
        {
            var orderDetailData = new Model.OrderDetail();
            try
            {
                orderDetailData.Products = GetProducts();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return orderDetailData;
        }

        public Model.Order GetOrderDetail(long orderId, bool buildEmptyRows = true)
        {
            var orderData = new Model.Order();
            try
            {
                if (orderId > 0)
                {
                    orderData = MapOrderData(orderId);
                    if (buildEmptyRows)
                    {
                        if (orderData.OrderDetails != null)
                        {
                            var totalDetailCount = orderData.OrderDetails.Count;
                            for (int i = totalDetailCount; i < 50; i++)
                            {
                                orderData.OrderDetails.Add(new Model.OrderDetail { IsVisible = false });
                            }
                        }
                    }
                    orderData.OldOrderStatus = orderData.OrderStatus;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return orderData;
        }

        public bool UpdateOrder(Model.Order modelData, string mailContent, string surgeryOrderMailContent, FCMNotificationModel FCMModelData)
        {
            bool bSuccess = false;
            try
            {
                
                if (modelData.OrderId > 0)
                {
                    ProOrder orderData = orderRepository.GetOrder(modelData.OrderId);
                    orderData.ModifiedOn = DateTime.Now;
                    orderData.ModifiedBy = userService.GetLoggdInUser().UserId;
                    orderData.OrderTypeId = modelData.OrderTypeId;
                    orderData.IntervalCount = modelData.IntervalCount;
                    orderData.DeliveryDate = modelData.DeliveryDate;
                    orderData.PickupTypeId = modelData.PickupTypeId;
                    orderData.IsStorageFridge = modelData.IsStorageFridge;
                    orderData.IsControlledDrugs = modelData.IsControlledDrugs;
                    orderData.OrderStatus = modelData.OrderStatus;
                    orderData.RackNo = modelData.RackNo;
                    orderData.RouteId = modelData.RouteId;
                    orderData.CaseTypeId = modelData.CaseTypeId;
                    orderData.RecentDeliveryNote = modelData.RecentDeliveryNote;
                    orderData.DeliveryNote = modelData.DeliveryNote;
                    orderData.SurgeryNote = modelData.SurgeryNote;
                    orderData.BranchNote = modelData.BranchNote;
                    ProOrderDetail orderDetail;
                    orderData.ProOrderDetail = new List<ProOrderDetail>();
                    string emailItems = string.Empty;
                    foreach (var item in modelData.OrderDetails.Where(od => od.IsVisible).ToList())
                    {
                        if (!string.IsNullOrEmpty(item.MedicineName) && Converters.ConvertInt(item.Quantity) > 0)
                        {
                            orderDetail = new ProOrderDetail
                            {
                                ProductId = item.ProductId,
                                Strength = item.Strength,
                                Morning = item.Morning,
                                AfterNoon = item.AfterNoon,
                                Evening = item.Evening,
                                Night = item.Night,
                                Quantity = item.Quantity,
                                Duration = item.Duration,
                                Remarks = item.Remarks,
                                OrderDetailId = item.OrderDetailId
                            };
                            orderData.ProOrderDetail.Add(orderDetail);
                            emailItems += $"<tr><td>{item.MedicineName}</td><td>{item.Duration}</td><td>{Converters.ConvertInt(item.Quantity)}</td></tr>";
                        }
                    }
                    bSuccess = orderRepository.SaveOrder(orderData, modelData);
                    if (bSuccess && modelData.OldOrderStatus != modelData.OrderStatus)
                    {
                        string branchName = orderData.BranchId.HasValue ? branchRepositry.GetBranch((long)orderData.BranchId).BranchName : string.Empty;
                        //Check customer has registered mobile  APP and application created with User for that customer
                        var customerUserMapData = userRepository.GetCustomerUserMap(orderData.CustomerId.Value);
                        string orderUpdateSMSMessage = messageTemplateService.GetMessageTemplate(orderData.CompanyId.Value, (long)orderData.BranchId, (int)MessageTypes.SMS, (int)MessageEvents.OrderUpdate);

                        if (customerUserMapData != null)
                        {
                            FCMModelData.UserId = customerUserMapData.UserId;
                            FCMModelData.CompanyId = customerUserMapData.CompanyId;
                            FCMModelData.BranchId = customerUserMapData.BranchId;
                            //FCMModelData.Notification = string.Format(ResourceService.Resource.GetMessage("NMOrderStatusChange"), orderData.OrderId, ResourceService.Resource.GetCaption(((OrderStatus)orderData.OrderStatus).ToString()));
                            FCMModelData.Notification = string.Format(orderUpdateSMSMessage, ResourceService.Resource.GetCaption(((OrderStatus)orderData.OrderStatus).ToString()), orderData.OrderId, modelData.Customer.Mobile, branchName);
                            FCMService.SaveFCMNotificationUser(FCMModelData);
                        }

                        FCMModelData.CompanyId = orderData.CompanyId.Value;
                        FCMModelData.BranchId = orderData.BranchId;
                        //FCMModelData.Notification = string.Format(ResourceService.Resource.GetMessage("NMOrderStatusChangeForDriver"), orderData.OrderId, ResourceService.Resource.GetCaption(((OrderStatus)orderData.OrderStatus).ToString()), modelData.Customer.Mobile);
                        FCMModelData.Notification = string.Format(orderUpdateSMSMessage, ResourceService.Resource.GetCaption(((OrderStatus)orderData.OrderStatus).ToString()), orderData.OrderId, modelData.Customer.Mobile, branchName);
                        FCMService.SaveFCMNotificationDrivers(FCMModelData);

                    }



                    if (bSuccess && modelData.ActionType.HasValue && modelData.ActionType.Value == 1)
                    {
                        var branchId = userService.GetLoggdInUser().BranchId;
                        var customerDtls = customerService.GetCustomer(orderData.CustomerId.Value);
                        var branchDtls = branchRepositry.GetBranch(branchId);
                        var branchInfo = $@"<tr><td class='header-sm'>{branchDtls.BranchName}</td></tr><tr><td>{branchDtls.Address?.Address1}</td></tr><tr><td>{branchDtls.Address?.Address2}</td></tr>
                                                <tr><td>{branchDtls.Address.PostCode}</td></tr><tr><td>{branchDtls.Address.City}</td></tr>";

                        if (!string.IsNullOrEmpty(customerDtls?.Email))
                        {
                            mailContent = mailContent.Replace("$BRANCHDTLS$", branchInfo);
                            mailContent = mailContent.Replace("$CURRENTDATE$", DateTime.Now.ToString("dd MMMM yyyy"));
                            mailContent = mailContent.Replace("$PATIENTDTLS$", string.Concat(customerDtls.FirstName, " ", customerDtls.LastName, "  DOB:", customerDtls.Dob.ToShortDateString(), "  NHS NO:", customerDtls.NHSNumber.ToString()));
                            mailContent = mailContent.Replace("$ITEMLIST$", emailItems);
                            mailContent = mailContent.Replace("$REQUESTEDBY$", string.Concat(customerDtls.FirstName, " ", customerDtls.LastName, "  DOB:", customerDtls.Dob.ToShortDateString(), "  NHS NO:", customerDtls.NHSNumber.ToString()));
                            emailService.SendMail(new string[] { customerDtls.Email }, $"Order Updated [{orderData.OrderId}]", mailContent);
                        }
                        //if (Converters.ConvertInt(modelData.SurgeryId) > 0)
                        //{
                        //    var surgeryData = masterService.GetSurgery(modelData.SurgeryId.Value);
                        //    if (surgeryData != null && !string.IsNullOrEmpty(surgeryData.Email))
                        //    {
                        //        if (!string.IsNullOrEmpty(surgeryOrderMailContent))
                        //        {
                        //            surgeryOrderMailContent = surgeryOrderMailContent.Replace("$ITEMLIST$", emailItems);
                        //            emailService.SendMail(new string[] { surgeryData.Email.Trim() }, $"Order Created [{orderData.OrderId}]", surgeryOrderMailContent);
                        //        }
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return bSuccess;
        }

        public Dashboard GetDashboardOrderCount(int dateType)
        {
            InputParameter param = new InputParameter
            {
                CompanyId = userService.GetLoggdInUser().CompanyId,
                BranchId = userService.GetLoggdInUser().BranchId,
                DateType = dateType
            };
            var dashboardData = orderRepository.GetDashboardOrders(param);
            return (dashboardData != null) ? dashboardData : new Dashboard();
        }

        public List<Model.Order> GetUpcommingOrders()
        {
            List<Model.Order> orderList = new List<Model.Order>();
            try
            {
                int?[] upcommingOrderStatus = { 1, 2 };
                orderRepository.GetOrders().Where(c => c.CompanyId == userService.GetLoggdInUser().CompanyId && c.BranchId == userService.GetLoggdInUser().BranchId && upcommingOrderStatus.Contains(c.OrderStatus)).OrderByDescending(o => o.OrderId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return orderList;
        }

        public List<PDM.API.Model.Order> GetCustomerOrders(long customerId)
        {
            List<PDM.API.Model.Order> orderList = new List<PDM.API.Model.Order>();
            try
            {

                orderRepository.GetCustomerOrders(customerId).OrderByDescending(o => o.OrderStatus).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return orderList;
        }
        

        public bool SaveAPIOrder(Model.Order modelData, string customerOrderMailContent, string surgeryOrderMailContent, FCMNotificationModel FCMModelData)
        {
            bool bSuccess = false;
            try
            {
                ProOrder orderData;
                orderData = new ProOrder();
                orderData.CreatedOn = DateTime.Now;
                orderData.CreatedBy = modelData.CreatedBy;
                orderData.CompanyId = modelData.CompanyId;
                orderData.BranchId = modelData.BranchId;
                orderData.OrderDate = DateTime.Now;
                orderData.OrderTypeId = modelData.OrderTypeId;
                orderData.PickupTypeId = modelData.PickupTypeId;
                orderData.OrderStatus = (int)OrderStatus.Requested;
                orderData.RequestPrescription = modelData.RequestPrescription;
                orderData.Customer = customerRepository.GetCustomer(modelData.CustomerId);
                orderData.DocumentCaption = modelData.DocumentCaption;
                orderData.DocumentPath = modelData.DocumentPath;
                orderData.RecentDeliveryNote = modelData.RecentDeliveryNote;
                orderData.DeliveryNote = modelData.DeliveryNote;
                orderData.BranchNote = modelData.BranchNote;
                orderData.SurgeryNote = modelData.SurgeryNote;
                ProOrderDetail orderDetail;
                orderData.ProOrderDetail = new List<ProOrderDetail>();
                string emailItems = string.Empty;
                
                bSuccess = orderRepository.SaveOrder(orderData, modelData);
                if (bSuccess)
                {
                    if (orderData.CustomerId.HasValue)
                    {
                        var customerDtls = customerService.GetCustomer(orderData.CustomerId.Value);
                        var branchDtls = branchRepositry.GetBranch(modelData.BranchId);
                        var branchInfo = $@"<tr><td class='header-sm'>{branchDtls.BranchName}</td></tr><tr><td>{branchDtls.Address?.Address1}</td></tr><tr><td>{branchDtls.Address?.Address2}</td></tr>
				                        	<tr><td>{branchDtls.Address.PostCode}</td></tr><tr><td>{branchDtls.Address.City}</td></tr>";

                        
                        

                        //Send notification
                        if (FCMModelData != null)
                        {
                            string orderUpdateSMSMessage = messageTemplateService.GetMessageTemplate(orderData.CompanyId.Value, (long)orderData.BranchId, (int)MessageTypes.SMS, (int)MessageEvents.OrderUpdate);

                            string branchName = orderData.BranchId.HasValue ? branchRepositry.GetBranch((long)orderData.BranchId).BranchName : string.Empty;
                            //FCMModelData.Notification = string.Format(ResourceService.Resource.GetMessage("NMOrderCreation"), orderData.OrderId);
                            FCMModelData.Notification = string.Format(orderUpdateSMSMessage, ResourceService.Resource.GetCaption(((OrderStatus)orderData.OrderStatus).ToString()), orderData.OrderId, customerDtls?.Mobile, branchName);
                            FCMService.SaveFCMNotificationUser(FCMModelData);

                            FCMModelData.CompanyId = orderData.CompanyId.Value;
                            FCMModelData.BranchId = orderData.BranchId;
                            //FCMModelData.Notification = string.Format(ResourceService.Resource.GetMessage("NMOrderStatusChangeForDriver"), orderData.OrderId, ResourceService.Resource.GetCaption(((OrderStatus)orderData.OrderStatus).ToString()), customerDtls?.Mobile);
                            FCMModelData.Notification = string.Format(orderUpdateSMSMessage, ResourceService.Resource.GetCaption(((OrderStatus)orderData.OrderStatus).ToString()), orderData.OrderId, customerDtls?.Mobile, branchName);
                            FCMService.SaveFCMNotificationDrivers(FCMModelData);
                        }
                    }
                }
                modelData.OrderId = orderData.OrderId;
            }
            catch (Exception ex)
            {

            }
            return bSuccess;
        }

        public List<Medicine> GetCustomerOrdersMedicine(long customerId)
        {
            List<Medicine> medicineList = new List<Medicine>();
            try
            {
                var customerMedicines = orderRepository.GetCustomerOrdersMedicine(customerId);
                if (customerMedicines != null)
                {
                    return customerMedicines.Select(od =>
                    new Medicine
                    {
                        DeliveryDate = (od.DeliveryDate.HasValue) ? od.DeliveryDate.Value.ToString(ConfigurationKeys.DateFormat) : string.Empty,
                        OrderDate = (od.OrderDate.HasValue) ? od.OrderDate.Value.ToString(ConfigurationKeys.DateFormat) : string.Empty,
                        ProductId = od.ProductId,
                        MedicineName = od.MedicineName.Trim().Replace("'", "").Replace(",", ""),
                        Strength = od.Strength,
                        Morning = od.Morning,
                        AfterNoon = od.AfterNoon,
                        Evening = od.Evening,
                        Night = od.Night,
                        Duration = Converters.ConvertInt(od.Duration),
                        Quantity = Converters.ConvertInt(od.Quantity)
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return medicineList;
        }

        public List<Medicine> GetMasterMedicine(long companyId, long branchId)
        {
            List<Medicine> medicineList = new List<Medicine>();
            try
            {
                medicineList = GetMasterMedicine(companyId, branchId, string.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return medicineList;
        }

        public List<Medicine> GetMasterMedicine(long companyId, long branchId, string medicineName)
        {
            List<Medicine> medicineList = new List<Medicine>();
            try
            {

                bool isExist = memoryCache.TryGetValue("MasterMedicine", out medicineList);
                if (!isExist)
                {
                    var masterMedicines = orderRepository.GetMasterMedicine(companyId, branchId, medicineName);
                    if (masterMedicines != null)
                    {
                        medicineList = masterMedicines.Select(od =>
                        new Medicine
                        {
                            ProductId = od.ProductId,
                            MedicineName = od.MedicineName.Trim().Replace("'", "").Replace(",", ""),
                            Strength = Converters.ConvertString(od.Strength)
                        }).ToList();
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromDays(30));

                    memoryCache.Set("MasterMedicine", medicineList, cacheEntryOptions);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return medicineList;
        }

        public List<Model.Order> GetOrdersInline(long companyId, long branchId)
        => orderRepository.GetOrdersInline(companyId, branchId);

        private DeliveryStatus GetDeliveryStatusForOrderStatus(OrderStatus orderStatus)
        {
            switch (orderStatus)
            {
                case OrderStatus.Requested:
                case OrderStatus.Received:
                    return DeliveryStatus.NotReady;
                case OrderStatus.Ready:
                    return DeliveryStatus.ReadyForDelivery;
                case OrderStatus.OutForDelivery:
                    return DeliveryStatus.OutForDelivery;
                case OrderStatus.Completed:
                    return DeliveryStatus.Completed;
                case OrderStatus.Failed:
                    return DeliveryStatus.Cancelled;
                default:
                    return DeliveryStatus.NotReady;
            }
        }

        public bool IsOrderExist()
        => orderRepository.IsOrderExist(userService.GetLoggdInUser().CompanyId, userService.GetLoggdInUser().BranchId);

        

        public Dashboard GetDashboardOrderCount(string startDate, string endDate)
        {
            InputParameter param = new InputParameter
            {
                CompanyId = userService.GetLoggdInUser().CompanyId,
                BranchId = userService.GetLoggdInUser().BranchId,
                FromDate = startDate,
                ToDate = endDate
            };
            param.Customer = "-1";
            if (userService.GetLoggdInUser().Role == Role.Customer)
            {
                param.Customer = userService.GetLoggdInUser().CustomerId.Value.ToString();
            }
            var dashboardData = orderRepository.GetDashboardOrderCount(param);
            return (dashboardData != null) ? dashboardData : new Dashboard();
        }

    }
}
