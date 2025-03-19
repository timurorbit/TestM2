using UnityEngine;

namespace _3_Scripts.Data
{
    public class SaveSettingsSystem : AbstractSavedProgress<PlayerSavedSettings>
    {
        protected override string FilePath => Application.persistentDataPath + "/progress.json";

        protected override PlayerSavedSettings CreateNewSave()
        {
           return new PlayerSavedSettings();
        }
    }
}