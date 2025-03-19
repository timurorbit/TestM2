using _3_Scripts.Data.Structure;
using UnityEngine;

namespace _3_Scripts.Data.Services
{
    public class SaveSettingsSystem : AbstractSavedProgress<PlayerUISettings>
    {
        protected override string FilePath => Application.persistentDataPath + "/settings.json";

        protected override PlayerUISettings CreateNewSave()
        {
           return new PlayerUISettings(0,false);
        }
    }
}