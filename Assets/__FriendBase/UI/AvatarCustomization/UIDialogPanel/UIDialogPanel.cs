using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIDialogPanel : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtDesc;
    [SerializeField] protected TextMeshProUGUI txtBtnAccept;
    [SerializeField] protected TextMeshProUGUI txtBtnDiscard;

    private Action callbackOnAccept;

    public void Open(string title, string desc, string btnAccept, string btnDiscard, Action callback)
    {
        txtTitle.text = title;
        txtDesc.text = desc;
        txtBtnAccept.text = btnAccept;
        txtBtnDiscard.text = btnDiscard;
        callbackOnAccept = callback;
        base.Open();
    }

    public void OnBtnAccept()
    {
        Close();
        if (callbackOnAccept != null)
        {
            callbackOnAccept();
        }
    }

    public void OnBtnDiscard()
    {
        Close();
    }
}
