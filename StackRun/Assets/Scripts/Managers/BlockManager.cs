using System;
using Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class BlockManager : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private BlockComponent blockComponent;
        [SerializeField] private Transform blockParent;
        [SerializeField] private float blockYPos = -0.55f;
        [SerializeField] private float horizontalSpace;
    
        private BlockComponent _previousBlock;
        private int _totalStackCount;
        private int _stackCounter;
        private bool _isRight;
        private bool _isClicked;

        private void Start()
        {
            inputManager.OnClick += OnClick;
        }

        public void CreateFirstBlock()
        {
            Instantiate(blockComponent, new Vector3(0f, blockYPos, 0f), Quaternion.identity, blockParent);
        }

        public void SetTotalStackCount(int totalStackCount)
        {
            _totalStackCount = totalStackCount;
        }

        public async UniTask StartBlockSequenceAsync()
        {
            while (_stackCounter < _totalStackCount)
            {
                var positionZ = blockComponent.transform.localScale.z * (_stackCounter + 1);
                var newBlock = Instantiate(blockComponent,
                    new Vector3(_isRight ? horizontalSpace : -horizontalSpace, blockYPos, positionZ), Quaternion.identity,
                    blockParent);
                newBlock.StartMoving(_isRight);
                
                await UniTask.WaitUntil(() => _isClicked);
                newBlock.StopMoving();
                
                
                _isClicked = false;
                _stackCounter++;
                _isRight = !_isRight;
            }
        }
        
        private void OnClick()
        {
            _isClicked = true;
        }

        private void OnDestroy()
        {
            inputManager.OnClick -= OnClick;
        }
    }
}
