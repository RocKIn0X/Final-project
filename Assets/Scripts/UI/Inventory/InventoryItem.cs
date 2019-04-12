using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CropAssets cropAsset;
    public int amount;
    [SerializeField] Image cropImage;
    [SerializeField] TextMeshProUGUI seedAmountText;
    
    public void OnBeginDrag(PointerEventData data)
    {
        Debug.Log("OnBeginDrag: " + data.position);
    }

    public void OnDrag(PointerEventData data)
    {
        Debug.Log("Dragging:" + data.position);
    }

    public void OnEndDrag(PointerEventData data)
    {
        Debug.Log("OnEndDrag: " + data.position);
    }

    public void SetInventory(CropAssets _cropAsset, int _amount)
    {
        cropAsset = _cropAsset;
        amount = _amount;
        cropImage.sprite = _cropAsset.cropSprite;
        seedAmountText.text = amount.ToString();
    }
}
