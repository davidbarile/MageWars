using System;
using System.Collections.Generic;

public enum EEditNoteType
{
    None,
    Count,
    Name,
    Level,
    Type,
    Description,
    Resources,
    Requirements,
    Card,
    Decks,
    Illustration,
    PermanentResource,
    Health,
    DrawCards,
    MaxHand,
    MaxArmor,
    MaxPets,
    MaxPlants,
    MaxItems,
    AttackNumbers,
    AttackDeck,
    DefenseDeck
}

[Serializable]
public class EditNoteData
{
    public string ID;
    public string TimeStamp;
    public string EditNoteType;
    public string OldValue;
    public string NewValue;
    public List<string> OldListValue = new();
    public List<string> NewListValue = new();
    public string Comment;
    public bool IsViewed;
}