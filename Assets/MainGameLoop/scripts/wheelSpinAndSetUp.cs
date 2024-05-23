using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        UpdateButtonVisibility();
        SetWheelToLevel(manager.highestLevelReached);
        
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
            
            wheelUpdateComp.gameManager = manager;
            wheelUpdateComp.GameBoard = GameBaord;
            wheelUpdateComp.WheelScene = WheelScene;
            wheelUpdateComp.UpdateText(i);

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

    public void SetWheelToLevel(int level)
    {
        Debug.Log("setting wheel to level: " + level);
        List<int> buttonsThatAreActive = new List<int>();
        List<GameObject> ActiveButtonsRef = new List<GameObject>();
        GameObject MostForwardButtonRef = buttons[0];
        int activeButtons = 0;
        float forwardZ = 0;
        int mostForwardButton = 0;
        for (int i = 0; i < numOfButtons; i++)
        {
            if (buttons[i].transform.position.z <= 0)
            {
                activeButtons++;
                buttonsThatAreActive.Add(i);
                ActiveButtonsRef.Add(buttons[i]);
                if (buttons[i].transform.position.z <= forwardZ)
                {
                    mostForwardButton = i;
                    forwardZ = buttons[i].transform.position.z;
                    MostForwardButtonRef = buttons[i].gameObject;
                }
            }
        }
        foreach(GameObject go in ActiveButtonsRef)
        {
            ActiveButtonsRef = ActiveButtonsRef.OrderBy(go => go.transform.position.y).ToList();
        }

        //Debug.Log("there are: " + ActiveButtonsRef.Count + " in the active buttons");
       

        for(int i = 0; i < ActiveButtonsRef.Count; i++)
        {
            if (ActiveButtonsRef[i] ==  MostForwardButtonRef)
            {
                mostForwardButton = i;
                break;
            }
        }
        Debug.Log("the most forward button is: " + MostForwardButtonRef);
        //MostForwardButtonRef.transform.position = Vector3.zero;
        for(int i = mostForwardButton; i < ActiveButtonsRef.Count; i++)
        {
            int diff = i - mostForwardButton;
            Debug.Log("the diff for above is: " +  diff); 
            ActiveButtonsRef[i].GetComponent<wheelButtonUpdate>().UpdateText(level + diff);
        }

        for(int i = mostForwardButton; i >= 0; i--)
        {
            int diff = i - mostForwardButton;
            Debug.Log("the diff for below is: " + diff);
            ActiveButtonsRef[i].GetComponent<wheelButtonUpdate>().UpdateText(level + diff);
        }
        Debug.Log("the forward button is: " + MostForwardButtonRef.GetComponent<wheelButtonUpdate>().valueOnText.ToString());





       
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

        for(int i = 0; i < numOfButtons; i++)
        {
            wheelButtonUpdate tempWheelStat = buttons[i].GetComponent<wheelButtonUpdate>();
            tempWheelStat.UpdateText(tempWheelStat.valueOnText);
        }

        

     
    }

    public void TouchLoc(InputAction.CallbackContext context)
    {
        if(!gameObject.activeSelf)
        {
            Debug.Log("gameboj not active going back now, bye bye");
            return;
        }

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

        if (!gameObject.activeSelf)
        {
            return;
        }
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

        if (!gameObject.activeSelf)
        {
            return;
        }
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

            if(button.GetComponent<wheelButtonUpdate>().valueOnText <= 0)
            {
                button.SetActive(false);
            }

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

