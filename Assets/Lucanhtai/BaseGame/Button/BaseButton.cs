using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseButton : MonoBehaviour
{
    protected Button button;
   
    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
   
    public abstract void Enable(bool isEnable);

    public virtual void SetBackGroundColor(Color32 backgroundColor) { }
    public virtual void SetTextColor(Color32 textColor) { }

    public abstract void OnClick();
}
