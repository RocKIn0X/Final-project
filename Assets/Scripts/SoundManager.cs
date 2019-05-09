using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SoundManager>();
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<SoundManager>();
                }
            }
            return instance;
        }
    }

    public MusicManager musicmanager;
    public SFXManager sfxManager;
    private void Awake()
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

    private void Start()
    {
        musicmanager = this.GetComponent<MusicManager>();
        sfxManager = this.GetComponent<SFXManager>();
        musicmanager.PlayFromLibrary("Background");
    }
}
