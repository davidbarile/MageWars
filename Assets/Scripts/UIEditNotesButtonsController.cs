using System;
using UnityEngine;

public class UIEditNotesButtonsController : MonoBehaviour
{
    [Serializable]
    struct EditButtonLookup
    {
        public EEditNoteType EditNoteType;
        public EditedNoteButton Button;
    }

    [SerializeField] private EditButtonLookup[] editButtonLookups;

    private void Start()
    {
        UIEditNotesPanel.OnPlayerDataChanged += RefreshButtonStates;
    }

    private void OnDestroy()
    {
        UIEditNotesPanel.OnPlayerDataChanged -= RefreshButtonStates;
    }

    private void RefreshButtonStates()
    {
        foreach (var lookup in this.editButtonLookups)
        {
            var cardData = UIPackEditor.SelectedCardData;
            var editNoteType = lookup.EditNoteType.ToString();

            if (cardData.CardEditNotesDict != null && cardData.CardEditNotesDict.ContainsKey(editNoteType))
            {
                lookup.Button.SetVisibility(true);
                var hasComment = !string.IsNullOrEmpty(cardData.CardEditNotesDict[editNoteType].Comment);
                lookup.Button.SetIconColor(hasComment);
            }
            else
            {
                lookup.Button.SetVisibility(false);
            }
        }
    }
}