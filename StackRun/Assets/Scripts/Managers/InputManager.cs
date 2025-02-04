using System;
using UnityEngine;

namespace Managers
{
    public class InputManager : MonoBehaviour
    {
        public Action OnClick;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClick?.Invoke();
            }
        }
    }
}
