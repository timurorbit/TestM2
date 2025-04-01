using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using _3_Scripts.Data.Structure;
using _3_Scripts.Infrastructure.Logging;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    /// <summary>
    /// An abstract base class for a JSON-based save system that handles saving and loading progress data of type <typeparamref name="T"/>.
    /// Provides debounced saving, retry mechanisms with exponential backoff, and basic error handling for file operations.
    /// Implementers must define the file path and a method to create a new save instance.
    /// </summary>
    /// <typeparam name="T">The type of progress data to save, must implement <see cref="ISavedProgress"/>.</typeparam>
    /// <remarks>
    /// This class uses asynchronous operations for non-blocking I/O and includes logging for debugging and error reporting.
    /// </remarks>
    public abstract class JsonSaveSystemBase<T> : ISaveSystem<T> where T : ISavedProgress
    {
        protected abstract string FilePath { get; }


        private readonly int _saveDebounceDelayMs;
        private readonly ILogReporter _logReporter;
        private readonly int _maxRetries;
        private readonly TimeSpan _baseRetryDelay;

        private CancellationTokenSource _saveDebounceToken;

        /// <summary>
        /// Initializes the save system with configurable debounce delay, logging, and retry settings.
        /// TODO: Add FilePath validation
        /// </summary>
        /// <param name="saveDebounceDelay">Delay in milliseconds to debounce save operations and avoid repeated saves.</param>
        /// <param name="logReporter">Logging implementation for reporting errors and debug information.</param>
        /// <param name="maxRetries">Number of retry attempts for the save operation in case of failure. Default is 3.</param>
        /// <param name="baseRetryDelayMs"> 
        /// Base retry delay in milliseconds. Each retry increases the delay exponentially (baseDelay * 2^retryCount).
        /// Minimum is 50 ms.
        /// </param>
        protected JsonSaveSystemBase(int saveDebounceDelay, ILogReporter logReporter, int maxRetries = 3,
            int baseRetryDelayMs = 100)
        {
            if (saveDebounceDelay is < 100 or > 600)
            {
                logReporter?.ReportLog(
                    "Save debounce delay must be between 100 and 600ms. Setting default value is 100.");
                _saveDebounceDelayMs = 100;
            }
            else
            {
                _saveDebounceDelayMs = saveDebounceDelay;
            }

            // validate maxRetries and baseRetryDelay
            _maxRetries = Math.Max(1, maxRetries); // Ensure at least 1 retry
            _baseRetryDelay = TimeSpan.FromMilliseconds(Math.Max(50, baseRetryDelayMs));
            _logReporter = logReporter;
        }
        
        
        /// <summary>
        /// Loads <see cref="ISavedProgress"/> of type <typeparamref name="T"/> from the specified <c>FilePath</c>.
        /// Checks if the save file exists and validates its content.
        /// Handles <see cref="IOException"/>, <see cref="UnauthorizedAccessException"/>, and general exceptions to cover unexpected scenarios.
        /// TODO: Rework backup loading behavior.
        /// </summary>
        /// /// <returns>A task that resolves to the loaded progress data of type <typeparamref name="T"/>. If loading fails or the save is invalid, returns a backup or new save.</returns>
        public async Task<T> Load()
        {
            try
            {
                if (SaveExists())
                {
                    
                    // reading json and parsing it to data
                    var json = await File.ReadAllTextAsync(FilePath);
                    var saved = JsonUtility.FromJson<T>(json);
                    if (saved == null || !saved.isValid())
                    {
                        _logReporter.ReportError("Latest save is invalid or null.");
                        return GetBackupSave();
                    }

                    _logReporter.ReportLog("Loaded save file.");
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
                // Consider retrying once before falling back to backup.
                _logReporter.ReportError($"Failed to read save file due to I/O error: {e.Message}");
                return GetBackupSave();
            }
            catch (UnauthorizedAccessException e)
            {
                // TODO: Request permissions and retry if possible.
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

        /// <summary>
        /// Saves the provided progress data of type <typeparamref name="T"/> to the specified <see cref="FilePath"/>.
        /// Validates the data before saving and cancels any ongoing save operations using <see cref="_saveDebounceToken"/>.
        /// </summary>
        /// <param name="data">The progress data to save.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public Task Save(T data)
        {
            
            // Checks if data is valid
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

        /// <summary>
        /// Checks if save exists on path.
        /// If any exception happens during process just return false
        /// </summary>
        /// <returns>True if a save file exists at <see cref="FilePath"/>; otherwise, false.</returns>
        public bool SaveExists()
        {
            try
            {
                return File.Exists(FilePath);
            }
            catch (Exception e)
            {
                _logReporter.ReportError($"Exception: {e.Message}");
                return false;
            }
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
        
        /// <summary>
        /// Performs the actual file write operation for saving progress of type <typeparamref name="T"/>.
        /// Saves data atomically by writing it based on <see cref="FilePath"/> to a temporary and then replacing or moving it to the target location.
        /// Handles common file-related exceptions and attempts retries where appropriate.
        /// TODO: Handle exceptions cases properly
        /// </summary>
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
            // Catching all kind of exceptions
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
                        await RetrySave(progress);
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
                //TODO: Ask permissions and try again & notify user & return Failed to handle it in manager with different system

                _logReporter.ReportError($"Permission denied saving file: {e.Message}");
            }
            catch (IOException e)
            {
                _logReporter.ReportError($"I/O error during save: {e.Message}");
                await RetrySave(progress);
            }
            catch (Exception e)
            {
                // Fallback for truly unanticipated issues, watch in prod logs and review
                _logReporter.ReportError($"Unexpected error during save: {e.Message}");
            }
        }

        /// <summary>
        /// Attempts to retry the save operation with exponential backoff in case of failure.
        /// Retries up to <see cref="_maxRetries"/> times before reporting failure.
        /// </summary>
        private async Task RetrySave(T data)
        {
            for (int attempt = 0; attempt < _maxRetries; attempt++)
            {
                try
                {
                    // Apply exponential backoff delay before retrying (e.g., 1s, 2s, 4s...)
                    var delay = TimeSpan.FromTicks(_baseRetryDelay.Ticks * (long)Math.Pow(2, attempt));
                    await Task.Delay(delay);
                    await SaveOperation(data);
                    return;
                }
                catch (Exception ex)
                {
                    _logReporter.ReportLog($"Retry {attempt + 1}/{_maxRetries} failed: {ex.Message}");
                }
            }

            _logReporter.ReportError("Save failed after all retries.");
        }

        // TODO: implement another logic of backup saving - e.g. versioning
        private T GetBackupSave()
        {
            return CreateNewSave();
        }

        protected abstract T CreateNewSave();
    }
}