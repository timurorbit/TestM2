using System;

namespace _3_Scripts.Data
{
    [Serializable]
    public class PlayerProgress : SavedProgress
    {
        public int CurrentLevel;
        public float PreviousHighScore;

        public PlayerProgress(int currentLevel, float previousHighScore)
        {
            CurrentLevel = currentLevel;
            PreviousHighScore = previousHighScore;
        }

        public PlayerProgress()
        {
            CurrentLevel = 0;
            PreviousHighScore = 0;
        }
    }
}