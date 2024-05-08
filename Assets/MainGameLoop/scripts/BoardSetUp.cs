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
    private RectTransform BoardRect;
    [SerializeField]
    private int GridSize;

    private List<List<int>> GridList = new List<List<int>>();
    private void Start()
    {
        BoardRect = BoardCanvas.GetComponent<RectTransform>();
        BoardWidth = BoardRect.rect.width;
        BoardHeight = BoardRect.rect.height;

        GridWidth = (BoardWidth / 10) * 9;
        GridHeight = GridWidth;

        PrefabDims = GridWidth / 10;

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
            GridList.Add(new List<int>());
            for (int y = 0; y < GridSize; y++)
            {
                if(GridList[x][y] == 1)
                {
                    GameObject temp = Instantiate(MatchPrefab, gameObject.transform);
                    //temp.transform.position = new Vector3(0 + ((x * PrefabDims) + ((PrefabDims/10)*x)), (BoardCanvas.transform.position.y - (GridSize/2) + (y * PrefabDims)), 0);
                    //temp.transform.localPosition = new Vector3((((x * PrefabDims) ) - (BoardWidth/2) + (PrefabDims/2)) + ((PrefabDims/10)*x) + (PrefabDims/10), (BoardCanvas.transform.position.y - (GridSize / 2) + (y * PrefabDims))- BoardHeight + ((PrefabDims/10)*y), 0);
                    temp.transform.localPosition = new Vector3((((x * PrefabDims)) - (BoardWidth / 2) + (PrefabDims / 2)) + ((PrefabDims / 10) * x) + (PrefabDims / 10), (((y * PrefabDims)) - (BoardHeight / 4) + (PrefabDims / 2)) + ((PrefabDims / 10) * y) + (PrefabDims / 10), 0);
                    temp.GetComponent<RectTransform>().sizeDelta = new Vector2(PrefabDims, PrefabDims);
                }
                if (y == 0)
                {
                    Debug.Log(0 + ((x * PrefabDims) + ((PrefabDims / 10)*x)));
                }
            }

            
        }
    }

}
