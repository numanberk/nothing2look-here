using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class EntitySFX : MonoBehaviour
{
    [SerializeField] public AudioClip[] barrelHit;
    [SerializeField] public AudioClip[] barrelDeath;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void BarrelHitSFX()
    {
        if (barrelHit.Length > 0)
        {
            int randomIndex;
            randomIndex = Random.Range(0, barrelHit.Length);
            audioSource.PlayOneShot(barrelHit[randomIndex]);
        }
    }

    public void BarrelDeathSFX()
    {

        float volume = 1.4f;
        int randomIndex;
        randomIndex = Random.Range(0, barrelDeath.Length);

        GameObject tempAudio = new GameObject("TempDeathSound");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = barrelDeath[randomIndex];
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Makes it 3D sound if applicable
        audioSource.Play();

        Destroy(tempAudio, barrelDeath[randomIndex].length); // Destroy the temporary object after the sound finishes
    }
}
