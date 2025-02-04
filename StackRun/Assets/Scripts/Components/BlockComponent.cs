using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Components
{
    public class BlockComponent : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float fallSpeed = 2.5f;
            
        private bool _isRight;
        private bool _isMoving;
        private bool _isFalling;
        
        public void StartMoving(bool isRight)
        {
            _isRight = isRight;
            _isMoving = transform;
        }
        
        private void Update()
        {
            if (_isMoving)
            {
                transform.position += (_isRight ? Vector3.left : Vector3.right) * Time.deltaTime * moveSpeed;
            }
            else if (_isFalling)
            {
                transform.position += Vector3.down * Time.deltaTime * fallSpeed;
            }
        }

        public void StopMoving()
        {
            _isMoving = false;
        }

        public void PerfectPlace(float xPos)
        {
            var newPos = new Vector3(xPos, transform.position.y, transform.position.z);
            transform.position = newPos;
        }

        public void StartFalling()
        {
            _isFalling = true;
            Destroy(gameObject, 3);
        }

        public void SetMaterial(Material blockMaterial)
        {
            meshRenderer.sharedMaterial = blockMaterial;
        }

        public void SetXScale(float xScale)
        {
            var scale = transform.localScale;
            scale.x = xScale;
            transform.localScale = scale;
        }
    }
}
