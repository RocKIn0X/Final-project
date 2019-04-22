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

    private MusicManager musicmanager;
    private SFXManager sfxManager;
    private PlaylistManager playlistManager;
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
        musicmanager = this.GetComponent<MusicManager>();
        sfxManager = this.GetComponent<SFXManager>();
        playlistManager = this.GetComponent<PlaylistManager>();
        musicmanager.PlayFromLibrary("Background");
    }


}
