using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private GameObject audioSourcePrefab;

    public static SoundManager Instance { get; private set; }

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

    public void PlaySound(AudioClip clip)
    {
        GameObject audioObject = Instantiate(audioSourcePrefab, transform);
        SoundEffect soundEffect = audioObject.GetComponent<SoundEffect>();
        soundEffect.PlaySound(clip);
    }
}
