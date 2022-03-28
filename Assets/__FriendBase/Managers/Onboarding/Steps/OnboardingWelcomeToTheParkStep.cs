using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingWelcomeToTheParkStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.WELCOME_TO_THE_PARK;

        public OnboardingWelcomeToTheParkStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            SetParkBackground();
            references.PopUpThisIsYourHome.Show("Welcome to the Park");
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