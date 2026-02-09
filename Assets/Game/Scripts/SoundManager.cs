using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

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

        yield return new WaitForSeconds(clip.length);

        sfxSource.pitch = originalPitch;
        sfxSource.volume = originalVolume;
    }
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = volume;
        musicSource.Play();
    }
}
