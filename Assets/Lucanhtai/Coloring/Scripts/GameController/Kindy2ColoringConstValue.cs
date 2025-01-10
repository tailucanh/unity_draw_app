using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucanhtai.Coloring
{
    public enum DrawMode
    {
        Default,
        CustomBrush,
        Eraser
    }

    public enum Kindy2ColoringState
    {
        INIT_SCENE_STATE,
        INIT_DATA_STATE,
        INTRO_STATE,
        CLICK_OBJECT_STATE,
        PLAY_STATE,
        PAUSE_STATE,
        OUTRO_STATE,
        GAME_FINISH_STATE,
    }


}