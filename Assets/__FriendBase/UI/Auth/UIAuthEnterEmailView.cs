using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AuthFlow;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthEnterEmailView : AbstractUIPanel
    {
        public UIComponentInput inputField;
        public TMP_Text errorText;
        public GameObject continueButton;

        public AuthFirebaseManager authFirebaseManager;
        private AuthFlowManager _authFlowManager;

        private void Awake()
        {
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        private async void OnValidEmailSubmit(string inputText)
        {
            // Set the email
            authFirebaseManager.Email = inputText;
      
            // Set the providers associates with the email
            IEnumerable<string> providers = await JesseUtils.EmailProviders(inputText);
            authFirebaseManager.EmailProviders = providers.ToList();

            // Go to next screen
            _authFlowManager.SetView(_authFlowManager.UILoginView);
        }

        public void OnSubmit()
        {
            Regex regexEmail = new Regex(@"^([\w\.\-\+]+)@([\w\-]+)((\.(\w){2,3})+)$");
            string inputText = inputField.value;
        
            if (string.IsNullOrEmpty(inputText)) {
                inputField.SetError("Field is required");
            } else if (regexEmail.Match(inputText).Success) {
                OnValidEmailSubmit(inputText);
            } else {
                inputField.SetError("Enter a valid email");
            }
        }
        
        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UILandingView);
        }
    }
}
