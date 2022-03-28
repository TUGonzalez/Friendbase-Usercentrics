using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using Architecture.Injector.Core;
using Architecture.MVP;
using BurguerMenu.Core.Domain;
using BurguerMenu.Presentation;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

namespace BurguerMenu.View
{
    public class BurguerView : WidgetBase, IBurguerView
    {
        const string friendbaseFaqUrl = "https://friendbase.com/faq/";
        const string termsAndConditionsUrl = "https://friendbase.com/terms-of-use-cookie-policy-app/";
        const string codeOfConductUrl = "https://friendbase.com/code-of-conduct-app/";
        const string privacyPolicyUrl = "https://friendbase.com/privacy-policy/";

        //WebViewObject webViewObject;

        public CanvasWebView webView;
        public GameObject webViewContainer;

        [SerializeField] Button closeButton;
        [SerializeField] GameObject screenBlocker;
        private Audio.AudioModule AM;


        [Header("Menu Buttons")]
        [Space(10)]
        [SerializeField]
        Button helpButton;

        [SerializeField] Button logOutButton;

        [Header("Help Buttons")]
        [Space(10)]
        [SerializeField]
        Button supportButton;

        [SerializeField] Button termsButton;

        [SerializeField] Button codeOfConduct;

        [SerializeField] Button privacyPolicy;

        [SerializeField] Button closeWebViewButton;
        [SerializeField] GameObject AudioOnGraphic;
        [SerializeField] GameObject AudioOffGraphic;
        [SerializeField] TextMeshProLabelWidget SoundText;



        [Header("Sections")]
        [Space(10)]
        [SerializeField]
        GameObject menuSection;

        [SerializeField] GameObject helpSection;

        [SerializeField] List<GameObject> sections;

        readonly ISubject<Unit> onEnabled = new Subject<Unit>();

        public IObservable<Unit> OnEnabled => onEnabled;
        public IObservable<Unit> OnHelpButton => helpButton.OnClickAsObservable();
        public IObservable<Unit> OnLogOutButton => logOutButton.OnClickAsObservable();
        public IObservable<Unit> OnCloseButton => closeButton.OnClickAsObservable();
        public IObservable<Unit> OnSupportButton => supportButton.OnClickAsObservable();
        public IObservable<Unit> OnTermsButton => termsButton.OnClickAsObservable();
        public IObservable<Unit> OnCodeOfConductButton => codeOfConduct.OnClickAsObservable();
        public IObservable<Unit> OnPrivacyPolicyButton => privacyPolicy.OnClickAsObservable();

        readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            this.CreatePresenter<BurgerPresenter, IBurguerView>();
            AM = FindObjectOfType<Audio.AudioModule>();
            SetSoundGraphics();
        }

        void OnEnable()
        {
            onEnabled.OnNext(Unit.Default);
            closeButton
                .OnClickAsObservable()
                .Subscribe(_ => gameObject.SetActive(false)).AddTo(disposables);
        }

        public void ShowSection(BurgerSection section)
        {
            HideSections();
            closeButton.gameObject.SetActive(true);
            screenBlocker.gameObject.SetActive(true);

            switch (section)
            {
                case BurgerSection.Menu:
                    menuSection.SetActive(true);
                    break;
                case BurgerSection.Help:
                    helpSection.SetActive(true);
                    break;
            }
        }

        public void HideSections()
        {
            closeButton.gameObject.SetActive(false);
            screenBlocker.gameObject.SetActive(false);


            foreach (var section in sections)
            {
                section.SetActive(false);
            }
        }

        public void ToggleSound()
        {
            AM.ToggleSound();
            AudioOnGraphic.SetActive(!AudioOnGraphic.activeSelf);
            AudioOffGraphic.SetActive(!AudioOnGraphic.activeSelf);

            SoundText.Value = AM.IsAudioMuted() ? "Sound Off" : "Sound On";
        }

        private void SetSoundGraphics()
        {
            AudioOnGraphic.SetActive(!AM.IsAudioMuted());
            AudioOffGraphic.SetActive(AM.IsAudioMuted());
            SoundText.Value = AM.IsAudioMuted() ? "Sound Off" : "Sound On";
        }

        public void VisitExternalSectionUrl(HelpButton buttonKey)
        {
            switch (buttonKey)
            {
                case HelpButton.Support:
#if UNITY_EDITOR
                    Application.OpenURL(friendbaseFaqUrl);
#endif
#if UNITY_ANDROID || UNITY_IOS
                    OpenWebView(friendbaseFaqUrl);
#endif
                    break;
                case HelpButton.TermsAndConditions:
#if UNITY_EDITOR
                    Application.OpenURL(termsAndConditionsUrl);
#endif
#if UNITY_ANDROID || UNITY_IOS
                    OpenWebView(termsAndConditionsUrl);
#endif
                    break;
                case HelpButton.CodeOfConduct:
#if UNITY_EDITOR
                    Application.OpenURL(codeOfConductUrl);
#endif
#if UNITY_ANDROID || UNITY_IOS
                    OpenWebView(codeOfConductUrl);
#endif
                    break;
                case HelpButton.PrivacyPolicy:
#if UNITY_EDITOR
                    Application.OpenURL(privacyPolicyUrl);
#endif
#if UNITY_ANDROID || UNITY_IOS
                    OpenWebView(privacyPolicyUrl);
#endif
                    break;
            }
        }

        public void CloseWebView()
        {
            webView.close();
            webViewContainer.SetActive(false);
        }

        public void OpenWebView(string Url)
        {
            webViewContainer.SetActive(true);
            webView.setUrl(Url);
        }
    }
}