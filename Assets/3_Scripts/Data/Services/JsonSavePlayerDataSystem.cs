using UnityEngine;

namespace _3_Scripts.Data.Services
{
    public class JsonSavePlayerDataSystem : JsonSaveSystemBase<PlayerData>
    {
        protected override string FilePath => Application.persistentDataPath + "/playerData.json";

        protected override PlayerData CreateNewSave()
        {
            return new ();
        }
    }
}