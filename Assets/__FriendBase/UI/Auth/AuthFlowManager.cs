using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.EndAuth.Repo;
using Firebase.Auth;
using MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Auth
{
    public class AuthFlowManager : MonoBehaviour
    {
        public UIAuthLandingView UILandingView;
        public UIAuthEnterEmailView UIEnterEmailView;
        public UIAuthLoginView UILoginView;
        public UIAuthCheckEmailView UICheckEmailView;
        public UIAuthSetNewPasswordView UISetNewPasswordView;
        public UIAuthTermsView UITermsView;
        public UIAuthAboutYouView UIAboutYouView;

        public AbstractUIPanel viewCurrent;
        public AbstractUIPanel viewPrev;

        public CanvasGroup loadingPanel;

        private ILocalUserInfo _userInfo;
        private IAboutYouStateManager _aboutYouState;

        private static AuthFlowManager Instance { get; set; }
        private string _deeplinkURL;

        // Deep linking
        // More info: https://docs.unity3d.com/Manual/enabling-deep-linking.html
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;                
                Application.deepLinkActivated += OnDeepLinkActivated;
                if (!string.IsNullOrEmpty(Application.absoluteURL))
                {
                    // Cold start and Application.absoluteURL not null so process Deep Link.
                    OnDeepLinkActivated(Application.absoluteURL);
                }
                // Initialize DeepLink Manager global variable.
                else _deeplinkURL = "";
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            _userInfo = Injection.Get<ILocalUserInfo>();
            _aboutYouState = Injection.Get<IAboutYouStateManager>();

            CloseAll();
            SetLoading(true);

            // Check if the user is already logged in
            string loginToken = await JesseUtils.IsUserLoggedIn();

            if (string.IsNullOrEmpty(loginToken))
            {
                _userInfo["firebase-login-token"] = "";

                // If the user is not logged in, show the landing view
                UILandingView.Open();
            }
            else
            {
                // If the user is logged in, try go to the main menu
                Finish();
            }

            SetLoading(false);
        }

        private void CloseAll()
        {
            UILandingView.Close();
            UIEnterEmailView.Close();
            UILoginView.Close();
            UICheckEmailView.Close();
            UISetNewPasswordView.Close();
            UITermsView.Close();
            UIAboutYouView.Close();
        }

        public void SetView(AbstractUIPanel viewNext)
        {
            CloseAll();
            viewPrev = viewCurrent;
            viewCurrent = viewNext;
            viewNext.Open();
        }

        // Attempts to finish the authentication process
        public void Finish()
        {
            // The user needs to verify his email address
            if (!FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified)
            {
                CloseAll();
                UICheckEmailView.Open();
                return;
            }

            // The user needs to accept the terms and conditions
            if (_userInfo["terms"] != "True")
            {
                CloseAll();
                UITermsView.Open();
                return;
            }

            // The user must complete information on the About You screen
            if (!_aboutYouState.BirthDate.HasValue
                || !_aboutYouState.FirstName.HasValue
                || !_aboutYouState.LastName.HasValue
                || !_aboutYouState.UserName.HasValue
               )
            {
                CloseAll();
                UIAboutYouView.Open();
                return;
            }

            LoadGame();
        }

        // Loads the game, assuming the user is already logged in
        private static void LoadGame()
        {
            // The user is ready to play
            SceneManager.UnloadSceneAsync("AuthFlow");
            Injection.Get<IViewManager>().Show<MainMenuView>();
        }

        // Show the loading panel
        public void SetLoading(bool isLoading)
        {
            loadingPanel.gameObject.SetActive(isLoading);
            loadingPanel.alpha = isLoading ? 1 : 0;
            loadingPanel.interactable = isLoading;
        }

        private void OnDeepLinkActivated(string url)
        {
            // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
            _deeplinkURL = url;
            string link = url.Split(new string[] { "://" }, StringSplitOptions.None)[1];

            switch (link)
            {
                case "login":
                    CloseAll();
                    UILoginView.Open();
                    break;
            }
        }
    }
}