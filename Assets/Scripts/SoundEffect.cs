using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);

        Destroy(gameObject, clip.length);
    }
}
