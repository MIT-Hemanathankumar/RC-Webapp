using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Esf;
using PDM.Data.Entity;
using PDM.Data.Entity.Models;
using PDM.Data.EntityMapper;
using PDM.Helper;
using PDM.Model;
using PDM.Model.Parameter;
using PDM.Repositories;
using PDM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDM.Services
{
    public sealed class TaskService : BaseService, ITaskService
    {
        private readonly ITaskRepository taskRepositry;
        private readonly IUserService userService;
        private readonly ILogger<BranchService> logger;
        private readonly IOrderService orderService;
        private readonly ICustomerService customerService;
        private readonly UserManager<IdentityUser> userManager;
        public TaskService(ITaskRepository taskRepositry, IUserService userService, IOrderService orderService, ICustomerService _customerService, ILogger<BranchService> logger, UserManager<IdentityUser> userManager) : base(taskRepositry)
        {
            this.taskRepositry = taskRepositry;
            this.userService = userService;
            this.orderService = orderService;
            this.customerService = _customerService;
            this.logger = logger;
            this.userManager = userManager;
        }

        public List<TaskHead> GetTasks()
        {
            List<TaskHead> taskList = new List<TaskHead>();
            try
            {
                taskRepositry.GetTasks(userService.GetLoggdInUser().CompanyId, userService.GetLoggdInUser().BranchId).Where(ts => ts.TaskStatus != (int)TodoStatus.Completed).ToList().ForEach(f =>
                        {
                            var taskData = MapperConfig.Mapper.Map<TaskHead>(f);
                            if (taskData.TaskStatus > 0)
                                taskData.TaskStatusDesc = ResourceService.Resource.GetCaption(((TodoStatus)taskData.TaskStatus).ToString());
                            if (taskData.AssignedTo != null)
                            {
                                var userData = userService.GetUser(taskData.AssignedTo.Value);
                                if (userData != null)
                                    taskData.AssignedToDesc = $"{userData.FirstName} {userData.LastName}";
                            }
                            taskList.Add(taskData);

                        });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return taskList;
        }

        public TaskHead GetTask(long taskId)
        {
            var taskData = new TaskHead();
            try
            {
                if (taskId > 0)
                {
                    var proTask = taskRepositry.GetTask(taskId);
                    taskData = MapperConfig.Mapper.Map<TaskHead>(proTask);
                                        
                    taskData.CompanyId = userService.GetLoggdInUser().CompanyId;
                    taskData.BranchId = userService.GetLoggdInUser().BranchId;

                    
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return taskData;
        }
        public bool SaveTask(TaskHead modelData)
        {
            bool bSuccess = false;
            try
            {
                ProTask taskData;
                if (modelData.TaskId > 0)
                {
                    taskData = taskRepositry.GetTask(modelData.TaskId);
                    modelData.ModifiedOn = DateTime.Now;
                    modelData.ModifiedBy = userService.GetLoggdInUser().UserId;
                }
                else
                {
                    taskData = new ProTask();
                    modelData.CreatedOn = DateTime.Now;
                    modelData.CreatedBy = userService.GetLoggdInUser().UserId;
                }
                modelData.RepeatTask = (modelData.IsRepeatTask) ? 1 : 0;
                taskData = MapperConfig.Mapper.Map<TaskHead, ProTask>(modelData, taskData);
                taskData.CompanyId = userService.GetLoggdInUser().CompanyId;
                taskData.BranchId = userService.GetLoggdInUser().BranchId;
                bSuccess = taskRepositry.SaveTask(taskData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return bSuccess;
        }

        public List<SelectListItem> GetBranchUsers()
        {
            List<SelectListItem> userList = new List<SelectListItem>();
            userService.GetBranchUser(userService.GetLoggdInUser().BranchId);
            return userList;
        }

        public List<SelectListItem> GetOrders()
        {
            List<SelectListItem> selectOrderList = new List<SelectListItem>();
            List<Order> orderList = orderService.GetOrdersInline(userService.GetLoggdInUser().CompanyId, userService.GetLoggdInUser().BranchId);
            
            return selectOrderList;
        }

        public List<TaskHead> GetDashboardTasks()
        {
            List<TaskHead> taskList = new List<TaskHead>();
            try
            {
                taskRepositry.GetDashboardTasks(userService.GetLoggdInUser().CompanyId, userService.GetLoggdInUser().BranchId).OrderByDescending(t => t.TaskId).Take(3).ToList().ForEach(f =>
                  {
                      var taskData = MapperConfig.Mapper.Map<TaskHead>(f);
                      if (taskData.TaskStatus > 0)
                          taskData.TaskStatusDesc = ResourceService.Resource.GetCaption(((TodoStatus)taskData.TaskStatus).ToString());
                      
                      string orderInfo = string.Empty;
                      
                      taskList.Add(taskData);

                  });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return taskList;
        }

        public Dashboard GetDashboardTaskCount(int dateType)
        {
            InputParameter param = new InputParameter
            {
                CompanyId = userService.GetLoggdInUser().CompanyId,
                BranchId = userService.GetLoggdInUser().BranchId,
                DateType = dateType
            };
            var dashboardData = taskRepositry.GetDashboardTaskCount(param);
            return (dashboardData != null) ? dashboardData : new Dashboard();
        }

        public Dashboard GetDashboardTaskCount(string startDate, string endDate)
        {
            InputParameter param = new InputParameter
            {
                CompanyId = userService.GetLoggdInUser().CompanyId,
                BranchId = userService.GetLoggdInUser().BranchId,
                FromDate = startDate,
                ToDate = endDate
            };
            var dashboardData = taskRepositry.GetDashboardTaskCount(param);
            return (dashboardData != null) ? dashboardData : new Dashboard();
        }

        public List<TaskHead> GetTaskList(InputParameter param)
        {
            List<TaskHead> taskList = new List<TaskHead>();
            param.CompanyId = userService.GetLoggdInUser().CompanyId;
            param.BranchId = userService.GetLoggdInUser().BranchId;
            List<ProTask> proTaskList = new List<ProTask>();
            
                proTaskList = taskRepositry.GetTasks(userService.GetLoggdInUser().CompanyId, userService.GetLoggdInUser().BranchId)
                    .Where(ts =>  ts.DateOfStart.Value.Date < DateTime.Now.Date).ToList();
            }

            
            return taskList;
        }

        public bool IsTaskExist()
        => taskRepositry.GetTasks(userService.GetLoggdInUser().CompanyId, userService.GetLoggdInUser().BranchId).Any(t => t.TaskStatus != (int)TodoStatus.Cancelled);

    }
}
