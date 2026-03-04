using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class WheelWildLogic : MonoBehaviour
{
    [SerializeField] WheelWildAnimation visual;
    public WheelSlot[] slots;

    Action<bool> Action;

    [Serializable]
    public class WheelSlot
    {
        public string name;     // opcional
        public Color color;     // rojo o negro
        public float startAngle; // ángulo inicial de la casilla
        public float endAngle;   // ángulo final de la casilla
    }

    public void PlayWheel(Color playerColorChoice, Action <bool> OnComplete)
    {
        bool playerWins = DecideIfPlayerWins();
        List<WheelSlot> possibleSlots;
        if (playerWins)
            possibleSlots = slots.Where(s => s.color == playerColorChoice).ToList();
        else
            possibleSlots = slots.Where(s => s.color != playerColorChoice).ToList();

        // Elegir uno al azar
        var chosenSlot = possibleSlots[UnityEngine.Random.Range(0, possibleSlots.Count)];

        // Calcular el ángulo central
        float targetAngle = (chosenSlot.startAngle + chosenSlot.endAngle) / 2f;

        // Llamar a RouletteVisual para animar
        visual.Play(targetAngle, () =>
        {
            OnFinished(playerWins);
        });
    }

    public void AutoSetupSlots(int totalSlots)
    {
        slots = new WheelSlot[totalSlots];
        float slotSize = 360f / totalSlots;

        for (int i = 0; i < totalSlots; i++)
        {
            slots[i] = new WheelSlot
            {
                name = "Slot " + (i + 1),
                color = (i % 2 == 0) ? Color.red : Color.black, // alterna rojo/negro
                startAngle = i * slotSize,
                endAngle = (i + 1) * slotSize
            };
        }
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