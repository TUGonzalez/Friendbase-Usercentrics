using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingShowRoomPopUp : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.ROOM_POP_UP;

        public OnboardingShowRoomPopUp(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.OnboardingRoomsList.SetActive(true);
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.NextStep();
        }

        public override void Destroy()
        {
            references.OnboardingRoomsList.SetActive(false);
            base.Destroy();
        }
    }
}