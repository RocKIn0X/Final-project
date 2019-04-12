using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropShowCase : ItemShowCase
{
    public CropAssets cropAsset;

    public override void PrepareToBuy()
    {
        MarketManager.Instance.PrepareToBuy(cropAsset);
    }
}
