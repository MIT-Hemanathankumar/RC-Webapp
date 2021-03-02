using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PDM.Data.EntityMapper;
using PDM.Model;
using PDM.Model.Parameter;
using PDM.Services;
using PDM.Session;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PDM.Web.Controllers
{

    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IWebHostEnvironment hostingEnvironment;

        public TaskController(ITaskService taskService, IWebHostEnvironment hostingEnvironment)
        {
            this.taskService = taskService;
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

        public IActionResult Create(int taskId)
        {
            var modelData = taskService.GetTask(taskId);
            ViewBag.Drivers = taskService.GetBranchUsers();
            ViewBag.TaskStatusList = taskService.GetTaskStatus();
            modelData.CompletionPeriod = 1;
            modelData.ReminderPeriod = 1;
            return View(modelData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskHead modelData, string Command)
        {
            if (modelData != null)
            {
                bool isSaved = taskService.SaveTask(modelData);
            }

            return RedirectToAction("List");
        }

        public IActionResult GetTaskList(string startDate, string endDate, int taskStatusType)
        {
            List<TaskHead> taskList = new List<TaskHead>();
            InputParameter param = new InputParameter
            {
                FromDate = startDate,
                ToDate = endDate,
                TaskStatusType= taskStatusType
            };
            taskList = taskService.GetTaskList(param);
            return PartialView("TaskListPartial", taskList);
        }

    }
}
