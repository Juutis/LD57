using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class MusicManager : MonoBehaviour
{

    public static MusicManager main;
    private AudioSource elevatorMusic;
    private AudioSource menuMusic;
    private AudioSource gameMusic;


    [SerializeField]
    private AudioClip gameMusicClip;
    [SerializeField]
    private AudioClip elevatorMusicClip;
    [SerializeField]
    private AudioClip menuMusicClip;

    private List<AudioFade> fades = new List<AudioFade>();


    [SerializeField]
    float volumeelevator = 0.5f;
    [SerializeField]
    float volumeMenu = 0.5f;
    [SerializeField]
    float volumeGame = 0.5f;

    [SerializeField]
    float crossfadeDurationOut = 2.5f;
    [SerializeField]
    float crossfadeDurationIn = 2.5f;

    [SerializeField]
    private AudioSource audioSourcePrefab;


    private AudioSource currentMusic;


    [SerializeField]
    private AudioSource outsideAmbience;

    private AudioFade ambienceFade;

    private void Awake()
    {
        InitializeAudioSources();
        main = this;
        StartMusic(MusicType.Menu);
    }

    void Start()
    {

    }

    public void FadeOutAmbience() {
        ambienceFade = new AudioFade(0.5f, 0f, outsideAmbience, 1f);
    }

    public void FadeInAmbience()
    {
        ambienceFade = new AudioFade(0.5f, 0.4f, outsideAmbience, 1f);
    }

    public void StartMusic(MusicType musicType)
    {
        InitializeAudioSources();
        if (musicType == MusicType.Menu)
        {
            currentMusic = menuMusic;
            currentMusic.volume = volumeMenu;
        }
        if (musicType == MusicType.Game)
        {
            currentMusic = gameMusic;
            currentMusic.volume = volumeGame;
        }
        if (musicType == MusicType.Elevator)
        {
            currentMusic = elevatorMusic;
            currentMusic.volume = volumeelevator;
        }
        if (!currentMusic.isPlaying)
        {
            currentMusic.Play();
        }
    }

    public void SwitchMusic(MusicType musicType)
    {
        AudioSource newSource = null;
        if (musicType == MusicType.Menu)
        {
            newSource = menuMusic;
            newSource.volume = volumeMenu;
        }
        if (musicType == MusicType.Game)
        {
            newSource = gameMusic;
            newSource.volume = volumeGame;
        }
        if (musicType == MusicType.Elevator)
        {
            newSource = elevatorMusic;
            newSource.volume = volumeelevator;
        }
        if (currentMusic == newSource)
        {
            return;
        }
        CrossFade(currentMusic, newSource, crossfadeDurationOut, crossfadeDurationIn, newSource.volume, 1.0f);
        currentMusic = newSource;

    }


    private void InitializeAudioSources()
    {
        if (gameMusic == null)
        {
            gameMusic = InitializeAudioSource("Game music", gameMusicClip);
        }
        if (elevatorMusic == null)
        {
            elevatorMusic = InitializeAudioSource("elevator music", elevatorMusicClip);
        }
        if (menuMusic == null)
        {
            menuMusic = InitializeAudioSource("Menu music", menuMusicClip);
        }
    }

    private AudioSource InitializeAudioSource(string name, AudioClip clip)
    {
        AudioSource source = Instantiate(audioSourcePrefab);
        source.clip = clip;
        source.volume = 0;
        source.transform.SetParent(transform);
        source.transform.position = Vector2.zero;
        source.playOnAwake = false;
        source.loop = true;
        source.name = name;
        return source;
    }

    public void Fade(AudioSource fadeSource, float targetVolume, float duration = 0.5f, float targetPitch = 1.0f)
    {
        AudioFade fade = new AudioFade(duration, targetVolume, fadeSource, targetPitch);
        fades.Add(fade);
    }

    public void FadeOutMusic(float duration = 0.5f)
    {
        Fade(currentMusic, 0, duration);
    }

    public void CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource, float durationOut, float durationIn, float volume, float targetPitch)
    {
        fades.Clear();
        AudioFade fadeOut = new AudioFade(durationOut, 0f, fadeOutSource, targetPitch);
        AudioFade fadeIn = new AudioFade(durationIn, volume, fadeInSource, targetPitch);
        if (!fadeInSource.isPlaying) {
            fadeInSource.Play();
        }
        fades.Add(fadeOut);
        fades.Add(fadeIn);
    }

    public void Update()
    {
        for (int index = 0; index < fades.Count; index += 1)
        {
            AudioFade fade = fades[index];
            if (fade != null && fade.IsFading)
            {
                fade.Update();
            }
            if (!fade.IsFading)
            {
                fades.Remove(fade);
            }
        }
        if (ambienceFade != null && ambienceFade.IsFading) {
            ambienceFade.Update();
        }
    }

}

public enum MusicType
{
    Menu,
    Game,
    Elevator
}

public class AudioFade
{
    public AudioFade(float duration, float target, AudioSource track, float targetPitch)
    {
        if (!track.isPlaying)
        {
            track.Play();
        }
        this.duration = duration;
        IsFading = true;
        timer = 0f;
        originalVolume = track.volume;
        targetVolume = target;
        audioSource = track;

        originalPitch = track.pitch;
        this.targetPitch = targetPitch;
    }
    public bool IsFading { get; private set; }
    private float duration;
    private float timer;
    private float targetVolume;
    private AudioSource audioSource;
    private float originalVolume;

    private float originalPitch, targetPitch;

    public void Update()
    {
        timer += Time.unscaledDeltaTime / duration;
        audioSource.volume = Mathf.Lerp(originalVolume, targetVolume, timer);
        audioSource.pitch = Mathf.Lerp(originalPitch, targetPitch, timer);
        if (timer >= 1)
        {
            audioSource.volume = targetVolume;
            audioSource.pitch = targetPitch;
            IsFading = false;
        }
    }
}