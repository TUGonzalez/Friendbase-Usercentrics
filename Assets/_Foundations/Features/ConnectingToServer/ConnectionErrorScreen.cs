using System;
using UnityEngine;
using UnityEngine.UI;
using static AuthFlow.JesseUtils;

namespace ConnectingToServer
{
    public class ConnectionErrorScreen : MonoBehaviour
    {
        public Button tryAgainButton;
        public Button exitButton;

        void OnEnable()
        {
            tryAgainButton.onClick.AddListener(() =>
            {
                Clear();
                StartGame();
            });
            if (exitButton)
                exitButton.onClick.AddListener(QuitGame);
        }

        void Clear()
        {
            tryAgainButton.onClick.RemoveAllListeners();
            if (exitButton)
                exitButton.onClick.RemoveAllListeners();
        }

        void OnDisable()
        {
            Clear();
        }
    }
}