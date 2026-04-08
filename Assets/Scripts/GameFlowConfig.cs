using System.Collections.Generic;
public static class GameFlowConfig
{
    public static List<List<GameEventType>> days = new List<List<GameEventType>>()
    {
        new List<GameEventType>() // Día 1
        {
            GameEventType.Shop,
            GameEventType.ChanceGame,
            GameEventType.Transition,
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier
        },
        new List<GameEventType>() // Día 2
        {
            GameEventType.Transition,
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.Shop
        },
        new List<GameEventType>() // Día 3
        {
            GameEventType.Transition,
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.ChanceGame
        },
        new List<GameEventType>() // Día 4
        {
            GameEventType.Transition,
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.Shop
        },
        new List<GameEventType>() // Día 5
        {
            GameEventType.Transition,
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.ChanceGame
        },
        new List<GameEventType>() // Día 6
        {
            GameEventType.Transition,
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.Shop
        },
        new List<GameEventType>() // Día 7
        {
            GameEventType.ChanceGame,
            GameEventType.Transition,
            GameEventType.FinalGame,
            GameEventType.EndRun
        }
    };
}