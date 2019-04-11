using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private TextMeshProUGUI weekCountText;
    private float timeCounter = 0f;
    private float timeToStartGame = 1f;

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
        InvokeRepeating("CallSecondEvent", timeToStartGame, second);
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;
        if ((timeCounter / secondInAWeek) > weekCount)
        {
            UpdateWeekCount();
        }
    }

    private void UpdateWeekCount ()
    {
        weekCount++;
        weekCountText.text = "WEEK " + weekCount.ToString();
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
}
