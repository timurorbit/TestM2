using _3_Scripts.Infrastructure.Logging;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    /// <summary>
    /// A save system implementation for handling <see cref="PlayerData"/> using JSON serialization.
    /// Inherits core save logic from <see cref="JsonSaveSystemBase{T}"/>.
    /// </summary>
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