using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lucanhtai.Coloring
{
    [CreateAssetMenu(fileName = "Kindy2ColoringScriptableObject", menuName = "ScriptableObjects/Kindy2Coloring/Setting", order = 1)]
    public class Kindy2ColoringScriptableObject : ScriptableObject
    {
        public Kindy2ColoringInitDataConfig initDataConfig;
        public Kindy2ColoringIntroConfig introDataConfig;
        public Kindy2ColoringLocalizationConfig localizationConfig;
        public Kindy2ColoringClickConfig clickDataConfig;
        public Kindy2ColoringGuidingConfig guidingDataConfig;
        public Kindy2ColoringDataColor dataColorConfig;
        public Kindy2ColoringDataFinish dataFinishGame;
        public Kindy2ColoringOutroConfig outroConfig;
    }

    [Serializable]
    public class Kindy2ColoringLocalizationConfig
    {
        public string keyLangagueTitle;
        public string keyLangagueDescription;
        public string keyLangagueCancel;
        public string keyLangagueExit;
    }

    [Serializable]
    public class Kindy2ColoringInitDataConfig
    {
        public float SCENCE_RESOLUTION = 1.77f;
        public Color32 colorWashOut;
        public Color32 colorWashIn;
        public int timeDelayOut;
        public int timeDelayIn;
        public string ellieIntroGallery = "Intro 1 - ";
        public string ellieOutroGallery = "Outro - ";
        public string EVENT_FIRST_RUN = "Chay 1";
        public string EVENT_BRAKE = "Phanh";
        public string EVENT_SECOND_RUN = "chay 2";
        public string EVENT_FIRST_SPEECH_INTRO_GALLERY_1 = "Look!";
        public string EVENT_FIRST_SPEECH_INTRO_GALLERY_2 = "I want to have a lovely picture to hang on the wall!";
        public string EVENT_BLINK = "Chop mat";
        public string EVENT_SECOND_SPEECH_INTRO_GALLERY = "I've got an idea!";
        public string EVENT_PENTOSS_1 = "Tung but 1";
        public string EVENT_PENTOSS_2 = "Tung but 2";
        public string EVENT_SPEECH_INTRO_DRAWING = "Maybe my friends can help!";
        public string EVENT_OUTRO = "chay 2";
        public AudioClip audioBackground;
    }
    [Serializable]
    public class Kindy2ColoringIntroConfig
    {
        public List<AudioClip> audioFirstSpeechIntroGallery;
        public AudioClip audioSecondSpeechIntroGallery;
        public AudioClip audioSpeechIntroDrawing;
        public List<AudioClip> sfxsRun;
        public AudioClip sfxsBlink;
        public AudioClip sfxsBrake;
        public List<AudioClip> sfxsPenToss;
        public int miliSecondDelayFadeIn;
        public float audioVolume;
        public bool loop;
    }

    [Serializable]
    public class Kindy2ColoringClickConfig
    {
        public AudioClip sfxClick;
        public int miliSecondDelay;
        public int idImage;
        public float audioVolume;
        public bool loop;
    }


    [Serializable]
    public class Kindy2ColoringGuidingConfig
    {
        public AudioClip audioGuiding;
        public float secondDelay;
        public float audioVolume;
        public bool loop;
    }

    [Serializable]
    public class Kindy2ColoringDataColor
    {
        public List<Color32> colorButtons;
        public Color32 colorSelectWhite;
        public Color32 colorSelectBlue;
        public Color32 colorSelectRed;
        public Color32 colorSelectBrown;
        public Color32 colorSelectBrownBorder;
    }

    [Serializable]
    public class Kindy2ColoringDataFinish
    {
        public float timeSpent;
        public float timeStart;
        public float timeEnd;
        public int score;
        public bool isSendEvent;
        public string key_pref;
    }
    [Serializable]
    public class Kindy2ColoringOutroConfig
    {
        public AudioClip sfxsCheered;
        public AudioClip sfxsVictory;
        public float audioVolume;
        public bool loop;
    }
}