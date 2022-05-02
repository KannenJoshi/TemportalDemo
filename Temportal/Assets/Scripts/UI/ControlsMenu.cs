using System;
using UnityEngine;

namespace UI
{
    public class ControlsMenu : MonoBehaviour
    {
        [SerializeField] private static GameObject controlsScreen;

        private void Awake()
        {
            controlsScreen = transform.GetChild(0).gameObject;
        }

        public static void OpenMenu()
        {
            controlsScreen.SetActive(true);
        }
        
        public static void CloseMenu()
        {
            controlsScreen.SetActive(false);
        }
    }
}