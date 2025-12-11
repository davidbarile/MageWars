using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Card;

public class UISpriteSelectPanel : UIPanelBase
{
    public static UISpriteSelectPanel IN;

    public static Action OnStatsVisibilityChanged;

    [SerializeField] private UISpriteSelectItem spriteSelectItemPrefab;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private TextMeshProUGUI titleText;
    [Space]
    [SerializeField] private GameObject showHideStatsButtons;
    [SerializeField] private GameObject showStatsButton;
    [SerializeField] private GameObject hideStatsButton;
    [Space]
    [SerializeField] private GameObject resourcesReqsButtons;
    [SerializeField] private CanvasGroup resourcesButton;
    [SerializeField] private CanvasGroup requirementsButton;

    [SerializeField] private RectTransform bgRectTrans;
    [Space, SerializeField] private GameObject showAllButtonText;
    [SerializeField] private GameObject showUsedButtonText;

    [Header("Manually Serialize All Sprites Here")]
    [SerializeField] private SpritesConfig spritesConfig;

    private List<string> allSpriteNames = new();
    private List<string> spriteItemNames;

    private int spriteIndex;

    private bool isViewOnly;

    private string permanentResourceSpriteName;

    private bool isPermanentResourceMode;

    public static Dictionary<EElement, ElementSpriteLookup> ElementsToSpritesDict = new();

    private bool isResources;

    private Action onResourceChanged;

    public void CreateElementSpriteDict()
    {
        foreach (var item in this.spritesConfig.ElementSpriteLookups)
        {
            if (!ElementsToSpritesDict.ContainsKey(item.Element))
                ElementsToSpritesDict.Add(item.Element, item);
            else
                Debug.LogError($"<color-red>ElementsToSpritesDict already contains entry for Element: {item.Element}!</color>");
        }
    }

    public void Show(List<string> inSpritesList, int inIndex, bool inIsViewOnly, bool inIsResources = true, Action inOnResourceChanged = null)
    {
        this.isPermanentResourceMode = false;
        this.grid.transform.DestroyAllChildren();

        this.spriteItemNames = inSpritesList;
        this.spriteIndex = inIndex;
        this.isViewOnly = inIsViewOnly;
        this.isResources = inIsResources;
        this.onResourceChanged = inOnResourceChanged;

        this.showHideStatsButtons.SetActive(!inIsViewOnly);
        this.resourcesReqsButtons.SetActive(inIsViewOnly);

        SetShowAllButtonText(PlayerData.Data.ShouldShowAllResources);

        if (inIsViewOnly)
        {
            SetStatsVisibility(true);
            this.bgRectTrans.offsetMin = new Vector2(212, 0);
            this.titleText.text = inIsResources ? "Resources Stats" : "Reqs Stats";
        }
        else
        {
            SetStatsVisibility(UISpriteSelectItem.ShouldShowCounts);
            this.bgRectTrans.offsetMin = new Vector2(0, 0);
            this.titleText.text = "Select Sprite";
        }

        var emptyItem = Instantiate(this.spriteSelectItemPrefab, this.grid.transform);
        emptyItem.SetSprite(null);
        emptyItem.OnClick = OnSpriteClicked;
        emptyItem.SetCountText(0, 0);

        if (!inIsResources)
        {
            for (int i = 0; i < this.spritesConfig.SpecialSprites.Count; i++)
            {
                var item = Instantiate(this.spriteSelectItemPrefab, this.grid.transform);
                var sprite = this.spritesConfig.SpecialSprites[i];

                item.SetSprite(sprite);
                item.OnClick = OnSpriteClicked;
                UISpriteSelectPanel.OnStatsVisibilityChanged += item.RefreshVisibility;

                float resourceCount = 0;
                var allResourcesCount = 0;

                if (inIsViewOnly)
                {
                    //from Pack Editor
                    for (int j = 0; j < UIPackEditor.IN.PackCardItems.Count; j++)
                    {
                        var deckItem = UIPackEditor.IN.PackCardItems[j];

                        if (deckItem.gameObject.activeSelf)
                        {
                            var cardData = deckItem.Card.Data;

                            var spriteNamesList = inIsResources ? cardData.ResourceNames : cardData.RequirementNames;

                            for (int k = 0; k < spriteNamesList.Count; k++)
                            {
                                var resource = CardsManager.GetResourceSpriteFromName(spriteNamesList[k]);

                                if (sprite != null && resource != null)
                                {
                                    if (resource.Equals(sprite))
                                        resourceCount += deckItem.Count;

                                    allResourcesCount += deckItem.Count;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //from Selected Card click
                    for (int j = 0; j < CardsManager.IN.ActivePacksCardDatas.Count; j++)
                    {
                        var cardData = CardsManager.IN.ActivePacksCardDatas[j];

                        for (int k = 0; k < cardData.ResourceNames.Count; k++)
                        {
                            var resource = CardsManager.GetResourceSpriteFromName(cardData.ResourceNames[k]);

                            if (sprite != null && resource != null)
                            {
                                if (resource.Equals(sprite))
                                    resourceCount += cardData.Count;

                                allResourcesCount += cardData.Count;
                            }
                        }
                    }
                }

                item.SetCountText(0, 0);

                item.gameObject.SetActive(PlayerData.Data.ShouldShowAllResources || resourceCount > 0);
            }
        }

        for (int i = 0; i < this.spritesConfig.AllSprites.Count; i++)
        {
            var item = Instantiate(this.spriteSelectItemPrefab, this.grid.transform);
            var sprite = CardsManager.GetResourceSpriteFromName(this.spritesConfig.AllSprites[i].name);

            item.SetSprite(sprite);
            item.OnClick = OnSpriteClicked;
            UISpriteSelectPanel.OnStatsVisibilityChanged += item.RefreshVisibility;

            float resourceCount = 0;
            var allResourcesCount = 0;

            if (inIsViewOnly)
            {
                //from Pack Editor
                for (int j = 0; j < UIPackEditor.IN.PackCardItems.Count; j++)
                {
                    var deckItem = UIPackEditor.IN.PackCardItems[j];

                    if (deckItem.gameObject.activeSelf)
                    {
                        var cardData = deckItem.Card.Data;

                        var spriteNamesList = inIsResources ? cardData.ResourceNames : cardData.RequirementNames;

                        for (int k = 0; k < spriteNamesList.Count; k++)
                        {
                            var spriteName = spriteNamesList[k];
                            var resource = CardsManager.GetResourceSpriteFromName(spriteName);

                            if (sprite != null && resource != null && !spriteName.Contains("@"))
                            {
                                if (resource.Equals(sprite))
                                    resourceCount += deckItem.Count;

                                allResourcesCount += deckItem.Count;
                            }
                        }
                    }
                }
            }
            else
            {
                //from Selected Card click
                for (int j = 0; j < CardsManager.IN.ActivePacksCardDatas.Count; j++)
                {
                    var cardData = CardsManager.IN.ActivePacksCardDatas[j];

                    var spriteNamesList = inIsResources ? cardData.ResourceNames : cardData.RequirementNames;

                    for (int k = 0; k < spriteNamesList.Count; k++)
                    {
                        var spriteName = spriteNamesList[k];
                        var resource = CardsManager.GetResourceSpriteFromName(spriteName);

                        if (sprite != null && resource != null && !spriteName.Contains("@"))
                        {
                            if (resource.Equals(sprite))
                                resourceCount += cardData.Count;

                            allResourcesCount += cardData.Count;
                        }
                    }
                }
            }

            var percent = allResourcesCount == 0 ? 0 : (resourceCount / allResourcesCount) * 100;
            item.SetCountText((int)resourceCount, percent);

            item.gameObject.SetActive(PlayerData.Data.ShouldShowAllResources || resourceCount > 0);
        }

        base.Show();
    }

    public void Show(string inPermResourceSpriteName, bool inIsViewOnly, Action inOnResourceChanged = null)
    {
        this.isPermanentResourceMode = true;
        this.grid.transform.DestroyAllChildren();

        this.permanentResourceSpriteName = inPermResourceSpriteName;
        this.isViewOnly = inIsViewOnly;
        this.onResourceChanged = inOnResourceChanged;

        this.showHideStatsButtons.SetActive(!inIsViewOnly);
        this.resourcesReqsButtons.SetActive(inIsViewOnly);

        SetShowAllButtonText(PlayerData.Data.ShouldShowAllResources);

        if (inIsViewOnly)
        {
            SetStatsVisibility(true);
            this.bgRectTrans.offsetMin = new Vector2(212, 0);//change
            this.titleText.text = "Permanent Resources Stats";
        }
        else
        {
            SetStatsVisibility(UISpriteSelectItem.ShouldShowCounts);
            this.bgRectTrans.offsetMin = new Vector2(0, 0);
            this.titleText.text = "Select Sprite";
        }

        var emptyItem = Instantiate(this.spriteSelectItemPrefab, this.grid.transform);
        emptyItem.SetSprite(null);
        emptyItem.OnClick = OnPermanentResourceSpriteClicked;
        emptyItem.SetCountText(0, 0);//change?

        for (int i = 0; i < this.spritesConfig.AllSprites.Count; i++)
        {
            var item = Instantiate(this.spriteSelectItemPrefab, this.grid.transform);
            var sprite = CardsManager.GetResourceSpriteFromName(this.spritesConfig.AllSprites[i].name);

            item.SetSprite(sprite);
            item.OnClick = OnPermanentResourceSpriteClicked;
            UISpriteSelectPanel.OnStatsVisibilityChanged += item.RefreshVisibility;

            float resourceCount = 0;
            var allResourcesCount = 0;

            if (inIsViewOnly)
            {
                //from Pack Editor
                for (int j = 0; j < UIPackEditor.IN.PackCardItems.Count; j++)
                {
                    var deckItem = UIPackEditor.IN.PackCardItems[j];

                    if (deckItem.gameObject.activeSelf)
                    {
                        var cardData = deckItem.Card.Data;

                        var spriteName = cardData.PermanentResourceName;

                        var permResource = CardsManager.GetResourceSpriteFromName(spriteName);

                        if (sprite != null && permResource != null)
                        {
                            if (permResource.Equals(sprite))
                                resourceCount += deckItem.Count;

                            allResourcesCount += deckItem.Count;
                        }
                    }
                }
            }
            else
            {
                //from Selected Card click
                for (int j = 0; j < CardsManager.IN.ActivePacksCardDatas.Count; j++)
                {
                    var cardData = CardsManager.IN.ActivePacksCardDatas[j];

                    var permanentResource = CardsManager.GetResourceSpriteFromName(cardData.PermanentResourceName);

                    if (sprite != null && permanentResource != null)
                    {
                        if (permanentResource.Equals(sprite))
                            resourceCount += cardData.Count;

                        allResourcesCount += cardData.Count;
                    }
                }
            }

            var percent = allResourcesCount == 0 ? 0 : (resourceCount / allResourcesCount) * 100;
            item.SetCountText((int)resourceCount, percent);

            item.gameObject.SetActive(PlayerData.Data.ShouldShowAllResources || resourceCount > 0);
        }

        base.Show();
    }

    public void OnSpriteClicked(Sprite inSprite)
    {
        if (this.isViewOnly) return;

        if (UIPackEditor.ShouldBlockEdit) return;

        var oldListValue = new List<string>();

        if (this.isResources)
            oldListValue = new List<string>(UIPackEditor.SelectedCardData.ResourceNames);
        else
            oldListValue = new List<string>(UIPackEditor.SelectedCardData.RequirementNames);

        if (this.spriteIndex >= this.spriteItemNames.Count)
        {
            this.spriteItemNames.Add(string.Empty);
            this.spriteIndex = this.spriteItemNames.Count - 1;
        }

        var isDirty = false;

        if (string.IsNullOrWhiteSpace(this.spriteItemNames[this.spriteIndex]) && inSprite != null)
            isDirty = true;
        else if (!string.IsNullOrWhiteSpace(this.spriteItemNames[this.spriteIndex]) && inSprite == null)
            isDirty = true;
        else if (inSprite != null && !this.spriteItemNames[this.spriteIndex].Equals(inSprite.name))
            isDirty = true;

        this.spriteItemNames[this.spriteIndex] = inSprite != null ? inSprite.name : string.Empty;

        //remove null elements
        var count = this.spriteItemNames.Count;

        for (int i = 0; i < count; i++)
        {
            if (this.spriteItemNames[i] == null)
            {
                this.spriteItemNames.RemoveAt(i);
                --count;
            }
        }

        if (this.isResources)
        {
            UIPackEditor.SelectedCardData.ResourceNames = new List<string>(this.spriteItemNames);

            if (isDirty)
                PlayerData.Data.SetEditNote(UIPackEditor.SelectedCardData, EEditNoteType.Resources.ToString(), oldListValue, new List<string>(this.spriteItemNames));
        }
        else
        {
            UIPackEditor.SelectedCardData.RequirementNames = new List<string>(this.spriteItemNames);

            if (isDirty)
                PlayerData.Data.SetEditNote(UIPackEditor.SelectedCardData, EEditNoteType.Requirements.ToString(), oldListValue, new List<string>(this.spriteItemNames));
        }

        if (isDirty)
        {
            this.onResourceChanged?.Invoke();

            UIEditNotesPanel.OnPlayerDataChanged?.Invoke();//is dirty check?
        }

        UIPackEditor.OnCardDataChanged?.Invoke();

        Hide();
    }

    public void OnPermanentResourceSpriteClicked(Sprite inSprite)
    {
        if (this.isViewOnly) return;

        if (UIPackEditor.ShouldBlockEdit) return;

        var oldValue = UIPackEditor.SelectedCardData.PermanentResourceName;

        var isDirty = false;

        if (string.IsNullOrWhiteSpace(this.permanentResourceSpriteName) && inSprite != null)
            isDirty = true;
        else if (!string.IsNullOrWhiteSpace(this.permanentResourceSpriteName) && inSprite == null)
            isDirty = true;
        else if (inSprite != null && !this.permanentResourceSpriteName.Equals(inSprite.name))
            isDirty = true;

        this.permanentResourceSpriteName = inSprite != null ? inSprite.name : string.Empty;


        UIPackEditor.SelectedCardData.PermanentResourceName = this.permanentResourceSpriteName;

        if (isDirty)
        {
            PlayerData.Data.SetEditNote(UIPackEditor.SelectedCardData, EEditNoteType.PermanentResource.ToString(), oldValue, this.permanentResourceSpriteName);
        }

        if (isDirty)
        {
            this.onResourceChanged?.Invoke();

            UIEditNotesPanel.OnPlayerDataChanged?.Invoke();
        }

        UIPackEditor.OnCardDataChanged?.Invoke();

        Hide();
    }

    public void SetStatsVisibility(bool inIsVisible)
    {
        UISpriteSelectItem.ShouldShowCounts = inIsVisible;

        OnStatsVisibilityChanged?.Invoke();

        this.showStatsButton.SetActive(!inIsVisible);
        this.hideStatsButton.SetActive(inIsVisible);
    }

    public void SetResourcesOrRequirements(bool inIsResources)
    {
        this.resourcesButton.alpha = inIsResources ? 1 : 0;
        this.resourcesButton.interactable = !inIsResources;

        this.requirementsButton.alpha = inIsResources ? 0 : 1;
        this.requirementsButton.interactable = inIsResources;

        var spriteNames = GetAllSpriteNamesList();

        Show(spriteNames, 0, true, inIsResources);
    }

    public List<string> GetAllSpriteNamesList()
    {
        if (this.allSpriteNames.Count == 0)
        {
            this.allSpriteNames = this.spritesConfig.SpecialSprites.Select(sprite => sprite.name).ToList();
            this.allSpriteNames.AddRange(this.spritesConfig.AllSprites.Select(sprite => sprite.name));
        }

        return this.allSpriteNames;
    }

    public override void Hide()
    {
        this.grid.transform.DestroyAllChildren();

        this.onResourceChanged = null;

        base.Hide();
    }

    public void HandleShowAllToggleChanged()
    {
        PlayerData.Data.ShouldShowAllResources = !PlayerData.Data.ShouldShowAllResources;
        PlayerData.Data.SetDirty();

        SetShowAllButtonText(PlayerData.Data.ShouldShowAllResources);

        if (this.isPermanentResourceMode)
            Show(this.permanentResourceSpriteName, this.isViewOnly, this.onResourceChanged);
        else
            Show(this.spriteItemNames, this.spriteIndex, this.isViewOnly, this.isResources, this.onResourceChanged);
    }

    private void SetShowAllButtonText(bool inShouldShowAll)
    {
        this.showAllButtonText.SetActive(!inShouldShowAll);
        this.showUsedButtonText.SetActive(inShouldShowAll);
    }
}