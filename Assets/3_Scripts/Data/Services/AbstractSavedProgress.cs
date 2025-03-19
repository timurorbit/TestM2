using System.IO;
using _3_Scripts.Data.Structure;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    public abstract class AbstractSavedProgress<T> : ISaveSystem<T> where T : ISavedProgress
    {
        private ISaveSystem<T> m_saveSystemImplementation;
        protected abstract string FilePath { get; }

        public void Save(T progress)
        {
            var json = JsonUtility.ToJson(progress, true);
            File.WriteAllText(FilePath, json);
            Debug.Log("Saved progress.");
        }

        public T Load()
        {
            if (!SaveExists())
            {
                Debug.Log("No save file found.");
                return CreateNewSave();
            }

            var json = File.ReadAllText(FilePath);
            Debug.Log(json);
            return JsonUtility.FromJson<T>(json);
        }

        public bool SaveExists()
        {
            return File.Exists(FilePath);
        }

        public T Reset()
        {
            if (!SaveExists())
            {
                File.Delete(FilePath);
            }
            
            var data = CreateNewSave();
            Save(data);
            return data;
        }

        protected abstract T CreateNewSave();
    }
}