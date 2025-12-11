using System;
using System.Collections.Generic;
using UnityEngine;
using static Card;

[CreateAssetMenu(fileName = "SpritesConfig", menuName = "ScriptableObjects/SpritesConfig", order = 1)]
public class SpritesConfig : ScriptableObject
{
    [Header("Sprites")]
    public List<Sprite> AllSprites;
    public List<Sprite> SpecialSprites;
    public ElementSpriteLookup[] ElementSpriteLookups;
}

[Serializable]
public class ElementSpriteLookup
{
    public EElement Element;
    public Sprite Sprite;
}