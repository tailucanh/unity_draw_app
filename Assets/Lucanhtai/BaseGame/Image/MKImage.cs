using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKImage : BaseImage
{
    public override float SetColor(Color color, float time)
    {
        Color startColor = image.color;

        StartCoroutine(LerpColorsOverTime(startColor, color, time));

        return time;
    }

    public override void SetColor(Color color)
    {
        image.color = color;
    }

    private IEnumerator LerpColorsOverTime(Color startColor, Color endingColor, float time)
    {
        float inversedTime = 1 / time; // Compute this value **once**
        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            image.color = Color.Lerp(startColor, endingColor, step);
            yield return new WaitForEndOfFrame();
        }
    }
}
