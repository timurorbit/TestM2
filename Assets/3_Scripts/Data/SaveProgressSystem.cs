using System.IO;
using UnityEngine;

namespace _3_Scripts.Data
{
    public class SaveProgressSystem : AbstractSavedProgress<PlayerProgress>
    {
        protected override string FilePath => Application.persistentDataPath + "/settings.json";

        protected override PlayerProgress CreateNewSave()
        {
            return new PlayerProgress();
        }
    }
}