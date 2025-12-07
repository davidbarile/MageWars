using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIPackListPanel : UIPanelBase
{
    public static UIPackListPanel IN;

    public static Action OnActivePacksChanged;

    [SerializeField] private TextMeshProUGUI totalActiveCountText;
    [SerializeField] private VerticalLayoutGroup content;
    [SerializeField] private UIPackListItem packItemPrefab;

    [Space]
    [SerializeField] private UIImportPackPopup importPackPopup;

      [Space]
    [SerializeField] private GameObject savePacksButton;

    private int totalActiveCount;

    public override void Show()
    {
        UIPackListPanel.OnActivePacksChanged += UpdateTotalActiveCount;
        base.Show();

        this.importPackPopup.Hide();

#if !UNITY_EDITOR
        this.savePacksButton.SetActive(false);
#endif

        RefreshList();
    }

    private void UpdateTotalActiveCount()
    {
        this.totalActiveCount = 0;

        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            if (kvp.Value.IsActiveInGame)
                this.totalActiveCount += kvp.Value.GetTotalCardsCount();
        }

        this.totalActiveCountText.text = this.totalActiveCount.ToString();
    }

    public void SpawnItem(Pack inPack)
    {
        var go = Instantiate(this.packItemPrefab.gameObject, this.content.transform);
        var packListItem = go.GetComponent<UIPackListItem>();
        packListItem.Configure(inPack);
    }

    public Pack CopyPack(Pack inPackToCopy)
    {
        var pack = Pack.Clone(inPackToCopy);

        var index = pack.Name.LastIndexOf(" (");
        var nameRoot = index > 0 ? inPackToCopy.Name.Substring(0, index) : inPackToCopy.Name;

        int counter = 0;
        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            if (kvp.Value.Name.Contains(nameRoot))
                ++counter;
        }

        if (counter > 0)
            pack.Name = $"{nameRoot} ({counter})";

        SpawnItem(pack);

        PlayerData.Data.PacksDict.Add(pack.ID, pack);

        UIPackListPanel.OnActivePacksChanged?.Invoke();

        PlayerData.Data.SetDirty();

        return pack;
    }

    public override void Hide()
    {
        UIPackListPanel.OnActivePacksChanged -= UpdateTotalActiveCount;

        PlayerData.Data.SetDirty();//must refresh decks so Attack Numbers update

        if (UICardBacksPanel.IN.IsShowing)
            UICardBacksPanel.IN.Show();

        this.content.transform.DestroyAllChildren<UIPackListItem>();

        base.Hide();
    }

    public void RefreshList()
    {
        this.content.transform.DestroyAllChildren<UIPackListItem>();

        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            SpawnItem(kvp.Value);
        }

        UIPackListPanel.OnActivePacksChanged?.Invoke();
    }

    public void HandleNewPackButtonPress()
    {
        var pack = Pack.Clone(DeckManager.IN.DefaultPack);

        foreach (var cardData in pack.CardDatas)
        {
            cardData.ClearDeckCounts();
        }

        int counter = 0;

        var newPackName = "New Pack";

        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            if (kvp.Value.Name.Equals(newPackName) || (kvp.Value.Name.Contains(newPackName) && kvp.Value.Name.Contains("(")))
                ++counter;
        }

        if (counter > 0)
            pack.Name = $"{newPackName} ({counter})";
        else
            pack.Name = newPackName;

        SpawnItem(pack);

        UIPackListPanel.OnActivePacksChanged?.Invoke();

        PlayerData.Data.PacksDict.Add(pack.ID, pack);
        PlayerData.Data.SetDirty();
    }

    public void HandleImportPackButtonPress()
    {
        this.importPackPopup.Show();
    }

    public void HandleSavePacksButtonPress()
    {
#if UNITY_EDITOR
        UIConfirmPanel.IN.Show("Save Packs", "Are you sure you want to save Packs to Config File?\n\n(This will overwrite the previous config file)", () =>
        {
            SaveManager.IN.SaveDecksToConfig();
        });
#endif
    }
}