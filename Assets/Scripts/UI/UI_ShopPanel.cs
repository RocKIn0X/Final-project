using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopPanel : MonoBehaviour
{
    [SerializeField] List<UI_ShopShowcase> showcase_List = new List<UI_ShopShowcase>();

    private void Start()
    {
        foreach (UI_ShopShowcase showcase in showcase_List) showcase.UpdateDisplay();
    }
}
