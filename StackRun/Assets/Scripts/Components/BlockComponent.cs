using System;
using UnityEngine;

namespace Components
{
    public class BlockComponent : MonoBehaviour
    {
        [SerializeField] private float speed;
            
        private bool _isRight;
        private bool _isMoving;
        
        public void StartMoving(bool isRight)
        {
            _isRight = isRight;
            _isMoving = transform;
        }
        
        private void Update()
        {
            if (!_isMoving)
                return;

            transform.position += (_isRight ? Vector3.left : Vector3.right) * Time.deltaTime * speed;
        }

        public void StopMoving()
        {
            _isMoving = false;
        }
    }
}
