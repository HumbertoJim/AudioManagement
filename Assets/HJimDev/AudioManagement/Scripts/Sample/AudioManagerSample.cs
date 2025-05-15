using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioManagement
{
    namespace Sample
    {
        public class AudioManagerSample : MonoBehaviour
        {
            public AudioClip[] musics;

            public void PlaySound()
            {
                AudioManager.DefaultManager.PlaySound("explosion");
            }

            public void PlayMainMusic()
            {
                AudioManager.DefaultManager.PlayMusicMain();
            }

            public void PlaySecondaryMusic()
            {
                int index = Random.Range(0, musics.Length);
                Debug.Log("Playing " + musics[index].name);
                AudioManager.DefaultManager.PlayMusic(musics[index], MusicType.Secondary);
            }
        }
    }
}