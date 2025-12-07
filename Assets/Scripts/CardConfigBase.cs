using UnityEngine;

public abstract class CardConfigBase : ScriptableObject
{
    [Header("Base Card Config")]
    public int Count;

    //unused
    [HideInInspector] public UICardFrontBase CardFrontPrefab;
    [HideInInspector] public UICardBackBase CardBackPrefab;
}