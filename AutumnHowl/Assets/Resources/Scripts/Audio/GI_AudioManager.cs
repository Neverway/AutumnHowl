using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;

// ReSharper disable once HollowTypeName
public class GI_AudioManager : MonoBehaviour
{
    public static GI_AudioManager Instance => GameInstance.Get<GI_AudioManager>();

    /// <summary>
    /// enum for organizing our music so we can call it from scripts.
    /// </summary>
    public enum Music
    {
        none,
        Title,
        Town,
        Shop,
        Forest,
        Battle,
        BattleExtra,
        FinalBattleIntro,
        FinalBattle,
        GameOver,
        Victory,
        Neverway,
        Credits,
    }
    /// <summary>
    /// Stores the current music track.
    /// </summary>
    public Music currentTrack = Music.none;


    //========Audio Clips========//
    [Header("Audio Tracks")]
    /// This space is for references to sounds, which are assigned in the Inspector in an AudioManager prefab.
    public AudioClip slash1;
    public AudioClip slash2;
    public AudioClip slash3;
    public AudioClip hitBounce;
    public AudioClip hitDamage;
    public AudioClip hitKill;
    public AudioClip failBuzz;
    public AudioClip splat;
    public AudioClip vineRumble;
    public AudioClip vineAttack;
    public AudioClip growl;

    //==========Music============//
    [Header("Music Tracks")]
    public AudioClip mus_Title;
    public AudioClip mus_Town;
    public AudioClip mus_Shop;
    public AudioClip mus_Forest;
    public AudioClip mus_Battle;
    public AudioClip mus_BattleExtra;
    public AudioClip mus_FinalBattleIntro;
    public AudioClip mus_FinalBattle;
    public AudioClip mus_GameOver;
    public AudioClip mus_Victory;
    public AudioClip mus_Neverway;
    public AudioClip mus_Credits;

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
    [SerializeField] private AudioSource ambientSource;

    /// <summary>
    ///Initialize some things.
    /// </summary>
    void Start()
    {
        soundSource = gameObject.GetComponent<AudioSource>();
    }

    /// <summary>
    ///Play the given sound effect once at the specified volume.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogError ("AudioClip wasn't assigned.");
            return;
        }
        soundSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    ///Play the given sound effect once at the specified volume, on the sword-slash AudioSource specifically.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public void PlaySlashClip (AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogError ("AudioClip wasn't assigned.");
            return;
        }
        slashSource.Stop ();
        slashSource.PlayOneShot (clip, volume);
    }
    public void StopSlashClip ()
    {
        slashSource.Stop ();
    }

    public void SetMusicPitch(float _pitch, float _speed = 0.5f)
    {
        musicSource.DOPitch(_pitch, _speed);
        //ambientSource.DOPitch(_pitch, 0.5f);
    }

    /// <summary>
    /// Used during the boss cutscene to make the ambience disapear
    /// </summary>
    /// <param name="_pitch"></param>
    public void SetAmbiencePitch(float _pitch)
    {
        ambientSource.DOPitch(_pitch, 0.5f);
    }

    //Play a random clip from the Goal Mix. Uesd when a level is completed.
    public void PlayRandomSound (AudioClip[] _list)
    {
        if (_list == null)
        {
            Debug.LogError ("AudioClip wasn't assigned.");
            return;
        }
        if (_list.Count()== 0)
        {
            return;
        }
        int sound = Random.Range(0, _list.Length - 1);
        soundSource.PlayOneShot(_list[sound], 0.7f);
    }

    //Start playing the specified song, unless the song is already playing, in which case do nothing.
    public void SetMusic(Music song)
    {
        if (song == currentTrack)
            return;

        currentTrack = song;

        if (song == Music.none)
        {
            musicSource.Stop();
            return;
        }

        switch (song)
        {
            case Music.Title:
                musicSource.clip = mus_Title;
                break;
            case Music.Town:
                musicSource.clip = mus_Town;
                break;
            case Music.Shop:
                musicSource.clip = mus_Shop;
                break;
            case Music.Forest:
                musicSource.clip = mus_Forest;
                break;
            case Music.Battle:
                musicSource.clip = mus_Battle;
                break;
            case Music.BattleExtra:
                musicSource.clip = mus_BattleExtra;
                break;
            case Music.FinalBattleIntro:
                musicSource.clip = mus_FinalBattleIntro;
                break;
            case Music.FinalBattle:
                musicSource.clip = mus_FinalBattle;
                break;
            case Music.GameOver:
                musicSource.clip = mus_GameOver;
                break;
            case Music.Victory:
                musicSource.clip = mus_Victory;
                break;
            case Music.Neverway:
                musicSource.clip = mus_Neverway;
                break;
            case Music.Credits:
                musicSource.clip = mus_Credits;
                break;
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