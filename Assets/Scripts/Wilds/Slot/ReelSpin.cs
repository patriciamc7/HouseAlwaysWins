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

    public reelSlot(float angle, IconReels iconReels)
    {
        this.angle = angle;
        this.iconReels = iconReels;
    }
}

public class reelSpin : MonoBehaviour
{
    [SerializeField] List<Reel> reelList;
    [SerializeField] List<reelSlot> reelIcon = new List<reelSlot>();

    private void Start()
    {
        reelIcon.Add(new reelSlot(0,IconReels.cherry));
        reelIcon.Add(new reelSlot(72,IconReels.coin));
        reelIcon.Add(new reelSlot(144,IconReels.skull));
        reelIcon.Add(new reelSlot(216,IconReels.star));
        reelIcon.Add(new reelSlot(288,IconReels.bar));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spin();
        }
    }
    void Spin()
    {
        int index = Random.Range(0, reelIcon.Count + 1);
        bool isWin = index == reelIcon.Count ? false : true;
        float finalAngle = GetAngle(isWin, index);

        for (int i = 0; i < reelList.Count; i++)
        {
            if (!isWin)
                finalAngle = GetAngle(isWin, index);

            StartCoroutine(reelList[i].SpinReel(finalAngle));
        }
    }

    float GetAngle(bool isWin, int index)
    {
        if (isWin)
            return reelIcon[index].angle;
        else
        {
            index = Random.Range(0, reelIcon.Count);
            float nextAngle = reelIcon[(index + 1) % reelIcon.Count].angle;
            return (reelIcon[index].angle + nextAngle) / 2f;
        }
    }
}
