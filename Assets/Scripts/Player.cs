using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    static Player instance;
    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<Player>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
