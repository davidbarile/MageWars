using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using TMPro;
using Sirenix.OdinInspector;

public class UIImportPackPopup : UIPanelBase
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI outputText;

    [ReadOnly]
    [SerializeField] private ExportItem exportItem;

    public string ImportPackJsonText;

    public override void Show()
    {
        this.inputField.text = string.Empty;
        this.ImportPackJsonText = string.Empty;
        ClearOutputText();
        this.exportItem = new ExportItem();

        base.Show();
    }

    public void HandleSubmitButtonPress()
    {
        ClearOutputText();

        if (this.inputField.text.Length <= 0 && string.IsNullOrEmpty(this.ImportPackJsonText))
        {
            SetOutputTextThenHide("Input field is empty!");
            return;
        }

        try
        {
            var deserializeSettings = new DeserializeSettings()
            {
                RequireAllFieldsArePopulated = false
            };

            var jsonTextToParse = string.IsNullOrEmpty(this.ImportPackJsonText) ? this.inputField.text : this.ImportPackJsonText;
            this.ImportPackJsonText = string.Empty;

            JSON jsonObject = JSON.ParseString(jsonTextToParse);
            this.exportItem = jsonObject.Deserialize<ExportItem>(deserializeSettings);

            var deck = Pack.Clone(DeckManager.IN.DefaultPack);

            deck.Name = this.exportItem.Name;
            deck.ID = UnityEngine.Random.Range(9, 9999999).ToString();

            int counter = 0;
            foreach (var kvp in PlayerData.Data.PacksDict)
            {
                if (kvp.Value.Name.Contains(deck.Name))
                    ++counter;
            }

            if (counter > 0)
                deck.Name = $"{deck.Name} ({counter})";

            deck.CardDatas = new List<CardData>(this.exportItem.Cards);

            UIPackListPanel.IN.SpawnItem(deck);
            UIPackListPanel.OnActivePacksChanged?.Invoke();

            PlayerData.Data.PacksDict.Add(deck.ID, deck);

            PlayerData.Data.SetDirty();

            this.inputField.text = string.Empty;

            Hide();
        }
        catch
        {
            SetOutputTextThenHide("Invalid JSON Format!");
        }
    }

    public void SetOutputTextThenHide(string text)
    {
        this.outputText.text = text;
        Invoke(nameof(ClearOutputText), 3);
    }

    private void ClearOutputText()
    {
        this.outputText.text = string.Empty;
    }
}