
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InnitBoardSetUp : MonoBehaviour
{
    public Canvas BoardCanvas;
    [SerializeField]
    private float BoardWidth;
    [SerializeField]
    private float BoardHeight;
    [SerializeField]
    private float PrefabDims;
    [SerializeField]
    private float GridOffset;
    private RectTransform BoardRect;
    public int width = 8;
    public int height = 8;
    public int HiddenHeight;
    public GameObject[] items; // Array of different item prefabs
    public Color[] ItemColours;
    public GameObject[,] grid;
    public int Seed;
    public TMP_Text ScoreToReachText;
    public int ScoreNeeded;

    
    
    //public GameManager manager;
    
    private void Awake()
    {
        BoardRect = BoardCanvas.GetComponent<RectTransform>();
        BoardWidth = (BoardRect.rect.width * BoardRect.localScale.x);
        BoardHeight = BoardRect.rect.height;
        PrefabDims = BoardWidth / (width + 1);
        GridOffset = (PrefabDims / (width + 1));
        HiddenHeight = height * 2;
        transform.position = new Vector3(transform.position.x + (PrefabDims / 2) + GridOffset, transform.position.y, transform.position.z);
        grid = new GameObject[width, HiddenHeight];
        //Seed = 0;
    }
    void Start()
    {
        
       
        
    }

    private void OnEnable()
    {

        PopulateGrid();
        SetScoreAndMoves();
        GetComponent<GridInputHandler>().StartUp();
    }

    private void OnDisable()
    {
        ClearGrid();
    }

    void SetScoreAndMoves()
    {
        ScoreNeeded = ((Seed + 1) * 10) * 100;
        ScoreToReachText.text = ScoreNeeded.ToString();

    }

    void PopulateGrid()
    {
        Random.InitState(Seed);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < HiddenHeight; y++)
            {
                float XPos = (x * PrefabDims) + (GridOffset * x) + transform.position.x;
                float YPos = (y * PrefabDims) + (GridOffset * y) + transform.position.y;
                Vector2 position = new Vector2(XPos, YPos);
                int itemToUse = Random.Range(0, ItemColours.Length);
                int oddOREven = x + y;
                if (oddOREven % 2 == 0)
                {
                    //itemToUse = 0;
                }
                else
                {
                    //itemToUse = 1;
                }
                GameObject newItem = Instantiate(items[0], position, Quaternion.identity);
                newItem.GetComponent<RectTransform>().sizeDelta = new Vector2(PrefabDims, PrefabDims);
                newItem.GetComponent<BoxCollider2D>().size = new Vector2(PrefabDims,PrefabDims);
                newItem.transform.parent = this.transform;
                ItemStats newItemStats = newItem.GetComponent<ItemStats>();
                newItemStats.CurrentGridPos = new Vector2Int(x, y);
                newItemStats.GridStartPos = new Vector2Int(x, y);
                newItemStats.type = itemToUse;
                newItem.GetComponent<Image>().color = ItemColours[itemToUse];
                newItemStats.CanBeUsedInMatch = true;
                if(y >= height)
                {
                    newItem.GetComponent<Image>().color = Color.clear;
                    
                    newItemStats.CanBeUsedInMatch = false;
                }
                grid[x, y] = newItem;
                
            }
        }
        
    }

    private void ClearGrid()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < HiddenHeight; y++)
            {
                GameObject.Destroy(grid[x, y]);
                grid[x, y] = null;
            }
        }
    }

    public float GetPrefabDims()
    {
        return PrefabDims;
    }
}
