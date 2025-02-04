using System;
using System.Collections.Generic;
using Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
        
        [SerializeField] private List<Material> blockMaterials;
    
        private BlockComponent _previousBlock;
        private Vector3 _defaultScale;
        private int _totalStackCount;
        private int _stackCounter;
        private bool _isRight;
        private bool _isClicked;

        private void Start()
        {
            inputManager.OnClick += OnClick;
            _defaultScale = blockComponent.transform.localScale;
        }

        public void CreateFirstBlock()
        {
            _previousBlock = Instantiate(blockComponent, new Vector3(0f, blockYPos, 0f), Quaternion.identity, blockParent);
        }

        public void SetTotalStackCount(int totalStackCount)
        {
            _totalStackCount = totalStackCount;
        }

        public async UniTask StartBlockSequenceAsync()
        {
            while (_stackCounter < _totalStackCount)
            {
                var positionZ = _defaultScale.z * (_stackCounter + 1);
                var newBlock = Instantiate(blockComponent,
                    new Vector3(_isRight ? horizontalSpace : -horizontalSpace, blockYPos, positionZ), Quaternion.identity,
                    blockParent);
                newBlock.SetMaterial(blockMaterials[_stackCounter % blockMaterials.Count]);
                newBlock.transform.localScale = _previousBlock.transform.localScale;
                newBlock.StartMoving(_isRight);
                
                await UniTask.WaitUntil(() => _isClicked);
                newBlock.StopMoving();
                var result = CheckBlock(newBlock);
                await characterManager.RunToPositionAsync(result.targetPos);
                if (!result.isSuccess)
                {
                    return;
                }
                
                _isClicked = false;
                _stackCounter++;
                _isRight = !_isRight;
            }
        }
        
        private void OnClick()
        {
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

        private void OnDestroy()
        {
            inputManager.OnClick -= OnClick;
        }
    }
}
