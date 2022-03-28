using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingMyProfileStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.SHOW_MY_PROFILE;


        public OnboardingMyProfileStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PanelBlackScreen.SetActive(true);
            references.ProfileCardManager.ShowMyProfile();
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.NextStep();
        }

        public override void Destroy()
        {
            references.PanelBlackScreen.SetActive(false);
            references.ProfileCardManager.Hide();
            base.Destroy();
        }
    }
}