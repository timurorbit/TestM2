using System;
using System.IO;
using System.Threading.Tasks;
using _3_Scripts.Data.Structure;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    public abstract class AbstractSaveSystem<T> : ISaveSystem<T> where T : ISavedProgress
    {
        private ISaveSystem<T> m_saveSystemImplementation;
        protected abstract string FilePath { get; }

        public async Task Save(T progress)
        {
            try
            {
                var json = JsonUtility.ToJson(progress, true);
                await File.WriteAllTextAsync(FilePath, json);
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
        }

        public async Task<T> Load()
        {
            if (!SaveExists())
            {
                return CreateNewSave();
            }

            try
            {
                var json = await File.ReadAllTextAsync(FilePath);
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return CreateNewSave();
            }
        }

        public bool SaveExists()
        {
            return File.Exists(FilePath);
        }

        public async Task<T> Reset()
        {
            try
            {
                if (!SaveExists())
                {
                    File.Delete(FilePath);
                }

                var data = CreateNewSave();
                await Save(data);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return CreateNewSave();
            }
        }

        protected abstract T CreateNewSave();
    }
}