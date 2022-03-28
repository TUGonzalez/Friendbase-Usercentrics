using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingTapOnFriendStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.TAP_ON_FRIEND;

        private bool isTapping;
        private float tapTime;
        private float longTapSeconds = 0.5f;

        public OnboardingTapOnFriendStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            Vector2 canvasPosition = RectTransformUtility.WorldToScreenPoint(references.Camera, references.AvatarFriendController.transform.position) / references.MyCanvas.scaleFactor;

            canvasPosition.x -= references.PopUpThisIsYou.GetComponent<RectTransform>().sizeDelta.x * 0.8f;
            canvasPosition.y += references.PopUpThisIsYou.GetComponent<RectTransform>().sizeDelta.y * 2.2f;

            references.PopUpTapOnFriend.GetComponent<RectTransform>().anchoredPosition = canvasPosition;

            references.PopUpTapOnFriend.Show("Long tap on them for more info");
            references.PopUpTapOnFriend.ShowArrowRightDown();

            isTapping = false;
        }

        public override void Update()
        {
            if (InputFunctions.GetMouseButtonDown(0))
            {
                Vector3 worldTouch = references.Camera.ScreenToWorldPoint(InputFunctions.mousePosition);
                RaycastHit2D hitItem;
                hitItem = Physics2D.Raycast(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero);

                if (hitItem.collider != null && hitItem.collider.gameObject.name.Equals("AvatarFriend"))
                {
                    isTapping = true;
                    tapTime = 0;
                }
            }

            if (InputFunctions.GetMouseButton(0) && isTapping)
            {
                tapTime += Time.deltaTime;
                if (tapTime >= longTapSeconds)
                {
                    isTapping = false;
                    onboardingManager.NextStep();
                }
            }

            if (InputFunctions.GetMouseButtonUp(0))
            {
                isTapping = false;
            }
        }

        public override void Destroy()
        {
            references.PopUpTapOnFriend.Hide();
            base.Destroy();
        }
    }
}