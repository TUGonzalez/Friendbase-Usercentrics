using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using AuthFlow.AboutYou.Core.Services;
using Firebase.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Auth
{
    public class UIAuthAboutYouView : AbstractUIPanel
    {
        public List<GameObject> stepGameObjects;
        public int stepCurrent = 0;

        public UIComponentInput inputFirstName;
        public UIComponentInput inputLastName;
        public UIComponentInput inputUsername;

        [Header("Birthday")]
        public UIComponentInput inputBirthday;

        public TMP_Dropdown birthdayDay;
        public TMP_Dropdown birthdayMonth;
        public TMP_Dropdown birthdayYear;

        [Header("Gender")]
        public UIComponentInput inputGender;

        public GameObject modalGender;
        public GameObject modalGenderOptionGameObject;
        public GameObject modalGenderOptionsContainer;

        private AuthFlowManager _authFlowManager;
        private List<string> _modalGenderOptions;
        private IAboutYouStateManager _aboutYouState;
        private IAboutYouWebClient _ayWebClient;

        private const string ErrorRequired = "Field is required";

        private void Awake()
        {
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        private async void Start()
        {
            IAboutYouWebClient ayWebClient = Injection.Get<IAboutYouWebClient>();
            _modalGenderOptions = await ayWebClient.GetGendersOptions();
            _aboutYouState = Injection.Get<IAboutYouStateManager>();
            _ayWebClient = Injection.Get<IAboutYouWebClient>();

            // Set first option in Gender dropdown
            inputGender.placeholder = _modalGenderOptions[0];
            inputGender.value = _modalGenderOptions[0];
            inputGender.SetValues();
        }

        public override void OnOpen()
        {
            int currentStep = 0;
            if (_aboutYouState.FirstName != null && _aboutYouState.LastName != null)
                currentStep = 1;
            SetStep(currentStep);
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UIEnterEmailView);
        }

        public void SetLanding()
        {
            _authFlowManager.SetView(_authFlowManager.UILandingView);
        }

        private void SetStep(int newStep)
        {
            foreach (GameObject stepGameObject in stepGameObjects)
            {
                stepGameObject.SetActive(false);
            }

            if (stepGameObjects[newStep] != null)
            {
                stepGameObjects[newStep].SetActive(true);
            }

            stepCurrent = newStep;
        }

        public async void NextStep()
        {
            // Step 1
            if (stepCurrent == 0)
            {
                // Validation: FirstName (required)
                if (!ValidateString(inputFirstName.value))
                {
                    inputFirstName.SetError(ErrorRequired);
                    return;
                }

                // Validation: LastName (required)
                if (!ValidateString(inputLastName.value))
                {
                    inputLastName.SetError(ErrorRequired);
                    return;
                }
            }

            // Step 2
            if (stepCurrent == 1)
            {
                // Validation: Birthday (required)
                if (!ValidateString(inputBirthday.value))
                {
                    inputBirthday.SetError(ErrorRequired);
                    return;
                }

                // TODO: Additional Birthday validation
                // ..

                // Validation: Gender (required)
                if (!ValidateString(inputGender.value))
                {
                    inputGender.SetError(ErrorRequired);
                    return;
                }
            }

            // Step 3
            if (stepCurrent == 2)
            {
                // Validation: Username (required)
                if (!ValidateString(inputUsername.value))
                {
                    inputUsername.SetError(ErrorRequired);
                    return;
                }

                // Validation: Username (valid)
                _authFlowManager.SetLoading(true);
                (bool _, string errorMessage) = await ValidateUsername(inputUsername.value)
                    .ContinueWithOnMainThread(task => task.Result);
                _authFlowManager.SetLoading(false);

                if (errorMessage != null)
                {
                    inputUsername.SetError(errorMessage);
                    return;
                }
            }

            // Advance step
            if (stepCurrent < stepGameObjects.Count - 1)
            {
                SetStep(stepCurrent + 1);
                return;
            }

            // Save data
            _authFlowManager.SetLoading(true);
            _aboutYouState.FirstName = inputFirstName.value.Trim();
            _aboutYouState.LastName = inputLastName.value.Trim();
            _aboutYouState.Gender = inputGender.value.Trim();
            _aboutYouState.UserName = inputUsername.value.Trim();
            _aboutYouState.BirthDate = DateTime.ParseExact(inputBirthday.value, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            await _ayWebClient.UpdateUserData(
                _aboutYouState.FirstName,
                _aboutYouState.LastName,
                _aboutYouState.BirthDate,
                _aboutYouState.Gender,
                _aboutYouState.UserName
            );
            _authFlowManager.SetLoading(false);

            // Try to finish
            _authFlowManager.Finish();
        }

        public void ToggleModalGender()
        {
            modalGender.SetActive(!modalGender.activeSelf);

            if (!modalGender.activeSelf)
            {
                return;
            }

            foreach (Transform child in modalGenderOptionsContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (string option in _modalGenderOptions)
            {
                GameObject optionGameObject =
                    Instantiate(modalGenderOptionGameObject, modalGenderOptionsContainer.transform);

                // Set option text
                optionGameObject.GetComponentInChildren<TMP_Text>().SetText(option);

                // Set option onClick
                optionGameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    string optionSelected = optionGameObject.GetComponentInChildren<TMP_Text>().text;
                    inputGender.placeholder = optionSelected;
                    inputGender.value = optionSelected;
                    inputGender.SetValues();
                    inputGender.SetError(null);
                    inputGender.SetInfo(null);
                    ToggleModalGender();
                });
            }

            List<VerticalLayoutGroup> modalGenderVerticalLayoutGroups =
                modalGender.GetComponentsInChildren<VerticalLayoutGroup>().ToList();
            foreach (VerticalLayoutGroup layout in modalGenderVerticalLayoutGroups.Where(layout => layout != null))
            {
                // Update Vertical Layout Group to fix content position                
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
            }
        }

        public void SetBirthdayDateValue()
        {
            string day = birthdayDay.options[birthdayDay.value].text.PadLeft(2, '0');
            string month = (birthdayMonth.value + 1).ToString().PadLeft(2, '0');
            string year = birthdayYear.options[birthdayYear.value].text;

            inputBirthday.value = $"{month}/{day}/{year}";
            inputBirthday.SetError(null);
            inputBirthday.SetInfo(null);
        }

        private static bool ValidateString(string text)
        {
            text = text.Trim();
            return !string.IsNullOrEmpty(text) && text.Length >= 3;
        }

        private async Task<(bool, string)> ValidateUsername(string username)
        {
            bool usernameIsAvailable = false;
            const int userNameCharLimit = 4;
            const int userNameCharMaxLimit = 12;
            string errorMessage = null;

            if (string.IsNullOrEmpty(username))
                errorMessage = "Please choose a Username.";
            else if (username.Length < userNameCharLimit)
                errorMessage = $"Username must contain at least {userNameCharLimit} characters";
            else if (username.Length > userNameCharMaxLimit)
                errorMessage = $"Username must contain less than {userNameCharMaxLimit} characters";

            if (errorMessage == null)
            {
                try
                {
                    usernameIsAvailable = await _ayWebClient.IsAvailableUserName(username);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }

                if (!usernameIsAvailable)
                    errorMessage = $"the name {username} is already taken";
            }

            return (usernameIsAvailable, errorMessage);
        }
    }
}