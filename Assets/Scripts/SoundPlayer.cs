

using UnityEngine;
using UnityUtils;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;

    public void PlaySound(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip);
    }
}
