using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    private AudioSource[] audioSources;

    private void Awake()
    {
        instance = this;
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

}
