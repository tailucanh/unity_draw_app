
using TMPro;
using UnityEngine;

public abstract class BaseText : MonoBehaviour
{
    protected TextMeshProUGUI labeText;

    protected virtual void Awake()
    {
        labeText = gameObject.GetComponent<TextMeshProUGUI>();
    }
    public abstract void SetText(string text);
    public abstract void SetText(string text, float maxWidth);
}
