using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }

        private set
        {
            if (instance != null && instance != value)
            {
                Destroy(instance.gameObject);
            }
            instance = value;
        }
    }
    #endregion

    public delegate void SecondAction();
    public static event SecondAction SecondEvent;

    private float second = 1f;
    private float secondInAWeek = 10f;

    public int weekCount = 0;
    [SerializeField] TextMeshProUGUI weekCountText;
    [SerializeField] RectTransform dotCountRect;
    private UI_TextNotif notifManager;
    private UI_ShopPanel uiShopPanel;
    private int dotCount = 0;
    private float timeCounter = 0f;
    private float timeToStartGame = 1f;

    public static bool isGameRunning;

    private void OnEnable()
    {
        SecondEvent += UpdateDotCount;
    }

    private void OnDisable()
    {
        SecondEvent -= UpdateDotCount;
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

    void Start ()
    {
        isGameRunning = true;
        InvokeRepeating("CallSecondEvent", timeToStartGame, second);
    }

    private void Update()
    {
        if (isGameRunning)
        {
            timeCounter += Time.deltaTime;
            if ((timeCounter / secondInAWeek) > weekCount)
            {
                UpdateWeekCount();
            }
        }
    }

    private void UpdateWeekCount()
    {
        weekCount++;
        weekCountText.text = weekCount.ToString();

        if (notifManager == null)
            notifManager = (UI_TextNotif)FindObjectOfType(typeof(UI_TextNotif));
        notifManager.WeekTicks();
        if (uiShopPanel == null)
            uiShopPanel = (UI_ShopPanel)FindObjectOfType(typeof(UI_ShopPanel));
        uiShopPanel.WeekTicks();
    }

    private void UpdateDotCount()
    {
        dotCount++;
        float posY = 80 - ((dotCount % 10) * 10) >= -10 ? 80 - ((dotCount % 10) * 10) : 80;
        dotCountRect.anchoredPosition = new Vector3(0, posY, 0);
    }

    private void CallSecondEvent ()
    {
        if (SecondEvent != null)
        {
            SecondEvent();
        }
        else
        {
            Debug.Log("No func in this event");
        }
    }

    public void PauseGame ()
    {
        isGameRunning = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame ()
    {
        isGameRunning = true;
        Time.timeScale = 1f;
    }

    public void PauseButton()
    {
        if (isGameRunning) PauseGame();
        else ResumeGame();
    }
}
