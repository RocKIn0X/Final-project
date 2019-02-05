using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinText;
    public int coinAmount = 0;

    private void Start()
    {
        SetCointText();
    }

    public void AddCoin(int amount)
    {
        coinAmount += amount;
        SetCointText();
    }

    public void UseCoin(int amount)
    {
        if (coinAmount - amount >= 0) coinAmount -= amount;
        SetCointText();
    }

    private void SetCointText()
    {
        coinText.text = "coin : " + coinAmount.ToString();
    }
}
