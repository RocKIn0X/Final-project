using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler
{
    private CropAssets cropAsset;
    public int amount;
    [SerializeField] Canvas PopupCanvase;
    [SerializeField] Image cropImage;
    [SerializeField] TextMeshProUGUI seedAmountText;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        PlayerManager.Instance.cursorImage.enabled = true;
        PlayerManager.Instance.cropAsset_selectToPlant = this.cropAsset;
        PlayerManager.Instance.cursorImage.sprite = PlayerManager.Instance.cropAsset_selectToPlant.cropSprite;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(PopupCanvase.transform as RectTransform, Input.mousePosition, PopupCanvase.worldCamera, out pos);
        PlayerManager.Instance.cursorImage.transform.position = PopupCanvase.transform.TransformPoint(pos);
    }
    
    public void OnBeginDrag(PointerEventData data)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(PopupCanvase.transform as RectTransform, Input.mousePosition, PopupCanvase.worldCamera, out pos);
        PlayerManager.Instance.cursorImage.transform.position = PopupCanvase.transform.TransformPoint(pos);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask layerMask = LayerMask.GetMask("Tile");
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000, layerMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<WorkTile>() != null) hit.collider.gameObject.GetComponent<WorkTile>().PlantFromPlayer(cropAsset);
        }
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(PopupCanvase.transform as RectTransform, Input.mousePosition, PopupCanvase.worldCamera, out pos);
        PlayerManager.Instance.cursorImage.transform.position = PopupCanvase.transform.TransformPoint(pos);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask layerMask = LayerMask.GetMask("Tile");
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000, layerMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<WorkTile>() != null) hit.collider.gameObject.GetComponent<WorkTile>().PlantFromPlayer(cropAsset);
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        PlayerManager.Instance.cursorImage.enabled = false;
    }

    void OnDisable()
    {
        PlayerManager.Instance.cursorImage.enabled = false;
    }

    public void SetInventory(CropAssets _cropAsset, int _amount)
    {
        cropAsset = _cropAsset;
        amount = _amount;
        cropImage.sprite = _cropAsset.cropSprite;
        PlayerManager.Instance.cursorImage.sprite = _cropAsset.cropSprite;
        seedAmountText.text = amount.ToString();
    }
}
