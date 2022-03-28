using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIStoreFurnitureNoItems : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtDesc;

    void Start()
    {
        txtTitle.text = "You don't have any furniture";
        txtDesc.text = "When you buy something, it'll show up here.";
    }
}
