using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Managers
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float runDuration = 2f;
        
        private static readonly int Run = Animator.StringToHash("Run");

        private void Start()
        {
            animator.speed = 0f;
        }

        public async UniTask RunToPositionAsync(Vector3 position)
        {
            position.y = 0f;
            animator.speed = 1f;
            await transform.DOMove(position, runDuration);
            animator.speed = 0f;
        }
    }
}
