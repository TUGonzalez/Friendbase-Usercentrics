using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI;
namespace BurguerMenu.View
{
    public class AboutManager : MonoBehaviour
    {
        [SerializeField] TextMeshProLabelWidget versionText;
        [SerializeField] BurguerView burgerView;
        const string creditsUrl = "https://friendbase.com/credits/";

        private void OnEnable()
        {
            versionText.Value = "2013 - 2022 Friendbase AB Version " + Application.version; //Should be getting version from Firebase
        }

        public void OpenCredits()
        {
            burgerView.OpenWebView(creditsUrl);
        }
    }
}