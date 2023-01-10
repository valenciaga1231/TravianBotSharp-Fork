using FluentResults;
using MainCore.Enums;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Splat;
using System;
using System.Threading;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace MainCore.Tasks
{
    public abstract class BotTask
    {
        public BotTask(CancellationToken cancellationToken = default)
        {
            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = Locator.Current.GetService<IEventManager>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
            _chromeManager = Locator.Current.GetService<IChromeManager>();
            _logManager = Locator.Current.GetService<ILogManager>();
            CancellationToken = cancellationToken;
        }

        public TaskStage Stage { get; set; }
        public DateTime ExecuteAt { get; set; }

<<<<<<< HEAD
        protected IDbContextFactory<AppDbContext> _contextFactory;
        protected ITaskManager _taskManager;
        protected IEventManager _eventManager;
        protected IChromeManager _chromeManager;
        protected ILogManager _logManager;
=======
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly ITaskManager _taskManager;
        protected readonly IEventManager _eventManager;
        protected readonly IChromeManager _chromeManager;
        protected readonly ILogManager _logManager;
>>>>>>> master
        protected string _name;

        public CancellationToken CancellationToken { get; set; }

        public abstract string GetName();

        public abstract Result Execute();
    }
}