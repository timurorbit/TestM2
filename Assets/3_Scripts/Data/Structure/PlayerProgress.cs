using System;

namespace _3_Scripts.Data.Structure
{
    [Serializable]
    public class PlayerProgress : ISavedProgress
    {
        public int CurrentLevel;
        public float PreviousHighScore;

        public PlayerProgress(int currentLevel, float previousHighScore)
        {
            CurrentLevel = currentLevel;
            PreviousHighScore = previousHighScore;
        }
    }
}