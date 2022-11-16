﻿using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Helper;
using MainCore.Tasks.Update;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace MainCore.Tasks.Misc
{
    public class ImproveTroopsTask : VillageBotTask
    {
        private TroopEnums _troop;
        public TroopEnums Troop => _troop;

        public ImproveTroopsTask(int villageId, int accountId) : base(villageId, accountId, $"Improve troops")
        {
        }

        public override void Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
            if (IsUpdateFail()) return;
            if (IsStop()) return;

            _troop = GetTroop();
            if (!IsVaild()) return;

            if (!IsEnoughResource()) return;
            if (IsTroopImproving())
            {
                NextExecute();
                return;
            }

            Upgrade();
            if (IsStop()) return;

            NextExecute();
        }

        private bool IsUpdateFail()
        {
            var taskUpdate = new UpdateTroopLevel(VillageId, AccountId);
            taskUpdate.CopyFrom(this);
            taskUpdate.Execute();
            return taskUpdate.IsFail;
        }

        private TroopEnums GetTroop()
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSettings.Find(VillageId);
            var boolean = settings.GetTroopUpgrade();
            var troops = context.VillagesTroops.Where(x => x.VillageId == VillageId).ToArray();

            var buildings = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
            var smithy = buildings.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);

            for (int i = 0; i < boolean.Length; i++)
            {
                if (!boolean[i]) continue;
                var troop = troops[i];
                if (troop.Level == -1)
                {
                    _logManager.Warning(AccountId, $"{(TroopEnums)troop.Id} is not researched");
                    boolean[troop.Id % 10 - 1] = false;
                    settings.SetTroopUpgrade(boolean);
                    context.Update(settings);
                    continue;
                }
                if (troop.Level == 20)
                {
                    _logManager.Warning(AccountId, $"{(TroopEnums)troop.Id} is max level");
                    boolean[(int)Troop % 10 + 1] = false;
                    settings.SetTroopUpgrade(boolean);
                    context.Update(settings);
                    continue;
                }

                if (smithy.Level == troop.Level)
                {
                    continue;
                }

                context.SaveChanges();
                return (TroopEnums)troop.Id;
            }
            context.SaveChanges();
            return TroopEnums.None;
        }

        private bool IsVaild()
        {
            if (Troop == TroopEnums.None)
            {
                _logManager.Information(AccountId, "There isn't any troop to upgrade.");
                return false;
            }
            return true;
        }

        private bool IsTroopImproving()
        {
            var html = _chromeBrowser.GetHtml();
            var table = html.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
            if (table is null) return false;

            var rows = table.Descendants("tbody").FirstOrDefault().Descendants("tr");

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            if (rows.Count() == 1)
            {
                using var context = _contextFactory.CreateDbContext();
                var accountInfo = context.AccountsInfo.Find(AccountId);
                var hasPlus = accountInfo.HasPlusAccount;
                if (hasPlus)
                {
                    return false;
                }
            }
#elif TTWARS
#else

#error You forgot to define Travian version here

#endif
            var timer = table.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            var time = timer.GetAttributeValue("value", 0);
            if (time < 0) ExecuteAt = DateTime.Now;
            else ExecuteAt = DateTime.Now.AddSeconds(time);
            _logManager.Warning(AccountId, $"Smithy is upgrading another troops");
            return true;
        }

        private bool IsEnoughResource()
        {
            var html = _chromeBrowser.GetHtml();
            var researches = html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));
            foreach (var research in researches)
            {
                if (GetTroop(research) != (int)Troop) continue;
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                var resourceDiv = research.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
#elif TTWARS

                var resourceDiv = research.Descendants("div").FirstOrDefault(x => x.HasClass("showCosts"));
#else

#error You forgot to define Travian version here

#endif
                var resNodes = resourceDiv.ChildNodes.Where(x => x.HasClass("resource") || x.HasClass("resources")).ToList();
                var resNeed = new int[4];
                for (var i = 0; i < 4; i++)
                {
                    var node = resNodes[i];
                    resNeed[i] = node.InnerText.ToNumeric();
                }
                using var context = _contextFactory.CreateDbContext();
                var resCurrent = context.VillagesResources.Find(VillageId);
                if (resNeed[0] > resCurrent.Wood || resNeed[1] > resCurrent.Clay || resNeed[2] > resCurrent.Iron || resNeed[3] > resCurrent.Crop)
                {
                    var resMissing = new long[] { resNeed[0] - resCurrent.Wood, resNeed[1] - resCurrent.Clay, resNeed[2] - resCurrent.Iron, resNeed[3] - resCurrent.Crop };

                    _logManager.Warning(AccountId, $"Don't have enough resource (missing W: {resMissing[0]}, C: {resMissing[1]}, I: {resMissing[2]}, C: {resMissing[3]}");
                    return false;
                }
                return true;
            }
            return true;
        }

        private void Upgrade()
        {
            var html = _chromeBrowser.GetHtml();
            var researches = html.DocumentNode.Descendants("div").Where(x => x.HasClass("research")).ToList();
            foreach (var research in researches)
            {
                if (GetTroop(research) != (int)Troop) continue;
                var upgradeButton = research.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
                if (upgradeButton is null) throw new Exception("Cannot found upgrade button");
                var chrome = _chromeBrowser.GetChrome();
                var upgradeElements = chrome.FindElements(By.XPath(upgradeButton.XPath));
                if (upgradeElements.Count == 0) throw new Exception("Cannot found upgrade button");
                using var context = _contextFactory.CreateDbContext();
                upgradeElements.Click(_chromeBrowser, context, AccountId);
            }
        }

        private void NextExecute()
        {
            var html = _chromeBrowser.GetHtml();
            var table = html.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
            var timer = table.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            var time = timer.GetAttributeValue("value", 0);
            if (time < 0) ExecuteAt = DateTime.Now;
            else ExecuteAt = DateTime.Now.AddSeconds(time);
        }

        private static int GetTroop(HtmlNode node)
        {
            var img = node.Descendants("img").FirstOrDefault(x => x.HasClass("unit"));
            if (img is null) return 0;
            var troopNum = img.GetClasses().FirstOrDefault(x => x != "unit");
            var value = troopNum.ToNumeric();
            return value;
        }
    }
}