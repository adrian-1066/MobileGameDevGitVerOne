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

    private GameObject[] Buttons;
    // Start is called before the first frame update
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
            Vector3 otherTempRot = new Vector3(otherAng , 0, 0);
            temp.transform.position = newPos;
            temp.transform.transform.eulerAngles = otherTempRot;
            Buttons[i] = temp;
            temp.GetComponent<wheelButtonUpdate>().UpdateText(i);
            //temp.transform.rotation = newRot;
        }
    }

    private void checkThenUpdateWheel()
    {

        //float currentXRotation = WheelPanal.transform.localRotation.eulerAngles.x;
        //float deltaRotation = Mathf.DeltaAngle(previousXRotation, currentXRotation); 
        //totalXRotation += deltaRotation;
        //previousXRotation = currentXRotation;

        float wheelSetCheckTemp = (totalXRotation / 360);
        //Debug.Log("the total x rot is " + totalXRotation);
        //Debug.Log("total x rot / 360 is " + wheelSetCheckTemp);
       
        int wheelCheck = Mathf.FloorToInt(wheelSetCheckTemp);
        //Debug.Log("num of rots is " + wheelCheck);
        if (wheelCheck != wheelSet)
        {
            wheelSet = wheelCheck;
            for(int i = 0; i < numOfButtons; i++)
            {
                Buttons[i].GetComponent<wheelButtonUpdate>().UpdateText(i + (wheelCheck*numOfButtons));
            }
        }
    }

    public void AddRotation(InputAction.CallbackContext context)
    {
        totalXRotation += 30;
        previousXRotation = totalXRotation;
        WheelPanal.transform.Rotate(new Vector3(totalXRotation, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        checkThenUpdateWheel();
    }
}
