using System;
using System.Collections.Generic;
using UnityEngine;
using static DeckConfig;

[Serializable]
public class Pack
{
    public string Name;
    public string ID;
    public bool IsDefault;
    public bool IsActiveInGame;
    public List<CardData> CardDatas = new();

    public AttackNumsPresets[] AttackNumsPresets = new AttackNumsPresets[4];

    public static Pack Clone(Pack inSource)
    {
        var clone = new Pack
        {
            ID = UnityEngine.Random.Range(9, 9999999).ToString(),
            IsDefault = false,
            IsActiveInGame = inSource.IsActiveInGame,
            Name = inSource.Name,
            CardDatas = new List<CardData>(),
            AttackNumsPresets = new AttackNumsPresets[4]
        };

        foreach (var card in inSource.CardDatas)
        {
            clone.CardDatas.Add(card.Clone(card));
        }

        for (int i = 0; i < inSource.AttackNumsPresets.Length; i++)
        {
            clone.AttackNumsPresets[i] = new AttackNumsPresets
            {
                DeckType = inSource.AttackNumsPresets[i].DeckType,
                Counts = new List<int>(inSource.AttackNumsPresets[i].Counts)
            };
        }

        return clone;
    }

    public int GetTotalCardsCount()
    {
        int totalCount = 0;

        foreach (var card in this.CardDatas)
        {
            if (card.DeckCounts[0] <= 0)
                totalCount += card.Count;
        }

        return totalCount;
    }
}

[Serializable]
public class AttackNumsPresets
{
    [Header("3 = Enchanted, 4 = Mystic, 5 = Arcane, 6 = Quest")]
    public EDeckType DeckType;
    public List<int> Counts = new(9);
}