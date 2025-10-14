using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    /// <summary>
    /// enum for organizing our music so we can call it from scripts.
    /// </summary>
    public enum music
    {
        none,
        FinalBattle
    }
    /// <summary>
    /// Stores the current music track.
    /// </summary>
    public music currentTrack = music.none;


    //========Audio Clips========//

    /// This space is for references to sounds, which are assigned in the Inspector in an AudioManager prefab.
    public AudioClip slash1;
    public AudioClip slash2;
    public AudioClip slash3;
    public AudioClip hitBounce;
    public AudioClip hitDamage;
    public AudioClip hitKill;

    //==========Music============//

    public AudioClip mus_FinalBattle;

    //===========================//

    /// <summary>
    /// AudioSource for general sfx.
    /// </summary>
    AudioSource soundSource;
    /// <summary>
    /// Unique audio source for playing sword slash sounds, so that they will be interruptible.
    /// </summary>
    [SerializeField] private AudioSource slashSource;
    /// <summary>
    /// Unique audio source for looping music.
    /// </summary>
    [SerializeField] private AudioSource musicSource;

    /// <summary>
    ///Initialize some things.
    /// </summary>
    void Start()
    {
        if (Instance != null)
        {
            Destroy (gameObject);
            return;
        }
        Instance = this;

        soundSource = gameObject.GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    ///Play the given sound effect once at the specified volume.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        soundSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    ///Play the given sound effect once at the specified volume, on the sword-slash AudioSource specifically.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public void PlaySlashClip (AudioClip clip, float volume = 1f)
    {
        slashSource.Stop ();
        slashSource.PlayOneShot (clip, volume);
    }

    //Play a random clip from the Goal Mix. Uesd when a level is completed.
    public void PlayRandomSound (AudioClip[] _list)
    {
        int sound = Random.Range(0, _list.Length - 1);
        soundSource.PlayOneShot(_list[sound], 0.7f);
    }

    //Start playing the specified song, unless the song is already playing, in which case do nothing.
    public void SetMusic(music song)
    {
        if (song == currentTrack)
            return;

        currentTrack = song;

        if (song == music.none)
        {
            musicSource.Stop();
            return;
        }

        switch (song)
        {
            case music.FinalBattle:
            {
                musicSource.clip = mus_FinalBattle;
                break;
            }
        }

        musicSource.Play();
    }

    //Mute music, but not sound.
    public void MuteMusic()
    {
        musicSource.Pause();
    }

    //Unmute music.
    public void UnmuteMusic()
    {
        musicSource.UnPause();
    }
}