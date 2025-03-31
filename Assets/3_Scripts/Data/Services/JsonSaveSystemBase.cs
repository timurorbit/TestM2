using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using _3_Scripts.Data.Structure;
using _3_Scripts.Infrastructure.Logging;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    public abstract class JsonSaveSystemBase<T> : ISaveSystem<T> where T : ISavedProgress
    {
        protected abstract string FilePath { get; }


        private readonly int _saveDebounceDelayMs;
        private readonly ILogReporter _logReporter;

        private CancellationTokenSource _saveDebounceToken;

        protected JsonSaveSystemBase(int saveDebounceDelay, ILogReporter logReporter)
        {
            if (saveDebounceDelay is < 100 or > 600)
            {
                logReporter.ReportLog("Save debounce delay must be between 100 and 600ms. Setting default value is 100.");
                _saveDebounceDelayMs = 100;
            }
            else
            {
                _saveDebounceDelayMs = saveDebounceDelay; 
            }
            _logReporter = logReporter;
        }

        public async Task<T> Load()
        {
            try
            {
                if (SaveExists())
                {
                    var json = await File.ReadAllTextAsync(FilePath);
                    var saved = JsonUtility.FromJson<T>(json);
                    if (saved == null || !saved.isValid())
                    {
                        _logReporter.ReportError("Latest save is invalid or null.");
                        return GetBackupSave();
                    }
                    return saved;
                }
                else
                {
                    _logReporter.ReportLog("Save is not exists, creating new save ");
                    return CreateNewSave();
                }
            }
            catch (Exception e)
            {
                _logReporter.ReportError("Cant read saveFile. \n" + e.Message);
                return GetBackupSave();
            }
        }

        public Task Save(T data)
        {
            if (data == null || !data.isValid())
            {
                _logReporter.ReportError("The provided progress to save is invalid.");
                return Task.CompletedTask;
            }
            _saveDebounceToken?.Cancel();
            _saveDebounceToken = new CancellationTokenSource();
            var token = _saveDebounceToken.Token;

            return DebouncedSave(data, token);
        }

        public bool SaveExists()
        {
            return File.Exists(FilePath);
        }

        private async Task DebouncedSave(T data, CancellationToken token)
        {
            try
            {
                await Task.Delay(_saveDebounceDelayMs, token);
                await SaveOperation(data);
            }
            catch (OperationCanceledException)
            {
                _logReporter.ReportLog("Debounced save operation cancelled.");
            }
        }

        private async Task SaveOperation(T progress)
        {
            try
            {
                var json = JsonUtility.ToJson(progress, true);
                var tempFilePath = $"{FilePath}.tmp";
                await File.WriteAllTextAsync(tempFilePath, json);
                if (SaveExists())
                {
                    File.Replace(tempFilePath, FilePath, null);
                }
                else
                {
                    File.Move(tempFilePath, FilePath);
                }
            }
            catch (Exception e)
            {
                _logReporter.ReportError(e.Message);
            }
        }

        // TODO implement another logic of backup saving

        // versioning

        private T GetBackupSave()
        {
            return CreateNewSave();
        }

        protected abstract T CreateNewSave();
    }
}