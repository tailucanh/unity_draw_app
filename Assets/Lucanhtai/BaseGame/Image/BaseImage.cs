using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseImage : MonoBehaviour
{
    protected Image image;

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
    }

    public abstract void SetColor(Color color);

    public abstract float SetColor(Color color, float time);
}
