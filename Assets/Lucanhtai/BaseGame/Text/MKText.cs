
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MKText : BaseText
{
    protected ContentSizeFitter contentSizeFitter;
    protected override void Awake()
    {
        base.Awake();
        contentSizeFitter = GetComponent<ContentSizeFitter>();

    }
    public override void SetText(string content)
    {
        labeText.text = content;      
    }

    void OnEnable()
    {
        StartCoroutine(IEOnEnable());
    }
    IEnumerator IEOnEnable()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        yield return new WaitForEndOfFrame();
        //ActiveLayout(false);
    }

    private void ActiveLayout(bool isActive)
    {
        if (contentSizeFitter != null)
            contentSizeFitter.enabled = isActive;
    }

    public override void SetText(string content, float maxWidth)
    {
        ActiveLayout(true);
        StartCoroutine(IESetText(content, maxWidth)); 
    }

    IEnumerator IESetText(string content, float maxWidth)
    {
        labeText.text = content;
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        yield return new WaitForEndOfFrame();
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (rectTransform.sizeDelta.x > maxWidth)
        {
            rectTransform.sizeDelta = new Vector2(maxWidth, 0);
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        else
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
}
