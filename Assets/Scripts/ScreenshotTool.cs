using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using static DeckConfig;

public class ScreenshotTool : MonoBehaviour
{
    private enum EBgMode
    {
        Transparent,
        Colored
    }

    private enum ECaptureMode
    {
        SingleCard,
        PageOfCards_3x3,
        PageOfCards_6x3
    }

    private enum EImageFormat
    {
        png,
        jpg
    }

    [SerializeField] private string path = "Assets/Captures/";
    [SerializeField] private int fileCounter = -1;
    [SerializeField] private EImageFormat imageFormat;
    [Space]
    [SerializeField] private bool makeEmptyResourcesAndReqs;
    [SerializeField] private bool printOnlyCardFronts;
    [Space]
    [SerializeField] private Camera captureCamera;
    [Space]
    [SerializeField] private RenderTexture singleCardRenderTexture;
    [SerializeField] private RenderTexture cardPageRenderTexture_3x3;
    [SerializeField] private RenderTexture cardPageRenderTexture_6x3;
    [Header("If serialized, CardsToScreenShot will be overridden)")]
    [SerializeField] private CardsManager cardsManager;
    [Space]
    public List<CardConfig> CardsToScreenShot = new();
    [Space]
    [SerializeField] private Transform singleCardParent;
    [SerializeField] private GridLayoutGroup pageCardGrid;
    [SerializeField] private Image bgImage;
    [Space]
    [SerializeField] private bool printOneOfEachUnique;
    [SerializeField] private bool isCardBack;
    [Space]
    [SerializeField] private EBgMode bgMode;
    [SerializeField] private Color bgColor = Color.white;
    [Space]
    [SerializeField] private float cardScale = 2.5f;
    [Space]
    [SerializeField] private int maxCardsToCapture = -1;

    [SerializeField] private ECaptureMode captureMode = ECaptureMode.PageOfCards_3x3;

    private void Start()
    {
        this.singleCardParent.DestroyAllChildren();

        if (this.cardsManager != null)
            this.CardsToScreenShot = this.cardsManager.AllCardConfigs;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            this.captureMode = ECaptureMode.SingleCard;
            CaptureImage();
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            this.captureMode = ECaptureMode.PageOfCards_3x3;
            CaptureActivePacksCardsPage();
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            this.captureMode = ECaptureMode.PageOfCards_6x3;
            CaptureActivePacksCardsPage();
        }

        this.bgImage.enabled = this.bgMode != EBgMode.Transparent;
        this.bgImage.color = this.bgColor;
    }

    //[Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
    private void CaptureImage()
    {
        if (!Application.isPlaying) return;

        var fileName = "Card";
        CaptureImage(fileName);
    }

    [Button(ButtonSizes.Large), GUIColor(0, .9f, .2f)]
    private void CaptureActivePacksCardsPage()
    {
        if (!Application.isPlaying) return;

        this.gameObject.SetActive(true);

        StartCoroutine(CaptureActivePacksCardsCo());
    }

    private void CaptureImage(string inFileName)
    {
        if (!Application.isPlaying) return;

        var cam = this.captureCamera;

        switch (this.captureMode)
        {
            case ECaptureMode.SingleCard:
                cam.targetTexture = this.singleCardRenderTexture;
                break;
            case ECaptureMode.PageOfCards_3x3:
                cam.targetTexture = this.cardPageRenderTexture_3x3;
                break;
            case ECaptureMode.PageOfCards_6x3:
                cam.targetTexture = this.cardPageRenderTexture_6x3;
                break;
        }

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        cam.Render();

        var tex2D = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGBA64, false, true);
        tex2D.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);

        //fixes bug where image renders too dark - but SLOW
        var pixels = tex2D.GetPixels();
        for (int p = 0; p < pixels.Length; p++)
        {
            pixels[p] = pixels[p].gamma;
        }
        tex2D.SetPixels(pixels);

        tex2D.Apply();
        RenderTexture.active = currentRT;

        var bytes = tex2D.EncodeToPNG();
        DestroyImmediate(tex2D);

        var path = $"{this.path}{inFileName}.{imageFormat.ToString().ToLower()}";

        if (this.fileCounter >= 0)
        {
            path = $"{this.path}{inFileName}_{this.fileCounter}.{imageFormat.ToString().ToLower()}";
            this.fileCounter++;
        }

        File.WriteAllBytes(path, bytes);

        Debug.Log($"<color=#AAA>- Captured: {inFileName}</color>");
    }

    private IEnumerator CaptureActivePacksCardsCo()
    {
        this.singleCardParent.DestroyAllChildren();
        this.pageCardGrid.transform.DestroyAllChildren();

        this.singleCardParent.gameObject.SetActive(this.captureMode == ECaptureMode.SingleCard);
        this.pageCardGrid.gameObject.SetActive(this.captureMode != ECaptureMode.SingleCard);

        this.pageCardGrid.constraintCount = this.captureMode == ECaptureMode.PageOfCards_3x3 ? 3 : 6;

        var maxCardsPerCapture = this.captureMode == ECaptureMode.PageOfCards_3x3 ? 9 : 18;

        var allCards = new List<Card>();

        var captureCounter = 0;

        Debug.Log($"<color=white>Starting Screen Capture.  Please wait until finished.</color>");

        var cardDataList = CardsManager.IN.ActivePacksCardDatas;

        foreach (var cardData in cardDataList)
        {
            for (int i = 1; i < cardData.DeckCounts.Length; ++i)
            {
                var deckType = (EDeckType)i;
                var numCardsInDeck = cardData.DeckCounts[i];

                var deckAllowsAttackNumbers = deckType == EDeckType.Enchanted ||
                deckType == EDeckType.Mystic || deckType == EDeckType.Arcane ||
                deckType == EDeckType.Quest;

                if (numCardsInDeck < 1)
                    continue;

                if (this.printOneOfEachUnique)
                    numCardsInDeck = 1;

                for (int j = 0; j < numCardsInDeck; j++)
                {
                    if (Input.GetKey(KeyCode.Escape))
                    {
                        Debug.Log($"<color=red>Screen Capture Aborted!   {i}/{cardDataList.Count}</color>");
                        yield break;
                    }

                    var card = CardsManager.SpawnCard(cardData, this.pageCardGrid.transform, true);

                    card.transform.localScale = Vector3.one * this.cardScale;
                    card.Configure(cardData);
                    card.SpawnCardBack(deckType);

                    if (deckAllowsAttackNumbers)
                    {
                        var deckIndex = UICardBacksPanel.GetDeckIndex(deckType);
                        card.CardBack.SetAttackNumber(cardData.AttackNumbersObjects[deckIndex].AttackNumValues[j]);
                    }

                    card.SetDontFlipCornerVisible(false);
                    card.SetFlipCornerVisible(false);

                    card.CardFlip.SetAnimStateToFront();

                    if (this.makeEmptyResourcesAndReqs)
                        card.CardFront.MakeEmptyResourcesAndReqs(cardData);

                    allCards.Add(card);

                    yield return null;

                    card.gameObject.SetActive(false);
                }

                ++captureCounter;

                if (captureCounter == this.maxCardsToCapture)
                    break; 
            }
            
            if (captureCounter == this.maxCardsToCapture)
                break;
        }

        //if only 1 row of cards, add empty cards to fill the space or grid will fill from the left always
        if (!this.printOnlyCardFronts && allCards.Count < this.pageCardGrid.constraintCount)
        {
            var numSpaceRemaining = this.pageCardGrid.constraintCount - allCards.Count;

            for (int i = 0; i < numSpaceRemaining; i++)
            {
                var emptyGo = new GameObject("empty");
                var image = emptyGo.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0.1f);
                emptyGo.transform.SetParent(this.pageCardGrid.transform);
                var card = emptyGo.AddComponent<Card>();
                card.name = $"Dummy_{i}";
                card.transform.localScale = Vector3.one * .92f;
                allCards.Add(card);
            }
        }

        //show groups of 9 or 18 (maxCardsPerCapture) at a time
        var firstCardNum = 1;
        var lastCardNum = 0;
        var pageNum = 1;
        captureCounter = 0;
        var printedNum = 0;

        while (allCards.Count > 0)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Debug.Log($"<color=red>Screen Capture Aborted!   {printedNum}/{cardDataList.Count}   Page {pageNum}</color>");
                yield break;
            }

            //first capture fronts
            this.pageCardGrid.startCorner = GridLayoutGroup.Corner.UpperLeft;

            var numInBatch = Mathf.Min(maxCardsPerCapture, allCards.Count);

            for (int i = 0; i < numInBatch; i++)
            {
                var card = allCards[i];
                card.gameObject.SetActive(true);
                ++lastCardNum;
                ++printedNum;
            }

            var fileName = $"Page {pageNum} - (card fronts {firstCardNum}-{lastCardNum})";

            CaptureImage(fileName);

            if (!this.printOnlyCardFronts)
            {
                //then capture backs
                this.pageCardGrid.startCorner = GridLayoutGroup.Corner.UpperRight;

                for (int i = 0; i < numInBatch; i++)
                {
                    var card = allCards[i];

                    if (card.name.Contains("Dummy"))
                        continue;

                    card.CardFlip.SetAnimStateToBack();
                    card.SetDontFlipCornerVisible(false);
                }

                yield return null;

                if (Input.GetKey(KeyCode.Escape))
                {
                    Debug.Log($"<color=red>Screen Capture Aborted!   {printedNum}/{cardDataList.Count}   Page {pageNum}</color>");
                    yield break;
                }

                fileName = $"Page {pageNum} - (card backs {firstCardNum}-{lastCardNum})";

                CaptureImage(fileName);
            }

            for (int i = 0; i < numInBatch; i++)
            {
                var card = allCards[i];
                card.gameObject.SetActive(false);
            }

            firstCardNum = lastCardNum + 1;
            ++pageNum;

            allCards.RemoveRange(0, numInBatch);

            ++captureCounter;

            if (captureCounter == this.maxCardsToCapture)
                break;

            yield return null;
        }

        Debug.Log($"<color=yellow>Screen Capture Complete: {pageNum - 1} Pages, {lastCardNum} cards</color>");
    }
}