using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Onboarding
{
    public class OnboardingGenericPopUp : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI txtMessage;
        [SerializeField] protected Image arrowLeftDown;
        [SerializeField] protected Image arrowRightDown;
        [SerializeField] protected Image arrowDown;
        [SerializeField] protected Image arrowRight;

        public void SetText(string text)
        {
            txtMessage.text = text;
        }

        public void Show(string text)
        {
            this.gameObject.SetActive(true);
            txtMessage.text = text;
            HideArrows();

            this.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            this.gameObject.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void HideArrows()
        {
            if (arrowLeftDown!=null) arrowLeftDown.gameObject.SetActive(false);
            if (arrowRightDown != null) arrowRightDown.gameObject.SetActive(false);
            if (arrowDown != null) arrowDown.gameObject.SetActive(false);
            if (arrowRight != null) arrowRight.gameObject.SetActive(false);
        }

        public void ShowArrowLeftDown()
        {
            if (arrowLeftDown != null) arrowLeftDown.gameObject.SetActive(true);
        }

        public void HideArrowLeftDown()
        {
            if (arrowLeftDown != null) arrowLeftDown.gameObject.SetActive(false);
        }

        public void ShowArrowRightDown()
        {
            if (arrowRightDown != null) arrowRightDown.gameObject.SetActive(true);
        }

        public void HideArrowRightDown()
        {
            if (arrowRightDown != null) arrowRightDown.gameObject.SetActive(false);
        }

        public void ShowArrowDown()
        {
            if (arrowDown != null) arrowDown.gameObject.SetActive(true);
        }

        public void HideArrowDown()
        {
            if (arrowDown != null) arrowDown.gameObject.SetActive(false);
        }

        public void ShowArrowRight()
        {
            if (arrowRight != null) arrowRight.gameObject.SetActive(true);
        }

        public void HideArrowRight()
        {
            if (arrowRight != null) arrowRight.gameObject.SetActive(false);
        }
    }
}

