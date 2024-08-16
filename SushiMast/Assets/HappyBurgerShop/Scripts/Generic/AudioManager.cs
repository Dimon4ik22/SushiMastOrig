using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private bool isMuted;
    private bool isAdPlaying; // Flag to track ad state
    private List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMuteState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterAudioSource(AudioSource source)
    {
        if (source != null && !audioSources.Contains(source))
        {
            audioSources.Add(source);
            source.mute = isMuted;
        }
    }

    public void UnregisterAudioSource(AudioSource source)
    {
        if (source != null && audioSources.Contains(source))
        {
            audioSources.Remove(source);
        }
    }

    public void SetMuteState(bool mute)
    {
        isMuted = mute;
        foreach (var source in audioSources)
        {
            if (source != null)
            {
                source.mute = mute;
            }
        }
        PlayerPrefs.SetInt("IsMuted", mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool GetMuteState()
    {
        return isMuted;
    }

    public void LoadMuteState()
    {
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        foreach (var source in audioSources)
        {
            if (source != null)
            {
                source.mute = isMuted;
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        audioSources.Clear();
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (var source in sources)
        {
            RegisterAudioSource(source);
        }
        SetMuteState(isMuted);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (isAdPlaying)
            return;

        Silence(!hasFocus);
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isAdPlaying)
            return;

        Silence(isPaused);
    }

    public void OnAdStarted()
    {
        isAdPlaying = true;
        Silence(true);
    }

    public void OnAdEnded()
    {
        isAdPlaying = false;
        Silence(isMuted);
    }

    private void Silence(bool silence)
    {
        AudioListener.pause = silence;
    }
}
