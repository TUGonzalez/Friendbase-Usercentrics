using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Auth
{
    [ExecuteInEditMode]
    public class UIComponentInput : MonoBehaviour
    {
        public enum InputType
        {
            Text,
            Password,
            Other,
        }

        [Header("Properties")]
        public InputType type;

        public string label;
        public string value;
        public string placeholder;
        public string messageInfo;
        public string messageError;
        public bool interactable = true;

        [Header("References")]
        public TMP_Text labelText;

        public TMP_Text placeholderTextInput;
        public TMP_Text placeholderTextPassword;
        public TMP_InputField inputFieldText;
        public TMP_InputField inputFieldPassword;
        public TMP_Text messageInfoText;
        public TMP_Text messageErrorText;
        public GameObject containerInputText;
        public GameObject containerInputPassword;
        public Image inputTextOutline;
        public Image inputPasswordOutline;
        public List<VerticalLayoutGroup> verticalLayoutGroup;
        private CanvasGroup _canvasGroup;
        private Color regularColor = new Color(0.40625F, 0.1640625F, 0.09375F, 0.9960938F);
        private Color errorColor = new Color(0.8476563F, 0.2109375F, 0.07031025F, 0.9960938F);

        public void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            SetValues();
        }

        public void OnValidate()
        {
            SetValues();
        }

        public void SetValues()
        {
            if (containerInputText != null)
                containerInputText.SetActive(type == InputType.Text);

            if (containerInputPassword != null)
                containerInputPassword.SetActive(type == InputType.Password);

            if (labelText != null)
                labelText.SetText(label);

            if (placeholderTextInput != null)
                placeholderTextInput.SetText(placeholder);

            if (placeholderTextPassword != null)
                placeholderTextPassword.SetText(placeholder);

            if (inputFieldText != null)
                inputFieldText.text = value;

            if (inputFieldPassword != null)
                inputFieldPassword.text = value;

            SetError(messageError);

            SetInfo(messageInfo);

            if (_canvasGroup != null)
                _canvasGroup.interactable = interactable;

            UpdateLayout();
        }

        public void SetError(string message)
        {
            messageError = message;

            if (messageErrorText != null)
            {
                messageErrorText.SetText(messageError);
                messageErrorText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(messageError));
            }

            // Update outline color
            if (string.IsNullOrEmpty(messageError))
            {
                inputTextOutline.color = regularColor;
                inputPasswordOutline.color = regularColor;
            }
            else
            {
                inputTextOutline.color = errorColor;
                inputPasswordOutline.color = errorColor;
            }

            UpdateLayout();
        }

        public void SetInfo(string message)
        {
            messageInfo = message;

            if (messageInfoText != null)
            {
                messageInfoText.SetText(messageInfo);
                messageInfoText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(messageInfo));
            }

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (verticalLayoutGroup == null)
                return;

            foreach (VerticalLayoutGroup layout in verticalLayoutGroup.Where(layout => layout != null))
            {
                // Update Vertical Layout Group to fix content position
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
            }
        }

        public void OnChange(string text)
        {
            value = text;

            // Clear input status messages
            SetError("");
            SetInfo("");
        }
    }
}