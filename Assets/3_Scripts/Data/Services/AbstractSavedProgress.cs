using System;
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
            try
            {
                var json = JsonUtility.ToJson(progress, true);
                File.WriteAllText(FilePath, json);
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
        }

        public T Load()
        {
            if (!SaveExists())
            {
                return CreateNewSave();
            }

            try
            {
                var json = File.ReadAllText(FilePath);
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

        public T Reset()
        {
            try
            {
                if (!SaveExists())
                {
                    File.Delete(FilePath);
                }

                var data = CreateNewSave();
                Save(data);
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