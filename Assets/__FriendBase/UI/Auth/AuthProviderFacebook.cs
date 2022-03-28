using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using AuthFlow;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.EndAuth.Repo;
using Facebook.Unity;
using Firebase.Auth;
using UnityEngine;

namespace UI.Auth
{
    public class AuthProviderFacebook : MonoBehaviour
    {
        public GameObject uiButtonFacebook;

        private AuthFirebaseManager _authFirebaseManager;
        private AuthFlowManager _authFlowManager;
        private ILocalUserInfo _userInfo;
        private IAboutYouStateManager _aboutYouState;

        public async void Start()
        {
            _userInfo = Injection.Get<ILocalUserInfo>();
            _aboutYouState = Injection.Get<IAboutYouStateManager>();
            _authFirebaseManager = FindObjectsOfType<AuthFirebaseManager>()[0];
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFirebaseManager == null)
            {
                throw new Exception("AuthFirebaseManager not found");
            }

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }

            // Disable Facebook button
            uiButtonFacebook.SetActive(false);

            // Initialize Facebook and toggle the Facebook Login button
            if (!FB.IsInitialized)
            {
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                    {
                        uiButtonFacebook.SetActive(true);
                    }
                });
            }
            else
            {
                // Enable Facebook button 
                uiButtonFacebook.SetActive(true);

                // Enable Facebook features
                // More info: https://developers.facebook.com/docs/unity/reference/current/FB.ActivateApp/
                FB.ActivateApp();
            }
        }

        public void SignInWithFacebook()
        {
            _authFlowManager.SetLoading(true);

            var permissions = new List<string>()
            {
                "email",
                "user_birthday",
                "user_friends",
                "public_profile",
            };

            FB.LogInWithReadPermissions(permissions, result =>
            {
                if (FB.IsLoggedIn)
                {
                    JesseUtils.SignInUserFacebook(SignInWithFacebookCallback);
                }
                else
                {
                    _authFlowManager.SetLoading(false);
                    Debug.Log("User cancelled login");
                }
            });
        }

        private void SignInWithFacebookCallback(FirebaseUser firebaseUser)
        {
            _authFlowManager.SetLoading(false);

            if (firebaseUser != null)
            {
                UpdateUserInfo(result =>
                {
                    // Set the email because some accounts need to be verified
                    _authFirebaseManager.Email = FirebaseAuth.DefaultInstance.CurrentUser.ProviderData.First().Email;

                    // Continue with the AuthFlow
                    _authFlowManager.Finish(); 
                });
            }
        }

        private void UpdateUserInfo(Action<bool> callback)
        {
            // Get first name from Facebook
            FB.API("/me?fields=first_name,last_name", HttpMethod.GET, result =>
            {
                if (result.Error != null)
                {
                    Debug.Log("Error retrieving data");
                    callback(false);
                }
                else
                {
                    _aboutYouState.FirstName = result.ResultDictionary["first_name"] as string;
                    _aboutYouState.LastName = result.ResultDictionary["last_name"] as string;

                    callback(true);
                }
            });
        }
    }
}