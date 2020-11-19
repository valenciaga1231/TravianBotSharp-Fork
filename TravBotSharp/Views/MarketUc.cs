﻿using System;
using System.Windows.Forms;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;

namespace TravBotSharp.Views
{
    public partial class MarketUc : TbsBaseUc
    {
        public MarketUc()
        {
            InitializeComponent();
        }

        public void UpdateMarketTab()
        {
            var vill = GetSelectedVillage();
            TargetLimitWood.Value = vill.Market.Settings.Configuration.TargetLimit.Wood;
            TargetLimitClay.Value = vill.Market.Settings.Configuration.TargetLimit.Clay;
            TargetLimitIron.Value = vill.Market.Settings.Configuration.TargetLimit.Iron;
            TargetLimitCrop.Value = vill.Market.Settings.Configuration.TargetLimit.Crop;
            FillLimitWood.Value = vill.Market.Settings.Configuration.FillLimit.Wood;
            FillLimitClay.Value = vill.Market.Settings.Configuration.FillLimit.Clay;
            FillLimitIron.Value = vill.Market.Settings.Configuration.FillLimit.Iron;
            FillLimitCrop.Value = vill.Market.Settings.Configuration.FillLimit.Crop;
            transitResEnabled.Checked = vill.Market.Settings.Configuration.Enabled;
            TransitArrival.Text = vill.Market.Settings.Configuration.TransitArrival.ToString();
            LastTransit.Text = vill.Market.Settings.Configuration.LastTransit.ToString();
            //Send res to main vill config
            woodSend.Value = vill.Market.Settings.Configuration.SendResLimit.Wood;
            claySend.Value = vill.Market.Settings.Configuration.SendResLimit.Clay;
            ironSend.Value = vill.Market.Settings.Configuration.SendResLimit.Iron;
            cropSend.Value = vill.Market.Settings.Configuration.SendResLimit.Crop;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            var vill = GetSelectedVillage();
            var limitFill = new Resources();
            var targetLimit = new Resources();
            targetLimit.Wood = (int)TargetLimitWood.Value;
            targetLimit.Clay = (int)TargetLimitClay.Value;
            targetLimit.Iron = (int)TargetLimitIron.Value;
            targetLimit.Crop = (int)TargetLimitCrop.Value;
            limitFill.Wood = (int)FillLimitWood.Value;
            limitFill.Clay = (int)FillLimitClay.Value;
            limitFill.Iron = (int)FillLimitIron.Value;
            limitFill.Crop = (int)FillLimitCrop.Value;
            vill.Market.Settings.Configuration.Enabled = transitResEnabled.Checked;
            vill.Market.Settings.Configuration.FillLimit = limitFill;
            vill.Market.Settings.Configuration.TargetLimit = targetLimit;

            //For npc
            npcEnabled.Checked = vill.Market.Npc.Enabled;
            overflowProtection.Checked = vill.Market.Npc.NpcIfOverflow;
            numericUpDown4.Value = vill.Market.Npc.ResourcesRatio.Wood;
            numericUpDown3.Value = vill.Market.Npc.ResourcesRatio.Clay;
            numericUpDown2.Value = vill.Market.Npc.ResourcesRatio.Iron;
            numericUpDown1.Value = vill.Market.Npc.ResourcesRatio.Crop;
        }

        private void transitResEnabled_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedVillage().Market.Settings.Configuration.Enabled = transitResEnabled.Checked;
        }

        private void overflowProtection_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedVillage().Market.Npc.NpcIfOverflow = overflowProtection.Checked;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedVillage().Market.Npc.ResourcesRatio.Wood = (long)numericUpDown4.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedVillage().Market.Npc.ResourcesRatio.Clay = (long)numericUpDown3.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedVillage().Market.Npc.ResourcesRatio.Iron = (long)numericUpDown2.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedVillage().Market.Npc.ResourcesRatio.Crop = (long)numericUpDown1.Value;
        }

        private void npcEnabled_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedVillage().Market.Npc.Enabled = npcEnabled.Checked;
        }

        // Send to main village configuration
        private void button1_Click(object sender, EventArgs e)
        {
            var vill = GetSelectedVillage();
            Resources limit = new Resources()
            {
                Wood = (int)woodSend.Value,
                Clay = (int)claySend.Value,
                Iron = (int)ironSend.Value,
                Crop = (int)cropSend.Value,
            };
            vill.Market.Settings.Configuration.SendResLimit = limit;
        }
    }
}
