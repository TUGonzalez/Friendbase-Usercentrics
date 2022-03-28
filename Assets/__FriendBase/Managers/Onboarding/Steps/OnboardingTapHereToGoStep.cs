using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Onboarding
{
    public class OnboardingTapHereToGoStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.TAP_HERE_TO_GO;

        private Button btnTapHere;

        public OnboardingTapHereToGoStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpTapHereToGo.Show("Tap here to go");
            references.PopUpTapHereToGo.ShowArrowDown();
            btnTapHere = references.PopUpTapHereToGo.transform.Find("BtnTapHere").gameObject.GetComponent<Button>();
            btnTapHere.onClick.AddListener(OnTapHere);
        }

        private void OnTapHere()
        {
            Vector3 worldPoint = references.Camera.ScreenToWorldPoint(InputFunctions.mousePosition);

            //Set z position
            Vector3 avatarPosition = references.AvatarController.transform.position;
            worldPoint.z = avatarPosition.z;

            references.PopUpTapHereToGo.Hide();
            //Move Avatar
            AvatarAnimationController avatarAnimationController = references.AvatarController.GetComponent<AvatarAnimationController>();
            avatarAnimationController.SetWalkState();

            float timeTransition = GetTimeFromWalkDestination(avatarPosition, worldPoint);

            references.AvatarController.transform.DOMove(worldPoint, timeTransition).SetEase(Ease.Linear).OnComplete(()=> {
                avatarAnimationController.SetIdleState();
                onboardingManager.NextStep();
            });
        }

        public override void Destroy()
        {
            references.PopUpTapHereToGo.Hide();
            btnTapHere.onClick.RemoveListener(OnTapHere);
            base.Destroy();
        }
    }
}
