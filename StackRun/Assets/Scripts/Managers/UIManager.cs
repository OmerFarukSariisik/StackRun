using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button tapToStartButton;
        
        public Action OnTapToStart;

        private void Start()
        {
            tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
        }

        public void ActivateTapToStart()
        {
            tapToStartButton.gameObject.SetActive(true);
        }

        private void OnTapToStartButtonClicked()
        {
            OnTapToStart?.Invoke();
            tapToStartButton.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            tapToStartButton.onClick.RemoveAllListeners();
        }
    }
}
