using System;
using System.IO;
using System.Threading.Tasks;
using _3_Scripts.Data.Structure;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    public abstract class JsonSaveSystemBase<T> : ISaveSystem<T> where T : ISavedProgress
    {
        private ISaveSystem<T> m_saveSystemImplementation;
        protected abstract string FilePath { get; }

        public async Task Save(T progress)
        {
            if (progress == null || !progress.isValid())
            {
                Debug.LogError("The provided progress to save is invalid.");
                return;
            }

            try
            {
                var json = JsonUtility.ToJson(progress, true);
                
                var tempFilePath = $"{FilePath}.tmp";
                //todo do debounced logic or semaphore to avoid double save
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
                Debug.LogError(e);
            }
        }

        public async Task<T> Load()
        {
            try
            {
                if (SaveExists())
                {
                    var json = await File.ReadAllTextAsync(FilePath);
                    var saved = JsonUtility.FromJson<T>(json);
                    if (saved != null && saved.isValid())
                    {
                        return saved;
                    }
                    else
                    {
                        //todo add external analytics instead of Debug.LogError;
                        Debug.LogError("Latest save is invalid or null.");
                        return GetBackupSave();
                    }
                }
                else
                {
                    return CreateNewSave();
                }
            }
            catch (Exception e)
            {
                try
                {
                    Debug.LogError("Cant read saveFile.");
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in analytics" + exception);
                }

                return GetBackupSave();
            }
        }

        //TODO implement another logic of backup saving
        private T GetBackupSave()
        {
            return CreateNewSave();
        }

        public bool SaveExists()
        {
            return File.Exists(FilePath);
        }

        public async Task<T> Reset()
        {
            if (!SaveExists())
            {
                File.Delete(FilePath);
            }

            var data = CreateNewSave();
            await Save(data);
            return data;
        }

        protected abstract T CreateNewSave();
    }
}