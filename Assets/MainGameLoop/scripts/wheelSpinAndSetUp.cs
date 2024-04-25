using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class wheelSpinAndSetUp : MonoBehaviour
{
    public GameObject WheelPanal;
    public GameObject Canvas;

    public GameObject LevelButtonPrefab;

    public int numOfButtons;
    public int radius;
    public int wheelSet;

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
        float ahhhh = Mathf.Rad2Deg * WheelPanal.transform.rotation.x;
        Debug.Log(ahhhh);
        float wheelSetCheckTemp = (ahhhh / 360);
        Debug.Log(wheelSetCheckTemp);
        int wheelCheck = Mathf.FloorToInt(wheelSetCheckTemp);
        Debug.Log(wheelCheck);
        if (wheelCheck != wheelSet)
        {
            wheelSet = wheelCheck;
            for(int i = 0; i < numOfButtons; i++)
            {
                Buttons[i].GetComponent<wheelButtonUpdate>().UpdateText(i + (wheelCheck*numOfButtons));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkThenUpdateWheel();
    }
}
