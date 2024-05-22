using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSetUp : MonoBehaviour
{
    public GameObject GridObj;

    public int GridSize;

    public int SeedBaseVal = 0;
    // Start is called before the first frame update
    void Start()
    {
        SpawnGrid();
    }

    private void OnEnable()
    {
        
        //SpawnGrid();
    }

    private void OnDisable()
    {
        
    }

    private void SpawnGrid()
    {
        for(int i = 0; i < GridSize; i++)
        {
            for(int x = 0; x < GridSize; x++)
            {
                GameObject temp = Instantiate(GridObj, new Vector3(i, x, 0), transform.rotation);
                temp.GetComponent<GridFillerStats>().GridPos = new Vector2(i, x);
            }
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
