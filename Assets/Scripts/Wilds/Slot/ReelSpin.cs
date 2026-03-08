using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum IconReels
{
    star,
    skull,
    coin,
    cherry,
    bar
}

public class reelSlot
{
    public float angle;
    public IconReels iconReels;
}

public class reelSpin : MonoBehaviour
{
    [SerializeField] List<Reel> reelList;
    [SerializeField] List<reelSlot> reelSlots;

    private void Start()
    {
        Spin();
    }

    void Spin()
    {
        for (int i = 0; i < reelList.Count; i++)
        {
            StartCoroutine(reelList[i].SpinReel(30));
        }
    }
}
