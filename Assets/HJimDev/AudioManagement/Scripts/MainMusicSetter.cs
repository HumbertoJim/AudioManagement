using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioManagement
{
    public class MainMusicSetter : MonoBehaviour
    {
        [Header("Defaults")]
        [SerializeField] AudioClip defaultMusic;

        protected virtual void Start()
        {
            AudioManager.DefaultManager.PlayMusic(defaultMusic, MusicType.Main);
        }
    }
}
