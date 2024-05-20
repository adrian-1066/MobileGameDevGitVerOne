using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GridInputHandler : MonoBehaviour
{
    public InnitBoardSetUp gameBoard;
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

    private List<List<GameObject>> MatchsMade = new List<List<GameObject>>();
    private List<Vector2Int> ListOfItemsToPop = new List<Vector2Int>();
    public List<GameObject> ListOfItemsToMove = new List<GameObject>();

    public TMP_Text ScoreText;
    private int Score;

    private bool MatchInProgress;
    public bool AbilitySelected;

    public BaseAbility[] GridAbilities;

    public int AbilityIndex;
    

    void Awake()
    {
        gameBoard = GetComponent<InnitBoardSetUp>();
        
    }


    private void Start()
    {
        CurrentDragDist = 0;
        MaxDragDist = gameBoard.GetPrefabDims();
        MatchsMade.Clear();
        MatchsMade.Add(new List<GameObject>());
        MatchsMade.Add(new List<GameObject>());
        MatchInProgress = true;
        CheckForMatch();

        for(int i = 0; i < GridAbilities.Length; i++)
        {
            GridAbilities[i].InputHandler = this;
        }
       
        //Debug.Log("matchs made num is: " + MatchsMade.Count);
    }

    private void UpdateScoreText()
    {
        ScoreText.text = Score.ToString();
    }
    public void OnTouchStart(InputAction.CallbackContext context)
    {
        
        if(MatchInProgress)
        {
            return;
        }

        if(AbilitySelected)
        {
            return;
        }
        if(Touchscreen.current.primaryTouch.press.isPressed == true)
        {
            if(!isDragging)
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
    }

    public void AbilitySelection(int index)
    {
        AbilityIndex = index;
        AbilitySelected = true;
    }

    public void OnUseAbility(InputAction.CallbackContext context)
    {
        Debug.Log("on use ability activated");
        if (MatchInProgress)
        {
            return;
        }
        if (AbilitySelected)
        {
            Vector2 touchPos = Touchscreen.current.position.ReadValue();
            GameObject GridSquare = GetItemAtPosition(touchPos);
            if (GridSquare != null)
            {

                GridAbilities[AbilityIndex].UseAbility(GridSquare);
                //TempAbility(GridSquare);
                AbilitySelected = false;
                //use ability selected
            }
            else
            {
                Debug.Log("no grid square was chosen");
            }
        }
    }

    private void TempAbility(GameObject StartPoint)
    {
        ItemStats tempStats = StartPoint.GetComponent<ItemStats>();
        int xAxis = tempStats.CurrentGridPos.x;
        int yAxis = tempStats.CurrentGridPos.y;
        ListOfItemsToMove.Add(StartPoint);
        for(int x = 0; x < gameBoard.width; x++)
        {
            if(x == xAxis)
            {
                continue;
            }
            ListOfItemsToMove.Add(gameBoard.grid[x, yAxis]);
        }

        for(int y = 0; y < gameBoard.height; y++)
        {
            if (y == yAxis)
            {
                continue;
            }
            ListOfItemsToMove.Add(gameBoard.grid[xAxis, y]);
        }
        MatchMadeClear();
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
        
        if(Touchscreen.current.primaryTouch.press.isPressed == false)
        {
            Debug.Log("touch ended via touchscreen");
            if(isDragging)
            {
                Debug.Log("touch ended");
                // Logic to check for matches and reset position if no match
                if (!CheckForMatch())
                {
                    StartCoroutine(IE_ResetPos(selectedItem));
                    // ResetPosition(selectedItem);
                }
                else
                {
                    MatchMadeClear();
                }
                isDragging = false;
                selectedItem.GetComponent<Image>().color = gameBoard.ItemColours[selectedItem.GetComponent<ItemStats>().type];
                selectedItem = null;
            }
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
            if (hit.transform.gameObject.GetComponent<ItemStats>().CanBeUsedInMatch)
            {


                return hit.transform.gameObject;
            }
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
               
            }
        }
        else if(Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            // Vertical move
            //Debug.Log("moving on y");
            int targetY = y + (direction.y > 0 ? 1 : -1);
            if (targetY >= 0 && targetY < gameBoard.HiddenHeight)
            {
                if (y < gameBoard.height && targetY >= gameBoard.height)
                {
                    UpdateItem(gameBoard.grid[x, targetY]);
                }
                SwapItems(x, y, x, targetY);
                
            }
        }
        else
        {
            Debug.Log("get fucked");
        }
    }

    void MoveItemsNoMatch(GameObject item, Vector2 direction)
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
                
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            // Vertical move
            //Debug.Log("moving on y");
            int targetY = y + (direction.y > 0 ? 1 : -1);
            if (targetY >= 0 && targetY < gameBoard.HiddenHeight)
            {
                if(y < gameBoard.height && targetY >= gameBoard.height)
                {
                    UpdateItem(gameBoard.grid[x,targetY]);
                }
                SwapItems(x, y, x, targetY);
                
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

    bool CheckForMatch()
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
                            //MatchsMade[0].Add(new Vector2Int(x, y));
                            if (!CheckAdjacentSpot(gameBoard.grid[x,y].GetComponent<ItemStats>().CurrentGridPos, Vector2Int.up, gameBoard.grid[x, y].GetComponent<ItemStats>().type,0) )
                            {
                                MatchsMade[0].Clear();
                            }

                        break;

                        case 1:
                            //MatchsMade[1].Add(new Vector2Int(x, y));
                            if (!CheckAdjacentSpot(gameBoard.grid[x, y].GetComponent<ItemStats>().CurrentGridPos, Vector2Int.right, gameBoard.grid[x, y].GetComponent<ItemStats>().type,1))
                            {
                                MatchsMade[1].Clear();
                            }

                        break;
                    }

                   
                }
                //Debug.LogError("the num in matched made 0 is: " + MatchsMade[0].Count);
                //Debug.LogError("the num in matched made 1 is: " + MatchsMade[1].Count);
                if (MatchsMade[0].Count > 2)
                {
                    //matchmade
                    if (MatchsMade[0].Count > 3)
                    {
                        GameObject temp = MatchsMade[0][0];
                        GridAbilities[0].UseAbility(temp);
                    }
                    else
                    {


                        ListOfItemsToMove.AddRange(MatchsMade[0]);
                    }
                    MatchsMade[0].Clear();
                }
                else
                {
                    MatchsMade[0].Clear();
                }
                if (MatchsMade[1].Count > 2)
                {
                    if (MatchsMade[0].Count > 3)
                    {
                        GameObject temp = MatchsMade[1][0];
                        GridAbilities[1].UseAbility(temp);
                    }
                    else
                    {


                        ListOfItemsToMove.AddRange(MatchsMade[1]);
                    }
                    MatchsMade[1].Clear();
                }
                else
                {
                    MatchsMade[1].Clear();
                }
            }
        }
        // Check if there's a match after moving

        if(ListOfItemsToMove.Count > 0)
        {
            MatchMadeClear();
            return true;
        }
        return false; // Placeholder
    }

    public void MatchMadeClear()
    {
        MatchInProgress = true;
        //Debug.Log("match has been made");
        for(int i = 0; i < ListOfItemsToMove.Count; i++)
        {
            //gameBoard.grid[ListOfItemsToPop[i].x, ListOfItemsToPop[i].y].GetComponent<Image>().color = Color.white;
            ListOfItemsToMove[i].GetComponent<Image>().color = Color.clear;
            //gameBoard.grid[ListOfItemsToPop[i].x, ListOfItemsToPop[i].y].GetComponent<ItemStats>().CanBeUsedInMatch = false;
            ListOfItemsToMove[i].GetComponent<ItemStats>().CanBeUsedInMatch = false;
        }
        Score += ListOfItemsToMove.Count * 100;
        UpdateScoreText();
        //ListOfItemsToPop.Clear();
        StartCoroutine(IE_MoveMatch());
        //ListOfItemsToMove.Clear();
        //selectedItem.GetComponent<Image>().color = Color.green;
    }

    bool CheckAdjacentSpot(Vector2Int Spot,Vector2Int dir, int type, int matchDir)
    {
        //this aint right 
        MatchsMade[matchDir].Add(gameBoard.grid[Spot.x,Spot.y]);
        Vector2Int PosToCheck = new Vector2Int(Spot.x + dir.x, Spot.y + dir.y);

        if (PosToCheck.x >= gameBoard.width || PosToCheck.x < 0 ||
        PosToCheck.y >= gameBoard.height || PosToCheck.y < 0)
        {
            return false;
        }

        if (!gameBoard.grid[PosToCheck.x, PosToCheck.y].GetComponent<ItemStats>().CanBeUsedInMatch)
        {
            return false;
        }

        if (gameBoard.grid[Spot.x + dir.x, Spot.y + dir.y].GetComponent<ItemStats>().type == type)
        {
            CheckAdjacentSpot(Spot + dir, dir, type, matchDir);
            return true;
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
            MoveItemsNoMatch(item, dir);
        }
        PrePos = stats.GridStartPos;
        CurPos = stats.CurrentGridPos;
        if(CurPos != PrePos)
        {
            StartCoroutine(IE_ResetPos(item));
        }

        //item.transform.position = new Vector3(x, y, 0);
    }

    IEnumerator IE_ResetPos(GameObject item)
    {
        //Debug.Log("reset timer started");
        yield return new WaitForSeconds(0.1f);
        ResetPosition(item);
        //Debug.Log("reset timer done");
    }

    /*public void CheckAndMatch()
    {
        if(CheckForMatch())
        {
            MatchMadeClear();
        }
        else
        {
            MatchInProgress = false;
        }
    }*/


    //need this to take in own list of items that will clear them after moving
    IEnumerator IE_MoveMatch()
    {
        for(int i = 0; i < ListOfItemsToMove.Count; i++)
        {
            bool isAtTop = false;
            for(int y = 0; y < gameBoard.HiddenHeight; y++)
            {
                //Debug.Log("moving item up: " + i);
                MoveItemsNoMatch(ListOfItemsToMove[i], Vector2.up);

                yield return new WaitForSeconds(0.01f);
            }
           

            //yield return new WaitForSeconds(1.1f);
        }
        ListOfItemsToMove.Clear();
        UpdateGridStartPos();
        CheckForMatch();
        MatchInProgress = false;
    }

    void UpdateGridStartPos()
    {
        for(int x = 0; x < gameBoard.width; x++)
        {
            for(int y =0; y < gameBoard.HiddenHeight; y++)
            {
                gameBoard.grid[x,y].GetComponent<ItemStats>().GridStartPos = new Vector2Int(x,y);
            }
        }
    }

    void UpdateItem(GameObject itemToUpdate)
    {
        int randNum = Random.Range(0, gameBoard.ItemColours.Length);
        itemToUpdate.GetComponent<ItemStats>().type = randNum;
        itemToUpdate.GetComponent<ItemStats>().CanBeUsedInMatch = true;
        itemToUpdate.GetComponent<Image>().color = gameBoard.ItemColours[randNum];
        
    }
}