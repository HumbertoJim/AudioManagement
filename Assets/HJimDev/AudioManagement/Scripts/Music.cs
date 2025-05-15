using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScheduleManagement.Schedulers;
using ScheduleManagement.Schedulables;


namespace AudioManagement
{
    public enum MusicType { Main, Secondary }

    [RequireComponent(typeof(AudioSource))]
    public class Music : MonoBehaviour
    {
        enum Schedulers { UpVolume, DownVolume, PauseLater, StopLater, DestroyLater }

        AudioManager manager;
        AudioSource audioSource;
        Scheduler scheduler;
        float volume;
        bool initialized;

        public AudioClip Clip { get { return audioSource.clip; } }

        public MusicType Type { get; private set; }

        public bool IsPlaying { get { return initialized && !scheduler.IsActive(Schedulers.DownVolume.ToString()) && audioSource.isPlaying; } }

        bool Ready { get { return initialized && !scheduler.IsActive(Schedulers.DestroyLater.ToString()); } }

        public void Initialize(AudioClip clip, MusicType type)
        {
            if (!initialized)
            {
                initialized = true;
                Type = type;
                audioSource = GetComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.playOnAwake = false;
                audioSource.volume = 0;
                manager = AudioManager.DefaultManager;
                scheduler = new();
                scheduler.Add(Schedulers.UpVolume.ToString(), new Schedulable(manager.TimeToUpDownMusicVolume / manager.IntervalsToUpDownMusicVolume, () =>
                {
                    volume += manager.MusicVolume / manager.IntervalsToUpDownMusicVolume;
                    if(volume < manager.MusicVolume) scheduler.Start(Schedulers.UpVolume.ToString()); else volume = manager.MusicVolume;
                    audioSource.volume = volume;
                }));
                scheduler.Add(Schedulers.DownVolume.ToString(), new Schedulable(manager.TimeToUpDownMusicVolume / manager.IntervalsToUpDownMusicVolume, () =>
                {
                    volume -= manager.MusicVolume / manager.IntervalsToUpDownMusicVolume;
                    if (volume > 0) scheduler.Start(Schedulers.DownVolume.ToString()); else volume = 0;
                    audioSource.volume = volume;
                }));
                scheduler.Add(Schedulers.PauseLater.ToString(), new Schedulable(manager.TimeToUpDownMusicVolume, () => audioSource.Pause()));
                scheduler.Add(Schedulers.StopLater.ToString(), new Schedulable(manager.TimeToUpDownMusicVolume, () => audioSource.Stop()));
                scheduler.Add(Schedulers.DestroyLater.ToString(), new Schedulable(manager.TimeToUpDownMusicVolume, () => DestroyImmediate(gameObject)));
            }
        }

        private void Update()
        {
            if (initialized) scheduler.Process();
        }

        public void Play()
        {
            if(Ready)
            {
                if (scheduler.IsActive(Schedulers.StopLater.ToString())) scheduler.Finish(Schedulers.StopLater.ToString());
                if (scheduler.IsActive(Schedulers.PauseLater.ToString())) scheduler.Finish(Schedulers.PauseLater.ToString());
                scheduler.Cancel(Schedulers.DownVolume.ToString());
                scheduler.Start(Schedulers.UpVolume.ToString());
                audioSource.Play();
            }
        }

        public void Pause()
        {
            if (Ready)
            {
                scheduler.Cancel(Schedulers.UpVolume.ToString());
                scheduler.Start(Schedulers.DownVolume.ToString());
                scheduler.Start(Schedulers.PauseLater.ToString());
            }
        }

        public void Stop()
        {
            if (Ready)
            {
                scheduler.Cancel(Schedulers.UpVolume.ToString());
                scheduler.Start(Schedulers.DownVolume.ToString());
                scheduler.Start(Schedulers.StopLater.ToString());
            }
        }

        public void Destroy()
        {
            if (Ready)
            {
                Stop();
                scheduler.Start(Schedulers.DestroyLater.ToString());
            }
        }
    }
}