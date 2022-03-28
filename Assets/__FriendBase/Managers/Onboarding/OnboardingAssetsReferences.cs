using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingAssetsReferences : MonoBehaviour
    {
        [SerializeField] public Camera Camera;
        [SerializeField] public Canvas MyCanvas;
        [SerializeField] public GameObject RioBackground;
        [SerializeField] public GameObject ParkBackground;
        [SerializeField] public GameObject PanelBlackScreen;
        [SerializeField] public AvatarCustomizationController AvatarController;
        [SerializeField] public GameObject AvatarSnapshot;
        [SerializeField] public Button BtnRooms;
        [SerializeField] public Button BtnHome;
        [SerializeField] public Button BtnFurnitures;
        [SerializeField] public Button BtnChat;
        [SerializeField] public OnboardingGenericPopUp PopUpWelcome;
        [SerializeField] public OnboardingGenericPopUp PopUpThisIsYourHome;
        [SerializeField] public OnboardingGenericPopUp PopUpThisIsYou;
        [SerializeField] public OnboardingGenericPopUp PopUpTapHereToGo;
        [SerializeField] public OnboardingGenericPopUp PopUpSeeYourProfileCard;
        [SerializeField] public OnboardingProfileCardManager ProfileCardManager;
        [SerializeField] public OnboardingGenericPopUp PopUpEditYourHome;
        [SerializeField] public OnboardingGenericPopUp PopUpGoToOtherRooms;
        [SerializeField] public AvatarCustomizationController AvatarFriendController;
        [SerializeField] public GameObject FriendStartPoint;
        [SerializeField] public GameObject FriendEndPoint;
        [SerializeField] public OnboardingGenericPopUp PopUpTapOnFriend;
        [SerializeField] public OnboardingGenericPopUp PopUpTapChatIcon;
        [SerializeField] public OnboardingGenericPopUp PopUpTapGoHome;
        [SerializeField] public GameObject ScreenTransition;
        [SerializeField] public Button BtnSkipOnboarding;
        [SerializeField] public GameObject OnboardingRoomsList;
        
    }
}