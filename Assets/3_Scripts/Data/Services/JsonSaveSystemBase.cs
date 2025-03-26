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
            var json = JsonUtility.ToJson(progress, true);
            await File.WriteAllTextAsync(FilePath, json);
        }

        public async Task<T> Load()
        {
            if (!SaveExists())
            {
                return CreateNewSave();
            }

            var json = await File.ReadAllTextAsync(FilePath);
            return JsonUtility.FromJson<T>(json);
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