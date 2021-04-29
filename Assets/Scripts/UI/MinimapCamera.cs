using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform player;
    private float miniCameraX;
    private float miniCameraY;
    public Camera minimapC;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float cameraHalfWidth = minimapC.orthographicSize;
        float cameraHalfHeight = minimapC.orthographicSize;
        //Debug.Log(cameraHalfWidth + ", " + cameraHalfHeight);
        if(player != null)
        {
            miniCameraX = Mathf.Clamp(player.position.x, cameraHalfWidth, 100 - cameraHalfWidth);
            miniCameraY = Mathf.Clamp(player.position.y, cameraHalfHeight, 100 - cameraHalfHeight);
            transform.position = new Vector3(miniCameraX, miniCameraY, transform.position.z);
        }
    }
}
