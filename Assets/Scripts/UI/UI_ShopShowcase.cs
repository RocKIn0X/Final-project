using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopShowcase : MonoBehaviour
{
    public Sprite plantLocked;
    public Sprite plantIcon;
    public System.String plantName;
    public int plantCost;

    public UI_ShopPanel shopOwner;
    public Image displayImage;
    public Text displayText;
    public Button buyButton;

    public void UpdateDisplay()
    {
        if (plantIcon != null)
        {
            displayImage.sprite = plantIcon;
            displayText.text = plantName + "\n" + plantCost.ToString();
            buyButton.interactable = true;
        }
        else
        {
            displayImage.sprite = plantLocked;
            displayText.text = "Lockberry";
            buyButton.interactable = false;
        }

    }

    public void ActivateBuying()
    {
        shopOwner.BuyingPlant(plantIcon, plantCost);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
