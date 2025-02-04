using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Data
{
    public class LevelDataLoader
    {
        [Inject] private LevelProgressSaver _levelProgressSaver;
        
        public event Action<LevelData> OnLevelLoaded;

        private readonly string _path = "Assets/Addressables/Level";

        public void LoadLevelData(int levelNumber)
        {
            var assetPath = $"{_path}{levelNumber}.asset";
            Addressables.LoadAssetAsync<LevelData>(assetPath).Completed += LevelLoaded;
        }

        private void LevelLoaded(AsyncOperationHandle<LevelData> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log(handle.Result);
                OnLevelLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError("Failed to load level data, going to level 1");
                _levelProgressSaver.ResetLevelData();
                LoadLevelData(1);
            }
        }
    }
}
