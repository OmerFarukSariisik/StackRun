using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> notes;
        [SerializeField] private AudioClip breakSound;
        [SerializeField] private AudioSource audioSource;

        public void PlayPerfectNote(int stack)
        {
            var index = Mathf.Min(stack, notes.Count - 1);
            var clip = notes[index];
            audioSource.PlayOneShot(clip);
        }

        public void PlayBreakSound()
        {
            audioSource.PlayOneShot(breakSound);
        }
    }
}
