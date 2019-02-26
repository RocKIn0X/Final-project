using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Market : MonoBehaviour
{
    private Player player;
    [SerializeField] TextMeshProUGUI seedCountText;
    public int hellSeedCount = 0;

    private void Start()
    {
        player = FindObjectOfType<Player>().GetComponent<Player>();
        SetSeedCountText();
    }

    public void BuySeed()
    {
        if (player.coinAmount >= 50)
        {
            player.UseCoin(50);
            hellSeedCount += 10;
            SetSeedCountText();
        }
    }

    private void SetSeedCountText()
    {
        seedCountText.text = "Hell Seed : " + hellSeedCount.ToString();
    }
}
