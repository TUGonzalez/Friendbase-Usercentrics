using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public abstract class OnboardingAbstractStep 
    {
        public abstract OnboardingStepType StepType { get; }
        protected IOnboarding onboardingManager;
        protected OnboardingAssetsReferences references;
        protected Coroutine limitTimeCoroutine;

        private bool flagTap;

        public OnboardingAbstractStep(IOnboarding onboardingManager, OnboardingAssetsReferences references)
        {
            this.onboardingManager = onboardingManager;
            this.references = references;
            limitTimeCoroutine = references.StartCoroutine(WaitLimitTime());

            flagTap = false;
        }

        public void SetParkBackground()
        {
            references.ParkBackground.SetActive(true);
            references.RioBackground.SetActive(false);
        }

        public void SetRioBackground()
        {
            references.RioBackground.SetActive(true);
            references.ParkBackground.SetActive(false);
        }

        public virtual void Update()
        {
            if (InputFunctions.GetMouseButtonDown(0))
            {
                flagTap = true;
            }
            
            if (InputFunctions.GetMouseButtonUp(0) && flagTap)
            {
                OnTapScreen(InputFunctions.mousePosition);
                flagTap = false;
            }
        }

        protected virtual void OnTapScreen(Vector3 mousePosition)
        {

        }

        protected float GetTimeFromWalkDestination(Vector3 startPoint, Vector3 endPoint)
        {
            Vector3 delta = endPoint - startPoint;
            float distance = delta.magnitude;
            return distance / 2f;
        }

        public virtual void Destroy()
        {
            if (limitTimeCoroutine!=null)
            {
                references.StopCoroutine(limitTimeCoroutine);
            }
        }

        IEnumerator WaitLimitTime()
        {
            yield return new WaitForSeconds(8);
            limitTimeCoroutine = null;
            OnLimitTimePassed();
        }

        protected virtual void OnLimitTimePassed()
        {

        }

        public static OnboardingAbstractStep GetStepInstance(OnboardingStepType stepType, IOnboarding onboardingManager, OnboardingAssetsReferences references)
        {
            switch (stepType)
            {
                case OnboardingStepType.WELCOME:
                    return new OnboardingWelcomeStep(onboardingManager, references);
                case OnboardingStepType.THIS_IS_YOUR_HOME:
                    return new OnboardingThisIsYourHomeStep(onboardingManager, references);
                case OnboardingStepType.THIS_IS_YOU:
                    return new OnboardingThisIsYouStep(onboardingManager, references);
                case OnboardingStepType.TAP_HERE_TO_GO:
                    return new OnboardingTapHereToGoStep(onboardingManager, references);
                case OnboardingStepType.SEE_YOUR_PROFILE:
                    return new OnboardingSeeYourProfileStep(onboardingManager, references);
                case OnboardingStepType.SHOW_MY_PROFILE:
                    return new OnboardingMyProfileStep(onboardingManager, references);
                case OnboardingStepType.EDIT_YOUR_HOME:
                    return new OnboardingEditYourHomeStep(onboardingManager, references);
                case OnboardingStepType.GO_TO_OTHER_ROOMS:
                    return new OnboardingEditGoToOtherRoomsStep(onboardingManager, references);
                case OnboardingStepType.ROOM_POP_UP:
                    return new OnboardingShowRoomPopUp(onboardingManager, references);
                case OnboardingStepType.WELCOME_TO_THE_PARK:
                    return new OnboardingWelcomeToTheParkStep(onboardingManager, references);
                case OnboardingStepType.FRIEND_APPEAR:
                    return new OnboardingFriendsAppearStep(onboardingManager, references);
                case OnboardingStepType.TAP_ON_FRIEND:
                    return new OnboardingTapOnFriendStep(onboardingManager, references);
                case OnboardingStepType.FRIEND_PROFILE:
                    return new OnboardingFriendProfileStep(onboardingManager, references);
                case OnboardingStepType.TAP_CHAT_ICON:
                    return new OnboardingTapIconChat(onboardingManager, references);
                case OnboardingStepType.TAP_GO_HOME:
                    return new OnboardingTapGoHome(onboardingManager, references);

            }
            return null;
        }
    }
}