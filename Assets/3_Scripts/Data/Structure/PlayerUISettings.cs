using System;

namespace _3_Scripts.Data.Structure
{
    [Serializable]
    public class PlayerUISettings : ISavedProgress
    {
        public int CurrentColorList;
        public bool VibrationEnabled;

        public PlayerUISettings(int currentColorList, bool vibrationEnabled)
        {
            CurrentColorList = currentColorList;
            VibrationEnabled = vibrationEnabled;
        }
    }
}