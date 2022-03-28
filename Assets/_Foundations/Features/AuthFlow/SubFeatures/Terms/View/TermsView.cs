using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.Terms.Presentation;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.Terms.View
{
    public class TermsView : ViewNode, ITermsView
    {
        [SerializeField] Button acceptButton;
        [SerializeField] Button selectLanguageButton;
        //[SerializeField] Image languageSelectedIcon;
        [SerializeField] StringWidget languageSelectedLabel;
        [SerializeField] CanvasWebView webView;
        [SerializeField] GameObject webViewContainer;
        //[SerializeField] StringWidget title;
        //[SerializeField] StringWidget content;
        const string termsAndConditionsUrl = "https://friendbase.com/terms-of-use-cookie-policy-app/";
        [SerializeField] LanguageInfo langInfos;
        ISelectLanguageView selectLanguageView;

        [SerializeField] GameObject loadingPanel;

        protected override void OnInit()
        {
            selectLanguageView = GetComponentInChildren<ISelectLanguageView>(true);
            this.CreatePresenter<TermsPresenter, ITermsView>();

            webView.setUrl(termsAndConditionsUrl);
        }

        public IObservable<Unit> OnAccept => acceptButton.OnClickAsObservable();
        public IObservable<Unit> OnShowSelectLanguage => selectLanguageButton.OnClickAsObservable();
        public IObservable<Unit> OnSelectLanguageModalClose => selectLanguageView.ViewClosed;

        public void SetLanguageKey(string langKey)
        {
            //languageSelectedIcon.sprite = langInfos.GetSprite(langKey);
            languageSelectedLabel.Value = langKey.ToUpper();
        }

        public void ShowSelectLanguage()
        {
            webViewContainer.SetActive(false);
            webView.close();
            selectLanguageView.Show();
        }

        public void SetWebView()
        {
            webViewContainer.SetActive(true);
            webView.setUrl(termsAndConditionsUrl);
        }
        public void CloseWebView()
        {
            webViewContainer.SetActive(false);
            webView.close();
        }

        public void SetLoadingPanelActive(bool state)
        {
            loadingPanel.SetActive(state);
            acceptButton.gameObject.SetActive(!state);
        }

        public void SetTermsAndConditions(string termsAndConditions)
        {
            SetLoadingPanelActive(false);
            //content.Value = termsAndConditions;
        }

        public void SetTitle(string text)
        {
            //title.Value = text;
        }
    }
}
