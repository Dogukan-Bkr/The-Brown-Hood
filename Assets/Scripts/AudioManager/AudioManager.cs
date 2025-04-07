using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private AudioSource musicSource;

    private bool isMuted = false;
    private bool isMusicMuted = false;

    private float initialVolume = 1f;         // Tüm sistem sesi için
    private float initialMusicVolume = 1f;    // Müzik özel sesi için

    private void Awake()
    {
        instance = this;

        // Sistemin o anki ses deðerini sakla
        initialVolume = AudioListener.volume;

        if (musicSource != null)
        {
            initialMusicVolume = musicSource.volume;
        }
    }

    public void PlayAudio(int index)
    {
        if (index < 0 || index >= audioSources.Length)
        {
            Debug.LogError("Audio index out of range");
            return;
        }

        audioSources[index].Stop();
        audioSources[index].Play();
    }

    public void PlayRandomAudio(int index)
    {
        if (index < 0 || index >= audioSources.Length)
        {
            Debug.LogError("Audio index out of range");
            return;
        }

        audioSources[index].Stop();
        audioSources[index].pitch = Random.Range(0.8f, 1.2f);
        audioSources[index].Play();
    }

    public void ToggleAllSounds()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            initialVolume = AudioListener.volume; // Ses kapatýlmadan önceki deðeri sakla
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = initialVolume; // Eski ses seviyesine geri dön
        }

        Debug.Log("Tüm sesler " + (isMuted ? "kapalý" : "açýk"));
    }

    public void ToggleMusic()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("Müzik kaynaðý atanmadý!");
            return;
        }

        isMusicMuted = !isMusicMuted;

        if (isMusicMuted)
        {
            initialMusicVolume = musicSource.volume; // kapatmadan önceki deðer
            musicSource.volume = 0f;
        }
        else
        {
            musicSource.volume = initialMusicVolume;
        }

        Debug.Log("Müzik " + (isMusicMuted ? "kapalý" : "açýk"));
    }
}
