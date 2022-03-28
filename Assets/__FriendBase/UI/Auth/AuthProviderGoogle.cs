using System;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Architecture.Injector.Core;
using AuthFlow;
using AuthFlow.AppleLogin.Infrastructure;
using AuthFlow.EndAuth.Repo;
using Firebase.Auth;
using UnityEngine;

namespace UI.Auth
{
    public class AuthProviderGoogle : MonoBehaviour
    {
        private AuthFirebaseManager _authFirebaseManager;
        private AuthFlowManager _authFlowManager;
        private ILocalUserInfo _userInfo;

        public void Start()
        {
            _userInfo = Injection.Get<ILocalUserInfo>();
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
        }

        public void SignInWithGoogle()
        {
            // Show the loading overlay
            _authFlowManager.SetLoading(true);
            
           JesseUtils.SignInUserGoogle(SignInWithFirebaseCallback);
        }

        private void SignInWithFirebaseCallback(FirebaseUser firebaseUser)
        {
            _authFlowManager.SetLoading(false);

            if (firebaseUser != null)
                // Continue with the AuthFlow
                _authFlowManager.Finish();
        }
    }
}