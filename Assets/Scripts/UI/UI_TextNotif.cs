using Anonym.Isometric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TextNotif : MonoBehaviour
{
    private static bool DEBUG_MODE = true ;
    private static string DEBUG_NAME = "NotifSystem" ;

    private static void _Log(string text)
    {
        if (DEBUG_MODE == true)
            Debug.Log("[" + DEBUG_NAME + "] " + text) ;
    }

    public UI_Notification notifPrefab;
    public int lifetimeInWeeks = 2;

    private int activeCount = 0;
    private int totalCount = 0;

    private UI_Notification FindAvailableNotif()
    {
        foreach (Transform childNotif in this.transform)
        {
            if (childNotif.gameObject.activeSelf == false)
            {
                return childNotif.gameObject.GetComponent<UI_Notification>();
            }
        }
        return null;
    }

    public void Notify(string receive, Sprite icon)
    {
        UI_Notification targetNotif = null;
        if (activeCount >= totalCount)
            targetNotif = FindAvailableNotif();
        if (targetNotif == null)
        {
            _Log("No free notification, Creating new one.");
            targetNotif = (UI_Notification)Instantiate(notifPrefab, this.transform);
            totalCount = totalCount + 1;
        }

        targetNotif.GetComponentInChildren<Text>().text = receive;
        if (icon == null)
            _Log("Got null icon sprite");
        foreach (Transform child in targetNotif.transform)
        {
            if (child.gameObject.GetComponent<Image>() != null)
            {
                child.gameObject.GetComponent<Image>().sprite = icon;
                break;
            }
        }
        targetNotif.gameObject.SetActive(true);
        targetNotif.lifetime = lifetimeInWeeks;
        activeCount = activeCount + 1;
    }

    public void WeekTicks()
    {
        foreach (Transform child in this.transform)
        {
            UI_Notification notif = child.gameObject.GetComponent<UI_Notification>();
            if (notif.gameObject.activeSelf == false)
                continue;
            notif.lifetime = notif.lifetime - 1;
            if (notif.lifetime <= 0)
            {
                notif.gameObject.SetActive(false); // Need to use SetActive for the layout to change. (Probably)
                activeCount = activeCount - 1;
            }
        }
    }
}
