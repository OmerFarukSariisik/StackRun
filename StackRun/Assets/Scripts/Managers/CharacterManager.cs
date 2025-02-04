using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Managers
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem starParticle;
        [SerializeField] private ParticleSystem coinParticle;
        [SerializeField] private ParticleSystem diamondParticle;
        [SerializeField] private float runDuration = 2f;
        [SerializeField] private float danceDuration = 3.2f;
        
        private static readonly int Dance = Animator.StringToHash("Dancing");

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
        
        public async UniTask DanceAsync()
        {
            animator.SetBool(Dance, true);
            animator.speed = 1f;
            virtualCamera.Priority = 11;
            await UniTask.Delay(TimeSpan.FromSeconds(danceDuration));
            animator.SetBool(Dance, false);
            virtualCamera.Priority = 9;
        }

        private void OnTriggerEnter(Collider other)
        {
            ParticleSystem particle;
            switch (other.tag)
            {
                case "Coin":
                    particle = coinParticle;
                    break;
                case "Diamond":
                    particle = diamondParticle;
                    break;
                case "Star":
                    particle = starParticle;
                    break;
                default:
                    return;
            }
            
            particle.transform.position = other.transform.position;
            particle.Play();
            Destroy(other.gameObject);
        }
    }
}
