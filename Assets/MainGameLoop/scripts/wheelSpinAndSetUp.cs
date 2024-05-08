using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class wheelSpinAndSetUp : MonoBehaviour
{
    public GameObject WheelPanal;
    public GameObject Canvas;

    public GameObject LevelButtonPrefab;

    public int numOfButtons;
    public int radius;
    public int wheelSet;

    private float totalXRotation = 0;
    private float previousXRotation = 0;

    private Vector2 PreviousTouchLocation;
    private Vector2 CurrentTouchLocation;

    private GameObject[] Buttons;
  
    void Start()
    {
        Buttons = new GameObject[numOfButtons];
        SpawnPrefabs();
        
    }

    private void SpawnPrefabs()
    {
        float tempCheck = (WheelPanal.transform.rotation.x / 360);
        wheelSet = Mathf.FloorToInt(tempCheck);
        Vector3 PrefabRotation = new Vector3(0.0f, 0.0f, 0.0f);
        for(int i = 0; i < numOfButtons; i++)
        {
            float angle = (i+1) * Mathf.PI * 2 / numOfButtons;
            float otherAng = (360 / numOfButtons) * (i+1);
            otherAng += 90;
            Debug.Log(otherAng);
            Vector3 newPos = new Vector3(0, Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            newPos.x = Canvas.transform.position.x;
            newPos.y += Canvas.transform.position.y;
            newPos.z -= 200;

            GameObject temp = Instantiate(LevelButtonPrefab, WheelPanal.transform);
            Quaternion newRot = new Quaternion(otherAng , 0, 0, WheelPanal.transform.rotation.w);
            Vector3 otherTempRot = new Vector3(otherAng, 0, 0);
            temp.transform.position = newPos;
            temp.transform.transform.eulerAngles = otherTempRot;
            Buttons[i] = temp;
            temp.GetComponent<wheelButtonUpdate>().UpdateText(i);

            //Debug.Log("the y pos of button " + i + " is " + (temp.transform.position.y - Canvas.transform.position.y)); 
        }
    }

    private void checkThenUpdateWheel()
    {

       
        float wheelSetCheckTemp = (totalXRotation / 360);
     
        int wheelCheck = Mathf.FloorToInt(wheelSetCheckTemp);
        
        if (wheelCheck != wheelSet)
        {
            wheelSet = wheelCheck;
            for(int i = 0; i < numOfButtons; i++)
            {
                Buttons[i].GetComponent<wheelButtonUpdate>().UpdateText(i + (wheelCheck*numOfButtons));
            }
        }
    }



    public void TouchLoc(InputAction.CallbackContext context)
    {

        if (context.canceled)
        {
            //Debug.LogWarning("the duration of touch was: " + context.duration);
        }

        if (context.started)
        {
            //PreviousTouchLocation = Touchscreen.current.position.ReadValue();
            //Debug.LogWarning("touch started");
            //return;
        }

        
       

        
        //Touchscreen.current.primaryTouch.

        //Debug.Log( Touchscreen.current.position.ReadValue());
        //Debug.Log("touch doing stuff");
        float previousY = PreviousTouchLocation.y;
        float currentY = Touchscreen.current.position.ReadValue().y;
        float Difference = currentY - previousY;
        //Debug.Log("the touch screen difference is " + Difference);
        CurrentTouchLocation = Touchscreen.current.position.ReadValue();
        PreviousTouchLocation = CurrentTouchLocation;
        totalXRotation -= Difference;
        previousXRotation = totalXRotation;
        WheelPanal.transform.Rotate(new Vector3(Difference, 0, 0));

        


    }

    public void TouchStarted(InputAction.CallbackContext context)
    {
        PreviousTouchLocation = Touchscreen.current.position.ReadValue();



    }

    void UpdateButton()
    {
        for(int i = 0; i < Buttons.Length; i++)
        {
            float ButtonY = Buttons[i].transform.position.y;
            float ButtonZ = Buttons[i].transform.position.z;
            Color ButtonCol = Buttons[i].GetComponent<Image>().color;
            if (ButtonZ > 0)
            {
                Buttons[i].gameObject.SetActive(false);
                
            }

            if(ButtonZ <= 0)
            {
                Buttons[i].gameObject.SetActive(true);
               
            }

            float ButtonOpa = Buttons[i].transform.position.y - Canvas.transform.position.y;
           
            ButtonOpa /= radius;

            if(ButtonOpa < 0)
            {
                ButtonOpa = 0 - ButtonOpa; 
            } 
            ButtonOpa = 1 - ButtonOpa;
            Buttons[i].GetComponent<Image>().color = new Color(ButtonCol.r, ButtonCol.g, ButtonCol.b, ButtonOpa);
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkThenUpdateWheel();
        UpdateButton();
    }
}
