using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class PlayerData
{
    public static PlayerData Data = new();

    public bool IsDirty { get; private set; }
    public bool ShouldSaveOnQuit { get; set; }

    public bool ShouldShowAllResources = true;

    [Header("Pack Info")]
    public string ActivePackId;
    public Pack ActivePack => this.PacksDict[this.ActivePackId];

    [Header("(View Only)"), ShowInInspector]
    public Dictionary<string, Pack> PacksDict = new();

    public void SetDirty()
    {
        this.IsDirty = true;
        this.ShouldSaveOnQuit = true;
    }

    public void SetCardDirty(CardData cardData)
    {
        //Debug.Log($"<color=black>SetCardDirty({cardData.Name})   id = {cardData.ID}</color>");
        
        cardData.IsDirty = true;
        cardData.ShouldSaveOnQuit = true;

        SetDirty();
    }

    public void SetEditNote(CardData cardData, string editNoteType, string oldValue, string newValue, bool inShouldRefreshUI = true)
    {
        var editNoteData = new EditNoteData
        {
            ID = cardData.ID,
            EditNoteType = editNoteType,
            OldValue = oldValue,
            NewValue = newValue,
            Comment = string.Empty,
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        if(cardData.CardEditNotesDict == null)
            cardData.CardEditNotesDict = new();

        if (cardData.CardEditNotesDict.ContainsKey(editNoteType))
        {
            cardData.CardEditNotesDict[editNoteType] = editNoteData;
        }
        else
        {
            cardData.CardEditNotesDict.Add(editNoteType, editNoteData);
        }

        //fire event that refreshes EditNoteButtons
        if(inShouldRefreshUI)
            UIEditNotesPanel.OnPlayerDataChanged?.Invoke();
    }

    public void SetEditNote(CardData cardData, string editNoteType, List<string> oldListValue, List<string> newListValue)
    {
        var editNoteData = new EditNoteData
        {
            ID = cardData.ID,
            EditNoteType = editNoteType,
            OldListValue = oldListValue,
            NewListValue = newListValue,
            Comment = string.Empty,
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        if(cardData.CardEditNotesDict == null)
            cardData.CardEditNotesDict = new();

        if (cardData.CardEditNotesDict.ContainsKey(editNoteType))
        {
            cardData.CardEditNotesDict[editNoteType] = editNoteData;
        }
        else
        {
            cardData.CardEditNotesDict.Add(editNoteType, editNoteData);
        }

        //fire event that refreshes EditNoteButtons
        UIEditNotesPanel.OnPlayerDataChanged?.Invoke();
    }

    public void SetAllCardsClean(bool inShouldClearCardConfigFlag)
    {
        foreach (var kvp in this.PacksDict)
        {
            foreach (var cardData in kvp.Value.CardDatas)
            {
                cardData.IsDirty = false;

                if (inShouldClearCardConfigFlag)
                    cardData.ShouldSaveOnQuit = false;
            }
        }

        this.IsDirty = false;

        if (inShouldClearCardConfigFlag)
            this.ShouldSaveOnQuit = false;
    }
}