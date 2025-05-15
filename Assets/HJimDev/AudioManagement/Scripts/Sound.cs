using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class Sound : MonoBehaviour
    {
        AudioSource audioSource;
        bool initialized;
        bool destroy;

        public void Initialize(AudioClip clip)
        {
            if (!initialized)
            {
                initialized = true;
                destroy = true;
                audioSource = GetComponent<AudioSource>();
                audioSource.volume = AudioManager.DefaultManager.SoundVolume;
                audioSource.clip = clip;
                audioSource.loop = false;
                audioSource.playOnAwake = false;
            }
        }

        public void Play()
        {
            audioSource.Play();
        }

        void Update()
        {
            if(destroy)
            {
                if(!audioSource.isPlaying)
                {
                    destroy = false;
                    Destroy(gameObject);
                }
            }
        }
    }
}