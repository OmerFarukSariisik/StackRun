using UnityEngine;

namespace Data
{
    public class LevelProgressSaver
    {
        private const string LevelKey = "CurrentLevel";

        public int GetCurrentLevel()
        {
            return PlayerPrefs.GetInt(LevelKey, 1);
        }

        public void SetLevelCompleted()
        {
            var lastLevel = PlayerPrefs.GetInt(LevelKey, 1);
            PlayerPrefs.SetInt(LevelKey, lastLevel + 1);
        }

        public void ResetLevelData()
        {
            PlayerPrefs.DeleteKey(LevelKey);
        }
    }
}
