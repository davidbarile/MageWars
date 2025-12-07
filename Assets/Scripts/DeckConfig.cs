using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObjects/DeckConfig", order = 2)]
public class DeckConfig : ScriptableObject
{
    public enum EDeckType
    {
        None,
        Mage,
        Fate,
        Enchanted,
        Mystic,
        Arcane,
        Quest,
        Count
    }

    public enum EAttackDeckType
    {
        None = 0,
        Enchanted = 3,
        Mystic = 4,
        Arcane = 5,
        Quest = 6,
    }

    public string Name;
    public EDeckType DeckType;
    public UICardBackBase CardBackPrefab;
}