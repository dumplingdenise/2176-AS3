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
    public static AudioManager instance;

    [Header("Sound Effects")]
    public Sound[] sounds;

    [Header("Footstep System")]
    public Sound[] footstepSounds;
    public float timeBetweenSteps = 0.5f;

    // Internal variables for the footstep system
    private int footstepIndex = 0;
    private float footstepTimer = 0f;
    private bool isPlayerWalking = false; // This will be controlled by the PlayerMovement script

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        // Setup AudioSources for all sound types
        foreach (Sound s in sounds) { SetupAudioSource(s); }
        foreach (Sound f in footstepSounds) { SetupAudioSource(f); }
    }

    // Helper function to create an AudioSource for a Sound object
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
        // This is the core of the new footstep logic
        HandleFootsteps();
    }

    // A public method that the Player can call to update its walking state
    public void SetWalkingState(bool walking)
    {
        isPlayerWalking = walking;
    }

    private void HandleFootsteps()
    {
        // If the player is not walking, or if there are no footstep sounds, do nothing.
        if (!isPlayerWalking || footstepSounds.Length == 0)
        {
            footstepTimer = 0f; // Reset timer when not walking
            return;
        }

        // Increment the timer
        footstepTimer += Time.deltaTime;

        // If the timer has reached the desired interval
        if (footstepTimer >= timeBetweenSteps)
        {
            footstepTimer = 0f; // Reset the timer

            // Play the current footstep sound
            footstepSounds[footstepIndex].source.Play();

            // Move to the next footstep sound in the list, looping back to the start if necessary
            footstepIndex = (footstepIndex + 1) % footstepSounds.Length;
        }
    }

    // --- Standard Play Functions ---
    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { Debug.LogWarning("Sound: '" + name + "' not found!"); return; }
        s.source.Play();
    }


}