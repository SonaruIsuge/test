using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player;
    private float cameraX;
    private float cameraY;
    [SerializeField]private Camera mc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float cameraHalfWidth = mc.orthographicSize * ((float)Screen.width / Screen.height);
        float cameraHalfHeight = mc.orthographicSize;
        if(player != null)
        {
            cameraX = Mathf.Clamp(player.position.x, cameraHalfWidth, 100 - cameraHalfWidth);
            cameraY = Mathf.Clamp(player.position.y, cameraHalfHeight, 100 - cameraHalfHeight);
            transform.position = new Vector3(cameraX, cameraY, transform.position.z);
        }
    }
}
