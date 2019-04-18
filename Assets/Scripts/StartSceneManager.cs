using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI profileName_text;
    [SerializeField] TextMeshProUGUI touch_Text;

    private void Start()
    {
        touch_Text.text = DataManager.Instance.filePaths.Length > 0 ? "Touch To Start" : "Create new profile";
    }

    public void TouchButton()
    {
        if(!LoadingManager.Instance.onLoad) LoadingManager.Instance.LoadScene("Scenes/GameScene");
    }
}
