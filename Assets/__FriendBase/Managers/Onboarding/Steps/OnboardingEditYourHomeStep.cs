using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingEditYourHomeStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.EDIT_YOUR_HOME;


        public OnboardingEditYourHomeStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpEditYourHome.Show("Here you will edit your home");
            references.PopUpEditYourHome.ShowArrowLeftDown();
            references.BtnFurnitures.gameObject.SetActive(true);
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.NextStep();
        }

        public override void Destroy()
        {
            references.PopUpEditYourHome.Hide();
            references.BtnFurnitures.gameObject.SetActive(false);
            base.Destroy();
        }
    }
}