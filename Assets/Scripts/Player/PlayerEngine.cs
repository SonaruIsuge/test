using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerEngine : MonoBehaviour
{
    [SerializeField]private TankProperty property;
    private PlayerInput playerInput;

   private void Awake() 
   {
       playerInput = GetComponent<PlayerInput>();
   }

    // Update is called once per frame
    void Update()
    {
        transform.position += playerInput.Vertical * transform.up * property.MoveSpeed * Time.deltaTime;
        transform.Rotate(-Vector3.forward * playerInput.Horizontal * property.RotateSpeed * Time.deltaTime);
    }
}
