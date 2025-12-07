using JoshH.UI;
using UnityEngine;
using UnityEngine.UI;
using static CardConfig;

public abstract class UICardFrontBase : MonoBehaviour
{
    [SerializeField] private Image bgFrame;
    [SerializeField] private UIGradient bgGradient;

    [SerializeField] protected Color[] gradientColors_default = new Color[2];
    [SerializeField] protected Color[] gradientColors_good = new Color[2];
    [SerializeField] protected Color[] gradientColors_evil = new Color[2];

    /// <summary>
    /// Override this with specificy UI display code depending on prefab
    /// </summary>
    public virtual void RefreshDisplay(CardData inData)
    {

    }

    protected virtual void SetBgGradientColors(EAlignment inAlignment)
    {
        var topColor = this.gradientColors_default[0];
        var bottomColor = this.gradientColors_default[1];

        if (inAlignment == EAlignment.Good)
        {
            topColor = this.gradientColors_good[0];
            bottomColor = this.gradientColors_good[1];
        }
        else if (inAlignment == EAlignment.Evil)
        {
            topColor = this.gradientColors_evil[0];
            bottomColor = this.gradientColors_evil[1];
        }

        this.bgGradient.LinearColor1 = topColor;
        this.bgGradient.LinearColor2 = bottomColor;
    }

    public virtual void MakeEmptyResourcesAndReqs(CardData inData)
    {
        
    }
}