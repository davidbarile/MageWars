using UnityEngine;
using TMPro;

public class UITextIconPanel : UIPanelBase
{
    public static UITextIconPanel IN;

    [Header("Set by code")]
    public TMP_InputField TargetInputField;
    [Space]
    [SerializeField] private TMP_SpriteAsset iconSpriteAsset;
    [SerializeField] private Transform gridTransform;
    [SerializeField] private UITextIconItem textIconItemPrefab;

    public override void Show()
    {
        base.Show();

        this.gridTransform.DestroyAllChildren();

        var iconStringList = this.iconSpriteAsset.spriteCharacterTable.ConvertAll(x => x.name);

        foreach (var spriteName in iconStringList)
        {
            GameObject go = Instantiate(this.textIconItemPrefab.gameObject, this.gridTransform);

            var iconString = $"<sprite name=\"{spriteName}\">";
            go.GetComponent<UITextIconItem>().Init(iconString, HandleIconClick);
        }
    }

    public void HandleIconClick(string inIconString)
    {
        if (this.TargetInputField == null) return;

        GUIUtility.systemCopyBuffer = inIconString;

        this.TargetInputField.text += inIconString;
    }
}