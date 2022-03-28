using Architecture.Injector.Core;
using Architecture.ViewManager;
using Audio.Music;
using Data;
using PlayerMovement;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AvatarCustomization
{
    public class AvatarCustomizationPanel : ViewNode
    {
        [SerializeField] UICatalogAvatarManager catalogAvatarManager;

        IViewManager viewManager;
        IGameData gameData;
        IMusicPlayer musicPlayer;
        protected override void OnInit()
        {
            Injection.Get(out musicPlayer);
            gameData = Injection.Get<IGameData>();
            viewManager = Injection.Get<IViewManager>();
        }


        void OnCreateAvatarComplete()
        {
            SceneManager.LoadScene("Onboarding", LoadSceneMode.Additive);
            HideView();
        }

        void OnChangeAvatarComplete()
        {
            viewManager.GetOut("back");
        }

        protected override void OnShow()
        {
            RemotePlayersPool.Current?.ClearRemotesPool();
            musicPlayer.Play("cafe-music");
            bool flagComeFromRegistration = gameData.GetUserInformation().Do_avatar_customization;

            if (flagComeFromRegistration)
            {
                gameData.GetUserInformation().Do_avatar_customization = false;
                catalogAvatarManager.Open(UICatalogAvatarManager.AVATAR_PANEL_TYPE.CREATE_AVATAR);
            }
            else
            {
                catalogAvatarManager.Open(UICatalogAvatarManager.AVATAR_PANEL_TYPE.CHANGE_AVATAR);
            }

            catalogAvatarManager.OnCreateAvatarComplete += OnCreateAvatarComplete;
            catalogAvatarManager.OnChangeAvatarComplete += OnChangeAvatarComplete;
        }

        protected override void OnHide()
        {
            RemotePlayersPool.Current?.ClearRemotesPool();
            catalogAvatarManager.OnCreateAvatarComplete -= OnCreateAvatarComplete;
            catalogAvatarManager.OnChangeAvatarComplete -= OnChangeAvatarComplete;
            catalogAvatarManager.Close();
        }
    }
}