using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingSeeYourProfileStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.SEE_YOUR_PROFILE;

        private Button btnTapAvatar;
        private bool flagTapAvatar;

        public OnboardingSeeYourProfileStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpSeeYourProfileCard.Show("See your profile card");
            references.PopUpSeeYourProfileCard.ShowArrowLeftDown();

            references.AvatarSnapshot.SetActive(true);

            btnTapAvatar = references.AvatarSnapshot.transform.Find("BtnTapAvatar").GetComponent<Button>();
            btnTapAvatar.onClick.AddListener(OnTapAvatar);

            flagTapAvatar = false;
        }

        private void OnTapAvatar()
        {
            if (flagTapAvatar)
            {
                return;
            }
            flagTapAvatar = true;
            onboardingManager.WaitAndNextStep(0.25f);
        }

        public override void Destroy()
        {
            btnTapAvatar.onClick.RemoveListener(OnTapAvatar);
            references.PopUpSeeYourProfileCard.Hide();
            references.AvatarSnapshot.SetActive(false);
            base.Destroy();
        }
    }
}