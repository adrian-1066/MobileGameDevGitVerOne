using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoardSetUp : MonoBehaviour
{
    public Canvas BoardCanvas;
    public GameObject MatchPrefab;
    [SerializeField]
    private float BoardWidth;
    [SerializeField]
    private float BoardHeight;

    private float GridWidth;
    private float GridHeight;
    [SerializeField]
    private float PrefabDims;
    private float PrefabGap;
    private RectTransform BoardRect;
    [SerializeField]
    private int GridSize;

    private List<List<int>> GridList = new List<List<int>>();

    private List<List<Vector2>> BoardPositions = new List<List<Vector2>>();
    private void Start()
    {
        BoardRect = BoardCanvas.GetComponent<RectTransform>();
        BoardWidth = BoardRect.rect.width;
        BoardHeight = BoardRect.rect.height;

        GridWidth = (BoardWidth / 10) * 9;
        GridHeight = GridWidth;

        PrefabDims = GridWidth / 10;
        PrefabGap = PrefabDims / 10;
        MatchPrefab.GetComponent<RectTransform>().rect.Set(0,0,PrefabDims,PrefabDims);
        MatchPrefab.GetComponent<BoxCollider2D>().size = new Vector2(PrefabDims, PrefabDims);
       
        InnitGrid();
        SpawnMatch();

    }


    private void InnitGrid()
    {
        GridList.Clear();
        for (int x = 0; x < GridSize; x++)
        {
            GridList.Add(new List<int>());
            for(int y = 0; y < GridSize; y++)
            {
                GridList[x].Add(1);
                //GridList[x].Add((x*GridSize)+y);
                Debug.Log(GridList[x][y]);
            }
        }
    }

    private void SpawnMatch()
    {
        for (int x = 0; x < GridSize; x++)
        {
            //GridList.Add(new List<int>());
            BoardPositions.Add(new List<Vector2>());
            for (int y = 0; y < GridSize; y++)
            {
                if(GridList[x][y] == 1)
                {
                    GameObject temp = Instantiate(MatchPrefab, gameObject.transform);
                    Vector2 WorldPos = new Vector2((((x * PrefabDims)) - (BoardWidth / 2) + (PrefabDims / 2)) + (PrefabGap * x) + PrefabGap, (((y * PrefabDims)) - (BoardHeight / 4) + (PrefabDims / 2)) + (PrefabGap * y) + PrefabGap);
                    temp.transform.localPosition = new Vector3(WorldPos.x,WorldPos.y, 0);
                    BoardPositions[x].Add(WorldPos);
                    temp.GetComponent<RectTransform>().sizeDelta = new Vector2(PrefabDims, PrefabDims);
                    temp.GetComponent<ObjDrag>().CurrentPosInGrid = new Vector2(x, y);
                    temp.GetComponent<ObjDrag>().m_BoardSetup = this;
                }
               
            }

            
        }
    }

}
