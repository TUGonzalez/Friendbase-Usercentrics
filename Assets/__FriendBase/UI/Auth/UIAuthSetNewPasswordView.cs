using System;
using AuthFlow.Infrastructure;

namespace UI.Auth
{
    public class UIAuthSetNewPasswordView : AbstractUIPanel
    {
        public UIComponentInput newPasswordInputField;
        public UIComponentInput confirmNewPasswordInputField;

        private PasswordValidator _passwordValidator;
        private AuthFlowManager _authFlowManager;

        private void Awake()
        {
            _passwordValidator = new PasswordValidator();
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        private bool IsValidPassword()
        {
            string passwordInput = newPasswordInputField.value;
            (bool passwordIsValid, string validationError) = _passwordValidator.Validate(passwordInput);

            if (!passwordIsValid)
            {
                newPasswordInputField.SetError(validationError);
                return false;
            }

            return true;
        }

        private bool DoesPasswordMatch()
        {
            string newPassword = newPasswordInputField.value;
            string passwordValidation = confirmNewPasswordInputField.value;

            if (!passwordValidation.Equals(newPassword))
            {
                confirmNewPasswordInputField.SetError("Passwords do not match");
                return false;
            }

            return true;
        }

        public void OnSubmit()
        {
            if (IsValidPassword() && DoesPasswordMatch())
            {
                // Update password
            }
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UILoginView);
        }
    }
}