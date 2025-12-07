using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public class UICardFront_Fate : UICardFrontBase
{
    [Space]
    [SerializeField] private TextMeshProUGUI[] cardNameTexts;
    [SerializeField] private TextMeshProUGUI cardTypeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Space]
    [SerializeField] private Image illustrationImage;
    [SerializeField] private Image secondaryImage;

    [Space]
    [SerializeField] private UIResourceItem[] resourceItems;

    [Space]
    [SerializeField] private UIResourceItem[] requirementItems;

    public override void RefreshDisplay(CardData inData)
    {
        foreach (var text in this.cardNameTexts)
            text.text = inData.CardName;

        this.cardTypeText.text = !string.IsNullOrWhiteSpace(inData.CardSubtitle) ? inData.CardSubtitle : inData.CardType.ToString();
        this.levelText.text = inData.Level.ToString();

        this.descriptionText.text = inData.Description;
        this.descriptionText.horizontalAlignment = inData.DescriptionAlignment;

        if (inData.DescriptionFontSize > 0)
            this.descriptionText.fontSize = inData.DescriptionFontSize;

        var isValidIllustrationSprite = !string.IsNullOrWhiteSpace(inData.IllustrationSpriteName);
        this.illustrationImage.gameObject.SetActive(isValidIllustrationSprite);

        var isValidSecondarySprite = !string.IsNullOrWhiteSpace(inData.SecondarySpriteName);
        this.secondaryImage.gameObject.SetActive(isValidSecondarySprite);

        //if both are null, hide it so description text can fill the space
        this.illustrationImage.transform.parent.gameObject.SetActive(isValidIllustrationSprite || isValidSecondarySprite);

        if (isValidIllustrationSprite)
            this.illustrationImage.sprite = CardsManager.GetSpriteFromName(inData.CardType, inData.IllustrationSpriteName);

        if (isValidSecondarySprite)
            this.secondaryImage.sprite = CardsManager.GetSpriteFromName(inData.CardType, inData.SecondarySpriteName);


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

    public override void MakeEmptyResourcesAndReqs(CardData inData)
    {
        //Debug.Log($"FATE.MakeEmptyResourcesAndReqs() {name}. data Name = {inData.Name}   CardName = {inData.CardName}");
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

        // if (this.requirementsDisplay != null)
        //     this.requirementsDisplay.SetActive(true);

        // for (int i = 0; i < this.requirementItems.Length; i++)
        // {
        //     var shouldShow = i < 5;

        //     var item = this.requirementItems[i];

        //     item.gameObject.SetActive(shouldShow);

        //     if (shouldShow)
        //     {
        //         var iconSprite = CardsManager.GetResourceSpriteFromName("Empty");
        //         item.SetSprite(iconSprite);
        //     }
        // }

        // if (this.permanentResource != null)
        // {
        //     this.permanentResource.gameObject.SetActive(true);
        //     var iconSprite = CardsManager.GetResourceSpriteFromName("Empty");
        //     this.permanentResource.SetSprite(iconSprite);
        // }
    }
}
