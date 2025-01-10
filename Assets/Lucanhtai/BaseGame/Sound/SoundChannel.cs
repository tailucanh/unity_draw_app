using Lucanhtai.Observer;
using System;
using UnityEngine;

public struct SoundChannel : EventListener<SoundChannel>
{
    public void OnMMEvent(SoundChannel eventType)
    {
        throw new System.NotImplementedException();
    }

    public string EventName;
    public AudioClip AudioClip;
    public float Volume;
    public bool Loop;
    public Action ActionDone;
    

    public SoundChannel(string nameEvent, AudioClip audioClip, Action actionDone = null, float volume = 1, bool loop = false)
    {
        EventName = nameEvent;
        AudioClip = audioClip;
        Volume = volume;
        Loop = loop;
        ActionDone = actionDone;
    }

    public const string PLAY_SOUND = "PLAY_SOUND";
    public const string PLAY_SOUND_NEW_OBJECT = "PLAY_SOUND_NEW_OBJECT";
    public const string PAUSE_SOUND = "PAUSE_SOUND";
    public const string STOP_SOUND = "STOP_SOUND";
    public const string PLAY_MUSIC = "PLAY_MUSIC";
    public const string PAUSE_MUSIC = "PAUSE_MUSIC";
    public const string STOP_MUSIC = "STOP_MUSIC";
    public const string STOP_ALL_SOUND_BY_DESTROY = "STOP_ALL_SOUND_BY_DESTROY";
}
