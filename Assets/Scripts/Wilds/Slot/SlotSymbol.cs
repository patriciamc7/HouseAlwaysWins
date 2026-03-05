using UnityEngine;

public class SlotSymbol : MonoBehaviour
{
    public SymbolType type;
}

public enum SymbolType
{
    Sword,
    Cup,
    Coin,
    Club,
    Wild
}