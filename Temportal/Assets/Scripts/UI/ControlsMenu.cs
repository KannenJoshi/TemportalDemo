using System;
using UnityEngine;

namespace UI
{
    public class ControlsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject controlsScreen;

        private void Awake()
        {
            controlsScreen = transform.GetChild(0).gameObject;
        }

        public void OpenMenu()
        {
            controlsScreen.SetActive(true);
        }
        
        public void CloseMenu()
        {
            controlsScreen.SetActive(false);
        }
    }
}