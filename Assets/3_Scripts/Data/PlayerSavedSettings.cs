using System;

namespace _3_Scripts.Data
{
    [Serializable]
    public class PlayerSavedSettings : SavedProgress
    {
        public int CurrentColorList;
        public bool VibrationEnabled;

        public PlayerSavedSettings(int currentColorList, bool vibrationEnabled)
        {
            CurrentColorList = currentColorList;
            VibrationEnabled = vibrationEnabled;
        }

        public PlayerSavedSettings()
        {
            CurrentColorList = 0;
            VibrationEnabled = true;
        }
    }
}