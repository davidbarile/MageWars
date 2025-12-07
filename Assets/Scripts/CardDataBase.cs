using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[Serializable]
public abstract class CardDataBase
{
    [Header("Base Card Data"), ReadOnly]
    public string ID;
    public string Name;
    public int Index;
    [Space]
    public int Count;

    [ShowInInspector]
    public bool IsDirty { get; set; }

    [ShowInInspector]
    public bool ShouldSaveOnQuit { get; set; }

    //unused
    [HideInInspector] public string CardFrontPrefabName;
    [HideInInspector] public string CardBackPrefabName;

    public Dictionary<string, EditNoteData> CardEditNotesDict = null;
    //public Dictionary<string, EditNoteData> PackEditNotesDict = null;
}