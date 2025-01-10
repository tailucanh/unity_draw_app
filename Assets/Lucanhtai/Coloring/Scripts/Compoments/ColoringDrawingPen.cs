using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class ColoringDrawingPen : BaseImage
    {
        [SerializeField] Image box;
        [SerializeField] Image border;
        [SerializeField] Image shadow;
        [SerializeField] Image fillColor;

        public override void SetColor(Color color){
            fillColor.color = color;
        }

        public void SetColorPen(Color colorBox, Color colorBorder, Color colorShadow)
        {
            box.color = colorBox;
            border.color = colorBorder;
            shadow.color = colorShadow;
        }

        public override float SetColor(Color color, float time)
        {
            return 0;
        }
    }
}