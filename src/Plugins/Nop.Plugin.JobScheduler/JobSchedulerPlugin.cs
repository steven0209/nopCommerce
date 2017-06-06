﻿using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.JobScheduler.Data;
using Nop.Plugin.JobScheduler.Domain;
using Nop.Plugin.JobScheduler.SchedulerJobs;
using Nop.Plugin.JobScheduler.Services;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.JobScheduler
{
    public class JobSchedulerPlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly JobSchedulerObjectContext _objectContext;

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public JobSchedulerPlugin(JobSchedulerObjectContext objectContext,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _objectContext = objectContext;
            _webHelper = webHelper;
            _localizationService = localizationService;

            _workContext = workContext;
        }

        private string FormatEnumResourceName(TimeInterval timeInterval)
        {
            return string.Format("Enums.{0}.{1}", typeof(TimeInterval), timeInterval);
        }

        public override void Install()
        {
            _objectContext.Install();

            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.Menu.Title", "作业调度");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Menu.SchedulerList.Title", "作业列表");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.Page.SchedulerList.Title", "作业调度列表");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.Page.ClearAllScheduler", "清除所有作业");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.Page.BeginRunScheduler", "开始执行");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.SchedulerList.Name", "名称");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.SchedulerList.IntervalValue", "调度值");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.SchedulerList.TimeIntervalDesc", "间隔单位");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.SchedulerList.Enabled", "是否启用");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.SchedulerList.LastRunTime", "上次执行时间");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.SchedulerList.RunJobTime", "开始执行时间");

            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Menu.NewCustomer.Title", "(匿名/注册)用户");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.Page.NewCustomer.Title", "(匿名/注册)用户列表");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.NewCustomer.Username", "用户名");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.NewCustomer.Email", "邮箱");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.NewCustomer.RealName", "真实姓名");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.NewCustomer.IsRegisterCustomer", "是否注册");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.NewCustomer.VisitIpAddress", "IP地址");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.NewCustomer.VisitedTime", "访问/注册时间");

            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.CreatedJob.Done", "新建作业成功");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.UpdatedJob.Done", "修改作业成功");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.DeletedJob.Done", "删除作业成功");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.ClearJob.Done", "清除所有作业成功");
            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.Admin.RunCurrentJob.Done", "已开始执行该作业");

            this.AddOrUpdatePluginLocaleResource("Plugins.JobScheduler.VisitCustomer.Fields.SearchDateTime", "访问日期");

            this.AddOrUpdatePluginLocaleResource(FormatEnumResourceName(TimeInterval.Second), "秒");
            this.AddOrUpdatePluginLocaleResource(FormatEnumResourceName(TimeInterval.Minute), "分钟");
            this.AddOrUpdatePluginLocaleResource(FormatEnumResourceName(TimeInterval.Hour), "小时");
            this.AddOrUpdatePluginLocaleResource(FormatEnumResourceName(TimeInterval.Day), "天");
            this.AddOrUpdatePluginLocaleResource(FormatEnumResourceName(TimeInterval.Month), "月");
            this.AddOrUpdatePluginLocaleResource(FormatEnumResourceName(TimeInterval.Year), "年");

            base.Install();

            var schedulerList = new List<Scheduler>
            {
                new Scheduler
                {
                    Name = "自动备份数据库",
                    SystemName = typeof(BackupDatabaseJob).FullName,
                    IntervalValue = 5,
                    TimeInterval = TimeInterval.Minute,
                    Enabled  = false,
                    Deleted = false
                },
                new Scheduler
                {
                    Name = "统计昨日（匿名/注册）用户信息",
                    SystemName = typeof(ReportYesterdayCustomerJob).FullName,
                    IntervalValue = 1,
                    TimeInterval = TimeInterval.Day,
                    Enabled  = false,
                    Deleted = false
                },
                new  Scheduler
                {
                    Name = "定时清理未启用或已删除的作业",
                    SystemName = typeof(MaintenanceAllJobs).FullName,
                    IntervalValue = 5,
                    TimeInterval = TimeInterval.Day,
                    Enabled  = false,
                    Deleted = false
                }
            };

            var schedulerService = EngineContext.Current.Resolve<ISchedulerService>();
            schedulerService.CreateSchedulerBatch(schedulerList);
        }

        public override void Uninstall()
        {
            _objectContext.UnInstall();

            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.Menu.Title");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Menu.SchedulerList.Title");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.Page.SchedulerList.Title");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.Page.BeginRunScheduler");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.Page.ClearAllScheduler");

            this.DeletePluginLocaleResource("Plugins.JobScheduler.SchedulerList.Name");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.SchedulerList.IntervalValue");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.SchedulerList.TimeIntervalDesc");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.SchedulerList.Enabled");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.SchedulerList.LastRunTime");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.SchedulerList.RunJobTime");

            this.DeletePluginLocaleResource("Plugins.JobScheduler.Menu.NewCustomer.Title");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.Page.NewCustomer.Title");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.NewCustomer.Username");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.NewCustomer.Email");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.NewCustomer.RealName");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.NewCustomer.IsRegisterCustomer");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.NewCustomer.VisitIpAddress");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.NewCustomer.VisitedTime");

            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.CreatedJob.Done");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.UpdatedJob.Done");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.DeletedJob.Done");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.ClearJob.Done");
            this.DeletePluginLocaleResource("Plugins.JobScheduler.Admin.RunCurrentJob.Done");

            this.DeletePluginLocaleResource("Plugins.JobScheduler.VisitCustomer.Fields.SearchDateTime");

            this.DeletePluginLocaleResource(FormatEnumResourceName(TimeInterval.Second));
            this.DeletePluginLocaleResource(FormatEnumResourceName(TimeInterval.Minute));
            this.DeletePluginLocaleResource(FormatEnumResourceName(TimeInterval.Hour));
            this.DeletePluginLocaleResource(FormatEnumResourceName(TimeInterval.Day));
            this.DeletePluginLocaleResource(FormatEnumResourceName(TimeInterval.Month));
            this.DeletePluginLocaleResource(FormatEnumResourceName(TimeInterval.Year));

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            string pluginMenuName = _localizationService.GetResource("Plugin.JobScheduler.Admin.Menu.Title", _workContext.WorkingLanguage.Id, false, "作业调度");

            string listMenuName = _localizationService.GetResource("Plugin.JobScheduler.Menu.SchedulerList.Title", _workContext.WorkingLanguage.Id, false, "作业列表");
            string newCustomerMenuName = _localizationService.GetResource("Plugin.JobScheduler.Menu.NewCustomer.Title", _workContext.WorkingLanguage.Id, false, "(匿名/注册)用户");

            const string adminUrlPart = "Plugins/";

            var pluginMainMenu = new SiteMapNode
            {
                Title = pluginMenuName,
                Visible = true,
                SystemName = "JobScheduler-Main-Menu",
                IconClass = "fa-plug"
            };

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = listMenuName,
                Url = _webHelper.GetStoreLocation(false) + adminUrlPart + "JobSchedulerAdmin/SchedulerList",
                Visible = true,
                SystemName = "JobScheduler-SchedulerList-Menu",
                IconClass = "fa-genderless"
            });

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = newCustomerMenuName,
                Url = _webHelper.GetStoreLocation(false) + adminUrlPart + "JobSchedulerAdmin/NewCustomerList",
                Visible = true,
                SystemName = "JobScheduler-NewCustomer-Menu",
                IconClass = "fa-genderless"
            });

            rootNode.ChildNodes.Add(pluginMainMenu);
        }
    }
}