using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDrag : MonoBehaviour
{
    public Vector2 CurrentPosInGrid;
    public Vector2 TempPosInGrid;
    private Vector2 NewPosInGrid;
    private Vector2 Dir;
    public BoardSetUp m_BoardSetup;
    public float PrefabDims;
    public float XPosDiff;
    public float YPosDiff;
    public void ObjDropped()
    {

    }

    public void MoveToGridSpace(Vector2 DirToMove)
    {
        NewPosInGrid = TempPosInGrid + DirToMove;
        TempPosInGrid = NewPosInGrid;
        float tempXPos = (TempPosInGrid.x * PrefabDims) + XPosDiff;
        float tempYPos = (TempPosInGrid.y * PrefabDims) + YPosDiff;
        transform.position = new Vector3(tempXPos, tempYPos,0);
        
    }

    public void CheckToMoveOthers()
    {
        float tempX = transform.position.x - XPosDiff;
        float tempY = transform.position.y - YPosDiff;
    }

    private void ResetVars()
    {

    }


}
