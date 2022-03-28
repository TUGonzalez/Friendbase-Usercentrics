using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingEditGoToOtherRoomsStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.GO_TO_OTHER_ROOMS;


        public OnboardingEditGoToOtherRoomsStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpGoToOtherRooms.Show("Now, tap here to go to new places");
            references.PopUpGoToOtherRooms.ShowArrowLeftDown();
            references.BtnRooms.gameObject.SetActive(true);
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.NextStep();
        }

        public override void Destroy()
        {
            references.PopUpGoToOtherRooms.Hide();
            references.BtnRooms.gameObject.SetActive(false);
            base.Destroy();
        }
    }
}