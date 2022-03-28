using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Onboarding
{
    public class OnboardingFriendsAppearStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.FRIEND_APPEAR;
        private bool canGoToNextStep;

        public OnboardingFriendsAppearStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            canGoToNextStep = false;
            references.AvatarFriendController.transform.position = references.FriendStartPoint.transform.position;

            references.AvatarFriendController.gameObject.GetComponent<AvatarAnimationController>().SetWalkState();

            //Align finalPosition Friend to my Avatar
            Vector3 finalPosition = references.FriendEndPoint.transform.position;
            finalPosition.y = references.AvatarController.transform.position.y;
            finalPosition.x = references.AvatarController.transform.position.x + 3f;

            float timeTransition = GetTimeFromWalkDestination(references.FriendStartPoint.transform.position, finalPosition);

            //Flip Avatar
            Vector3 scaleAvatar = references.AvatarController.transform.localScale;
            scaleAvatar.x *= -1;
            references.AvatarController.transform.localScale = scaleAvatar;

            references.AvatarFriendController.transform.DOMove(finalPosition, timeTransition).SetEase(Ease.Linear).OnComplete(()=>
            {
                references.StartCoroutine(PlayHelloAnimations());
            });
        }

        IEnumerator PlayHelloAnimations()
        {
            references.AvatarFriendController.gameObject.GetComponent<AvatarAnimationController>().SetIdleState();

            yield return new WaitForSeconds(0.5f);

            references.AvatarFriendController.GetComponent<AvatarAnimationController>().SetHelloState();

            yield return new WaitForSeconds(1.5f);

            references.AvatarController.GetComponent<AvatarAnimationController>().SetHelloState();

            yield return new WaitForSeconds(1);

            references.PopUpThisIsYourHome.Show("Look, a friend!");

            canGoToNextStep = true;
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            if (canGoToNextStep)
            {
                onboardingManager.NextStep();
            }
        }

        public override void Destroy()
        {
            references.PopUpThisIsYourHome.Hide();
            base.Destroy();
        }
    }
}