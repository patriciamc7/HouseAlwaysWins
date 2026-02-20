using UnityEngine;

public class DimeGame
{
    public float dimeProb = 0.5f;

    public bool isWin()
    {
        return Random.value < dimeProb;
    }
}
