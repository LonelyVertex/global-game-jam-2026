using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private AudioSource musicAudioSource;

    public static SoundManager Instance { get; private set; }

    private bool _musicPlaying;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic()
    {
        if (_musicPlaying) return;

        musicAudioSource.Play();
        _musicPlaying = true;
    }

    public void PlaySound(AudioClip clip)
    {
        GameObject audioObject = Instantiate(audioSourcePrefab, transform);
        SoundEffect soundEffect = audioObject.GetComponent<SoundEffect>();
        soundEffect.PlaySound(clip);
    }
}
