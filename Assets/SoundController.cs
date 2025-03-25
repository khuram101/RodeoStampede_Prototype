using UnityEngine;

public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] FootstepAudioClips;
    [Range(0, 1), SerializeField] private float FootstepAudioVolume = 0.5f;
    private float nextFootstepTime = 0f;
    private float FootstepInterval = 0.35f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
   public void PlayFootstep()
    {
        if (FootstepAudioClips.Length == 0) return;
        if (Time.time >= nextFootstepTime)
        {
            int index = Random.Range(0, FootstepAudioClips.Length);
            float volume = Random.Range(0.1f, FootstepAudioVolume);

            audioSource.clip = FootstepAudioClips[index];
            audioSource.volume = volume;
            audioSource.Play();

            nextFootstepTime = Time.time + FootstepInterval;
        }
    }
}
