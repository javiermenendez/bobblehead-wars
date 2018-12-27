using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioClip gunFire;
    public AudioClip upgradedGunFire;
    public AudioClip hurt;
    public AudioClip alienDeath;
    public AudioClip marineDeath;
    public AudioClip victory;
    public AudioClip elevatorArrived;
    public AudioClip powerUpPickup;
    public AudioClip powerUpAppear;

    public static SoundManager Instance = null;
    private AudioSource soundEffectAudio;

	// Use this for initialization
	void Start () {

        CreateSingletonSoundManager();

        GetAudioSourceHandle();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// This is a simple coding design pattern, known as a singleton pattern, 
    /// that ensures that there is always only one copy of this object in existence.
    /// </summary>
    private void CreateSingletonSoundManager()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Gets the audio source component handle.
    /// </summary>
    private void GetAudioSourceHandle()
    { 
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (AudioSource src in sources)
        {
            // Check the clip property to distinguish the effect source as 
            // the one that doesn't play (background) music
            if (src.clip == null)
                soundEffectAudio = src;
        }
    }

    /// <summary>
    /// A wrapper call to PlayOneShot.
    /// </summary>
    /// <param name="clip">Clip.</param>
    public void PlayOneShot(AudioClip clip)
    {
        soundEffectAudio.PlayOneShot(clip);
    }
}
