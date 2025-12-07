using System;
using System.Collections.Generic;

[Serializable]
public class ExportItem
{
    public string Name;
    public int NumCards;
    public List<CardData> Cards = new();
}