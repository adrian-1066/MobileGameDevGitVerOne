using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossAbility : BaseAbility
{
    // Start is called before the first frame update


    
    protected override void AbilityLogic(GameObject StartPoint)
    {
        
        ItemStats tempStats = StartPoint.GetComponent<ItemStats>();
        int xAxis = tempStats.CurrentGridPos.x;
        int yAxis = tempStats.CurrentGridPos.y;
        InputHandler.ListOfItemsToMove.Add(StartPoint);
        for (int x = 0; x < InputHandler.gameBoard.width; x++)
        {
            if (x == xAxis)
            {
                continue;
            }
            InputHandler.ListOfItemsToMove.Add(InputHandler.gameBoard.grid[x, yAxis]);
        }

        for (int y = 0; y < InputHandler.gameBoard.height; y++)
        {
            if (y == yAxis)
            {
                continue;
            }
            InputHandler.ListOfItemsToMove.Add(InputHandler.gameBoard.grid[xAxis, y]);
        }
        InputHandler.MatchMadeClear();
    }
}
