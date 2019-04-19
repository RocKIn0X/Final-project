using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] CanvasGroup UI_CanvasGroup;
    [SerializeField] TextMeshProUGUI profileName_Text;
    [SerializeField] TextMeshProUGUI touch_Text;
    [SerializeField] TMP_InputField newProfileName_Input;
    [SerializeField] CanvasGroup newProfile_CanvasGroup;

    private void Start()
    {
        SetStartUI();
    }

    public void TouchButton()
    {
        if (!SceneLoadingManager.Instance.onLoad)
        {
            if (DataManager.Instance.playerData_dic.Count > 0) SceneLoadingManager.Instance.LoadScene("Scenes/GameScene");
            else SetActiveNewProfilePopup(isOn: true);
        }
    }

    public void CreateNewProfile()
    {
        if (newProfileName_Input.text != "" && !Regex.IsMatch(newProfileName_Input.text, @"^[a-zA-Z0-9]*$")) return;

        DataManager.Instance.current_playerData = new PlayerData();
        DataManager.Instance.current_playerData.playerName = newProfileName_Input.text;
        DataManager.Instance.SaveData();
        DataManager.Instance.LoadData();
        SetStartUI();
        SetActiveNewProfilePopup(isOn: false);
        SetActiveUI(isOn: true);
    }

    private void SetActiveNewProfilePopup(bool isOn)
    {
        SetActiveUI(isOn: false);
        newProfile_CanvasGroup.alpha = isOn ? 1 : 0;
        newProfile_CanvasGroup.blocksRaycasts = isOn;
        newProfile_CanvasGroup.interactable = isOn;
    }
    private void SetActiveUI(bool isOn)
    {
        UI_CanvasGroup.alpha = isOn ? 1 : 0;
        UI_CanvasGroup.blocksRaycasts = isOn;
        UI_CanvasGroup.interactable = isOn;
    }

    private void SetStartUI()
    {
        touch_Text.text = DataManager.Instance.playerData_dic.Count > 0 ? "Touch To Start" : "Create new profile";
        if (DataManager.Instance.playerData_dic.Count > 0) profileName_Text.text = PlayerPrefs.GetString("RecentPlayer");
    }
}
