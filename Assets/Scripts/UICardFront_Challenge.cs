using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public class UICardFront_Challenge : UICardFrontBase
{
    [Space]
    [SerializeField] private TextMeshProUGUI[] cardNameTexts;
    [SerializeField] private TextMeshProUGUI cardTypeText, levelText, descriptionText, rewardText, consequenceText;

    [Space]
    [SerializeField] private Image illustrationImage;

    [Space]
    [SerializeField] private UIResourceItem[] resourceItems;
    // [SerializeField] private GameObject requirementsDisplay;

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

        this.descriptionText.text = inData.Description;
        this.rewardText.text = inData.Reward;
        this.consequenceText.text = inData.Consequence;

        if (this.illustrationImage != null)
            this.illustrationImage.sprite = CardsManager.GetSpriteFromName(inData.CardType, inData.IllustrationSpriteName);

        // var hasRequirements = inData.RequirementNames.Count > 0;
        // if (this.requirementsDisplay != null)
        //     this.requirementsDisplay.SetActive(hasRequirements);

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
    }

    public override void MakeEmptyResourcesAndReqs(CardData inData)
    {
        //Debug.Log($"CHALLENGE.MakeEmptyResourcesAndReqs() {name}. data Name = {inData.Name}   CardName = {inData.CardName}");
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