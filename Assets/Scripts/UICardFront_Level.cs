using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public class UICardFront_Level : UICardFrontBase
{
    [Space]
    [SerializeField] private TextMeshProUGUI[] cardNameTexts;
    [SerializeField] private TextMeshProUGUI cardTypeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Space]
    [SerializeField] private Image illustrationImage;

    [Space]
    [SerializeField] private UIResourceItem[] resourceItems;

    [Space]
    [SerializeField] private UIResourceItem[] requirementItems;
    [SerializeField] private GameObject requirementsDisplay;

    [Space]
    [SerializeField] private UIResourceItem permanentResource;

    public override void RefreshDisplay(CardData inData)
    {
        foreach (var text in this.cardNameTexts)
            text.text = inData.CardName;

        this.cardTypeText.text = $"Level {inData.Level}";
        this.levelText.text = inData.Level.ToString();

        this.descriptionText.text = inData.Description;
        this.descriptionText.horizontalAlignment = inData.DescriptionAlignment;

        if (inData.DescriptionFontSize > 0)
            this.descriptionText.fontSize = inData.DescriptionFontSize;

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

        if (this.permanentResource != null)
        {
            var shouldShow = !string.IsNullOrWhiteSpace(inData.PermanentResourceName);
            this.permanentResource.gameObject.SetActive(shouldShow);
            if (shouldShow)
            {
                var iconSprite = CardsManager.GetResourceSpriteFromName(inData.PermanentResourceName);
                this.permanentResource.SetSprite(iconSprite);
            }
        }
    }

    public override void MakeEmptyResourcesAndReqs(CardData inData)
    {
        //Debug.Log($"LEVEL.MakeEmptyResourcesAndReqs() {name}. data Name = {inData.Name}   CardName = {inData.CardName}");
        for (int i = 0; i < this.resourceItems.Length; i++)
        {
            var shouldShow = i < 5;

            var item = this.resourceItems[i];

            item.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                var iconSprite = CardsManager.GetResourceSpriteFromName("Empty");
                item.SetSprite(iconSprite);
            }
        }

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

        if (this.permanentResource != null)
        {
            this.permanentResource.gameObject.SetActive(true);
            var iconSprite = CardsManager.GetResourceSpriteFromName("Empty");
            this.permanentResource.SetSprite(iconSprite);
        }
    }
}
