using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectRatioController : MonoBehaviour
{
    public float width;
    public float height;

    // Start is called before the first frame update
    void Start()
    {
        float desiredAspect = width / height;
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = screenAspect / desiredAspect;

        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f) // Add letter box
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else // Add pillar box
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f; ;
            rect.y = 0;
            camera.rect = rect;
        }
    }
}
