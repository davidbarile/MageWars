using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CardConfig;

public class UICardFront_Mage : UICardFrontBase
{
    [Space]
    [SerializeField] private Image alignmentFrame;

    [Space]
    [SerializeField] private TextMeshProUGUI[] cardNameTexts;
    [SerializeField] private TextMeshProUGUI alignmentText, statsText, descriptionText;

    [Space]
    [Range(0, 20), SerializeField] private float statsTextSize = 19.0f;
    [Range(0, 20), SerializeField] private float winConditionTextSize = 19.0f;

    [Space]
    [SerializeField] private Image illustrationImage;

    [Space]
    [SerializeField] private UIResourceItem[] requirementItems;
    [SerializeField] private GameObject requirementsDisplay;
    [SerializeField] private GameObject colorPatch;

    [Space]
    [SerializeField] private UIResourceItem permanentResource;

    public override void RefreshDisplay(CardData inData)
    {
        foreach (var text in this.cardNameTexts)
            text.text = inData.CardName;

        if (this.illustrationImage != null)
            this.illustrationImage.sprite = CardsManager.GetSpriteFromName(inData.CardType, inData.IllustrationSpriteName);

        //this.alignmentText.text = inData.Alignment.ToString();
        //this.alignmentText.text = $"{inData.CardSubtitle} (<i>{inData.Alignment}</i>)";
        this.alignmentText.text = $"{inData.CardSubtitle}";

        switch (inData.Alignment)
        {
            case EAlignment.Evil:
                this.alignmentFrame.color = Color.black;
                this.alignmentText.color = Color.white;
                break;

            case EAlignment.Good:
                this.alignmentFrame.color = Color.white;
                this.alignmentText.color = Color.black;
                break;

            default:
                this.alignmentFrame.color = Color.white;
                this.alignmentText.color = Color.black;
                break;
        }

        SetBgGradientColors(inData.Alignment);

        var indent = "     ";
        var statSizeStart = $"<size={this.statsTextSize}>";
        var winCondSizeStart = $"<size={this.winConditionTextSize}>";

        var sizeEnd = "</size>";

        var drawFate = $"<sprite name=\"Card_Fate_Draw>x{inData.DrawCardsPerTurn}";
        var maxHand = $"<sprite name=\"MaxHand>x{inData.MaxCardsInHand}";

        var maxHealth = CreateStringFromHealthCards(inData.MaxHealth, inData.HealthCards);
        var maxArmor = $"<sprite name=\"Armor\">{inData.MaxArmor}";
        var maxPets = $"<sprite name=\"Beast4\">{inData.MaxPets}";
        var maxPlants = $"<sprite name=\"Plant4\">{inData.MaxPlants}";
        var maxItems = $"<sprite name=\"Swords\">{inData.MaxItems}";
        var winCondition = $"<sprite name=\"Trophy\">{winCondSizeStart}{inData.WinCondition}{sizeEnd}";
        var elementIcons = LocalizationManager.GetElementsString(inData.Elements);

        //this.statsText.text = $"{drawFate} Draw/Turn{indent}{maxHand} Max Hand\n{maxHealth}{indent}{maxArmor}\n{maxPets}{indent}{maxPlants}{indent}{maxItems}{indent}{elementIcons}\n{winCondition}";
        this.statsText.text = $"{drawFate} {statSizeStart}Draw/Turn{sizeEnd}{indent}{maxHand} {statSizeStart}in Hand{sizeEnd}\n{maxHealth}\n{maxArmor}{indent}{maxPets}{indent}{maxPlants}{indent}{maxItems}\n{winCondition}";

        this.descriptionText.text = inData.Description;
        this.descriptionText.horizontalAlignment = inData.DescriptionAlignment;

        if (inData.DescriptionFontSize > 0)
            this.descriptionText.fontSize = inData.DescriptionFontSize;

        var hasRequirements = inData.RequirementNames.Count > 0;

        //(<sprite name="Card_Enchanted_x4">,<sprite name="Card_Mystic_x2">,<sprite name="Card_Fate_x1">)
        if (this.requirementsDisplay != null)
            this.requirementsDisplay.SetActive(hasRequirements);

        if (this.colorPatch != null)
            this.colorPatch.SetActive(hasRequirements);

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

        string CreateStringFromHealthCards(int inMaxHealth, int[] inHealthCards)
        {
            string healthString = $"<sprite name=\"Heart\">{inMaxHealth}";

            var sum = inHealthCards[0] + inHealthCards[1] + inHealthCards[2] + inHealthCards[3];

            if (sum > 0)
                healthString += "  {";

            if (inHealthCards[0] > 0)
                healthString += $"<sprite name=\"Card_Enchanted\">{inHealthCards[0]}";

            if (inHealthCards[0] > 0 && inHealthCards[1] > 0)
                healthString += $"  ";

            if (inHealthCards[1] > 0)
                healthString += $"<sprite name=\"Card_Mystic\">{inHealthCards[1]}";

            if (inHealthCards[0] + inHealthCards[1] > 0 && inHealthCards[2] > 0)
                healthString += $"  ";

            if (inHealthCards[2] > 0)
                healthString += $"<sprite name=\"Card_Arcane\">{inHealthCards[2]}";

            if (inHealthCards[0] + inHealthCards[1] + inHealthCards[2] > 0 && inHealthCards[3] > 0)
                healthString += $"  ";

            if (inHealthCards[3] > 0)
                healthString += $"<sprite name=\"Card_Quest\">{inHealthCards[3]}";

            if (sum > 0)
                healthString += "}";

            return healthString;
        }
    }

    public override void MakeEmptyResourcesAndReqs(CardData inData)
    {
        //Debug.Log($"MAGE.MakeEmptyResourcesAndReqs() {name}. data Name = {inData.Name}   CardName = {inData.CardName}");
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

        if (this.permanentResource != null)
        {
            this.permanentResource.gameObject.SetActive(true);
            var iconSprite = CardsManager.GetResourceSpriteFromName("Empty");
            this.permanentResource.SetSprite(iconSprite);
        }
    }
}
