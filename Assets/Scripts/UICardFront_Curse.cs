using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public class UICardFront_Curse : UICardFrontBase
{
    [Space]
    [SerializeField] private TextMeshProUGUI[] cardNameTexts;
    [SerializeField] private TextMeshProUGUI[] cardTypeTexts;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Space]
    [SerializeField] private Image illustrationImage;

    [Space]
    [SerializeField] private UIResourceItem[] resourceItems;
    [Space]
    [SerializeField] private UIResourceItem[] requirementItems;
    [SerializeField] private GameObject requirementsDisplay;

    public override void RefreshDisplay(CardData inData)
    {
        foreach (var text in this.cardNameTexts)
            text.text = inData.CardName;

        foreach (var text in this.cardTypeTexts)
            text.text = !string.IsNullOrWhiteSpace(inData.CardSubtitle) ? inData.CardSubtitle : inData.CardType.ToString();

        this.levelText.text = inData.Level.ToString();

        this.descriptionText.text = inData.Description;
        this.descriptionText.horizontalAlignment = inData.DescriptionAlignment;

        if (inData.DescriptionFontSize > 0)
            this.descriptionText.fontSize = inData.DescriptionFontSize;

        if (inData.ShouldGenerateDescription)
            this.descriptionText.text = GenerateDescription(inData);

        if (this.illustrationImage != null)
            this.illustrationImage.sprite = CardsManager.GetSpriteFromName(inData.CardType, inData.IllustrationSpriteName);

        for (int i = 0; i < this.resourceItems.Length; i++)
        {
            var shouldShow = i < inData.ResourceNames.Count;

            var item = this.resourceItems[i];

            item.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                var iconSprite = CardsManager.GetResourceSpriteFromName(inData.ResourceNames[i]);
                item.SetSprite(iconSprite);
            }
        }

        var hasRequirements = inData.RequirementNames.Count > 0;

        if (this.requirementsDisplay != null)
            this.requirementsDisplay.SetActive(hasRequirements);

        for (int i = 0; i < this.requirementItems.Length; i++)
        {
            var shouldShow = i < inData.RequirementNames.Count;

            var item = this.requirementItems[i];

            item.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                var iconSprite = CardsManager.GetResourceSpriteFromName(inData.RequirementNames[i]);
                item.SetSprite(iconSprite);
            }
        }
    }

    private string GenerateDescription(CardData inCardData)
    {
        var descriptionText = inCardData.Description;

        switch (inCardData.CardType)
        {
            case ECardType.Fate:
                descriptionText = $"Draw {inCardData.DrawCardsPerTurn} cards per turn";
                break;
        }

        return descriptionText;
    }

    public override void MakeEmptyResourcesAndReqs(CardData inData)
    {
        //Debug.Log($"CURSE.MakeEmptyResourcesAndReqs() {name}. data Name = {inData.Name}   CardName = {inData.CardName}");
        // for (int i = 0; i < this.resourceItems.Length; i++)
        // {
        //     var shouldShow = i < 5;

        //     var item = this.resourceItems[i];

        //     item.gameObject.SetActive(shouldShow);

        //     if (shouldShow)
        //     {
        //         var iconSprite = CardsManager.GetResourceSpriteFromName("Empty");
        //         item.SetSprite(iconSprite);
        //     }
        // }

        if (this.requirementsDisplay != null)
            this.requirementsDisplay.SetActive(true);

        for (int i = 0; i < this.requirementItems.Length; i++)
        {
            var shouldShow = i < 5;

            var item = this.requirementItems[i];

            item.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                var iconSprite = CardsManager.GetResourceSpriteFromName("Empty");
                item.SetSprite(iconSprite);
            }
        }

        // if (this.permanentResource != null)
        // {
        //     this.permanentResource.gameObject.SetActive(true);
        //     var iconSprite = CardsManager.GetResourceSpriteFromName("Empty");
        //     this.permanentResource.SetSprite(iconSprite);
        // }
    }
}
