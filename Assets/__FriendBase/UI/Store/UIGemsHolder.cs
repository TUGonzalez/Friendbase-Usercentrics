using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;
using Architecture.Injector.Core;

public class UIGemsHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtAmountGems;

    private IGameData gameData;

    void Start()
    {
        UIStoreManager.OnGemsChange += UpdateGemsCounter;
        gameData = Injection.Get<IGameData>();
        UpdateGems();
    }

    void UpdateGemsCounter(int amount)
    {
        txtAmountGems.text = amount.ToString();
    }

    void UpdateGems()
    {
        txtAmountGems.text = gameData.GetUserInformation().Gems.ToString();
    }

    private void OnDestroy()
    {
        UIStoreManager.OnGemsChange -= UpdateGemsCounter;
    }
}
