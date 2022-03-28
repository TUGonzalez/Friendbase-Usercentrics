using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractUIPanel : MonoBehaviour
{
    [SerializeField] protected GameObject container;

    public bool IsOpen { get; private set; }

    public virtual void Open()
    {
        IsOpen = true;
        container.SetActive(true);
        OnOpen();
    }

    public virtual void Close()
    {
        IsOpen = false;
        container.SetActive(false);
        OnClose();
    }

    public virtual void OnOpen()
    {
        // ..
    }
    
    public virtual void OnClose()
    {
        // ..
    }
}
