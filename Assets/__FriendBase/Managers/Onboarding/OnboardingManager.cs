
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onboarding;
using System;
using Data.Users;
using Architecture.Injector.Core;
using Data;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using Architecture.ViewManager;

//Wave animations DONE
//Skip onboarding DONE
//Room transition DONE
//Walk speed formula DONE
//Camera Position DONE

//Conditions to run the onboarding
//ROOM_POP_UP Step

namespace Onboarding
{
    public enum OnboardingStepType
    {
        WELCOME = 0,
        THIS_IS_YOUR_HOME = 1,
        THIS_IS_YOU = 2,
        TAP_HERE_TO_GO = 3,
        SEE_YOUR_PROFILE = 4,
        SHOW_MY_PROFILE = 5,
        EDIT_YOUR_HOME = 6,
        GO_TO_OTHER_ROOMS = 7,
        ROOM_POP_UP = 8, //Need to do
        WELCOME_TO_THE_PARK = 9,
        FRIEND_APPEAR = 10,
        TAP_ON_FRIEND = 11,
        FRIEND_PROFILE = 12,
        TAP_CHAT_ICON = 13,
        TAP_GO_HOME = 14
    };

    public class OnboardingManager : MonoBehaviour, IOnboarding
    {
        [SerializeField] private OnboardingAssetsReferences references;

        private OnboardingAbstractStep currentStep;

        private IViewManager viewManager;

        void Start()
        {
            //Set avatar skin
            AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
            avatarCustomizationData = Injection.Get<IGameData>().GetUserInformation().GetAvatarCustomizationData();
            references.AvatarController.SetAvatarCustomizationData(avatarCustomizationData.GetSerializeData());

            //Set Friend Skin
            TextAsset avatarSkinCarlos = Resources.Load("Avatar/SkinCarlos") as TextAsset;
            JObject jsonAvatar = JObject.Parse(avatarSkinCarlos.ToString());
            references.AvatarFriendController.SetAvatarCustomizationData(jsonAvatar);

            StartCoroutine(ShowFirstStep());

            SetCameraPosition();
        }

        private void SetCameraPosition()
        {
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            Vector3 cameraPosition = new Vector3(0, 0, -10);

            float widthBackground = references.RioBackground.GetComponent<SpriteRenderer>().size.x;
            float cameraSizeHeight = references.Camera.orthographicSize * 2;
            cameraPosition.x = (widthBackground / 2) - (aspectRatio * cameraSizeHeight / 2);
            references.Camera.transform.position = cameraPosition;
        }

        IEnumerator ShowFirstStep()
        {
            yield return new WaitForSeconds(0.5f);
            currentStep = OnboardingAbstractStep.GetStepInstance(OnboardingStepType.WELCOME, this, references);
            //currentStep = OnboardingAbstractStep.GetStepInstance(OnboardingStepType.WELCOME_TO_THE_PARK, this, references);
        }

        void Update()
        {
            if (currentStep!=null)
            {
                currentStep.Update();
            }
        }

        public void WaitAndNextStep(float time)
        {
            StartCoroutine( WaitAndNextStepCoroutine(time) );
        }

        IEnumerator WaitAndNextStepCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            NextStep();
        }

        public void NextStep()
        {
            int stepIndex = (int) currentStep.StepType;
            stepIndex++;

            OnboardingStepType nextStepType = (OnboardingStepType) stepIndex;

            if (Enum.IsDefined(typeof(OnboardingStepType), nextStepType))
            {
                currentStep.Destroy();
                currentStep = OnboardingAbstractStep.GetStepInstance(nextStepType, this, references);
            }
            else
            {
                EndOnboarding();
            }
        }

        public void EndOnboarding()
        {
            StartCoroutine(EndOnboardingCoroutine());
        }

        IEnumerator EndOnboardingCoroutine()
        {
            if (currentStep != null)
            {
                currentStep.Destroy();
                currentStep = null;
            }

            references.BtnSkipOnboarding.gameObject.SetActive(false);
            references.ScreenTransition.SetActive(true);

            yield return new WaitForSeconds(0.75f);
            Injection.Get<IViewManager>().Show<RoomTransition>();

            yield return new WaitForEndOfFrame();
            SceneManager.UnloadSceneAsync("Onboarding");

            Debug.Log("END ONBOARDING");
        }
    }
}