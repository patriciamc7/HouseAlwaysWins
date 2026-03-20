using System.Collections.Generic;
public static class GameFlowConfig
{
    public static List<List<GameEventType>> days = new List<List<GameEventType>>()
    {
        new List<GameEventType>() // Día 1
        {
            GameEventType.ChanceGame,
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier
        },
        new List<GameEventType>() // Día 2
        {
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.Shop
        },
        new List<GameEventType>() // Día 3
        {
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.ChanceGame
        },
        new List<GameEventType>() // Día 4
        {
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.Shop
        },
        new List<GameEventType>() // Día 5
        {
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.ChanceGame
        },
        new List<GameEventType>() // Día 6
        {
            GameEventType.SevenAndHalf,
            GameEventType.DeckModifier,
            GameEventType.Shop
        },
        new List<GameEventType>() // Día 7
        {
            GameEventType.ChanceGame,
            GameEventType.FinalGame,
            GameEventType.EndRun
        }
    };
}