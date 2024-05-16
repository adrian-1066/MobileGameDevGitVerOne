using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControls : MonoBehaviour
{

    public bool isDragActive;
    Vector2 CurrentTouchPos;
    List<GameObject> GridMatch = new List<GameObject>();
    private GameObject CurrentDrag;
    private ObjDrag CurrentDragObj;
    public LayerMask GridMask;
    private int m_LayerMask = 1 << 4;
    public Camera cam;

    
    // Start is called before the first frame update

    private void Awake()
    {
        

       
    }
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
            CurrentTouchPos = Touchscreen.current.position.ReadValue();
            Debug.Log("the current touch pos is: " +  CurrentTouchPos);
            Vector2 WorldPos = cam.ScreenToWorldPoint(CurrentTouchPos);
            Debug.Log("the world pos var is: " +  WorldPos);
            RaycastHit2D Hit = Physics2D.Raycast(CurrentTouchPos, Vector2.zero);
            if(Hit.transform.gameObject.tag == "MatchKind")
            {
                Debug.Log("drag started");
                isDragActive = true;
                CurrentDrag = Hit.transform.gameObject;
                CurrentDragObj = CurrentDrag.GetComponent<ObjDrag>();
                //CurrentDrag.transform.position = CurrentTouchPos;
            }
            Debug.Log("screen has been pressed");
            Debug.Log(Hit.point.ToString());
        }
    }

    public void IsDragging(InputAction.CallbackContext context)
    {
        CurrentTouchPos = Touchscreen.current.position.ReadValue();
        if (isDragActive == true)
        {
            //CurrentDrag.transform.position = CurrentTouchPos;
            CurrentDragObj.TempWorldPos = CurrentTouchPos;
        }
    }


}
