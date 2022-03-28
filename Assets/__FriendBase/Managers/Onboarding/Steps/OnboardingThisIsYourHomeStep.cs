using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingThisIsYourHomeStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.THIS_IS_YOUR_HOME;

        public OnboardingThisIsYourHomeStep(IOnboarding onboardingManager, OnboardingAssetsReferences references):base(onboardingManager, references)
        {
            references.PopUpThisIsYourHome.Show("This is your home");
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
            references.PopUpThisIsYourHome.Hide();
            base.Destroy();
        }
    }
}