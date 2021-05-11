using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadRotate : MonoBehaviour
{
    public GameObject Head;
    [SerializeField]private TankProperty property;

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        Vector3 direction = worldPos - Head.transform.position;
        direction.z = 0f;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Head.transform.rotation = Quaternion.RotateTowards(Head.transform.rotation, Quaternion.Euler(0, 0, -angle), property.HeadRotSpeed * Time.deltaTime);
    }
}
