using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayData
{
    private string m_audioName = string.Empty;
    public string audioName { get { return m_audioName; } }

    private AudioClip m_audioClip = null;
    public AudioClip audioClip { get { return m_audioClip; } }
    private AudioSource m_audioSource = null;
    public AudioSource audioSource { get { return m_audioSource; } }

    public bool audioLoading = false;

    public AudioPlayData(AudioClipData clip, AudioSource source, bool loop)
    {
        m_audioClip = clip.soundClip;
        m_audioSource = source;
        m_audioName = clip.soundName;
    }

    public float Progress
    {
        get
        {
            if (m_audioClip == null || m_audioSource == null) return 1f;

            return (float)m_audioSource.timeSamples / (float)m_audioClip.samples;
        }
    }

    public Action<AudioPlayData> onFinished;

    public bool IsFinished
    {
        get
        {
            if (Progress >= 1)
            {
                if (onFinished != null) onFinished(this);
                if (m_audioSource != null)
                {
                    GameObject.Destroy(m_audioSource);
                }
                m_audioSource = null;
                return true;
            }
            return false;
        }
    }

}
