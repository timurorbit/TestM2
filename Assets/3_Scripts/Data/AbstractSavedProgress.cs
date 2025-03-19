using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace _3_Scripts.Data
{
    public abstract class AbstractSavedProgress<T> : ISaveSystem<T> where T : SavedProgress
    {
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

        public bool Reset()
        {
            {
                if (!SaveExists())
                {
                    return false;
                }
                File.Delete(FilePath);
                return true;
            }
        }

        protected abstract T CreateNewSave();
    }
}