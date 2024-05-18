using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GridInputHandler : MonoBehaviour
{
    private InnitBoardSetUp gameBoard;
    private Vector2 initialTouchPosition;
    private Vector2 currentTouchPosition;
    private GameObject selectedItem;
    private bool isDragging;

    [SerializeField]
    private float CurrentDragDist;
    [SerializeField]
    private float MaxDragDist;
    [SerializeField]
    private int NumOfItemsInMatch = 0;

    private List<List<Vector2Int>> MatchsMade = new List<List<Vector2Int>>();
    private List<Vector2Int> ListOfItemsToPop = new List<Vector2Int>();

    void Awake()
    {
        gameBoard = GetComponent<InnitBoardSetUp>();
    }


    private void Start()
    {
        CurrentDragDist = 0;
        MaxDragDist = gameBoard.GetPrefabDims();
        MatchsMade.Clear();
        MatchsMade.Add(new List<Vector2Int>());
        MatchsMade.Add(new List<Vector2Int>());
        Debug.Log("matchs made num is: " + MatchsMade.Count);
    }
    public void OnTouchStart(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            initialTouchPosition = Touchscreen.current.position.ReadValue(); // context.ReadValue<Vector2>();
            //Debug.Log("getting item");
            selectedItem = GetItemAtPosition(initialTouchPosition);
            if (selectedItem != null)
            {
                //Debug.Log("touch started");
                selectedItem.GetComponent<Image>().color = Color.green;
                isDragging = true;
            }
        }
    }

    public void OnTouchMove(InputAction.CallbackContext context)
    {
        if (isDragging)
        {
            //Debug.Log("moving square");
            currentTouchPosition = Touchscreen.current.position.ReadValue(); // context.ReadValue<Vector2>();
            Vector2 direction = currentTouchPosition - initialTouchPosition;
            //Debug.Log("the dir  is: " + direction);
            float dist = Vector2.Distance(initialTouchPosition, currentTouchPosition);
            initialTouchPosition = currentTouchPosition;
            CurrentDragDist += dist;
            //Debug.Log("dist is: " + dist);
            if (CurrentDragDist >= MaxDragDist)
            {
                //Debug.Log("CurrentDragdist is: " +  CurrentDragDist);   
                CurrentDragDist = 0;
                // Move items here based on direction
                MoveItems(selectedItem, direction);
            }
        }
    }

    public void OnTouchEnd(InputAction.CallbackContext context)
    {
        if (context.canceled && isDragging)
        {
            // Logic to check for matches and reset position if no match
            if (!CheckForMatch(selectedItem))
            {
                ResetPosition(selectedItem);
            }
            isDragging = false;
        }
    }

    GameObject GetItemAtPosition(Vector2 position)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(position);
        //Debug.Log("the world pos being checked is: " + position);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null && hit.transform.CompareTag("MatchKind"))
        {
            //Debug.Log("Drag started");
            return hit.transform.gameObject;
        }
        return null;
    }

    void MoveItems(GameObject item, Vector2 direction)
    {
        ItemStats stats = item.GetComponent<ItemStats>();
        int x = stats.CurrentGridPos.x;
        int y = stats.CurrentGridPos.y;
        //Debug.Log("the dir is: " + direction);
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal move
            //Debug.Log("moving on x");
            int targetX = x + (direction.x > 0 ? 1 : -1);
            if (targetX >= 0 && targetX < gameBoard.width)
            {
                SwapItems(x, y, targetX, y);
                if (!CheckForMatch(item))
                {
                    StartCoroutine(timer(item));
                }
            }
        }
        else if(Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            // Vertical move
            //Debug.Log("moving on y");
            int targetY = y + (direction.y > 0 ? 1 : -1);
            if (targetY >= 0 && targetY < gameBoard.height)
            {
                SwapItems(x, y, x, targetY);
                if(!CheckForMatch(item))
                {
                    StartCoroutine(timer(item));
                    
                }
            }
        }
        else
        {
            Debug.Log("get fucked");
        }
    }

    void SwapItems(int x1, int y1, int x2, int y2)
    {
        GameObject temp = gameBoard.grid[x1, y1];
        gameBoard.grid[x1, y1] = gameBoard.grid[x2, y2];
        gameBoard.grid[x2, y2] = temp;

        gameBoard.grid[x1, y1].GetComponent<ItemStats>().CurrentGridPos = new Vector2Int(x1, y1);
        gameBoard.grid[x2, y2].GetComponent<ItemStats>().CurrentGridPos = new Vector2Int(x2, y2);

        Vector3 tempPosition = gameBoard.grid[x1, y1].transform.position;
        gameBoard.grid[x1, y1].transform.position = gameBoard.grid[x2, y2].transform.position;
        gameBoard.grid[x2, y2].transform.position = tempPosition;

       // Debug.Log("Items swapped");
    }

    bool CheckForMatch(GameObject item)
    {
        for(int x = 0; x < gameBoard.width; x++)
        {
            for(int y = 0; y <  gameBoard.height; y++)
            {
                for(int i = 0; i < 2; i++)
                {
                    switch(i)
                    {
                        case 0:
                            if (!CheckAdjacentSpot(item.GetComponent<ItemStats>().CurrentGridPos + Vector2Int.up, Vector2Int.up, gameBoard.grid[x, y].GetComponent<ItemStats>().type,0) )
                            {
                                MatchsMade[0].Clear();
                            }

                        break;

                        case 1:
                            if (!CheckAdjacentSpot(item.GetComponent<ItemStats>().CurrentGridPos + Vector2Int.right, Vector2Int.right, gameBoard.grid[x, y].GetComponent<ItemStats>().type,1))
                            {
                                MatchsMade[1].Clear();
                            }

                            break;
                    }

                    if (MatchsMade[0].Count > 0)
                    {
                        //matchmade

                        ListOfItemsToPop.AddRange(MatchsMade[0]);
                        MatchsMade[0].Clear();
                    }
                    if (MatchsMade[1].Count > 0)
                    {
                        ListOfItemsToPop.AddRange(MatchsMade[1]);
                        MatchsMade[1].Clear();
                    }
                }
            }
        }
        // Check if there's a match after moving
        return false; // Placeholder
    }

    bool CheckAdjacentSpot(Vector2Int Spot,Vector2Int dir, int type, int matchDir)
    {
        //this aint right 
        if (gameBoard.grid[Spot.x + dir.x, Spot.y + dir.y].GetComponent<ItemStats>().type == type)
        {
            MatchsMade[matchDir].Add(Spot);
            CheckAdjacentSpot(Spot + dir, dir, type, matchDir);
        }

        return false;
    }

    void ResetPosition(GameObject item)
    {
        
        // Reset the item to its original position
        ItemStats stats = item.GetComponent<ItemStats>();
       
        Vector2Int CurPos = stats.CurrentGridPos;
        Vector2Int PrePos = stats.GridStartPos;
        Vector2 dir =  PrePos - CurPos;
        if (CurPos != PrePos)
        {
            MoveItems(item, dir);
        }

        //item.transform.position = new Vector3(x, y, 0);
    }

    IEnumerator timer(GameObject item)
    {
        Debug.Log("reset timer started");
        yield return new WaitForSeconds(1f);
        ResetPosition(item);
        Debug.Log("reset timer done");
    }


    /*
    private InnitBoardSetUp gameBoard;
    private Vector2 initialTouchPosition;
    private Vector2 currentTouchPosition;
    private GameObject selectedItem;
    private bool isDragging;

    void Awake()
    {
        gameBoard = GetComponent<InnitBoardSetUp>();
    }

    public void OnTouchStart(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            initialTouchPosition = Touchscreen.current.position.ReadValue();//context.ReadValue<Vector2>();
            selectedItem = GetItemAtPosition(initialTouchPosition);
            selectedItem.GetComponent<Image>().color = Color.green;
            isDragging = selectedItem != null;
        }
    }

    public void OnTouchMove(InputAction.CallbackContext context)
    {
        if (isDragging)
        {
            currentTouchPosition = Touchscreen.current.position.ReadValue();//context.ReadValue<Vector2>();
            Vector2 direction = currentTouchPosition - initialTouchPosition;
            initialTouchPosition = currentTouchPosition;
            
            // Move items here based on direction
            
            MoveItems(selectedItem, direction);

        }
    }

    public void OnTouchEnd(InputAction.CallbackContext context)
    {
        if (context.canceled && isDragging)
        {
            // Logic to check for matches and reset position if no match
            if (!CheckForMatch())
            {
                ResetPosition(selectedItem);
            }
            isDragging = false;
        }
    }

    GameObject GetItemAtPosition(Vector2 position)
    {
        
        RaycastHit2D Hit = Physics2D.Raycast(position, Vector2.zero);
        if (Hit.transform.gameObject.tag == "MatchKind")
        {
            Debug.Log("drag started");
            return Hit.transform.gameObject;
            
        }

        
        return null;
    }

    void MoveItems(GameObject item, Vector2 direction)
    {
        ItemStats stats = item.GetComponent<ItemStats>();
        int x = stats.CurrentGridPos.x;//Mathf.RoundToInt(item.transform.position.x);
        int y = stats.CurrentGridPos.y;
        // Determine the direction of movement
        if (direction.x > direction.y)//(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal move
            
            //int x = stats.CurrentGridPos.x;//Mathf.RoundToInt(item.transform.position.x);
            //int y = stats.CurrentGridPos.y;//Mathf.RoundToInt(item.transform.position.y);

            int targetX = x + (direction.x > 0 ? 1 : -1);

            if (targetX >= 0 && targetX < gameBoard.width)
            {
                SwapItems(x, y, targetX, y);
            }
        }
        else
        {
            // Vertical move
            //int x = Mathf.RoundToInt(item.transform.position.x);
            //int y = Mathf.RoundToInt(item.transform.position.y);

            int targetY = y + (direction.y > 0 ? 1 : -1);

            if (targetY >= 0 && targetY < gameBoard.height)
            {
                SwapItems(x, y, x, targetY);
            }
        }
    }

    void SwapItems(int x1, int y1, int x2, int y2)
    {
        GameObject temp = gameBoard.grid[x1, y1];
        gameBoard.grid[x1, y1] = gameBoard.grid[x2, y2];
        gameBoard.grid[x2, y2] = temp;

        gameBoard.grid[x1, y1].GetComponent<ItemStats>().CurrentGridPos = new Vector2Int(x1, y1);
        gameBoard.grid[x2, y2].GetComponent<ItemStats>().CurrentGridPos = new Vector2Int(x2, y2);

        Vector3 tempPosition = gameBoard.grid[x1, y1].transform.position;
        gameBoard.grid[x1, y1].transform.position = gameBoard.grid[x2, y2].transform.position;
        gameBoard.grid[x2, y2].transform.position = tempPosition;

        Debug.Log("items Swapped");


    }

    bool CheckForMatch()
    {
        // Check if there's a match after moving
        return false; // Placeholder
    }

    void ResetPosition(GameObject item)
    {
        // Reset the item to its original position
    }*/
}
