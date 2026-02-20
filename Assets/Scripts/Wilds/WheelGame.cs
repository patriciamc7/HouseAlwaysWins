using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelGame
{
    public float dimeProb = 0.5f;
    public bool isWin()
    {
        return Random.value < dimeProb;
    }
}
