using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] List<CinemachineVirtualCamera> virtualCamerasList;

    public void SetCameras(Transform player)
    {
        virtualCamerasList.ForEach(camera => camera.Follow = player);
    }
}