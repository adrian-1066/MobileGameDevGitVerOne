using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : MonoBehaviour
{
    public GridInputHandler InputHandler;

    public virtual void UseAbility(GameObject StartPoint)
    {
        AbilityLogic(StartPoint);
    }


    protected virtual void AbilityLogic(GameObject StartPoint)
    {

    }
}
