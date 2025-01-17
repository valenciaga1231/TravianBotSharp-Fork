﻿using DynamicData;
using MainCore.Models.Runtime;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class DebugViewModel : AccountTabBaseViewModel
    {
        private const string discordUrl = "https://discord.gg/DVPV4gesCz";

        public DebugViewModel()
        {
            _eventManager.TaskUpdate += OnTasksUpdate;
            _eventManager.LogUpdate += OnLogsUpdate;

            GetHelpCommand = ReactiveCommand.Create(GetHelpTask);
            LogFolderCommand = ReactiveCommand.Create(LogFolderTask);
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int accountId)
        {
            LoadTask(accountId);
            LoadLogs(accountId);
        }

        private void GetHelpTask()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = discordUrl,
                UseShellExecute = true
            });
        }

        private void LogFolderTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.Accounts.Find(_selectorViewModel.Account.Id);
            var name = info.Username;
            Process.Start(new ProcessStartInfo(Path.Combine(AppContext.BaseDirectory, "logs"))
            {
                UseShellExecute = true
            });
        }

        private void OnTasksUpdate(int accountId)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;
            LoadTask(accountId);
        }

        private void OnLogsUpdate(int accountId, LogMessage logMessage)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Logs.Insert(0, logMessage);
            });
        }

        private void LoadLogs(int accountId)
        {
            var logs = _logManager.GetLog(accountId);

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Logs.Clear();
                Logs.AddRange(logs);
            });
        }

        private void LoadTask(int accountId)
        {
            var tasks = _taskManager.GetList(accountId);
            var listItem = tasks.Select(item => new TaskModel()
            {
                Task = item.GetName(),
                ExecuteAt = item.ExecuteAt,
                Stage = item.Stage,
            }).ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Tasks.Clear();
                Tasks.AddRange(listItem);
            });
        }

        public ObservableCollection<TaskModel> Tasks { get; } = new();

        public ObservableCollection<LogMessage> Logs { get; } = new();
        public ReactiveCommand<Unit, Unit> GetHelpCommand { get; }
        public ReactiveCommand<Unit, Unit> LogFolderCommand { get; }
    }
}