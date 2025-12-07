using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteSelectItem : MonoBehaviour
{
    public static bool ShouldShowCounts;

    public Action<Sprite> OnClick;

    [SerializeField] private Image icon;
    [SerializeField] private GameObject emptyIcon;
    [Space]
    [SerializeField] private CanvasGroup countDisplay;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI percentText;

    private void Start()
    {
        this.countDisplay.gameObject.SetActive(false);

        RefreshVisibility();
    }

    public void SetSprite(Sprite inSprite)
    {
        this.icon.sprite = inSprite;

        this.icon.gameObject.SetActive(inSprite != null);
        this.emptyIcon.SetActive(inSprite == null);
    }

    public void SetCountText(int inCount, float inPercent)
    {
        this.countText.text = $"{inCount}";
        this.percentText.text = $"{inPercent:N1}<size=15>%</size>";
        this.countDisplay.alpha = inCount > 0 ? 1 : 0;
    }

    public void RefreshVisibility()
    {
        this.countDisplay.gameObject.SetActive(ShouldShowCounts);
    }

    public void HandleClick()
    {
        this.OnClick?.Invoke(this.icon.sprite);
    }

     public void HandleRequirementClick()
    {
        this.OnClick?.Invoke(this.icon.sprite);
    }

    private void OnDestroy()
    {
        this.OnClick = null;
        UISpriteSelectPanel.OnStatsVisibilityChanged -= RefreshVisibility;
    }
}