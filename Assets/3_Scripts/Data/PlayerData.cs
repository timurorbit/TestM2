using System;
using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data
{
    [Serializable]
    public class PlayerData : ISavedProgress
    {
        public int _currentLevel;
        
        public float _previousHighScore;
        
        public int _currentColorList;
        
        public bool _vibrationEnabled;

        public event Action DataChanged;

        private void InvokeChange() => DataChanged?.Invoke();

        public void ResetToDefaults()
        {
            _currentLevel = 0;
            _previousHighScore = 0;
            _currentColorList = 0;
            _vibrationEnabled = true;
            InvokeChange();
        }

        public bool isValid()
        {
            if (_currentLevel < 0)
            {
                return false;
            }
            if (_previousHighScore is > 100 or < 0)
            {
                return false;
            }

            return true;
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;

                InvokeChange();
            }
        }

        public float PreviousHighScore
        {
            get => _previousHighScore;
            set
            {
                _previousHighScore = value;

                InvokeChange();
            }
        }

        public int CurrentColorList
        {
            get => _currentColorList;
            set
            {
                _currentColorList = value;
                InvokeChange();
            }
        }

        public bool VibrationEnabled
        {
            get => _vibrationEnabled;
            set
            {
                _vibrationEnabled = value;

                InvokeChange();
            }
        }
    }
}