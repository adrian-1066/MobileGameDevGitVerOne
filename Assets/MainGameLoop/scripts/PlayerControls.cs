using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckScreenTopPos(InputAction.CallbackContext context)
    {
        if (context.performed == true)
        {
            
            Debug.Log("screen has been pressed");
        }
    }
}
