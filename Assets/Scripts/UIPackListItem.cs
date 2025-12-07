using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPackListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI packNameText, cardCountText, editButtonText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject inputFieldPopup;
    [SerializeField] private GameObject[] inputButtons;
    [SerializeField] private CanvasGroup activeCheckboxCanvasGroup;
    [Space]
    [SerializeField] private Button deleteButton, renameButton, editButton;
    [Space]
    [SerializeField] private Image background;
    [SerializeField] private GameObject activeCheckmark;
    [SerializeField] private Color[] backgroundColors;
    [SerializeField] private Color[] textColors;
    [SerializeField] private Color editButtonActiveColor, buttonInactiveColor;

    [Space, SerializeField] private EditedNoteButton editedNoteButton;

    public int Count { get; private set; }

    private Pack pack;
    private bool isEditingPack, isDefaultPack;

    public void Configure(Pack inPack)
    {
        this.pack = inPack;
        this.activeCheckmark.SetActive(inPack.IsActiveInGame);

        HideInputField();

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        this.inputField.onEndEdit.AddListener(delegate
        {
            OnInputSubmit(this.inputField.text);
        });

        foreach (var button in this.inputButtons)
        {
            button.SetActive(false);
        }   
#endif

        this.isEditingPack = inPack.ID.Equals(UIPackEditor.IN.EditingPack.ID);
        this.isDefaultPack = inPack.IsDefault;

        this.background.color = this.isEditingPack ? this.backgroundColors[0] : this.backgroundColors[1];

        this.deleteButton.gameObject.SetActive(!this.isDefaultPack);
        this.renameButton.gameObject.SetActive(!this.isDefaultPack);
        this.editButton.interactable = !this.isEditingPack;
        // this.activeCheckboxCanvasGroup.alpha = this.isDefaultPack ? 0.5f : 1f;
        // this.activeCheckboxCanvasGroup.interactable = !this.isDefaultPack;
        this.editButtonText.text = this.isDefaultPack ? "View" : "Edit";

        SetPackNameText(inPack.Name);

        this.Count = inPack.GetTotalCardsCount();

        this.cardCountText.text = this.Count.ToString();

        UIEditNotesPanel.OnPlayerDataChanged += RefreshEditedNotesButton;
        RefreshEditedNotesButton();
    }

    private void SetPackNameText(string inName)
    {
        this.packNameText.text = this.isEditingPack ? $"* {inName}" : inName;

        if (this.isEditingPack)
            this.packNameText.color = this.textColors[0];
        else if (this.isDefaultPack)
            this.packNameText.color = this.textColors[1];
        else
            this.packNameText.color = this.textColors[2];
    }

    //called from checkbox button press
    public void SetPackActive(bool inIsActive)
    {
        this.activeCheckmark.SetActive(inIsActive);
        this.pack.IsActiveInGame = inIsActive;
        PlayerData.Data.SetDirty();
        UIPackListPanel.OnActivePacksChanged?.Invoke();
    }

    public void HandleDeleteButtonPress()
    {
        var messageText = $"Are you sure you want to delete\n<color=#FFFFFF><i>{this.pack.Name}</i></color>?";
        UIConfirmPanel.IN.Show(messageText, () =>
        {
            PlayerData.Data.PacksDict.Remove(this.pack.ID);
            PlayerData.Data.SetDirty();

            Destroy(this.gameObject);

            UIPackListPanel.IN.RefreshList();

            if (this.isEditingPack)
                UIPackEditor.IN.DisplayPack(DeckManager.IN.DefaultPack);
        });
    }

    public void HandleRenameButtonPress()
    {
        this.inputFieldPopup.SetActive(true);
        this.inputField.Select();
        this.inputField.text = this.pack.Name;
    }

    public void OnInputSubmit(string inValue)
    {
        var enteredText = this.inputField.text;

        var index = pack.Name.LastIndexOf(" (");
        var nameRoot = index > 0 ? this.pack.Name.Substring(0, index) : this.pack.Name;

        if (enteredText.Length > 0 && !enteredText.Equals(this.pack.Name))
        {
            int counter = 0;
            var shouldAddCounter = false;
            foreach (var kvp in PlayerData.Data.PacksDict)
            {
                if (kvp.Value.Name.Equals(enteredText) || kvp.Value.Name.Contains("("))
                    ++counter;

                if (kvp.Value.Name.Equals(enteredText))
                    shouldAddCounter = true;
            }

            if (shouldAddCounter)
                enteredText = $"{enteredText} ({counter})";

            this.pack.Name = enteredText;
            SetPackNameText(enteredText);

            if (this.isEditingPack)
                UIPackEditor.IN.EditingPackNameText.text = enteredText;

            HideInputField();
            PlayerData.Data.SetDirty();
        }
        else
            HideInputField();
    }

    public void HideInputField()
    {
        this.inputFieldPopup.SetActive(false);
    }

    public void HandleCopyButtonPress()
    {
        UIPackListPanel.IN.CopyPack(this.pack);
    }

    public void HandleEditButtonPress()
    {
        if (this.isDefaultPack)
            UIPackEditor.IN.SetPasteButtonVisibility(false);
        else
            UIPackEditor.IN.SetPasteButtonVisibility(UIPackEditor.ClipboardCardData != null);

        UIPackEditor.IN.DisplayPack(this.pack);
        UIPackListPanel.IN.Hide();
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (this.inputField != null)
            this.inputField.onEndEdit.RemoveAllListeners();
#endif

        UIEditNotesPanel.OnPlayerDataChanged -= RefreshEditedNotesButton;
    }

    private void RefreshEditedNotesButton()
    {
        var hasEditNotes = false;
        var hasComment = false;
        foreach (var cardData in this.pack.CardDatas)
        {
            if (cardData.CardEditNotesDict != null && cardData.CardEditNotesDict.Count > 0)
            {
                hasEditNotes = true;
                foreach (var kvp in cardData.CardEditNotesDict)
                {
                    if (!string.IsNullOrEmpty(kvp.Value.Comment))
                    {
                        hasComment = true;
                        break;
                    }
                }

                if (hasComment)
                    break;
            }
        }

        this.editedNoteButton.SetVisibility(hasEditNotes);
        this.editedNoteButton.SetIconColor(hasComment);
    }
}