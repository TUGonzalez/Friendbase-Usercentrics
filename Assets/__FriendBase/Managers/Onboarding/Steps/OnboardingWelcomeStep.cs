using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingWelcomeStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.WELCOME;

        public OnboardingWelcomeStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpWelcome.Show("Welcome to Friendbase!");
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
            references.PopUpWelcome.Hide();
            base.Destroy();
        }
    }
}