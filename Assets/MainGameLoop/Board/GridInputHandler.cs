using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
//using System.Diagnostics;

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

    bool IsLevelComplete;
    public TMP_Text NumOfMoves;
    public int BaseMoveNum;
    public int CurrentMoveNum;

    private bool MatchInProgress;
    public bool AbilitySelected;

    public BaseAbility[] GridAbilities;

    public int AbilityIndex;

    public GameManager gameManager;

    public GameObject LevelCompleteScreen;
    public TMP_Text ScoreCompleteVal;
    public TMP_Text LevelStateText;

    public int CurrentLevelMultiplier;

    public TMP_Text Ability1AmountText;
    public TMP_Text Ability2AmountText;


    private bool isHorizontalMove;
    private bool directionSet;

    
   
    void Awake()
    {
        gameBoard = GetComponent<InnitBoardSetUp>();
        CurrentDragDist = 0;
     
    }


    private void Start()
    {
       
    }

    public void StartUp()
    {

        for (int i = 0; i < GridAbilities.Length; i++)
        {
            GridAbilities[i].InputHandler = this;
        }
        IsLevelComplete = false;
        gameBoard = GetComponent<InnitBoardSetUp>();
        MaxDragDist = gameBoard.GetPrefabDims();
        CurrentDragDist = 0;
        MatchsMade.Add(new List<GameObject>());
        MatchsMade.Add(new List<GameObject>());
        MatchInProgress = true;
        //MatchsMade.Clear();
        CurrentMoveNum = BaseMoveNum;
        NumOfMoves.text = CurrentMoveNum.ToString();
        LevelCompleteScreen.SetActive(false);
        CurrentLevelMultiplier = gameManager.levelMulti;
        Ability1AmountText.text = gameManager.AmountOfAbility1.ToString();
        Ability2AmountText.text = gameManager.AmountOfAbility2.ToString();



        CheckForMatch();
    }
    private void OnEnable()
    {
        LevelCompleteScreen.SetActive(false);
    }

    private void OnDisable()
    {
        gameBoard = null;
        //MatchsMade = null;
        MatchInProgress = false;
        selectedItem = null;
        Score = 0;
        ScoreText.text = Score.ToString();
        IsLevelComplete = false;
    }

    private void UpdateScoreText()
    {
        ScoreText.text = Score.ToString();
    }

    private void LevelComplete()
    {
        Debug.Log("level complete");
        if(gameManager.highScores.Count -1 < gameBoard.Seed - 1)
        {
            Debug.Log("new high level reached");
            gameManager.highScores.Add(Score);
            gameManager.highestLevelReached = gameManager.highScores.Count - 1;
        }
        else
        {
            Debug.Log("new highscore for this level");
            gameManager.highScores[gameBoard.Seed - 1] = Score;
        }
        gameManager.curFreeMoney += Mathf.RoundToInt(Score / 100);
        gameManager.UpdateNums();
        IsLevelComplete = true;
        LevelCompleteScreen.SetActive(true);
        ScoreCompleteVal.text = Score.ToString();
        LevelStateText.text = "Level Complete";
    }

    public void TriggerChangeOnWheel()
    {
        Debug.Log("seed Sending is: " + gameBoard.Seed);
        gameManager.SetWheelToCurrentLevel(gameBoard.Seed);
        
    }
    public void OnTouchStart(InputAction.CallbackContext context)
    {
        if (IsLevelComplete || MatchInProgress || AbilitySelected)
        {
            return;
        }

        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (!isDragging)
            {
                initialTouchPosition = Touchscreen.current.position.ReadValue();
                selectedItem = GetItemAtPosition(initialTouchPosition);
                if (selectedItem != null)
                {
                    if(selectedItem.GetComponent<ItemStats>().GridStartPos.y >= gameBoard.height)
                    {
                        selectedItem = null;
                        return;
                    }
                    selectedItem.GetComponent<Image>().color = Color.green;
                    isDragging = true;
                    directionSet = false;  // Reset direction flag
                }
            }
        }
    }
    
    public void AbilitySelection(int index)
    {
        if(index == 0)
        {
            if(gameManager.AmountOfAbility1 >= 1)
            {
                AbilityIndex = index;
                AbilitySelected = true;
            }
            else
            {
                Debug.Log("no ability 1");
            }
        }
        else if(index == 1)
        {
            if (gameManager.AmountOfAbility2 >= 1)
            {
                AbilityIndex = index;
                AbilitySelected = true;
            }
            else
            {
                Debug.Log("no ability 2");
            }
        }
       
    }

    public void OnUseAbility(InputAction.CallbackContext context)
    {
        //Debug.Log("on use ability activated");
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
                if(AbilityIndex == 0)
                {
                    gameManager.AmountOfAbility1--;
                }
                else if(AbilityIndex == 1)
                {
                    gameManager.AmountOfAbility2--;
                }
                //TempAbility(GridSquare);
                AbilitySelected = false;
                UpdateAbilityNum();
                //use ability selected
            }
            else
            {
                //Debug.Log("no grid square was chosen");
            }
        }
    }

    private void UpdateAbilityNum()
    {
        Ability1AmountText.text = gameManager.AmountOfAbility1.ToString();
        Ability2AmountText.text = gameManager.AmountOfAbility2.ToString();
    }
    public void OnTouchMove(InputAction.CallbackContext context)
    {
        if (isDragging)
        {
            currentTouchPosition = Touchscreen.current.position.ReadValue();
            Vector2 direction = currentTouchPosition - initialTouchPosition;
            float dist = Vector2.Distance(currentTouchPosition,initialTouchPosition);
            initialTouchPosition = currentTouchPosition;
            Debug.Log("adding dist: " + dist);
            
            CurrentDragDist += dist;

            if (CurrentDragDist >= MaxDragDist)
            {
                CurrentDragDist = 0;

                if (!directionSet) // Determine initial direction on first significant move
                {
                    isHorizontalMove = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
                    directionSet = true;
                }
                Debug.Log("direction is: " + direction);
                // Restrict movement to the determined direction
                if (directionSet)
                {
                    if (isHorizontalMove)
                    {
                        direction = new Vector2(direction.x, 0);
                    }
                    else
                    {
                        direction = new Vector2(0, direction.y);
                    }
                }
                if ((selectedItem.GetComponent<ItemStats>().CurrentGridPos + (direction).normalized).y >= gameBoard.height)
                {
                    Debug.Log("trying to drag item out of range");
                }
                else
                {
                    MoveItems(selectedItem, direction);
                }
            }
        }
    }
   
    public void OnTouchEnd(InputAction.CallbackContext context)
    {
        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                if (!CheckForMatch())
                {
                    StartCoroutine(IE_ResetPos(selectedItem));
                }
                else
                {
                    CurrentMoveNum--;
                    UpdateMoveNum();
                    MatchMadeClear();

                    if (CurrentMoveNum <= 0)
                    {
                        LoseGame();
                    }
                }
                isDragging = false;
                selectedItem.GetComponent<Image>().color = Color.white;//gameBoard.ItemColours[selectedItem.GetComponent<ItemStats>().type];
                selectedItem = null;
                isHorizontalMove = false;
                directionSet = false; // Reset direction flag for next drag
            }
        }
    }
    private void LoseGame()
    {
        IsLevelComplete = true;
        LevelCompleteScreen.SetActive(true);
        ScoreCompleteVal.text = Score.ToString();
        LevelStateText.text = "Level Failed!!!";
        gameManager.curFreeMoney += Mathf.RoundToInt((Score / 100)/2);
        gameManager.AmountOfLife--;
        gameManager.TriggerHealthRegen();
        gameManager.UpdateNums();
    }

    private void UpdateMoveNum()
    {
        NumOfMoves.text = CurrentMoveNum.ToString();
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

        if (isHorizontalMove)
        {
            // Horizontal move
            int targetX = x + (direction.x > 0 ? 1 : -1);
            if (targetX >= 0 && targetX < gameBoard.width)
            {
                SwapItems(x, y, targetX, y);
            }
        }
        else
        {
            // Vertical move
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
    }
   
    void MoveItemsNoMatch(GameObject item, Vector2 direction)
    {
        ItemStats stats = item.GetComponent<ItemStats>();
        int x = stats.CurrentGridPos.x;
        int y = stats.CurrentGridPos.y;
/*
        if (isHorizontalMove)
        {
            // Horizontal move
            int targetX = x + (direction.x > 0 ? 1 : -1);
            if (targetX >= 0 && targetX < gameBoard.width)
            {
                SwapItems(x, y, targetX, y);
            }
        }
        else
        {
            // Vertical move
            int targetY = y + (direction.y > 0 ? 1 : -1);
            if (targetY >= 0 && targetY < gameBoard.HiddenHeight)
            {
                if (y < gameBoard.height && targetY >= gameBoard.height)
                {
                    UpdateItem(gameBoard.grid[x, targetY]);
                    MakeItemInvis(gameBoard.grid[x, y]);
                }
                SwapItems(x, y, x, targetY);
            }
        }*/

        int targetY = y + (direction.y > 0 ? 1 : -1);
        int targetX = x + (direction.x > 0 ? 1 : -1);
        if (direction.y != 0)
        {


            if (targetY >= 0 && targetY < gameBoard.HiddenHeight)
            {
                if (y < gameBoard.height && targetY >= gameBoard.height)
                {
                    UpdateItem(gameBoard.grid[x, targetY]);
                    MakeItemInvis(gameBoard.grid[x, y]);
                }
                SwapItems(x, y, x, targetY);
            }
        }
        else
        {
            SwapItems(x, y, targetX, y);
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

                if (!gameBoard.grid[x, y].GetComponent<ItemStats>().CanBeUsedInMatch)
                {
                    gameBoard.grid[x, y].GetComponent<ItemStats>().GridStartPos.y += (gameBoard.height + 1);
                    StartCoroutine(IE_ResetPos(gameBoard.grid[x, y]));
                }
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
                        GridAbilities[2].UseAbility(temp);
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
                    if (MatchsMade[1].Count > 3)
                    {
                        GameObject temp = MatchsMade[1][0];
                        GridAbilities[3].UseAbility(temp);
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

        if (Score >= gameBoard.ScoreNeeded)
        {
            //end level
            if (!MatchInProgress)
            {
                LevelComplete();
            }

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
        Score += (ListOfItemsToMove.Count * CurrentLevelMultiplier);
        UpdateScoreText();

    
        List<GameObject> temp = new List<GameObject>();
        temp.AddRange(ListOfItemsToMove);
        ListOfItemsToMove.Clear();
        gameManager.audioManager.Play("MatchMade");
        StartCoroutine(IE_MoveMatch(temp));
        
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

 


  
    IEnumerator IE_MoveMatch(List<GameObject> itemsToMove)
    {
        for(int i = 0; i < itemsToMove.Count; i++)
        {
            bool isAtTop = false;
            int yPos = itemsToMove[i].GetComponent<ItemStats>().CurrentGridPos.y;
            for(int y = yPos; y < gameBoard.HiddenHeight; y++)
            {
                //Debug.Log("moving item up: " + i);
                MoveItemsNoMatch(itemsToMove[i], Vector2.up);

                yield return new WaitForSeconds(0.01f);
            }
           

            //yield return new WaitForSeconds(1.1f);
        }
        //ListOfItemsToMove.Clear();
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
        itemToUpdate.GetComponent<Image>().sprite = gameBoard.Fruits[randNum];
        itemToUpdate.GetComponent<Image>().color = Color.white;

    }

    void MakeItemInvis(GameObject itemToUpdate)
    {
        itemToUpdate.GetComponent<ItemStats>().CanBeUsedInMatch = false;
        itemToUpdate.GetComponent<Image>().color = Color.clear;
    }
}
