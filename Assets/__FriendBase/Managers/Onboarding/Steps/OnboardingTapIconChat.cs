using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingTapIconChat : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.TAP_CHAT_ICON;

        public OnboardingTapIconChat(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpTapChatIcon.Show("Here you can chat with them");
            references.PopUpTapChatIcon.ShowArrowRight();
            references.BtnChat.gameObject.SetActive(true);
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.NextStep();
        }

        public override void Destroy()
        {
            references.PopUpTapChatIcon.Hide();
            references.BtnChat.gameObject.SetActive(false);
            base.Destroy();
        }
    }
}