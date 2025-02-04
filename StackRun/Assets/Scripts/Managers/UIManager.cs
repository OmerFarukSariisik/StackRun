using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button tapToStartButton;
        [SerializeField] private GameObject retryPopup;
        [SerializeField] private Button retryButton;
        
        public Action OnTapToStart;

        private void Start()
        {
            tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
            retryButton.onClick.AddListener(OnRetryButtonClicked);
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

        public void ShowRetryPopup()
        {
            retryPopup.SetActive(true);
        }
        
        private void OnRetryButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            tapToStartButton.onClick.RemoveAllListeners();
            retryButton.onClick.RemoveAllListeners();
        }
    }
}
