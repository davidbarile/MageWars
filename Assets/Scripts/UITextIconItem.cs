using System;
using UnityEngine;
using TMPro;

public class UITextIconItem : MonoBehaviour
{
    public TextMeshProUGUI Text;

    private Action<string> onClick;

    public void Init(string inIconString, Action<string> inOnClick)
    {
        this.Text.text = inIconString;
        this.onClick = inOnClick;
    }

    public void HandleClick()
    {
        this.onClick?.Invoke(this.Text.text);
    }
}
