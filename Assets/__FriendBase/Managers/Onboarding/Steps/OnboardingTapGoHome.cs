using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingTapGoHome : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.TAP_GO_HOME;

        public OnboardingTapGoHome(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpTapGoHome.Show("Tap here to go back home and start playing!");
            references.PopUpTapGoHome.ShowArrowLeftDown();
            references.BtnHome.gameObject.SetActive(true);
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.NextStep();
        }

        public override void Destroy()
        {
            references.PopUpTapGoHome.Hide();
            references.BtnHome.gameObject.SetActive(false);
            base.Destroy();
        }
    }
}