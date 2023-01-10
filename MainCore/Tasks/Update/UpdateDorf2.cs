using FluentResults;
using MainCore.Errors;
<<<<<<< HEAD
using MainCore.Helper.Interface;
using Splat;
=======
>>>>>>> master
using System.Threading;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : VillageBotTask
    {
<<<<<<< HEAD
        private readonly INavigateHelper _navigateHelper;

=======
>>>>>>> master
        public UpdateDorf2(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _navigateHelper = Locator.Current.GetService<INavigateHelper>();
        }

        public override Result Execute()
        {
            {
                var result = _navigateHelper.ToDorf2(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var taskUpdate = new UpdateVillage(VillageId, AccountId, CancellationToken);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}