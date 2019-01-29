using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleScreenByCamera : MonoBehaviour
{
    public float orthographicSize = 5;
    public float width;
    public float height;

    [SerializeField]
    private float aspect;

    void Start()
    {
        aspect = width / height;

        Camera.main.projectionMatrix = Matrix4x4.Ortho(
                -orthographicSize * aspect, orthographicSize * aspect,
                -orthographicSize, orthographicSize,
                GetComponent<Camera>().nearClipPlane, GetComponent<Camera>().farClipPlane);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
