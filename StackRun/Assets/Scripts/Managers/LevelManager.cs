using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
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

        void Start()
        {
            _levelDataLoader.OnLevelLoaded += OnLevelLoaded;

            var levelNumber = _levelProgressSaver.GetCurrentLevel();
            _levelDataLoader.LoadLevelData(levelNumber);
        }

        private void OnLevelLoaded(LevelData levelData)
        {
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
                        
                        blockManager.CreateFirstBlock();
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

                var position = new Vector3(0, currencyYPos, blockZScale * i);
                Instantiate(objectToCreate, position, Quaternion.identity, currencyParent);
            }

            var finishPos = new Vector3(0, 0, (levelData.blocks.Count + 1) * blockZScale);
            Instantiate(finishPrefab, finishPos, Quaternion.identity);
            
            blockManager.StartBlockSequenceAsync().Forget();
        }
    }
}