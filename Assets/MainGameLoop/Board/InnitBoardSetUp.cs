using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject[] items; // Array of different item prefabs
    public GameObject[,] grid;
    private void Awake()
    {
        BoardRect = BoardCanvas.GetComponent<RectTransform>();
        BoardWidth = (BoardRect.rect.width * BoardRect.localScale.x);
        BoardHeight = BoardRect.rect.height;
        PrefabDims = BoardWidth / (width + 1);
        GridOffset = (PrefabDims / (width + 1));
    }
    void Start()
    {
        
        transform.position = new Vector3(transform.position.x + (PrefabDims/2) + GridOffset, transform.position.y, transform.position.z);
        grid = new GameObject[width, height];
        PopulateGrid();
    }

    void PopulateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float XPos = (x * PrefabDims) + (GridOffset * x) + transform.position.x;
                float YPos = (y * PrefabDims) + (GridOffset * y) + transform.position.y;
                Vector2 position = new Vector2(XPos, YPos);
                int itemToUse = Random.Range(0, items.Length);
                GameObject newItem = Instantiate(items[itemToUse], position, Quaternion.identity);
                newItem.GetComponent<RectTransform>().sizeDelta = new Vector2(PrefabDims, PrefabDims);
                newItem.GetComponent<BoxCollider2D>().size = new Vector2(PrefabDims,PrefabDims);
                newItem.transform.parent = this.transform;
                newItem.GetComponent<ItemStats>().CurrentGridPos = new Vector2Int(x, y);
                newItem.GetComponent<ItemStats>().GridStartPos = new Vector2Int(x, y);
                grid[x, y] = newItem;
            }
        }
    }

    public float GetPrefabDims()
    {
        return PrefabDims;
    }
}
