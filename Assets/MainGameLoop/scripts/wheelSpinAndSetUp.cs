using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WheelSpinAndSetUp : MonoBehaviour
{
    public GameObject WheelPanel;
    public GameObject Canvas;
    public GameObject LevelButtonPrefab;
    public GameObject wheelCentre;
    public int numOfButtons;
    public int radius;

    private float totalXRotation = 0;
    private Vector2 previousTouchLocation;

    private GameObject[] buttons;

    [SerializeField]
    private float rotationSpeed = 1.0f;

    bool isDragging;

    public GameManager manager;
    public GameObject GameBaord;
    public GameObject WheelScene;

    void Start()
    {
        buttons = new GameObject[numOfButtons];
        isDragging = false;
        SpawnPrefabs();
    }

    private void SpawnPrefabs()
    {
        float tempCheck = (wheelCentre.transform.rotation.x / 360);
        //wheelSet = Mathf.FloorToInt(tempCheck);
        Vector3 PrefabRotation = new Vector3(0.0f, 0.0f, 0.0f);
        for (int i = 0; i < numOfButtons; i++)
        {
            float angle = (i + 1) * Mathf.PI * 2 / numOfButtons;
            float otherAng = (360 / numOfButtons) * (i + 1);
            otherAng += 90;
            Debug.Log(otherAng);
            Vector3 newPos = new Vector3(0, Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            newPos.x = Canvas.transform.position.x;
            newPos.y += Canvas.transform.position.y;
            newPos.z -= 200;

            GameObject temp = Instantiate(LevelButtonPrefab, wheelCentre.transform);
            Quaternion newRot = new Quaternion(otherAng, 0, 0, wheelCentre.transform.rotation.w);
            Vector3 otherTempRot = new Vector3(otherAng, 0, 0);
            temp.transform.position = newPos;
            temp.transform.transform.eulerAngles = otherTempRot;
            buttons[i] = temp;
            wheelButtonUpdate wheelUpdateComp = temp.GetComponent<wheelButtonUpdate>();
            wheelUpdateComp.UpdateText(i);
            wheelUpdateComp.gameManager = manager;
            wheelUpdateComp.GameBoard = GameBaord;
            wheelUpdateComp.WheelScene = WheelScene;

            //Debug.Log("the y pos of button " + i + " is " + (temp.transform.position.y - Canvas.transform.position.y)); 
        }
        /*
        for (int i = 0; i < numOfButtons; i++)
        {
            float angle = i * Mathf.PI * 2 / numOfButtons;
            Vector3 newPos = new Vector3(0, Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            newPos.x = Canvas.transform.position.x;
            newPos.y += Canvas.transform.position.y;
            newPos.z -= 200;

            GameObject temp = Instantiate(LevelButtonPrefab, wheelCentre.transform);
            temp.transform.position = newPos;
            temp.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            buttons[i] = temp;
            temp.GetComponent<wheelButtonUpdate>().UpdateText(i);
        }*/
    }

    private void UpdateWheel()
    {
        float degreesPerButton = 360f / numOfButtons;
        float wheelRotationInButtons = totalXRotation / degreesPerButton;
        int baseButtonIndex = Mathf.FloorToInt(wheelRotationInButtons);
        List<int> buttonsThatAreActive = new List<int>();
        int activeButtons = 0;
        for (int i = 0; i < numOfButtons; i++)
        {
            if (buttons[i].activeSelf)
            {
                activeButtons++;
                buttonsThatAreActive.Add(i);
            }


            /*if (!buttons[i].activeSelf)
            {


                int levelNumber = baseButtonIndex + i;
                buttons[i].GetComponent<wheelButtonUpdate>().UpdateText(levelNumber);
            }*/
        }

        for(int i = 0; i < buttonsThatAreActive.Count; i++)
        {
            int buttonToCheck = buttonsThatAreActive[i] + 1;
            int lowerToCheck = buttonsThatAreActive[i] - 1;
            int numVal = buttons[buttonsThatAreActive[i]].GetComponent<wheelButtonUpdate>().valueOnText;
            if(buttonToCheck >= numOfButtons)
            {
                buttonToCheck = 0;
            }

            if(lowerToCheck < 0) 
            {
                lowerToCheck = numOfButtons - 1;
            }

            if (!buttons[buttonToCheck].activeSelf)
            {

                buttons[buttonToCheck].GetComponent<wheelButtonUpdate>().UpdateText(numVal + 1);
            }
            if (!buttons[lowerToCheck].activeSelf)
            {
                buttons[lowerToCheck    ].GetComponent<wheelButtonUpdate>().UpdateText(numVal - 1);
            }

        }

        

        /*
        float wheelRotationInRevolutions = totalXRotation / 360f;
        int wheelSet = Mathf.FloorToInt(wheelRotationInRevolutions);

        for (int i = 0; i < numOfButtons; i++)
        {
            int levelNumber = i + (wheelSet * numOfButtons);
            buttons[i].GetComponent<wheelButtonUpdate>().UpdateText(levelNumber);
        }*/
    }

    public void TouchLoc(InputAction.CallbackContext context)
    {
        if (isDragging)
        {
            //Debug.Log("is touching wheel my guy");
            Vector2 currentTouchLocation = Touchscreen.current.position.ReadValue();
            float differenceY = currentTouchLocation.y - previousTouchLocation.y;
            previousTouchLocation = currentTouchLocation;

            totalXRotation -= differenceY;
            wheelCentre.transform.Rotate(new Vector3(differenceY, 0, 0));
            previousTouchLocation = currentTouchLocation;
            UpdateWheel();
        }
    }

    public void TouchStarted(InputAction.CallbackContext context)
    {
        if (Touchscreen.current.primaryTouch.press.isPressed == true)
        {
            if (!isDragging)
            {
                isDragging = true;
                previousTouchLocation = Touchscreen.current.position.ReadValue();
            }
        }
    }

    public void OnTouchEnd(InputAction.CallbackContext context)
    {
        if (Touchscreen.current.primaryTouch.press.isPressed == false)
        {
            if (isDragging)
            {
                isDragging = false;

            }
        }
    }

    private void UpdateButtonVisibility()
    {
        foreach (var button in buttons)
        {
            float buttonZ = button.transform.position.z;
            button.SetActive(buttonZ <= 0);

            float buttonOpacity = Mathf.Clamp01(1 - Mathf.Abs(button.transform.position.y - Canvas.transform.position.y) / radius);
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                Color buttonColor = image.color;
                image.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, buttonOpacity);
            }
        }
    }

    void Update()
    {
        UpdateButtonVisibility();
    }
}

/*using System.Collections;
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
    public float wheelSet;

    private float totalXRotation = 0;
    private float previousXRotation = 0;

    private Vector2 PreviousTouchLocation;
    private Vector2 CurrentTouchLocation;

    private GameObject[] Buttons;

    [SerializeField]
    private float rotationSpeed;
  
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
        float wheelCheck = Mathf.Lerp(wheelSet, wheelSetCheckTemp, Time.deltaTime * rotationSpeed);

        if (Mathf.Abs(wheelCheck - wheelSet) > 0.01f)
        {
            wheelSet = wheelCheck;
            for (int i = 0; i < numOfButtons; i++)
            {
                Buttons[i].GetComponent<wheelButtonUpdate>().UpdateText(i + (Mathf.FloorToInt(wheelCheck) * numOfButtons));
            }
        }
        
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
        Debug.Log("wheel touch loc used");
        float previousY = PreviousTouchLocation.y;
        float currentY = Touchscreen.current.position.ReadValue().y;
        float Difference = currentY - previousY;
        //Debug.Log("the touch screen difference is " + Difference);
        CurrentTouchLocation = Touchscreen.current.position.ReadValue();
        PreviousTouchLocation = CurrentTouchLocation;
        totalXRotation -= Difference;
        previousXRotation = totalXRotation;
        WheelPanal.transform.Rotate(new Vector3(Difference, 0, 0));
        //checkThenUpdateWheel();
    }

    public void TouchStarted(InputAction.CallbackContext context)
    {
        Debug.Log("wheel TouchStarted");
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
}*/
