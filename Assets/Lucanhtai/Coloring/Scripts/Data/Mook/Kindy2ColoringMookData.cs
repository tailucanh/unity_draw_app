using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucanhtai.Coloring
{
    [CreateAssetMenu(fileName = "Kindy2ColoringMookDataScriptableObject", menuName = "ScriptableObjects/Kindy2Coloring/MockData", order = 1)]

    public class Kindy2ColoringMookData : ScriptableObject
    {
        public Kindy2ColoringConversationData dataMook;
    }
}