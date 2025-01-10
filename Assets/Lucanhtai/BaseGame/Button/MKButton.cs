
using Lucanhtai.Observer;
using UnityEngine;
using UnityEngine.UI;

public class MKButton : BaseButton
{
    public override void Enable(bool isEnable)
    {
       
        button.interactable = isEnable;
    }

    public override void OnClick()
    {
        
        
    }

   
}
