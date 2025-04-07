using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private AudioSource musicSource;

    private bool isMuted = false;
    private bool isMusicMuted = false;

    private float initialVolume = 1f;         // T�m sistem sesi i�in
    private float initialMusicVolume = 1f;    // M�zik �zel sesi i�in

    private void Awake()
    {
        instance = this;

        // Sistemin o anki ses de�erini sakla
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
            initialVolume = AudioListener.volume; // Ses kapat�lmadan �nceki de�eri sakla
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = initialVolume; // Eski ses seviyesine geri d�n
        }

        Debug.Log("T�m sesler " + (isMuted ? "kapal�" : "a��k"));
    }

    public void ToggleMusic()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("M�zik kayna�� atanmad�!");
            return;
        }

        isMusicMuted = !isMusicMuted;

        if (isMusicMuted)
        {
            initialMusicVolume = musicSource.volume; // kapatmadan �nceki de�er
            musicSource.volume = 0f;
        }
        else
        {
            musicSource.volume = initialMusicVolume;
        }

        Debug.Log("M�zik " + (isMusicMuted ? "kapal�" : "a��k"));
    }
}
