using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestPauseGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Pause game");
            Time.timeScale = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Resume game");
            Time.timeScale = 1f;
        }
    }

    IEnumerator TestPauseGame ()
    {
        Debug.Log("Start test");
        yield return new WaitForSeconds(5f);
        Debug.Log("End test");
    }
}
