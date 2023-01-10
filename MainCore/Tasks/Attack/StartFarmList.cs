using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Update;
using Splat;
using System;
<<<<<<< HEAD
=======
using System.Linq;
>>>>>>> master
using System.Threading;

namespace MainCore.Tasks.Attack
{
    public class StartFarmList : AccountBotTask
    {
        private readonly IClickHelper _clickHelper;

<<<<<<< HEAD
        public StartFarmList(int accountId, int farmId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _farmId = farmId;
            _clickHelper = Locator.Current.GetService<IClickHelper>();
        }

        private readonly int _farmId;
        public int FarmId => _farmId;

        public override string GetName()
=======
        public StartFarmList(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _clickHelper = Locator.Current.GetService<IClickHelper>();
        }

        public override Result Execute()
>>>>>>> master
        {
            if (string.IsNullOrEmpty(_name))
            {
<<<<<<< HEAD
                using var context = _contextFactory.CreateDbContext();
                var farm = context.Farms.Find(FarmId);
                if (farm is not null)
                {
                    _name = $"Start list farm [{farm.Name}]";
                }
                else
                {
                    _name = $"Start list farm [unknow]";
                }
            }
            return _name;
        }

        public override Result Execute()
        {
            {
=======
>>>>>>> master
                var updateTask = new UpdateFarmList(AccountId);
                var result = updateTask.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
<<<<<<< HEAD
                _logManager.Warning(AccountId, $"Farm {FarmId} is missing. Remove this farm from queue");
                return Result.Ok();
            }

            if (IsFarmDeactive())
            {
                _logManager.Warning(AccountId, $"Farm {FarmId} is deactive. Remove this farm from queue");
                return Result.Ok();
            }
            {
                var result = _clickHelper.ClickStartFarm(AccountId, FarmId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
=======
                var result = ClickStartFarm();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
>>>>>>> master

            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.AccountsSettings.Find(AccountId);
                var time = Random.Shared.Next(setting.FarmIntervalMin, setting.FarmIntervalMax);
                ExecuteAt = DateTime.Now.AddSeconds(time);
            }
            return Result.Ok();
        }

<<<<<<< HEAD
        private bool IsFarmExist()
        {
            using var context = _contextFactory.CreateDbContext();
            var farm = context.Farms.Find(FarmId);
            return farm is not null;
        }

        private bool IsFarmDeactive()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.FarmsSettings.Find(FarmId);
            if (!setting.IsActive) return true;
            return false;
=======
        private Result ClickStartFarm()
        {
            using var context = _contextFactory.CreateDbContext();
            var farms = context.Farms.Where(x => x.AccountId == AccountId).ToList();
            foreach (var farm in farms)
            {
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                var isActive = context.FarmsSettings.Find(farm.Id).IsActive;
                if (!isActive) continue;

                var result = _clickHelper.ClickStartFarm(AccountId, farm.Id);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
>>>>>>> master
        }
    }
}