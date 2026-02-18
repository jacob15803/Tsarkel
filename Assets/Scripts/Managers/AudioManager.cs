using UnityEngine;
using Tsarkel.Managers;

namespace Tsarkel.Managers
{
    /// <summary>
    /// Manages audio playback for the game.
    /// Handles environmental sounds, music, and sound effects.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [Tooltip("Music audio source")]
        [SerializeField] private AudioSource musicSource;
        
        [Tooltip("SFX audio source")]
        [SerializeField] private AudioSource sfxSource;
        
        [Tooltip("Ambient audio source")]
        [SerializeField] private AudioSource ambientSource;
        
        [Header("Audio Clips")]
        [Tooltip("Background music")]
        [SerializeField] private AudioClip backgroundMusic;
        
        [Tooltip("Ambient sound")]
        [SerializeField] private AudioClip ambientSound;
        
        [Header("Settings")]
        [Tooltip("Master volume (0-1)")]
        [SerializeField] private float masterVolume = 1f;
        
        [Tooltip("Music volume (0-1)")]
        [SerializeField] private float musicVolume = 0.7f;
        
        [Tooltip("SFX volume (0-1)")]
        [SerializeField] private float sfxVolume = 1f;
        
        [Tooltip("Ambient volume (0-1)")]
        [SerializeField] private float ambientVolume = 0.5f;
        
        private void Start()
        {
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
            
            if (ambientSource == null)
            {
                GameObject ambientObj = new GameObject("AmbientSource");
                ambientObj.transform.SetParent(transform);
                ambientSource = ambientObj.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }
            
            // Play background music and ambient sound
            if (backgroundMusic != null)
            {
                PlayMusic(backgroundMusic);
            }
            
            if (ambientSound != null)
            {
                PlayAmbient(ambientSound);
            }
        }
        
        /// <summary>
        /// Plays a music clip.
        /// </summary>
        public void PlayMusic(AudioClip clip)
        {
            if (musicSource != null && clip != null)
            {
                musicSource.clip = clip;
                musicSource.volume = musicVolume * masterVolume;
                musicSource.Play();
            }
        }
        
        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
            }
        }
        
        /// <summary>
        /// Plays an ambient sound.
        /// </summary>
        public void PlayAmbient(AudioClip clip)
        {
            if (ambientSource != null && clip != null)
            {
                ambientSource.clip = clip;
                ambientSource.volume = ambientVolume * masterVolume;
                ambientSource.Play();
            }
        }
        
        /// <summary>
        /// Stops the music.
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        
        /// <summary>
        /// Stops the ambient sound.
        /// </summary>
        public void StopAmbient()
        {
            if (ambientSource != null)
            {
                ambientSource.Stop();
            }
        }
        
        /// <summary>
        /// Sets the master volume.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        /// <summary>
        /// Sets the music volume.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        /// <summary>
        /// Sets the SFX volume.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        /// <summary>
        /// Updates all audio source volumes.
        /// </summary>
        private void UpdateVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
            
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume * masterVolume;
            }
            
            if (ambientSource != null)
            {
                ambientSource.volume = ambientVolume * masterVolume;
            }
        }
    }
}
