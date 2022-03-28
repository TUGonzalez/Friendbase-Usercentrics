using System;
using AuthFlow.Infrastructure;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthLoginView : AbstractUIPanel
    {
        public TMP_Text screenTitle;
        public UIComponentInput emailAddress;
        public UIComponentInput passwordInputField;
        public GameObject continueButton;
        public GameObject forgotPassButton;
        public AuthFirebaseManager authFirebaseManager;

        private PasswordValidator _passwordValidator;
        private AuthFlowManager _authFlowManager;

        const string SignInTitle = "Log in to your account";
        const string SignUpTitle = "Create an account";

        private void Awake()
        {
            _passwordValidator = new PasswordValidator();
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        public override void OnOpen()
        {
            
            screenTitle.text = SignInTitle;
            emailAddress.value = authFirebaseManager.Email;
            emailAddress.SetValues();

            // If user has no account with that email, show sign up screen
            if (authFirebaseManager.EmailProviders.Count == 0)
            {
                forgotPassButton.SetActive(false);
                screenTitle.text = SignUpTitle;
            }
            else
            {
                forgotPassButton.SetActive(true);
            }

        }

        public void OnForgotPassword()
        {
            _authFlowManager.SetView(_authFlowManager.UICheckEmailView);
        }

        public async void OnSubmit()
        {
            string inputText = passwordInputField.value;
            (bool passwordIsValid, string validationError) = _passwordValidator.Validate(inputText);

            if (!passwordIsValid)
            {
                passwordInputField.SetError(validationError);
                return;
            }

            _authFlowManager.SetLoading(true);

            // Login
            if (authFirebaseManager.UserHasAccount())
            {
                (bool userAuthenticated, string loginError) = await authFirebaseManager.SignInWithEmail(inputText);

                _authFlowManager.SetLoading(false);

                if (userAuthenticated)
                {
                    _authFlowManager.Finish();
                    return;
                }
                else
                {
                    passwordInputField.SetError(loginError);
                    return;
                }
            }

            // Create email account
            if (!authFirebaseManager.UserHasAccount())
            {
                bool userCreated = await authFirebaseManager.SignUpWithEmail(authFirebaseManager.Email, inputText);

                _authFlowManager.SetLoading(false);
                if (userCreated)
                {
                    _authFlowManager.Finish();
                }
            }
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UIEnterEmailView);
        }
    }
}