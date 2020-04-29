﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController m_instance = null;

    public static AudioController Instance
    {
        get { return m_instance; }
    }

    [SerializeField]
    private AudioClipData[] m_soundDataArray = null;

    private List<AudioPlayData> m_audioEffectsPlayDataList;
    private float _audioEffectVolume = 0.8f;
    private float m_audioEffectVolume
    {
        get { return _audioEffectVolume; }
        set
        {
            _audioEffectVolume = value < 0 ? 0 : value >= 1 ? 1 : value;
            PlayerPrefs.SetFloat("AudioEffectVolume", _audioEffectVolume);
        }
    }
    public float audioEffectVolume { get { return m_audioEffectVolume; } }
    private float m_audioEffectPauseVolume = 0;

    private List<AudioPlayData> m_audioBGPlayDataList;
    private float m_audioBGLastVolume = 0;
    private float _audioBGVolume = 0.8f;
    private float m_audioBGVolume
    {
        get { return _audioBGVolume; }
        set
        {
            _audioBGVolume = value < 0 ? 0 : value >= 1 ? 1 : value;
            PlayerPrefs.SetFloat("AudioBGVolume", _audioBGVolume);
        }
    }
    public float audioBGVolume { get { return m_audioBGVolume; } }

    private float m_audioBGPauseValume = 0;

    public AudioPlayData m_BGAudioData
    {
        get
        {
            if (m_audioBGPlayDataList == null || m_audioBGPlayDataList.Count <= 0) return null;
            return m_audioBGPlayDataList[0];
        }
    }

    private bool m_audioBGVolumeLoading = false;


    private void Awake()
    {
        m_instance = this;
        m_audioEffectsPlayDataList = new List<AudioPlayData>();
        m_audioBGPlayDataList = new List<AudioPlayData>();
        m_audioEffectVolume = PlayerPrefs.GetFloat("AudioEffectVolume", 0.8f);
        m_audioBGVolume = PlayerPrefs.GetFloat("AudioBGVolume", 0.8f);
    }

    private void Update()
    {
        for (int i = m_audioEffectsPlayDataList.Count - 1; i >= 0; i--)
        {
            AudioPlayData data = m_audioEffectsPlayDataList[i];
            if (data.IsFinished)
            {
                m_audioEffectsPlayDataList.RemoveAt(i);
            }
        }
        for (int i = m_audioBGPlayDataList.Count - 1; i >= 0; i--)
        {
            AudioPlayData data = m_audioBGPlayDataList[i];
            if (data.IsFinished)
            {
                m_audioBGPlayDataList.RemoveAt(i);
            }
        }
    }

    public void SetAudioEffectsVolume(float volume)
    {
        m_audioEffectVolume = volume;
        m_audioEffectPauseVolume = m_audioEffectVolume;
        for (int i = m_audioEffectsPlayDataList.Count - 1; i >= 0; i--)
        {
            AudioPlayData data = m_audioEffectsPlayDataList[i];
            if (data != null && data.audioSource != null)
            {
                data.audioSource.volume = volume;
            }
        }
    }

    public void SetAudioBGVolume(float volume)
    {
        m_audioBGVolume = volume;
        m_audioBGPauseValume = m_audioBGVolume;
        for (int i = m_audioBGPlayDataList.Count - 1; i >= 0; i--)
        {
            AudioPlayData data = m_audioBGPlayDataList[i];
            if (data != null && data.audioSource != null)
            {
                m_audioBGVolume = volume;
                StartCoroutine(PlayBGSoundIncrease(data));
            }
        }
    }

    public void PlayBGSound(string name, bool loop = true)
    {
        var bgSounds = m_audioBGPlayDataList.Where(bgData => { return bgData.audioName == name; });
        if (bgSounds != null && bgSounds.FirstOrDefault() != null)
        {
            Debug.Log("This BGM playing.");
            return;
        }
        var sounds = m_soundDataArray.Where((data) => { return data.soundName == name; });
        AudioClipData sound = sounds.FirstOrDefault();
        if (sound != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            AudioPlayData data = new AudioPlayData(sound, source, loop);
            m_audioBGPlayDataList.Add(data);
            source.clip = data.audioClip;
            source.volume = 0;
            StartCoroutine(PlayBGSoundIncrease(data));
            source.loop = loop;
            source.Play();
        }
    }
    public void PauseAudio(string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                var bgSounds = m_audioBGPlayDataList.Where(bgData => { return bgData.audioName == name; });
                if (bgSounds != null)
                {
                    var sound = bgSounds.FirstOrDefault();
                    if (sound != null)
                    {
                        m_audioBGPauseValume = m_audioBGVolume;
                        m_audioBGVolume = 0;
                        StartCoroutine(PlayBGSoundIncrease(sound));
                    }
                    return;
                }
                bgSounds = m_audioEffectsPlayDataList.Where(bgData => { return bgData.audioName == name; });
                if (bgSounds != null)
                {
                    var sound = bgSounds.FirstOrDefault();
                    if (sound != null)
                    {
                        m_audioEffectPauseVolume = m_audioEffectVolume;
                        m_audioEffectVolume = 0;
                        sound.audioSource.volume = m_audioEffectVolume;
                        if (m_audioEffectVolume <= 0) sound.audioSource.mute = true;
                    }
                }
            }
            else
            {
                m_audioBGPlayDataList.ForEach(item =>
                {
                    if (item.audioSource != null)
                    {
                        m_audioBGPauseValume = m_audioBGVolume;
                        m_audioBGVolume = 0;
                        StartCoroutine(PlayBGSoundIncrease(item));
                    }
                });
                m_audioBGPlayDataList.ForEach(item =>
                {
                    if (item.audioSource != null)
                    {
                        m_audioEffectPauseVolume = m_audioEffectVolume;
                        m_audioEffectVolume = 0;
                        item.audioSource.volume = m_audioEffectVolume;
                    }
                });
            }
        }
        catch (System.Exception error)
        {
            Debug.LogError("PauseBgSound name: " + name + " Error: " + error.Message);
        }
    }
    public void RestartAudio(string name = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(name))
            {
                var bgSounds = m_audioBGPlayDataList.Where(bgData => { return bgData.audioName == name; });
                if (bgSounds != null)
                {
                    var sound = bgSounds.FirstOrDefault();
                    if (sound != null)
                    {
                        m_audioBGVolume = m_audioBGPauseValume;
                        StartCoroutine(PlayBGSoundIncrease(sound));
                    }
                    return;
                }
                bgSounds = m_audioEffectsPlayDataList.Where(bgData => { return bgData.audioName == name; });
                if (bgSounds != null)
                {
                    var sound = bgSounds.FirstOrDefault();
                    if (sound != null)
                    {
                        m_audioEffectVolume = m_audioEffectPauseVolume;
                        sound.audioSource.volume = m_audioEffectVolume;
                    }
                }
            }
            else
            {
                m_audioBGPlayDataList.ForEach(item =>
                {
                    if (item.audioSource != null)
                    {
                        m_audioBGVolume = m_audioBGPauseValume;
                        StartCoroutine(PlayBGSoundIncrease(item));
                    }
                });
                m_audioBGPlayDataList.ForEach(item =>
                {
                    if (item.audioSource != null)
                    {
                        m_audioEffectVolume = m_audioEffectPauseVolume;
                        item.audioSource.volume = m_audioEffectVolume;
                    }
                });
            }
        }
        catch (System.Exception error)
        {
            Debug.LogError("RestartBgSound name: " + name + " Error: " + error.Message);
        }
    }
    private IEnumerator PlayBGSoundIncrease(AudioPlayData data)
    {
        if (data.audioLoading) yield break;
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        data.audioLoading = true;
        if (m_audioBGVolume > 0 && data.audioSource.mute) data.audioSource.mute = false;
        while (Mathf.Abs(m_audioBGVolume - m_audioBGLastVolume) > 0.01f)
        {
            m_audioBGLastVolume = Mathf.Lerp(m_audioBGLastVolume, m_audioBGVolume, Time.deltaTime * 1.2f);
            data.audioSource.volume = m_audioBGLastVolume;
            yield return null;
        }
        //sw.Stop();
        //Debug.Log("statrt time length: " + sw.ElapsedMilliseconds);
        data.audioSource.volume = m_audioBGVolume;
        if (m_audioBGVolume <= 0) data.audioSource.mute = true;
        m_audioBGLastVolume = m_audioBGVolume;
        data.audioLoading = false;
    }

    public void PlayEffectsSound(string name, bool loop = false)
    {
        var sounds = m_soundDataArray.Where((data) => { return data.soundName == name; });
        AudioClipData sound = sounds.FirstOrDefault();
        if (sound != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            AudioPlayData data = new AudioPlayData(sound, source, loop);
            m_audioEffectsPlayDataList.Add(data);
            source.clip = data.audioClip;
            source.volume = m_audioEffectVolume;
            source.loop = loop;
            source.Play();
        }
    }

}
