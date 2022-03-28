using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingThisIsYouStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.THIS_IS_YOU;

        public OnboardingThisIsYouStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            Vector2 canvasPosition = RectTransformUtility.WorldToScreenPoint(references.Camera, references.AvatarController.transform.position) / references.MyCanvas.scaleFactor;

            canvasPosition.x -= references.PopUpThisIsYou.GetComponent<RectTransform>().sizeDelta.x * 0.8f;
            canvasPosition.y += references.PopUpThisIsYou.GetComponent<RectTransform>().sizeDelta.y * 2.2f;

            references.PopUpThisIsYou.GetComponent<RectTransform>().anchoredPosition = canvasPosition;

            references.PopUpThisIsYou.Show("And this is you");
            references.PopUpThisIsYou.ShowArrowRightDown();
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.NextStep();
        }

        protected override void OnLimitTimePassed()
        {
            onboardingManager.NextStep();
        }

        public override void Destroy()
        {
            references.PopUpThisIsYou.Hide();
            base.Destroy();
        }
    }
}