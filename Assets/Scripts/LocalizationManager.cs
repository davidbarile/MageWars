using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Card;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager IN;

    public static string GetElementsString(EElement inElement)
    {
        var sb = new StringBuilder();

        var numElements = 0;

        if (inElement.HasFlag(EElement.Air))
        {
            sb.Append("<sprite name=\"Air\">");
            ++numElements;
        }

        if (inElement.HasFlag(EElement.Earth))
        {
            sb.Append("<sprite name=\"Earth\">");
            ++numElements;
        }

        if (inElement.HasFlag(EElement.Fire))
        {
            sb.Append("<sprite name=\"Fire\">");
            ++numElements;
        }

        if (inElement.HasFlag(EElement.Water))
        {
            sb.Append("<sprite name=\"Water\">");
            ++numElements;
        }

        if (numElements > 0)
        {
            if (numElements == 1)
                sb.Insert(0, "Element:");
            else
                sb.Insert(0, "Elements:");

            return sb.ToString();
        }
        return string.Empty;
    }

    public static string GetSpriteString(Sprite inSprite)
    {
        return $"<sprite name=\"{inSprite.name}\">";
    }
}