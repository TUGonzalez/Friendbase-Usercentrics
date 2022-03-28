using System;
using Architecture.Injector.Core;
using AuthFlow.EndAuth.Repo;
using AuthFlow.Terms.Core.Services;
using AuthFlow.Terms.Presentation;
using AuthFlow.Terms.View;
using Localization.Actions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Auth
{
    public class UIAuthTermsView : AbstractUIPanel
    {
        //public Button acceptButton;
        //public Image languageSelectedIcon;
        public StringWidget languageSelectedLabel;
        //public StringWidget title;
        //public StringWidget content;
        public LanguageInfo langInfos;

        private ISelectLanguageView _selectLanguageView;
        private GetLanguageKey _getLanguageKey;
        private SetLanguageKey _setLanguageKey;
        private ITermsService _termsService;
        private AuthFlowManager _authFlowManager;

        private void Start()
        {
            _getLanguageKey = Injection.Get<GetLanguageKey>();
            _setLanguageKey = Injection.Get<SetLanguageKey>();
            _termsService = Injection.Get<ITermsService>();
            _selectLanguageView = GetComponentInChildren<ISelectLanguageView>(true);
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        public override void OnOpen()
        {
        }

        public void OnAccept()
        {
            ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
            userInfo["terms"] = "True";

            _authFlowManager.Finish();
        }

        public void ShowLanguageSelector()
        {
            _selectLanguageView.Show();
        }

        public void SetLanguage(string langKey)
        {
            _setLanguageKey.Execute(langKey);

            UpdateUI();
            GetTerms();
        }

        private async void GetTerms()
        {

            (string termsTitle, string termsContent) = await _termsService
                .GetTerms(_getLanguageKey.Execute())
                .ObserveOnMainThread()
                .ToTask();

            // TODO: Error state
            // ..
            SetTermsAndConditions(termsContent);
            SetLoadingPanelActive(false);
        }

        private void SetLoadingPanelActive(bool state)
        {
            _authFlowManager.SetLoading(state);
        }

        private void SetTermsAndConditions(string termsAndConditions)
        {
            SetLoadingPanelActive(false);
        }

        private void UpdateUI()
        {
            string langKey = _getLanguageKey.Execute();

            languageSelectedLabel.Value = langKey.ToUpper();
        }
    }
}