using UnityEngine;

/// Audio Manager - Quan ly tat ca am thanh trong game
/// Gom co: Background Music, Sound Effects
public class AudioManager : MonoBehaviour
{
    // ====================
    // SINGLETON
    // ====================

    public static AudioManager Instance { get; private set; }

    // ====================
    // THONG SO NHAC NEN
    // ====================

    [Header("Background Music")]
    public AudioClip menuMusic;          // Nhac menu
    public AudioClip gameplayMusic;      // Nhac khi choi
    public AudioClip bossMusic;          // Nhac khi gap Boss
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    // ====================
    // THONG SO HIEU UNG AM THANH
    // ====================

    [Header("Sound Effects")]
    public AudioClip coinSound;          // Am thanh nhat coin
    public AudioClip attackSound;        // Am thanh tan cong
    public AudioClip hitSound;           // Am thanh bi danh
    public AudioClip deathSound;         // Am thanh chet
    public AudioClip dashSound;          // Am thanh dash
    public AudioClip jumpSound;          // Am thanh nhay
    public AudioClip healSound;          // Am thanh hoi mau
    public AudioClip purchaseSound;      // Am thanh mua hang
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;

    // ====================
    // THANH PHAN
    // ====================

    private AudioSource musicSource;
    private AudioSource sfxSource;

    // ====================
    // TRANG THAI
    // ====================

    private AudioClip currentMusic;

    // ====================
    // KHOI TAO
    // ====================

    void Awake()
    {
        SetupSingleton();
        CreateAudioSources();
    }

    void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateAudioSources()
    {
        // Tao AudioSource cho nhac nen
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        // Tao AudioSource cho hieu ung am thanh
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        // Tu dong phat nhac menu khi bat dau
        PlayMusic(menuMusic);
    }

    // ====================
    // QUAN LY NHAC NEN
    // ====================

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || clip == currentMusic) return;

        currentMusic = clip;
        musicSource.clip = clip;
        musicSource.Play();

        Debug.Log("Phat nhac: " + clip.name);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    public void PlayBossMusic()
    {
        PlayMusic(bossMusic);
    }

    public void StopMusic()
    {
        musicSource.Stop();
        currentMusic = null;
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    // ====================
    // QUAN LY HIEU UNG AM THANH
    // ====================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayCoinSound()
    {
        PlaySFX(coinSound);
    }

    public void PlayAttackSound()
    {
        PlaySFX(attackSound);
    }

    public void PlayHitSound()
    {
        PlaySFX(hitSound);
    }

    public void PlayDeathSound()
    {
        PlaySFX(deathSound);
    }

    public void PlayDashSound()
    {
        PlaySFX(dashSound);
    }

    public void PlayJumpSound()
    {
        PlaySFX(jumpSound);
    }

    public void PlayHealSound()
    {
        PlaySFX(healSound);
    }

    public void PlayPurchaseSound()
    {
        PlaySFX(purchaseSound);
    }

    // ====================
    // DIEU CHINH AM LUONG
    // ====================

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void MuteMusic(bool mute)
    {
        musicSource.mute = mute;
    }

    public void MuteSFX(bool mute)
    {
        sfxSource.mute = mute;
    }

    // ====================
    // GETTER
    // ====================

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    public bool IsMusicPlaying() => musicSource.isPlaying;
}