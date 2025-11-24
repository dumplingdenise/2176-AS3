using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    // static variable to hold the single instance of AudioManager so it can be accessed globally
    public static AudioManager instance;

    [Header("Sound Effects")]
    public Sound[] sounds;

    [Header("Music")]
    public Sound[] music;

    [Header("Footstep System")]
    [Tooltip("The list of footstep sounds to cycle through.")]
    public Sound[] footstepSounds;
    [Tooltip("The time in seconds between each footstep sound.")]
    public float timeBetweenSteps = 0.5f;

    // internal variables for footstep system to track timing & which audio clip to play next
    private int footstepIndex = 0;
    private float footstepTimer = 0f;
    private bool isPlayerWalking = false; // controlled by PlayerMovement script

    void Awake()
    {
        // implement singleton pattern -  ensures only 1 instance of audiomanager exists
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        // loops through each sound array & creates audio source component for every clip
        foreach (Sound s in sounds) { SetupAudioSource(s); }
        foreach (Sound m in music) { SetupAudioSource(m); }
        foreach (Sound f in footstepSounds) { SetupAudioSource(f); }
    }

    // helper function - creates & configures audio source component for a given sound
    void SetupAudioSource(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
    }

    // helper function to get length of audio file
    public float GetSoundDuration(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: '" + name + "' not found! Returning 0 duration.");
            return 0f;
        }
        return s.source.clip.length;
    }

    void Update()
    {
        // run footstep timer every frame 
        HandleFootsteps();
    }

    // public method that Player can call to update its walking state
    public void SetWalkingState(bool walking)
    {
        isPlayerWalking = walking;
    }

    private void HandleFootsteps()
    {
        // only run timer if player is walking & footstep audio is available
        if (!isPlayerWalking || footstepSounds.Length == 0)
        {
            // reset timer when not walking
            footstepTimer = 0f;
            return;
        }

        // increment timer
        footstepTimer += Time.deltaTime;

        // when timer reaches time between steps - play sound & reset
        if (footstepTimer >= timeBetweenSteps)
        {
            footstepTimer = 0f; // reset timer
            footstepSounds[footstepIndex].source.Play(); // play current footstep sound

            // use modulo operator for looping
            footstepIndex = (footstepIndex + 1) % footstepSounds.Length;
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { Debug.LogWarning("Sound: '" + name + "' not found!"); return; }
        s.source.Play();
    }

    public void PlayMusic(string name)
    {
        Sound m = Array.Find(music, mus => mus.name == name);
        if (m == null) { Debug.LogWarning("Music: '" + name + "' not found!"); return; }
        m.source.Play();
    }

}