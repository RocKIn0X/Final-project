using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopPanel : MonoBehaviour
{
    //[SerializeField] List<UI_ShopShowcase> showcase_List = new List<UI_ShopShowcase>();
    public Image bannerImage;
    public Image itemImage;
    public Text bannerText;

    public Sprite overpriceBanner;
    public Sprite saleBanner;

    public CropAssets currentPromoCrop;
    public List<CropAssets> cropList = new List<CropAssets>();

    public GameObject showcaseContainer;
    private int promoCooldown = 0;
    private UI_TextNotif notifManager;
    private void Start()
    {
        RefreshCropList();
        UpdateShop();
        RandomPromo();
        //foreach (UI_ShopShowcase showcase in showcase_List) showcase.UpdateDisplay();
    }

    public void UpdateShop()
    {
        foreach (Transform child in showcaseContainer.transform)
        {
            UI_ShopShowcase showcase = child.gameObject.GetComponent<UI_ShopShowcase>();
            showcase.UpdateDisplay();
        }
    }

    public void WeekTicks()
    {
        promoCooldown = promoCooldown - 1;
        if (promoCooldown <= 0)
        {
            RandomPromo();
        }
    }

    public void RefreshCropList()
    {
        cropList.Clear();
        foreach (Transform child in showcaseContainer.transform)
        {
            UI_ShopShowcase refShowcase = child.gameObject.GetComponent<UI_ShopShowcase>();
            if (refShowcase != null)
                if(refShowcase.cropAsset != null)
                    cropList.Add(refShowcase.cropAsset);
        }
    }

    public void RandomPromo()
    {
        if (cropList.Count < 1)
            RefreshCropList();
        CropAssets targetCrop = cropList[Random.Range(0, cropList.Count - 1)];
        float multiplier = 1f + ((float)Random.Range(-5, 5)) / 10.0f;
        if (multiplier == 1f)
        {
            RandomPromo();
            return;
        }
        if (targetCrop == null)
            return;
        CreatePromo(targetCrop, multiplier);
    }

    public void ResetCrop()
    {
        // Scriptables PERMANENTLY changes
        foreach (CropAssets crop in cropList)
            crop.priceMultiplier = 1f;
    }

    public void CreatePromo(CropAssets targetCrop, float multiplier)
    {
        ResetCrop();

        bannerImage.gameObject.SetActive(true);
        targetCrop.priceMultiplier = multiplier;
        int percentChange = Mathf.Abs(Mathf.RoundToInt((multiplier - 1f) * 100f));
        if (multiplier > 1.0f)
        {
            if (notifManager == null)
                notifManager = (UI_TextNotif)FindObjectOfType(typeof(UI_TextNotif));
            notifManager.Notify("Monthly Promo: " + percentChange.ToString() +
                                "% Markup for", targetCrop.cropSprite);
            bannerImage.sprite = overpriceBanner;
            bannerText.text = percentChange.ToString() + "% Markup";
        }
        else if (multiplier < 1.0f)
        {
            if (notifManager == null)
                notifManager = (UI_TextNotif)FindObjectOfType(typeof(UI_TextNotif));
            notifManager.Notify("Monthly Promo: " + percentChange.ToString() +
                                "% Off for", targetCrop.cropSprite);
            bannerImage.sprite = saleBanner;
            bannerText.text = percentChange.ToString() + "% Off";
        }
        else
        {
            promoCooldown = 4;
            bannerImage.gameObject.SetActive(false);
            UpdateShop();
            return;
        }
        itemImage.sprite = targetCrop.cropSprite;
        promoCooldown = 4;
        UpdateShop();
    }
}
