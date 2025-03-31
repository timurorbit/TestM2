using _3_Scripts.Infrastructure.Logging;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    public class JsonSavePlayerDataSystem : JsonSaveSystemBase<PlayerData>
    {
        public JsonSavePlayerDataSystem(int saveDebounceDelay, ILogReporter logReporter) : base(saveDebounceDelay, logReporter)
        {
            
        }

        protected override string FilePath => Application.persistentDataPath + "/playerData.json";

        protected override PlayerData CreateNewSave()
        {
            return new ();
        }
    }
}