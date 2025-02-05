using System;
using System.Collections.Generic;
using Components;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class BlockManager : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private BlockComponent blockComponent;
        [SerializeField] private Transform blockParent;
        [SerializeField] private float blockYPos = -0.55f;
        [SerializeField] private float horizontalSpace;
        [SerializeField] private float xDiffToleration = 0.1f;
        [SerializeField] private float clickTimeout = 3f;
        
        [SerializeField] private List<Material> blockMaterials;
        
        [Inject] private SoundManager _soundManager;
    
        public Action OnLevelComplete;
        public Action OnLevelFailed;
        
        private BlockComponent _previousBlock;
        private Vector3 _defaultScale;
        private Vector3 _finishPosition;
        private float _startZPos;
        private int _totalStackCount;
        private int _stackCounter;
        private int _perfectStackCounter = 0;
        private bool _isRight;
        private bool _isClicked;
        private bool _isClickBlocked = true;

        private void Start()
        {
            inputManager.OnClick += OnClick;
            _defaultScale = blockComponent.transform.localScale;
        }

        public void CreateFirstBlock(bool isInitialLevel)
        {
            _previousBlock = Instantiate(blockComponent, new Vector3(0f, blockYPos, _startZPos), Quaternion.identity,
                blockParent);
            if (!isInitialLevel)
                characterManager.RunToPositionAsync(_previousBlock.transform.position).Forget();
        }

        public void SetTotalStackCount(int totalStackCount)
        {
            _totalStackCount = totalStackCount;
        }

        public void SetStartZPos(float zPos)
        {
            _startZPos = zPos;
        }

        public async UniTaskVoid StartBlockSequenceAsync()
        {
            while (_stackCounter < _totalStackCount)
            {
                var positionZ = _startZPos + _defaultScale.z * (_stackCounter + 1);
                var newBlock = Instantiate(blockComponent,
                    new Vector3(_isRight ? horizontalSpace : -horizontalSpace, blockYPos, positionZ), Quaternion.identity,
                    blockParent);
                newBlock.SetMaterial(blockMaterials[_stackCounter % blockMaterials.Count]);
                newBlock.transform.localScale = _previousBlock.transform.localScale;
                newBlock.StartMoving(_isRight);
                
                _isClickBlocked = false;
                try
                {
                    await UniTask.WaitUntil(() => _isClicked).Timeout(TimeSpan.FromSeconds(clickTimeout))
                        .SuppressCancellationThrow();
                }
                catch (Exception e)
                {
                    //ignore timeout
                }
                
                _isClickBlocked = true;
                newBlock.StopMoving();
                var result = CheckBlock(newBlock);
                if (result.isSuccess)
                    await characterManager.RunToPositionAsync(result.targetPos);
                else
                {
                    OnLevelFailed?.Invoke();
                    return;
                }
                
                _isClicked = false;
                _stackCounter++;
                _isRight = !_isRight;
            }
            
            await characterManager.RunToPositionAsync(_finishPosition);
            await characterManager.DanceAsync();
            OnLevelComplete?.Invoke();
        }
        
        private void OnClick()
        {
            if (_isClickBlocked)
                return;
            _isClicked = true;
        }

        private (Vector3 targetPos, bool isSuccess) CheckBlock(BlockComponent newBlock)
        {
            var previousXPos = _previousBlock.transform.position.x;
            var previousScaleX = _previousBlock.transform.localScale.x;
            var xDiff = newBlock.transform.position.x - previousXPos;
            
            Debug.Log("xDiff: " + xDiff);
            if (Mathf.Abs(xDiff) < xDiffToleration)
            {
                Debug.Log("Tolerated by " + xDiff);
                newBlock.PerfectPlace(previousXPos);
                _soundManager.PlayPerfectNote(_perfectStackCounter);
                _perfectStackCounter++;
                _previousBlock = newBlock;
                return (_previousBlock.transform.position, true);
            }

            if (Mathf.Abs(xDiff) >= previousScaleX)
            {
                newBlock.StartFalling();
                var targetPos = _previousBlock.transform.position;
                targetPos.z += _defaultScale.z;
                return (targetPos, false);
            }

            _perfectStackCounter = 0;
            _soundManager.PlayBreakSound();
            return (BreakBlock(newBlock, xDiff, previousXPos, previousScaleX), true);
        }

        private Vector3 BreakBlock(BlockComponent newBlock,float xDiff, float previousXPos, float previousScaleX)
        {
            var fallingBlockPos = new Vector3(
                previousXPos + (xDiff / 2f) + (xDiff > 0 ? previousScaleX / 2f : -previousScaleX / 2f), blockYPos,
                newBlock.transform.position.z);
            var fallingBlock = Instantiate(blockComponent, fallingBlockPos, Quaternion.identity, blockParent);
            fallingBlock.SetMaterial(blockMaterials[_stackCounter % blockMaterials.Count]);
            fallingBlock.SetXScale(previousScaleX - (previousScaleX - Mathf.Abs(xDiff)));
            fallingBlock.StartFalling();
            fallingBlock.name = "FallingBlock" + _stackCounter;

            var remainingBlockPos =
                new Vector3(previousXPos + (xDiff / 2f), blockYPos, newBlock.transform.position.z);
            var remainingBlock = Instantiate(blockComponent, remainingBlockPos, Quaternion.identity, blockParent);
            remainingBlock.SetMaterial(blockMaterials[_stackCounter % blockMaterials.Count]);
            remainingBlock.SetXScale(previousScaleX - Mathf.Abs(xDiff));
            remainingBlock.name = "RemainingBlock" + _stackCounter;
                
            Destroy(newBlock.gameObject);
            _previousBlock = remainingBlock;
            return remainingBlock.transform.position;
        }

        public void SetFinishPosition(Vector3 targetPos)
        {
            _finishPosition = targetPos;
        }

        public void ResetValues()
        {
            _isRight = false;
            _isClicked = false;
            _stackCounter = 0;
            _perfectStackCounter = 0;
        }

        private void OnDestroy()
        {
            inputManager.OnClick -= OnClick;
        }
    }
}
