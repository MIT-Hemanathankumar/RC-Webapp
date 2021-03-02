using Microsoft.EntityFrameworkCore;
using PDM.Data.Entity.Models;
using PDM.Helper;
using PDM.Model;
using PDM.Model.Parameter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PDM.Repositories
{
    public class TaskRepository : BaseRepository, ITaskRepository
    {

        public TaskRepository(PharmatiseContext pharmatiseContext) : base(pharmatiseContext)
        {

        }

        public ProTask GetTask(long taskId) => dbContext.ProTask.AsQueryable().First(c => c.TaskId == taskId);

        public List<ProTask> GetTasks(long companyId, long branchId) => dbContext.ProTask.Where(w => w.CompanyId == companyId && w.BranchId == branchId).ToList();

        public bool SaveTask(ProTask proTask)
        {
            return dbContext.SaveChanges() > 0;
        }

        public List<ProUser> GetDrivers() => dbContext.ProUser.ToList();
        public List<ProTask> GetDashboardTasks(long companyId, long branchId) => dbContext.ProTask.AsQueryable().Where(w => w.CompanyId == companyId && w.BranchId == branchId).ToList();

        public Dashboard GetDashboardTaskCount(InputParameter param)
        {
            Dashboard dashboardData = null;

            DataSet dsDashboardData = GetData($"exec sp_phar_dashboard_task_count {param.CompanyId},{param.BranchId},'{param.FromDate}','{param.ToDate}'", "Dashboard");
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
