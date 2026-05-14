using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM")]
    public AudioClip titleBGM;
    public AudioClip stageBGM;

    [Header("Audio Clips")]
    public AudioClip DashClip;  
    public AudioClip EnemyStartClip;  
    public AudioClip ErrorEnemyClip;  
    public AudioClip ErrorFallClip;  
    public AudioClip ItemClip;  
    public AudioClip ItemPowerupClip;  
    public AudioClip JumpClip;  

    void Awake()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;

        source.Play();

        Destroy(source, clip.length);
    }

    public void PlayBGM(AudioClip clip)
    {
        // 이미 같은 음악이면 재생 안함
        if (bgmSource.clip == clip)
            return;

        bgmSource.clip = clip;

        bgmSource.Play();
    }

}
