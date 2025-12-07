using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DeckConfig;

public class UIDeckBalanceWidget : MonoBehaviour
{
    [SerializeField] private EDeckType deckType;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private UIAttackNumsSlider[] attackNumsSliders;

    public UIAttackNumsSlider[] AttackNumsSliders => this.attackNumsSliders;

    public int Count { get; private set; }
    public int MaxCount { get; private set; }

    private Action onCountChanged;

    public void Init(Action inOnCountChanged, List<int> inCounts)
    {
        this.onCountChanged = inOnCountChanged;

        this.MaxCount = UICardBacksPanel.GetCountInDeck(this.deckType);
        
        for(int i = 0; i < this.attackNumsSliders.Length; ++i)
        {
            var slider = this.attackNumsSliders[i];
            slider.Init(RefreshCounts, inCounts[i], this.MaxCount);
        }

        this.countText.gameObject.SetActive(this.MaxCount > 0);
    }

    public void Reset()
    {
        foreach (var slider in this.attackNumsSliders)
        {
            slider.Reset();
        }
    }

    private void RefreshCounts()
    {
        this.Count = 0;
        foreach (var slider in this.attackNumsSliders)
        {
            this.Count += slider.Count;
        }

        var colorString = this.Count == this.MaxCount ? "#00FF00" : "red";

        this.countText.text = $"<color={colorString}>{this.Count}/{this.MaxCount}</color>";

        this.onCountChanged?.Invoke();
    }
}