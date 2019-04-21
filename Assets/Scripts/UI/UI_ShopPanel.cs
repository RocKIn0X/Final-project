using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopPanel : MonoBehaviour
{
    //[SerializeField] List<UI_ShopShowcase> showcase_List = new List<UI_ShopShowcase>();
    public GameObject showcaseContainer;
    private void Start()
    {
        foreach (Transform child in showcaseContainer.transform)
        {
            UI_ShopShowcase showcase = child.gameObject.GetComponent<UI_ShopShowcase>();
            showcase.UpdateDisplay();
        }
        //foreach (UI_ShopShowcase showcase in showcase_List) showcase.UpdateDisplay();
    }
}
