using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDrag : MonoBehaviour
{
    public Vector2 CurrentPosInGrid;
    public Vector2 TempPosInGrid;
    public Vector2 TempWorldPos;
    private Vector2 NewPosInGrid;
    private Vector2 Dir;
    public BoardSetUp m_BoardSetup;
    public float PrefabDims;
    public float PrefabGap;
    public float BoardWidth;
    public float BoardHeight;
    public float XPosDiff;
    public float YPosDiff;

    public float totalSizeVal;
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

    public void CheckTempPos()
    {
        float Xpos = TempWorldPos.x;
        float Ypos = TempWorldPos.y;

        //Vector2 WorldPos = new Vector2((((x * PrefabDims)) - (BoardWidth / 2) + (PrefabDims / 2)) + (PrefabGap * x) + PrefabGap,
        //(((y * PrefabDims)) - (BoardHeight / 4) + (PrefabDims / 2)) + (PrefabGap * y) + PrefabGap);
        float tempX = Xpos + (BoardWidth * 2) - (PrefabDims * 2) - (PrefabGap * CurrentPosInGrid.x) - PrefabGap;
        tempX /= PrefabDims;
        float tempY = 0;
        Debug.Log("the pos in the grid is: " + CurrentPosInGrid + " the temp pos is: " + tempX + ", " + tempY);


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
