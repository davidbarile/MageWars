using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAttackNumsSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private CanvasGroup dimmer;
    [SerializeField] private Graphic[] colorizedGraphics;
    [SerializeField] private Graphic attackNumBg;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button increaseButton;

    public int Count { get; private set; }
    public int MaxCount => (int) this.slider.maxValue;

    private Action onCountChanged;

    public void Init(Action inOnCountChanged, int inCount, int inMaxCount)
    {
        this.onCountChanged = inOnCountChanged;
        this.slider.maxValue = inMaxCount;
        this.slider.value = inCount;
    }

    public void OnSliderValueChanged(Single inValue)
    {
        foreach (var graphic in colorizedGraphics)
        {
            graphic.color = gradient.Evaluate(inValue);
        }

        this.attackNumBg.color = inValue == 0 ? Color.grey : Color.white;

        this.Count = (int)inValue;

        RefreshUI();
    }

    public void HandleChangeValueButtonPress(int inAmount)
    {
        if(inAmount > 0)
        {
            if(this.Count < this.slider.maxValue)
            {
                this.Count += inAmount;
            }
        }
        else
        {
            if(this.Count > 0)
            {
                this.Count += inAmount;
            }
        }

        this.slider.value = this.Count;
    }

    public void Reset()
    {
        this.slider.value = 0;
    }

    private void RefreshUI()
    {
        this.countText.text = $"{this.Count}<size=20>/{this.MaxCount}</size>";

        this.slider.value = this.Count;
        this.decreaseButton.interactable = this.Count > 0;
        this.increaseButton.interactable = this.Count < this.slider.maxValue;

        this.dimmer.alpha = this.Count == 0 ? .85f : 0f;

        this.onCountChanged?.Invoke();
    }
}