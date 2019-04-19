using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI profileName_Text;
    [SerializeField] TextMeshProUGUI touch_Text;

    private void Start()
    {
        touch_Text.text = DataManager.Instance.filePaths.Capacity > 0 ? "Touch To Start" : "Create new profile";
        if (DataManager.Instance.filePaths.Capacity > 0)
            profileName_Text.text = DataManager.Instance.filePaths.Contains(PlayerPrefs.GetString("RecentPlayer")) ? PlayerPrefs.GetString("RecentPlayer") : DataManager.Instance.filePaths[0];
    }

    public void TouchButton()
    {
        if(!SceneLoadingManager.Instance.onLoad) SceneLoadingManager.Instance.LoadScene("Scenes/GameScene");
    }
}
