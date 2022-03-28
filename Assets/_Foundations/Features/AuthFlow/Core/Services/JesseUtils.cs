using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using Firebase.Auth;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Web;
using UniRx;
using AuthFlow.EndAuth.Repo;
using AuthFlow.AboutYou.Core.Services;
using Data;
using System.Linq;
using System.Text;
using AppleAuth.Interfaces;
using Data.Catalog;
using Data.Users;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using WebClientTools.Core.Services;
using Google;

class Constants
{
    public const string Protocol = "https://";
    public const string Hostname = "friendbase-staging.fly.dev";
    public const string ApiRoot = Protocol + Hostname + "/api";

    public const string CreateEmailURL = ApiRoot + "/users";
    public const string EmailExistsURL = ApiRoot + "/email-exists";
    public const string UsernameValidationEndpoint = ApiRoot + "/username-validation/";
    public const string ResetPasswordEndpoint =
        "https://us-central1-friendbase-dev.cloudfunctions.net/api/password-reset-email"; 
    public const string UsersEndpoint = ApiRoot + "/users";
    public const string UsersRootEndPoint = ApiRoot + "/users";
    public const string ReportUserEndPoint = ApiRoot + "/user-reports";
    public const string UserRelationshipEndpoint = ApiRoot + "/users-relationship";
    public const string StoreRootEndPoint = ApiRoot + "/store/";
    public const string ItemsEndPoint = ApiRoot + "/items/";
    public const string ChatRoomsEndpoint = ApiRoot + "/chat-rooms";
    public const string RoomTypesEndpoint = ApiRoot + "/room-types";
    public const string VerifyApplePurchaseEndpoint = ApiRoot + "/verify-apple-purchase";
    public const string VerifyGooglePurchaseEndpoint = ApiRoot + "/verify-google-purchase";
    public const string PurchaseEndpoint = ApiRoot + "/purchases";

    public const string SocketUrl = "wss://" + Hostname + "/socket/websocket/";
};

namespace AuthFlow
{
    public class JesseUtils
    {
        public static async Task<IEnumerable<string>>
            EmailProviders(string email)
        {
            return await FirebaseAuth.DefaultInstance.FetchProvidersForEmailAsync(email);
        }

        // NOTE(Jesse) This could probably be merged with SignUpUser and switched
        // on a flag.  I feel like the error handling is the same.
        public static async Task<(FirebaseUser, string, string)>
            SignInUser(string email, string password)
        {
            (FirebaseUser, string, string) Result = (null, null, "Unknown error signing in user.");

            try
            {
                FirebaseUser firebaseUser =
                    await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
                string loginToken = await firebaseUser.TokenAsync(true);

                /* Injection.Get<IAnalyticsService>().SendLoginEvent(); */
                Debug.LogFormat("Firebase user signed in successfully: {0} ({1})", firebaseUser.DisplayName,
                    firebaseUser.UserId);
                
                Result = (firebaseUser, loginToken, null);

                if (await FetchAndCachePhoenixUserInfo(firebaseUser, loginToken))
                {
                    Result = (firebaseUser, loginToken, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Result = (null, null, e.InnerException.Message);
            }

            return Result;
        }

        public static async Task<string>
            IsUserLoggedIn()
        {
            string Result = null;

            var firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
            if (firebaseUser != null)
            {
                ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
                string loginToken = userInfo["firebase-login-token"];

                if (!string.IsNullOrEmpty(loginToken))
                {
                    await FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
                    firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
                    if (firebaseUser != null)
                    {
                        loginToken = await firebaseUser.TokenAsync(true);

                        if (await FetchAndCachePhoenixUserInfo(firebaseUser, loginToken))
                        {
                            Result = loginToken;
                        }
                    }
                }
            }

            return Result;
        }

        public static async Task<bool>
            CreatePhoenixUser(FirebaseUser firebaseUser, string loginToken)
        {
            // TODO(Jesse): Why not just construct a JSON string directly?
            var json = new JObject
            {
                ["user"] = new JObject
                {
                    ["email"] = firebaseUser.Email,
                    ["firebase_uid"] = firebaseUser.UserId,
                    ["registration_type"] = "email",
                }
            }.ToString();

            var req = WebClient.Request(
                WebMethod.Post,
                Constants.CreateEmailURL,
                json,
                false,
                ("Content-Type", "application/json"),
                ("Authorization", "Bearer " + loginToken)
            );

            // NOTE(Jesse): If this request returns the actual user object (which I think it does)
            // we should return that instead of a boolean.  The call to this function in FetchAndCachePhoenixUserInfo
            // wants to know that information.
            RequestInfo response = null;
            try
            {
                response = await req.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                // Let the user know what went wrong?
            }

            bool Result = false;
            if (response != null)
            {
                var emailToken = response.json["data"]?["email"];
                // TODO(Jesse): This is wack.  There's got to be a better way.
                Result = emailToken != null && !(string.IsNullOrEmpty(emailToken.ToString()));
            }

            return Result;
        }


        // TODO(Jesse): The error handling in here is pretty janky .. fix it.
        public static async Task<(FirebaseUser, string, string)>
            SignUpUser(string email, string password)
        {
            string loginToken = null;
            string errorMessage = null;
            FirebaseUser firebaseUser = null;

            try
            {
                firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
                loginToken = await firebaseUser.TokenAsync(true);
            }
            catch (Exception e)
            {
                /* Debug.LogError("User Creation Exception: " + e.Message); */
                return (null, null, e.Message);
            }

            // NOTE(Jesse): I made the assumption this would return null if login/creation fails but I actually didn't check
            if (string.IsNullOrEmpty(loginToken) || firebaseUser == null)
            {
                return (null, null, "Unknown Error Creating Firebase User.");
            }
            else
            {
                Injection.Get<IAnalyticsService>().SendSignUpEvent("email");
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", firebaseUser.DisplayName,
                    firebaseUser.UserId);
            }

            // NOTE(Jesse): This call to CreatePhoenixUser _should_ actually be redundant
            // because FetchAndCachePhoenixUserInfo will create it if it's not found, but it would
            // be an extra request, so we do it here anyways.
            bool Success = false;
            if (await JesseUtils.CreatePhoenixUser(firebaseUser, loginToken))
            {
                if (await JesseUtils.FetchAndCachePhoenixUserInfo(firebaseUser, loginToken))
                {
                    Success = true;
                }
            }

            (FirebaseUser, string, string) Result = (null, null, null);
            if (Success)
            {
                Result = (firebaseUser, loginToken, null);
            }
            else
            {
                Result = (null, null, "Error Signing up user.");
            }

            return Result;
        }
        
        // Using the credentials returned by Apple Auth allows logging in with Firebase
        public static async void SignInUserApple(
            IAppleIDCredential appleIdCredential,
            string rawNonce,
            Action<FirebaseUser> firebaseAuthCallback
            )
        {
            string identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
            string authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
            Credential firebaseCredential = OAuthProvider.GetCredential(
                "apple.com",
                identityToken,
                rawNonce,
                authorizationCode
            );
            await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(
                task => SignInWithUser(task, firebaseAuthCallback)
            );
        }
        
        public static async void SignInUserFacebook(
            Action<FirebaseUser> facebookAuthCallback
        )
        {
            // AccessToken class will have session details
            Facebook.Unity.AccessToken fullAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            string accessToken = fullAccessToken.TokenString;
            
            Credential firebaseCredential = FacebookAuthProvider.GetCredential(accessToken);
            
            await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(
                task => SignInWithUser(task, facebookAuthCallback)
            );
        }

        public static async void SignInUserGoogle(
            Action<FirebaseUser> googleAuthCallback)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration {
                RequestIdToken = true,
                WebClientId = "1045385706868-0l0p89r0o4c8ro6kqnt43e2ali9loo1n.apps.googleusercontent.com"
            };

            Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
            TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

            await signIn.ContinueWith(task => {
                if (task.IsCanceled || task.IsFaulted)
                {
                    googleAuthCallback(null);
                } else {
                    Credential credential = GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                    FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(
                        signInTask => SignInWithUser(signInTask, googleAuthCallback)
                    );
                }
            });
        }


        // Access Firebase with a FirebaseUser, this should only be used by Providers
        private static async Task<(FirebaseUser, string, string)>
            SignInWithUser(Task<FirebaseUser> task, Action<FirebaseUser> firebaseUserCallback)
        {
            (FirebaseUser, string, string) Result = (null, null, "Unknown error signing in user.");
            
            if (task.IsCanceled)
            {
                Result = (null, null, "Firebase auth was canceled");
                firebaseUserCallback(null);
            }
            else if (task.IsFaulted)
            {
                Result = (null, null, "Firebase auth failed");
                firebaseUserCallback(null);
            }
            else
            {
                FirebaseUser firebaseUser = task.Result;
                Debug.Log("Firebase auth completed | User ID:" + firebaseUser.UserId);

                string loginToken = await firebaseUser.TokenAsync(true);
                Result = (firebaseUser, loginToken, null);

                if (await FetchAndCachePhoenixUserInfo(firebaseUser, loginToken))
                {
                    Result = (firebaseUser, loginToken, null);
                    firebaseUserCallback(firebaseUser);
                }
            }
            
            return Result;
        }

        // TODO(Jesse): This should actually return a bool and get called
        // internally by "LoginUser" and "SignUpUser" instead of where we're
        // logging in and signing up users.  Need to fix the AboutYouWebClient
        // situation first..
        public static async Task<bool>
            FetchAndCachePhoenixUserInfo(FirebaseUser firebaseUser, string loginToken)
        {
            bool Result = false;

            IAboutYouWebClient ayWebClient = Injection.Get<IAboutYouWebClient>();
            ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
            userInfo.Clear();

            IAboutYouStateManager aboutYouState = Injection.Get<IAboutYouStateManager>();
            aboutYouState.Clear();

            Web.RequestInfo userData = null;

            try
            {
                userData = await ayWebClient.GetUserData();
            }
            catch (Exception e)
            {
                // TODO(Jesse): This is pretty janky and could be improved.
                // Specifically, if the second GetUserData function fails .. what
                // then?  Instead of throwing exceptions we should just return null

                if (e.Message.StartsWith("HTTP error \"Not Found\"  HTTP/1.1 404 Not Found Server"))
                {
                    bool userCreated = await JesseUtils.CreatePhoenixUser(firebaseUser, loginToken);
                    if (userCreated)
                    {
                        userData = await ayWebClient.GetUserData();
                    }
                    else
                    {
                        // Hard failure case .. we have a firebase user but couldn't
                        // create a Phoenix user.
                        //
                        // At least logout the firebase user such that they can try
                        // again?
                        FirebaseAuth.DefaultInstance.SignOut();
                    }
                }
            }

            if (userData == null) // Hard failure case
            {
                Result = false;
            }
            else
            {
                var userJson = new JObject(userData.json).Value<JObject>("data");

                // NOTE(Jesse): this is a bit wonky .. custom_avatar is set to false
                // when a new account is created, and set to true once we've passed
                // through avatar customization.  Not an issue, but the code reads
                // a little weird.
                bool doAvatarCustomization = !userJson.Value<bool>("custom_avatar");

                IGameData gameData = Injection.Get<IGameData>();
                gameData.GetUserInformation().Do_avatar_customization = doAvatarCustomization;

                userInfo["firebase-login-token"] = loginToken;
                userInfo["terms"] = userJson.Value<string>("terms_accepted");
                aboutYouState.FirstName = userJson.Value<string>("first_name");
                aboutYouState.LastName = userJson.Value<string>("last_name");
                aboutYouState.Gender = userJson.Value<string>("gender");
                aboutYouState.UserName = userJson.Value<string>("username");

                var birthday = userJson.Value<string>("birthday");
                if (!string.IsNullOrEmpty(birthday))
                {
                    aboutYouState.BirthDate = DateTime.Parse(birthday);
                }

                if (await GetInitialAvatarEndpoints())
                {
                    Result = true;
                }
            }

            return Result;
        }


        public static async Task<bool>
            LoginTracking(UserInformation firebaseUser)
        {
            var token = await Injection.Get<IWebHeadersBuilder>().BearerToken;

            var platform = "";
            switch(Application.platform) {
                case RuntimePlatform.Android:
                    platform = "android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platform = "ios";
                    break;
                case RuntimePlatform.WebGLPlayer:
                    platform = "web";
                    break;
                default:
                    platform = "unknown";
                    break;
            }

            var json = new JObject
            {
                ["login"] = new JObject
                {
                    ["platform"] = platform
                }
            }.ToString();

            var req = WebClient.Request(
                WebMethod.Post,
                Constants.UsersEndpoint + "/" + firebaseUser.UserId + "/login-tracking",
                json,
                false,
                ("Content-Type", "application/json"),
                token
            );

            RequestInfo response = null;
            try
            {
                response = await req.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                // Let the user know what went wrong?
            }
            return true;
        }

        public static async Task<bool> GetInitialAvatarEndpoints()
        {
            bool Result = false;

            try
            {
                IGameData gameData = Injection.Get<IGameData>();

                //Get ALL Catalog of items
                var listItems = await Injection.Get<IAvatarEndpoints>().GetAvatarCatalogItemsList();

                //Initialize Catalof of items
                gameData.InitializeCatalogs(listItems.ToList<GenericCatalogItem>());

                //After we get the items we can ask for my avatar skin
                var json = await Injection.Get<IAvatarEndpoints>().GetAvatarSkin();

                //string jsonString = json.ToString(Formatting.Indented);
                //Debug.Log("GetAvatarSkin <color=green>" + jsonString + "</color>");
                AvatarCustomizationData avatarData = new AvatarCustomizationData();
                avatarData.SetDataFromUserSkin(json);
                gameData.GetUserInformation().GetAvatarCustomizationData().SetData(avatarData);

                //Get User Inventory
                var listBagItems = await Injection.Get<IAvatarEndpoints>().GetPlayerInventory();
                gameData.AddItemsToBag(listBagItems);

                //Get User Information
                var usInformation = await Injection.Get<IAvatarEndpoints>().GetUserInformation();
                gameData.GetUserInformation().Initialize(usInformation);

                Result = true;
            }
            catch (Exception e)
            {
                // TODO(Jesse): Write actual error handling code
                Debug.LogError("Something went wrong during GetInitialAvatarEndpoints()");
            }

            return Result;
        }

        public static void
            Logout()
        {
            // TODO(Jesse): Disconnect websocket if it's connected
            // TODO(Jesse): Stop audio track if it's playing

            // TODO(Jesse): Should we nuke all playerprefs at this point?
            // Probably not because it's nice if the users email is saved when
            // they go to login again .. but I'm not sure if that's even
            // happening right now.
            // PlayerPrefs.DeleteAll();

            FirebaseAuth.DefaultInstance.SignOut();

            // Try to sign out from Google
            try
            {
                GoogleSignIn.DefaultInstance.SignOut();
            } catch (Exception e)
            {
                // Is not necessary to do anything here
            }

            // This is a 'nuke from orbit' type of operation.  The Injection
            // system caches objects until you tell it to remove them which
            // causes a number of issues throughout the Auth flow.  Doing this
            // papers over them, however users may still hit them if they go back
            // and forth between views a bunch of times.
            Injection.ClearAll();

            SceneManager.LoadScene(0);
        }

        public static async void SendEmailResetPassword(string email)
        {
            JObject json = new JObject
            {
                ["email"] = email,
            };

            IObservable<RequestInfo> request = WebClient.Post(
                Constants.ResetPasswordEndpoint,
                json.ToString(),
                false,
                ("Content-Type", "application/json")
            );

            try
            {
                await request.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                // ..
            }
        }

        public static void ConnectionError()
        {
            ClearInjection();
            SceneManager.LoadScene("ConnectionError");
        }

        public static void StartGame()
        {
            SceneManager.LoadScene(0);
        }

        public static void Nuke()
        {
            ClearInjection();
            StartGame();
        }

        static void ClearInjection()
        {
            Injection.ClearAll();
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}