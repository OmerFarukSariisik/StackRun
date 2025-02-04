using System;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private BlockManager blockManager;
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private GameObject starPrefab;
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private GameObject gemPrefab;
        [SerializeField] private GameObject finishPrefab;
        [SerializeField] private Transform currencyParent;

        [SerializeField] private float currencyYPos = 1f;

        [Inject] private LevelDataLoader _levelDataLoader;
        [Inject] private LevelProgressSaver _levelProgressSaver;

        private float _startZPos = 0f;
        private bool _isInitialLevel = true;

        void Start()
        {
            blockManager.OnLevelComplete += OnLevelComplete;
            _levelDataLoader.OnLevelLoaded += OnLevelLoaded;
            uiManager.OnTapToStart += OnTapToStart;
            StartNextLevel();
        }

        private void StartNextLevel()
        {
            var levelNumber = _levelProgressSaver.GetCurrentLevel();
            _levelDataLoader.LoadLevelData(levelNumber);
        }

        private void OnLevelLoaded(LevelData levelData)
        {
            blockManager.SetStartZPos(_startZPos);
            blockManager.SetTotalStackCount(levelData.blocks.Count);
            var blockZScale = blockPrefab.transform.localScale.z;
            for (var i = 0; i < levelData.blocks.Count; i++)
            {
                GameObject objectToCreate = null;

                switch (levelData.blocks[i])
                {
                    case BlockType.Default:
                        if (i != 0)
                            continue;
                        
                        blockManager.CreateFirstBlock(_isInitialLevel);
                        continue;
                    case BlockType.Star:
                        objectToCreate = starPrefab;
                        break;
                    case BlockType.Coin:
                        objectToCreate = coinPrefab;
                        break;
                    case BlockType.Gem:
                        objectToCreate = gemPrefab;
                        break;
                }

                var position = new Vector3(0, currencyYPos, _startZPos + blockZScale * i);
                Instantiate(objectToCreate, position, Quaternion.identity, currencyParent);
            }

            var finishPos = new Vector3(0, 0,
                _startZPos + (levelData.blocks.Count) * blockZScale + blockZScale / 2f +
                finishPrefab.transform.localScale.z);
            Instantiate(finishPrefab, finishPos, Quaternion.identity);
            blockManager.SetFinishPosition(finishPos);

            _startZPos = finishPos.z + blockZScale / 2f + finishPrefab.transform.localScale.z;
            uiManager.ActivateTapToStart();
        }
        
        
        private void OnTapToStart()
        {
            blockManager.StartBlockSequenceAsync().Forget();
        }
        
        private void OnLevelComplete()
        {
            _isInitialLevel = false;
            _levelProgressSaver.SetLevelCompleted();
            blockManager.ResetValues();
            StartNextLevel();
        }

        private void OnDestroy()
        {
            blockManager.OnLevelComplete -= OnLevelComplete;
            _levelDataLoader.OnLevelLoaded -= OnLevelLoaded;
            uiManager.OnTapToStart -= OnTapToStart;
        }
    }
}