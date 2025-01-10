using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManager : TPRLSingleton<SoundManager>
{
    [SerializeField]
    private GameObject obMusic, obFx, obFxOneShot;
    private AudioSource _sourceMusic, _sourceFx, _sourceFxOneShot;
    private GameObject _audioWrapperGO = null;
    public GameObject GetAudioWrapperGO()
    {
        if (_audioWrapperGO == null)
        {
            _audioWrapperGO = new GameObject("Audio Wrapper");
            _audioWrapperGO.transform.SetParent(this.transform);
        }

        return _audioWrapperGO;
    }
    public AudioSource sourceMusic
    {
        get
        {
            if (_sourceMusic == null)
            {
                if (obMusic != null && obMusic.TryGetComponent<AudioSource>(out _sourceMusic) == false)
                    _sourceMusic = obMusic.AddComponent<AudioSource>();
            }
            return _sourceMusic;
        }
    }
    public AudioSource sourceFx
    {
        get
        {
            if (_sourceFx == null)
            {
                if (obFx != null && obFx.TryGetComponent<AudioSource>(out _sourceFx) == false)
                    _sourceFx = obFx.AddComponent<AudioSource>();
            }
            return _sourceFx;
        }
    }
    public AudioSource sourceFxOneShot
    {
        get
        {
            if (_sourceFxOneShot == null)
            {
                if (obFxOneShot != null && obFxOneShot.TryGetComponent<AudioSource>(out _sourceFxOneShot) == false)
                    _sourceFxOneShot = obFxOneShot.AddComponent<AudioSource>();
            }
            return _sourceFxOneShot;
        }
    }
    [SerializeField]
    private AudioClip[] musics;
    [SerializeField]
    private AudioClip[] fxs;
    private Dictionary<string, AudioClip> dictMusic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> dictFx = new Dictionary<string, AudioClip>();
    private List<AudioSource> lstAudioSourceOneShotPlaying = new List<AudioSource>();


    private int _isMuteFx = -1;
    public int isMuteFx
    {
        get
        {
            if (_isMuteFx == -1)
                _isMuteFx = PlayerPrefs.GetInt("is_fx_mute", 0);
            return _isMuteFx;
        }
        set
        {
            _isMuteFx = value;
            PlayerPrefs.SetInt("is_fx_mute", value);
            PlayerPrefs.Save();
            SetFxMute(value == 1 ? true : false);
        }
    }

    private int _isMuteFxOneShot = -1;
    public int isMuteFxOneShot
    {
        get
        {
            if (_isMuteFxOneShot == -1)
                _isMuteFxOneShot = PlayerPrefs.GetInt("is_fx_oneshot_mute", 0);
            return _isMuteFxOneShot;
        }
        set
        {
            _isMuteFxOneShot = value;
            PlayerPrefs.SetInt("is_fx_oneshot_mute", value);
            PlayerPrefs.Save();
            SetFxOneShotMute(value == 1 ? true : false);
        }
    }

    private void SetMusicMute(bool is_mute)
    {
        if (sourceMusic != null)
            sourceMusic.mute = is_mute;
    }

    private void SetFxMute(bool is_mute)
    {
        if (sourceFx != null)
            sourceFx.mute = is_mute;
    }

    private void SetFxOneShotMute(bool is_mute)
    {
        if (sourceFxOneShot != null)
            sourceFxOneShot.mute = is_mute;
    }



    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
    }

    protected override void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
    }

    private void Start()
    {
        AddAudioClips();
    }

    private void AddAudioClips()
    {
        int length = 0;
        if (musics != null && musics.Length > 0)
        {
            length = musics.Length;
            for (int i = 0; i < length; i++)
            {
                if (!dictMusic.ContainsKey(musics[i].name))
                    dictMusic.Add(musics[i].name, musics[i]);
            }
        }
        if (fxs != null && fxs.Length > 0)
        {
            length = fxs.Length;
            for (int i = 0; i < length; i++)
            {
                if (!dictFx.ContainsKey(fxs[i].name))
                    dictFx.Add(fxs[i].name, fxs[i]);
            }
        }
    }

    public void PlayMusic(AudioClip clip, Action on_done = null, float volume = 1, bool is_loop = true)
    {
        Debug.LogWarning("Call Play Music " + (sourceMusic == null) + " " + (clip == null));
        if (sourceMusic == null || clip == null)
        {
            on_done?.Invoke();
            return;
        }
        if (sourceMusic.clip != null && sourceMusic.clip.name.Equals(clip.name) && sourceMusic.isPlaying)
        {
            on_done?.Invoke();
            return;
        }
        sourceMusic.Stop();
        sourceMusic.clip = clip;
        sourceMusic.volume = volume;
        sourceMusic.loop = is_loop;
        sourceMusic.Play();
        if (on_done != null)
            StartCoroutine(IEYield(clip, on_done));
    }
    public async void PlayMusic(string music_name, Action on_done = null, float volume = 1, bool is_loop = true, AudioType type = AudioType.MPEG)
    {
        if (sourceMusic == null)
        {
            on_done?.Invoke();
            return;
        }
        if (sourceMusic.clip != null && sourceMusic.clip.name.Equals(music_name) && sourceMusic.isPlaying)
        {
            on_done?.Invoke();
            return;
        }
        AudioClip clip = dictMusic[music_name];
     
        sourceMusic.Stop();
        sourceMusic.clip = clip;
        sourceMusic.volume = volume;
        sourceMusic.loop = is_loop;
        sourceMusic.Play();
        Debug.Log($"Music volume {sourceMusic.volume} __ isPlaying {sourceMusic.isPlaying}");
        if (on_done != null)
            StartCoroutine(IEYield(clip, on_done));
    }
    public async void PlayMusic(string bundle_name, string music_name, Action on_done = null, float volume = 1, bool is_loop = true, AudioType type = AudioType.MPEG)
    {
        if (sourceMusic == null)
        {
            on_done?.Invoke();
            return;
        }
        if (sourceMusic.clip != null && sourceMusic.clip.name.Equals(music_name) && sourceMusic.isPlaying)
        {
            on_done?.Invoke();
            return;
        }
        bundle_name = GetFileName(bundle_name);
        music_name = GetFileName(music_name);
        AudioClip clip = dictMusic[music_name];
      
        sourceMusic.Stop();
        sourceMusic.clip = clip;
        sourceMusic.volume = volume;
        sourceMusic.loop = is_loop;
        sourceMusic.Play();
        Debug.Log($"Music volume {sourceMusic.volume} __ isPlaying {sourceMusic.isPlaying}");
        if (on_done != null)
            StartCoroutine(IEYield(clip, on_done));
    }

    public void PlayFx(AudioClip clip, Action on_done = null, bool is_loop = false, float volume = 1)
    {
        if (sourceFx == null || clip == null)
        {
            on_done?.Invoke();
            return;
        }
        sourceFx.Stop();
        sourceFx.volume = volume;
        sourceFx.clip = clip;
        sourceFx.loop = is_loop;
        sourceFx.Play();
        if (on_done != null)
            StartCoroutine(IEYield(clip, on_done));
    }

    public async void PlayFx(string bundle_name, string fx_name, Action on_done = null, bool is_cache_in_dict = false, bool is_loop = false, float volume = 1, AudioType type = AudioType.MPEG)
    {
        if (sourceFx == null)
        {
            on_done?.Invoke();
            return;
        }
        bundle_name = GetFileName(bundle_name);
        fx_name = GetFileName(fx_name);
        AudioClip clip = dictFx[fx_name];
        
        sourceFx.Stop();
        sourceFx.volume = volume;
        sourceFx.clip = clip;
        sourceFx.loop = is_loop;
        sourceFx.Play();
        if (on_done != null)
            StartCoroutine(IEYield(clip, on_done));
    }

    public void PlayFxOneShot(AudioClip clip, Action on_done = null, Action on_start = null, float volume = 1)
    {
        if (sourceFxOneShot == null || clip == null)
        {
            on_done?.Invoke();
            return;
        }
        on_start?.Invoke();
        sourceFxOneShot.volume = volume;
        sourceFxOneShot.PlayOneShot(clip);
#if NGA_TEST
        NgaTest.PrintAudio(clip.name);
#endif
        if (on_done != null)
            StartCoroutine(IEYield(clip, on_done));
    }

    public AudioSource PlayFxOnNewGameObject(AudioClip clip, Action on_done = null, float volume = 1, bool allowDuplicateAudioSource = false)
    {
        if (clip == null)
        {
            on_done?.Invoke();
            return null;
        }
        AudioSource audioSource = null;
        GameObject ob = new GameObject();
        ob.name = clip.name;
        if (allowDuplicateAudioSource || ob.TryGetComponent<AudioSource>(out audioSource) == false)
        {
            audioSource = ob.AddComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            Destroy(ob);
            on_done?.Invoke();
            return null;
        }
        lstAudioSourceOneShotPlaying.Add(audioSource);
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
#if NGA_TEST
        NgaTest.PrintAudio(clip.name);
#endif
        StartCoroutine(IEYield(clip, () =>
        {
            lstAudioSourceOneShotPlaying.Remove(audioSource);
            on_done?.Invoke();
            Destroy(ob);
        }));
        return audioSource;
    }

    public async void PlayFxOneShot(string bundle_name, string fx_name, Action on_done = null, bool is_cache_in_dict = false, float volume = 1, AudioType type = AudioType.MPEG)
    {
        if (sourceFxOneShot == null)
        {
            on_done?.Invoke();
            return;
        }
        bundle_name = GetFileName(bundle_name);
        fx_name = GetFileName(fx_name);
        AudioClip clip = null;
        
        if (dictFx.ContainsKey(fx_name))
            clip = dictFx[fx_name];
        else
        {
            if (is_cache_in_dict)
                dictFx.Add(fx_name, clip);
        }
        sourceFxOneShot.volume = volume;
        sourceFxOneShot.PlayOneShot(clip);
        if (on_done != null)
            StartCoroutine(IEYield(clip, on_done));
    }

    private Coroutine corPlayFxOnGameObjectOneShot = null;
    public AudioSource PlayFxOnGameObjectOneShot(GameObject ob, AudioClip clip, Action on_done = null, float volume = 1, bool allowDuplicateAudioSource = false)
    {
        if (clip == null)
        {
            on_done?.Invoke();
            return null;
        }
        AudioSource audioSource = null;
        if (ob == null) ob = GetAudioWrapperGO();

        if (!allowDuplicateAudioSource)
        {
            AudioSource source = ob.GetComponent<AudioSource>();

            if (source != null)
            {
                if (lstAudioSourceOneShotPlaying.Contains(source))
                    lstAudioSourceOneShotPlaying.Remove(source);
                Destroy(source);
            }
            if (corPlayFxOnGameObjectOneShot != null)
                StopCoroutine(corPlayFxOnGameObjectOneShot);
        }

        audioSource = ob.AddComponent<AudioSource>();

        if (audioSource == null)
        {
            on_done?.Invoke();
            return null;
        }
        lstAudioSourceOneShotPlaying.Add(audioSource);
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
#if NGA_TEST
        NgaTest.PrintAudio(clip.name);
#endif
        corPlayFxOnGameObjectOneShot = StartCoroutine(IEYield(clip, () =>
        {
            lstAudioSourceOneShotPlaying.Remove(audioSource);
            on_done?.Invoke();
        }));
        return audioSource;
    }

    public async UniTask<AudioSource> PlayFxOnGameObjectOneShot(GameObject ob, string bundle_name, string fx_name, Action on_done = null, bool is_cache_in_dict = false, float volume = 1, AudioType type = AudioType.MPEG)
    {
        if (ob == null)
        {
            on_done?.Invoke();
            return null;
        }
        AudioSource audioSource = null;
        if (ob.TryGetComponent<AudioSource>(out audioSource) == false)
            audioSource = ob.AddComponent<AudioSource>();
        if (audioSource == null)
        {
            on_done?.Invoke();
            return null;
        }
        bundle_name = GetFileName(bundle_name);
        fx_name = GetFileName(fx_name);
        AudioClip clip = dictFx[fx_name];
       
        lstAudioSourceOneShotPlaying.Add(audioSource);
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
        StartCoroutine(IEYield(clip, () =>
        {
            lstAudioSourceOneShotPlaying.Remove(audioSource);
            on_done?.Invoke();
        }));
        return audioSource;
    }


    public void PlayFxOnAudioSourceOneShot(AudioSource audio_source, AudioClip clip, Action on_done = null, float volume = 1)
    {
        if (clip == null)
        {
            on_done?.Invoke();
            return;
        }
        GameObject ob = GetAudioWrapperGO();


        if (audio_source == null)
        {
            audio_source = ob.AddComponent<AudioSource>();
        }
        lstAudioSourceOneShotPlaying.Add(audio_source);
        audio_source.volume = volume;
        audio_source.PlayOneShot(clip);
#if NGA_TEST
        NgaTest.PrintAudio(clip.name);
#endif
        StartCoroutine(IEYield(clip, () =>
        {
            lstAudioSourceOneShotPlaying.Remove(audio_source);
            on_done?.Invoke();
        }));
    }

    private IEnumerator IEYield(AudioClip clip, Action on_done)
    {
        if (clip == null)
        {
            on_done?.Invoke();
            yield break;
        }
        float duration = clip.length;
        yield return new WaitForSeconds(duration);
        on_done?.Invoke();
    }

    public bool isMusicPlaying()
    {
        if (sourceMusic == null) return false;
        return sourceMusic.isPlaying;
    }

    public bool isFxPlaying()
    {
        if (sourceFx == null) return false;
        return sourceFx.isPlaying;
    }

    public bool isFxOneShotPlaying()
    {
        if (sourceFxOneShot == null) return false;
        return sourceFxOneShot.isPlaying;
    }

    public void PauseMusic(bool is_pause = true)
    {
        if (sourceMusic == null) return;
        if (is_pause)
            sourceMusic.Pause();
        else
            sourceMusic.UnPause();
    }


    public void PauseFx(bool is_pause = true)
    {
        if (sourceFx == null) return;
        if (is_pause)
            sourceFx.Pause();
        else
            sourceFx.UnPause();
    }

    public void PauseFxOneShot(bool is_pause = true)
    {
        if (sourceFxOneShot != null)
        {
            if (is_pause)
                sourceFxOneShot.Pause();
            else
                sourceFxOneShot.UnPause();
        }
        if (lstAudioSourceOneShotPlaying.Count > 0)
        {
            int length = lstAudioSourceOneShotPlaying.Count;
            for (int i = 0; i < length; i++)
            {
                if (lstAudioSourceOneShotPlaying[i] != null)
                {
                    if (is_pause)
                        lstAudioSourceOneShotPlaying[i].Pause();
                    else
                        lstAudioSourceOneShotPlaying[i].UnPause();
                }
            }
        }
    }

    public void PauseFxAndOneShot(bool is_pause = true)
    {
        PauseFx(is_pause);
        PauseFxOneShot(is_pause);
    }


    public void StopMusic()
    {
        if (sourceMusic == null)
        {
            Debug.Log("SourceMusic null");
            return;
        }
        sourceMusic.Stop();
    }


    public void StopFx()
    {
        if (sourceFx == null) return;
        sourceFx.Stop();
    }

    public void StopFxOneShot()
    {
        if (sourceFxOneShot != null)
            sourceFxOneShot.Stop();
        if (lstAudioSourceOneShotPlaying.Count > 0)
        {
            int length = lstAudioSourceOneShotPlaying.Count;
            for (int i = 0; i < length; i++)
            {
                if (lstAudioSourceOneShotPlaying[i] != null)
                {
                    lstAudioSourceOneShotPlaying[i].Stop();
                }
            }
        }
        lstAudioSourceOneShotPlaying.Clear();
    }

    public void StopFxAndOneShot()
    {
        StopFx();
        StopFxOneShot();
    }

    public void StopAll()
    {
        StopMusic();
        StopFx();
        StopFxOneShot();
    }

    public string GetFileName(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        path = path.Replace("\\", "/");
        //MKHelper.DebugLogMessageColorCyan("path : " + path);
        if (path.Contains("/"))
        {
            path = path.Split("/").Last();
        }
        return path;
    }
}
