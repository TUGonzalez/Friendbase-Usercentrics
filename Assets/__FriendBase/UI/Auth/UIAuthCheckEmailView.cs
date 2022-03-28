using System;
using System.Collections;
using System.Linq;
using AuthFlow;
using Firebase.Auth;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthCheckEmailView : AbstractUIPanel
    {
        public TMP_Text screenTitle;
        public TMP_Text subtitle1;
        public TMP_Text subtitle2;
        public UIComponentInput emailAddress;
        public CanvasGroup resendCanvasGroup;

        private AuthFirebaseManager _authFirebaseManager;
        private AuthFlowManager _authFlowManager;

        public void Start()
        {
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];
            _authFirebaseManager = FindObjectsOfType<AuthFirebaseManager>()[0];

            if (_authFirebaseManager == null)
            {
                throw new Exception("AuthFirebaseManager not found");
            }
        }

        public override void OnOpen()
        {
            if (_authFirebaseManager.Email == "") {
                // Try to obtain the email address from the provider data
                _authFirebaseManager.Email = FirebaseAuth.DefaultInstance.CurrentUser.ProviderData.First().Email;
            }
            emailAddress.value = _authFirebaseManager.Email;
            emailAddress.SetValues();

            if (_authFirebaseManager.UserHasAccount())
            {
                screenTitle.text = "Check your email";
                subtitle1.text = "We sent you an recovery email to";
                subtitle2.text = "Click on the link in it to reset your password.";
            }
            else
            {
                screenTitle.text = "Verify your email";
                subtitle1.text = "We sent you an email to";
                subtitle2.text = "Click on the link in it to continue creating your account.";
                StartCoroutine(CheckVerifyEmail());
            }
        
            SendEmail();
        }
    
        public override void OnClose()
        {
            StopCoroutine(CheckVerifyEmail());
        }
    
        public void SendEmail()
        {
            // Send email
            if (_authFirebaseManager.UserHasAccount())
            {
                JesseUtils.SendEmailResetPassword(_authFirebaseManager.Email);
            }
            else
            {
                FirebaseAuth.DefaultInstance.CurrentUser.SendEmailVerificationAsync();
            }

            if (resendCanvasGroup != null)
            {
                resendCanvasGroup.alpha = 0;
                resendCanvasGroup.interactable = false;
                StartCoroutine(EnableResendButton());
            }
        }
    
        // Wait for email verification
        private IEnumerator CheckVerifyEmail()
        {
            while (!FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified)
            {
                yield return new WaitForSeconds(1);
                yield return FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
            }

            _authFlowManager.Finish();
        }

        // Wait for 3 seconds before enabling resend button
        private IEnumerator EnableResendButton()
        {
            yield return new WaitForSeconds(3);

            resendCanvasGroup.alpha = 1;
            resendCanvasGroup.interactable = true;
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UILoginView);
        }
    }
}
