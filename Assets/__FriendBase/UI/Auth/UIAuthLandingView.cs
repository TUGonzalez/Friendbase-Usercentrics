using System;

namespace UI.Auth
{
    public class UIAuthLandingView : AbstractUIPanel
    {
        private AuthFlowManager _authFlowManager;

        private void Awake()
        {
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        public void OnContinueEmail()
        {
            _authFlowManager.SetView(_authFlowManager.UIEnterEmailView);
        }
    }
}
