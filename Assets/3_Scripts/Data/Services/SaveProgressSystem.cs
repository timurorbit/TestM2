using _3_Scripts.Data.Structure;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    public class SaveProgressSystem : AbstractSavedProgress<PlayerProgress>
    {
        protected override string FilePath => Application.persistentDataPath + "/progress.json";

        protected override PlayerProgress CreateNewSave()
        {
            return new PlayerProgress(0,0);
        }
    }
}