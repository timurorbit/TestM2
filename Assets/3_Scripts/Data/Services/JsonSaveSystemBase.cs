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
                logReporter.ReportLog(
                    "Save debounce delay must be between 100 and 600ms. Setting default value is 100.");
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
            catch (IOException e)
            {
                // Maybe Try again instead of GetBackupSave
                _logReporter.ReportError($"Failed to read save file due to I/O error: {e.Message}");
                return GetBackupSave();
            }
            catch (UnauthorizedAccessException e)
            {
                //TODO ask for permissions and retry
                _logReporter.ReportError($"Permission denied accessing save file: {e.Message}");
                return GetBackupSave();
            }
            catch (Exception e)
            {
                // Fallback for unhandled exceptions to review
                _logReporter.ReportError($"Unexpected error loading save: {e.Message}");
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

            // cancelling save operation in progress if exists
            if (_saveDebounceToken != null)
            {
                _saveDebounceToken.Cancel();
                _saveDebounceToken.Dispose();
            }

            // request new SaveOperation
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
            // Catching all kind of exceptions with some behaviour
            catch (DirectoryNotFoundException e)
            {
                _logReporter.ReportError($"Missing directory for replace/move during SaveOperation: {e.Message}");
                // Try to create directory and retry
                try
                {
                    var directory = Path.GetDirectoryName(FilePath);
                    if (directory != null)
                    {
                        Directory.CreateDirectory(directory);
                        //TODO change retry logic to prevent recursion
                        await SaveOperation(progress);
                    }
                    else
                    {
                        _logReporter.ReportError("Provided directory is null");
                    }
                }
                catch (Exception retryEx)
                {
                    _logReporter.ReportError($"Failed to create directory and retry save: {retryEx.Message}");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                //TODO Ask permissions and try again & notify user & return Failed to handle it in manager with different system

                _logReporter.ReportError($"Permission denied saving file: {e.Message}");
            }
            catch (IOException e)
            {
                _logReporter.ReportError($"I/O error during save: {e.Message}");

                // Retries to perform saveOperation
                for (int i = 0; i < 3; i++)
                {
                    await Task.Delay(100);
                    try
                    {
                        //TODO change retry logic to prevent recursion
                        await SaveOperation(progress);
                        return;
                    }
                    catch (Exception retryEx)
                    {
                        _logReporter.ReportLog($"I/O error during save retry operation: {retryEx.Message}");
                    }
                }

                _logReporter.ReportError("Save failed after retries.");
            }
            catch (Exception e)
            {
                // Fallback for truly unanticipated issues, watch in prod logs and review
                _logReporter.ReportError($"Unexpected error during save: {e.Message}");
            }
        }

        // TODO implement another logic of backup saving - e.g. versioning
        private T GetBackupSave()
        {
            return CreateNewSave();
        }

        protected abstract T CreateNewSave();
    }
}