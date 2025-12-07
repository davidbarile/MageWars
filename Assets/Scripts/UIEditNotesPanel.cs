using System;
using UnityEngine;
using TMPro;

public class UIEditNotesPanel : UIPanelBase
{
    public static UIEditNotesPanel IN;

    public static Action OnPlayerDataChanged;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Transform listParent;
    [SerializeField] private UIEditNoteItem editNotesItemPrefab;

    private EditedNoteButton selectedEditNoteButton;

    private int counter = 0;

    private void Start()
    {
        UIEditNotesPanel.OnPlayerDataChanged += RefreshCountText;
    }

    private void OnDestroy()
    {
        UIEditNotesPanel.OnPlayerDataChanged -= RefreshCountText;
    }

    public void ShowNote(EditedNoteButton inButton)
    {
        this.selectedEditNoteButton = inButton;
        this.listParent.DestroyAllChildren();
        base.Show();

        var editNoteItem = Instantiate(this.editNotesItemPrefab, this.listParent);

        var editNoteData = UIPackEditor.SelectedCardData.CardEditNotesDict[inButton.EditNoteType.ToString()];
        editNoteItem.Configure(UIPackEditor.SelectedCardData, editNoteData);

        RefreshCountText();
    }

    public void HandleShowAllButtonPress()
    {
        this.listParent.DestroyAllChildren();
        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            foreach (var cardData in kvp.Value.CardDatas)
            {
                if (cardData.CardEditNotesDict != null)
                {
                    foreach (var editNote in cardData.CardEditNotesDict)
                    {
                        var editNoteItem = Instantiate(this.editNotesItemPrefab, this.listParent);
                        editNoteItem.Configure(cardData, editNote.Value);
                    }
                }
            }
        }

        RefreshCountText();
    }

    public void HandleShowCardButtonPress()
    {
        this.listParent.DestroyAllChildren();

        foreach (var editNote in UIPackEditor.SelectedCardData.CardEditNotesDict)
        {
            var editNoteItem = Instantiate(this.editNotesItemPrefab, this.listParent);
            editNoteItem.Configure(UIPackEditor.SelectedCardData, editNote.Value);
        }

        RefreshCountText();
    }

    public void HandleClearAllButtonPress()
    {
        UIConfirmPanel.IN.Show("Are you sure you want to clear all Edit Notes?", () =>
        {
            foreach (var kvp in PlayerData.Data.PacksDict)
            {
                kvp.Value.CardDatas.ForEach(cardData => cardData.CardEditNotesDict = null);
            }

            PlayerData.Data.SetDirty();
            this.listParent.DestroyAllChildren();

            //fire event that hides all EditNoteButtons
            UIEditNotesPanel.OnPlayerDataChanged?.Invoke();

            Hide();
        });
    }

    public void HandleClearCardButtonPress()
    {
        UIConfirmPanel.IN.Show($"Are you sure you want to clear {this.selectedEditNoteButton} ?", () =>
        {
            UIPackEditor.SelectedCardData.CardEditNotesDict = null;
            PlayerData.Data.SetDirty();
            this.listParent.DestroyAllChildren();

            //fire event that hides all EditNoteButtons
            UIEditNotesPanel.OnPlayerDataChanged?.Invoke();
        });
    }

    private void RefreshCountText()
    {
        this.counter = 0;

        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            foreach (var cardData in kvp.Value.CardDatas)
            {
                if (cardData.CardEditNotesDict != null)
                    this.counter += cardData.CardEditNotesDict.Count;
            }
        }

        this.countText.text = this.counter.ToString();
    }
}