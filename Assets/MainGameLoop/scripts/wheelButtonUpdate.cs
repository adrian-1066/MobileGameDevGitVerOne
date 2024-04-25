using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class wheelButtonUpdate : MonoBehaviour
{
    public TMP_Text text;

    public void UpdateText(int NewNum)
    {
        text.text = NewNum.ToString();
    }
}
