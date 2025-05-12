using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class EntitySFX : MonoBehaviour
{
    [SerializeField] public AudioClip skillCharge;
    [SerializeField] public AudioClip[] barrelHit;
    [SerializeField] public AudioClip[] barrelDeath;
    [SerializeField] public AudioClip[] enemyConfused;
    [SerializeField] public AudioClip[] bow;
    [SerializeField] public AudioClip[] stun;
    [SerializeField] public AudioClip chainProjectile;
    [SerializeField] public AudioClip chainHit;
    [SerializeField] public AudioClip chainActive;
    [SerializeField] public AudioClip chainSearchLoop;
    [SerializeField] public AudioClip chainLocked;
    [SerializeField] public AudioClip chainBreak;
    [SerializeField] public AudioClip annihilationCharge;
    [SerializeField] public AudioClip explosion;
    [SerializeField] public AudioClip dash;
    [SerializeField] public AudioClip shatter;
    [SerializeField] public AudioClip glow;

    public AudioSource audioSource;
    private float baseVolume;
    private float basePitch;
    private float lastSoundTime;
    private float soundCooldown = 0.15f;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        baseVolume = audioSource.volume;
        basePitch = audioSource.pitch;
    }

    private void Update()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    public void BarrelHitSFX()  //ÝKÝ SES ARASINDA DELAY VAR!
    {
        if (barrelHit.Length > 0 && Time.time >= lastSoundTime + soundCooldown) 
        {
            int randomIndex;
            randomIndex = Random.Range(0, barrelHit.Length);
            audioSource.PlayOneShot(barrelHit[randomIndex]);
            lastSoundTime = Time.time;
        }
    }

    public void BarrelDeathSFX()
    {

        float volume = 0.5f;
        int randomIndex;
        randomIndex = Random.Range(0, barrelDeath.Length);

        GameObject tempAudio = new GameObject("TempDeathSound");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = barrelDeath[randomIndex];
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(tempAudio, barrelDeath[randomIndex].length); // Destroy the temporary object after the sound finishes
    }

    public void EnemyConfusedSFX()
    {
        if (enemyConfused.Length > 0)
        {
            int randomIndex;
            randomIndex = Random.Range(0, enemyConfused.Length);
            audioSource.PlayOneShot(enemyConfused[randomIndex]);
        }
    }

    public void BowSFX()
    {
        if (bow.Length > 0)
        {
            int randomIndex;
            randomIndex = Random.Range(0, bow.Length);
            audioSource.PlayOneShot(bow[randomIndex]);
        }
    }

    public void StunSFX()
    {
        if (stun.Length > 0)
        {
            int randomIndex;
            randomIndex = Random.Range(0, stun.Length);
            audioSource.PlayOneShot(stun[randomIndex]);
        }
    }

    public void ChainProjectile()
    {
        audioSource.loop = false;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.PlayOneShot(chainProjectile);
    }

    public void ChainHit()
    {
        audioSource.loop = false;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.PlayOneShot(chainHit);
    }
    public void ChainActive()
    {
        audioSource.loop = false;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.PlayOneShot(chainActive);
        Debug.Log("activ");
    }

    public void ChainSearchLoop()
    {
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.clip = chainSearchLoop;
        audioSource.Play();

    }

    public void ChainLocked()
    {
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.PlayOneShot(chainLocked);
    }

    public void ChainBreak()
    {
        if(chainBreak != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.PlayOneShot(chainBreak);
        }

    }

    public void AnnihilationCharge()
    {
        audioSource.clip = annihilationCharge;
        audioSource.pitch = basePitch;
        audioSource.Play();
    }

    public void AnnihilationGo()
    {
        audioSource.Stop();
        audioSource.pitch = 1;
        audioSource.PlayOneShot(explosion);
        StartCoroutine(VolumeDelay());
    }

    IEnumerator VolumeDelay()
    {
        yield return new WaitForSeconds(explosion.length);
        audioSource.volume = baseVolume;
    }

    public void SkillCharge()
    {
        audioSource.PlayOneShot(skillCharge);
    }

    public void Dash()
    {
        audioSource.PlayOneShot(dash);
    }

    public void Shatter()
    {
        audioSource.PlayOneShot(shatter);
    }

    public void Glow()
    {
        audioSource.clip = glow;
        audioSource.loop = true;
        audioSource.Play();
    }



}
