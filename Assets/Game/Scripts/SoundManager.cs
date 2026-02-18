using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Music")]
    [SerializeField] private float musicFadeTime = 1.5f;

    private Coroutine musicRoutine;
    private AudioClip currentClip;
    private float currentTime = 0f; // время трека для возврата

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------------- SFX ----------------

    public void PlaySFX(AudioClip clip, float pitch = 1f, float volume = 1f)
    {
        if (clip == null) return;
        StartCoroutine(PlaySFXRoutine(clip, pitch, volume));
    }

    private IEnumerator PlaySFXRoutine(AudioClip clip, float pitch, float volume)
    {
        float originalPitch = sfxSource.pitch;
        float originalVolume = sfxSource.volume;

        sfxSource.pitch = pitch;
        sfxSource.volume = volume;
        sfxSource.PlayOneShot(clip);

        yield return new WaitForSecondsRealtime(clip.length);

        sfxSource.pitch = originalPitch;
        sfxSource.volume = originalVolume;
    }

    // ---------------- MUSIC ----------------

    public void PlayMusic(AudioClip clip, float volume = 0.3f, bool resume = false)
    {
        if (clip == null) return;

        // Если уже играет тот же трек и resume, ничего не делаем
        if (resume && clip == currentClip && musicSource.isPlaying)
            return;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(MusicTransition(clip, volume, resume));
    }

    private IEnumerator MusicTransition(AudioClip newClip, float targetVolume, bool resume)
    {
        // Fade out текущего трека
        while (musicSource.volume > 0.01f)
        {
            musicSource.volume -= Time.deltaTime / musicFadeTime;
            yield return null;
        }

        // Сохраняем время текущего трека
        if (musicSource.clip != null)
            currentTime = musicSource.time;

        musicSource.Stop();
        musicSource.clip = newClip;

        // Если resume и трек тот же, продолжаем с сохранённого момента
        musicSource.time = (resume && newClip == currentClip) ? currentTime : 0f;

        currentClip = newClip; // обновляем currentClip только после установки времени
        musicSource.Play();

        // Fade in
        while (musicSource.volume < targetVolume)
        {
            musicSource.volume += Time.deltaTime / musicFadeTime;
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}
