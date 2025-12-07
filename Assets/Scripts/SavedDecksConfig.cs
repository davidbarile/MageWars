using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SavedDecks", menuName = "ScriptableObjects/SavedDecksConfig", order = 3)]
public class SavedDecksConfig : ScriptableObject
{
    public string ActivePackId;
    public List<DeckDictData> SavedDecks = new();
}

[Serializable]
public class DeckDictData
{
    public string PackId;
    public Pack Pack;
}