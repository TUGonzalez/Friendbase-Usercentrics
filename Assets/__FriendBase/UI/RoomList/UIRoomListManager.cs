using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Newtonsoft.Json.Linq;
using Data.Bag;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using TMPro;
using Data.Rooms;
using System.Threading.Tasks;
using UI.ScrollView;

public class UIRoomListManager : AbstractUIPanel
{
    enum ROOM_LIST_STATES { PLACES, EVENTS, CHOOSE_ROOM}

    public delegate void OnOpen();
    public static event OnOpen OnOpenEvent;

    public delegate void OnClose();
    public static event OnClose OnCloseEvent;

    [SerializeField] private UIRoomListScrollView scrollView; 
    [SerializeField] private GameObject tabLeft;
    [SerializeField] private GameObject tabRight;
    [SerializeField] private GameObject chooseRoom;
    [SerializeField] protected TextMeshProUGUI txtTabLeft;
    [SerializeField] protected TextMeshProUGUI txtTabRight;
    [SerializeField] protected TextMeshProUGUI txtChooseRoom;
    [SerializeField] protected GameObject loader;

    private ROOM_LIST_STATES currentState;

    private void Start()
    {
        txtTabLeft.text = "Places";
        txtTabRight.text = "Events";

        scrollView.OnCardSelected += OnCardSelected;
    }

    private void OnDestroy()
    {
        scrollView.OnCardSelected -= OnCardSelected;
    }

    public override void Open()
    {
        base.Open();
        OnClickPlaces();
    }

    void UpdateState(ROOM_LIST_STATES newState, RoomInformation roomInformation)
    {
        currentState = newState;
        switch (newState)
        {
            case ROOM_LIST_STATES.PLACES:
                SetStatePlaces();
                break;
            case ROOM_LIST_STATES.EVENTS:
                SetStateEvents();
                break;
            case ROOM_LIST_STATES.CHOOSE_ROOM:
                SetStateChooseRoom(roomInformation);
                break;
        }
    }

    public void SetStatePlaces()
    {
        currentState = ROOM_LIST_STATES.PLACES;
        chooseRoom.SetActive(false);
        tabRight.SetActive(false);
        tabLeft.SetActive(true);
        txtTabLeft.gameObject.SetActive(true);
        txtTabRight.gameObject.SetActive(true);

        scrollView.ResetPosition();
        scrollView.gameObject.SetActive(false);
        loader.SetActive(true);

        Injection.Get<IRoomListEndpoints>().GetPublicRoomsList().Subscribe(listRooms =>
        {
            scrollView.gameObject.SetActive(true);
            loader.SetActive(false);
            List<RoomInformation> roomInformation = listRooms;
            scrollView.ShowObjects(roomInformation);
        });
    }

    public void SetStateEvents()
    {
        currentState = ROOM_LIST_STATES.EVENTS;
        chooseRoom.SetActive(false);
        tabRight.SetActive(true);
        tabLeft.SetActive(false);
        txtTabLeft.gameObject.SetActive(true);
        txtTabRight.gameObject.SetActive(true);

        scrollView.gameObject.SetActive(false);

        Injection.Get<IRoomListEndpoints>().GetPublicRoomsList().Subscribe(listRooms =>
        {
            scrollView.gameObject.SetActive(true);
            List<RoomInformation> roomInformation = listRooms;
            scrollView.ShowObjects(roomInformation);
        });
    }

    public void SetStateChooseRoom(RoomInformation roomInformation)
    {
        currentState = ROOM_LIST_STATES.CHOOSE_ROOM;

        txtChooseRoom.text = "Choose " + roomInformation.RoomName;
        chooseRoom.SetActive(true);
        tabRight.SetActive(false);
        tabLeft.SetActive(false);
        txtTabLeft.gameObject.SetActive(false);
        txtTabRight.gameObject.SetActive(false);

        scrollView.ResetPosition();
        scrollView.gameObject.SetActive(false);
        loader.SetActive(true);

        Injection.Get<IRoomListEndpoints>().GetPublicRoomsListInside().Subscribe(listRooms =>
        {
            List<RoomInformation> roomInformation = listRooms;
            scrollView.gameObject.SetActive(true);
            loader.SetActive(false);
            scrollView.ShowObjects(roomInformation);
        });
    }

    public void OnClickPlaces()
    {
        UpdateState(ROOM_LIST_STATES.PLACES, null);
    }

    public void OnClickEvents()
    {
        UpdateState(ROOM_LIST_STATES.EVENTS, null);
    }

    void OnCardSelected(RoomInformation roomInformation, UIAbstractCardController cardController)
    {
        switch (currentState)
        {
            case ROOM_LIST_STATES.PLACES:
                SetStateChooseRoom(roomInformation);
                break;
            case ROOM_LIST_STATES.EVENTS:
                break;
            case ROOM_LIST_STATES.CHOOSE_ROOM:
                break;
        }
    }
}
