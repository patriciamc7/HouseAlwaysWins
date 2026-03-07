using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class WheelWildLogic : MonoBehaviour
{
    [SerializeField] WheelWildAnimation visual;
    [SerializeField] WheelSlot[] slots;

    Action<bool> Action;

    [Serializable]
    public class WheelSlot
    {
        public Color color;     
        public float startAngle; 
        public float endAngle; 
    }

    public void PlayWheel(Color playerColorChoice, Action <bool> OnComplete)
    {
        bool playerWins = DecideIfPlayerWins();
        List<WheelSlot> possibleSlots;
        if (playerWins)
            possibleSlots = slots.Where(s => s.color == playerColorChoice).ToList();
        else
            possibleSlots = slots.Where(s => s.color != playerColorChoice).ToList();

        var chosenSlot = possibleSlots[UnityEngine.Random.Range(0, possibleSlots.Count)];
        float targetAngle = (chosenSlot.startAngle + chosenSlot.endAngle) / 2f;

        visual.Play(targetAngle, () =>
        {
            OnFinished(playerWins);
        });
    }

    bool DecideIfPlayerWins()
    {
        return UnityEngine.Random.value > 0.5f;
    }

    void OnFinished(bool playerWon)
    {
        Action?.Invoke(playerWon);
    }
}