using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Rooms;
using UnityEngine;
using UnityEngine.EventSystems;
using Socket;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera roomCamera;

    public delegate void TapAvatar(AvatarRoomController avatarRoomController);
    public static event TapAvatar OnTapAvatar;

    private bool canCheck = false; //Prevent walk over UI (we make it true on mousedown and ask state on mouseup)
    private float tapTimeAvatar;
    private bool isTappingAvatar;
    private const float longTapSeconds = 0.5f;
    private IGameData gameData;

    AvatarRoomController avatarRoomControllerTapped;

    private void Awake()
    {
        gameData = Injection.Get<IGameData>();
        isTappingAvatar = false;
    }

    public bool IsPointerOverGameObject()
    {
        //check mouse
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        // check touch
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
            {
                return true;
            }
        }

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void Update()
    {
        ControlInputUpdate();
    }

    void ControlInputUpdate()
    {
        //Mouse Down
        if (InputFunctions.GetMouseButtonDown(0))
        {
            canCheck = !IsPointerOverGameObject();

            if (canCheck)
            {
                //Get hit item
                Vector3 worldTouch = roomCamera.ScreenToWorldPoint(InputFunctions.mousePosition);
                RaycastHit2D hitItem;
                hitItem = Physics2D.Raycast(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero);

                //Check if it is Avatar
                if (hitItem.collider != null)
                {
                    avatarRoomControllerTapped = hitItem.collider.gameObject.GetComponent<AvatarRoomController>();

                    if (avatarRoomControllerTapped != null)
                    {
                        isTappingAvatar = true;
                        tapTimeAvatar = 0;
                    }
                }
            }
        }

        //Mouse still down
        if (InputFunctions.GetMouseButton(0) && isTappingAvatar)
        {
            tapTimeAvatar += Time.deltaTime;
            if (tapTimeAvatar >= longTapSeconds)
            {
                //TAP AVATAR
                isTappingAvatar = false;
                canCheck = false;

                if (OnTapAvatar!=null)
                {
                    OnTapAvatar(avatarRoomControllerTapped);
                }
                Debug.Log("CLICK AVATAR");
            }
        }

        if (InputFunctions.GetMouseButtonUp(0))
        {
            if (canCheck)
            {
                //Click Walk
                AvatarRoomController myAvatar = CurrentRoom.Instance.AvatarsManager.GetMyAvatar();
                Vector3 worldTouch = roomCamera.ScreenToWorldPoint(InputFunctions.mousePosition);
                //Local movement
                myAvatar.SetWalkToDestination(worldTouch.x, worldTouch.y);

                //Send movement to backend
                RoomInformation roomInformation = CurrentRoom.Instance.RoomInformation;
                SimpleSocketManager.Instance.SendAvatarMove(roomInformation.RoomName, roomInformation.RoomIdInstance, worldTouch.x, worldTouch.y);
            }
            canCheck = false;
            isTappingAvatar = false;
        }
    }
}
