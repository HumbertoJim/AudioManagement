using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField] [Range(0f, 1f)] float soundVolume = 1;
        [SerializeField] [Range(0f, 1f)] float musicVolume = 1;
        [SerializeField] [Range(0.1f, 10)] float timeToUpDownMusicVolume = 3;
        [SerializeField] [Range(1, 30)] int intervalsToUpDownMusicVolume = 15;
        [SerializeField] [Range(1, 30)] int maxSoundInstances = 20;

        [Header("Resources Folder")]
        [SerializeField] string musicsFolder = "Musics";
        [SerializeField] string soundsFolder = "Sounds";

        GameObject musicPrefab;
        GameObject soundPrefab;

        Music mainMusic;
        Music currentMusic;
        Transform soundContainer;

        private static AudioManager defaultManager;
        bool initialized;

        public static AudioManager DefaultManager
        {
            get
            {
                if (!defaultManager)
                {
                    GameObject instance = new("AudioManager");
                    DefaultManager = instance.AddComponent<AudioManager>();
                }
                return defaultManager;
            }
            private set
            {
                defaultManager = value;
                defaultManager.Initialize();
            }
        }

        GameObject MusicPrefab { get { return musicPrefab; } }

        GameObject SoundPrefab { get { return soundPrefab; } }

        public float MusicVolume
        {
            get { return musicVolume; }
            set { musicVolume = value > 1f ? 1f : (value < 0f ? 0f : value); }
        }

        public float SoundVolume
        {
            get { return soundVolume; }
            set { soundVolume = value > 1f ? 1f : (value < 0f ? 0f : value); }
        }

        public float TimeToUpDownMusicVolume
        {
            get { return timeToUpDownMusicVolume; }
            set { timeToUpDownMusicVolume = value < 0.1f ? 0.1f : value; }
        }

        public int IntervalsToUpDownMusicVolume
        {
            get { return intervalsToUpDownMusicVolume; }
            set { intervalsToUpDownMusicVolume = value < 1 ? 1 : value; }
        }

        private void Awake()
        {
            if (defaultManager && defaultManager != this)
            {
                DestroyImmediate(gameObject);
            }
            else if (!defaultManager)
            {
                DefaultManager = this;
            }
        }

        void Initialize()
        {
            if (!initialized)
            {
                initialized = true;
                GameObject container = new("SoundContainer");
                container.transform.SetParent(defaultManager.transform);
                container.transform.localPosition = Vector3.zero;
                defaultManager.soundContainer = container.transform;

                GameObject musicPrefab = new("Music");
                musicPrefab.transform.SetParent(defaultManager.transform);
                musicPrefab.transform.localPosition = Vector3.zero;
                musicPrefab.AddComponent<AudioSource>();
                musicPrefab.AddComponent<Music>();
                musicPrefab.SetActive(false);
                defaultManager.musicPrefab = musicPrefab;

                GameObject soundPrefab = new("Sound");
                soundPrefab.transform.SetParent(defaultManager.transform);
                soundPrefab.transform.localPosition = Vector3.zero;
                soundPrefab.AddComponent<AudioSource>();
                soundPrefab.AddComponent<Sound>();
                soundPrefab.SetActive(false);
                defaultManager.soundPrefab = soundPrefab;

                DontDestroyOnLoad(defaultManager.gameObject);
            }
        }

        public Music InstantiateMusic(string music, MusicType type)
        {
            AudioClip clip = Resources.Load<AudioClip>(JoinPaths(musicsFolder, music));
            return InstantiateMusic(clip, type);
        }

        public Music InstantiateMusic(AudioClip clip, MusicType type)
        {
            Music music = Instantiate(MusicPrefab, transform).GetComponent<Music>();
            music.Initialize(clip, type);
            music.gameObject.SetActive(true);
            return music;
        }

        public Sound InstantiateSound(string sound)
        {
            AudioClip clip = Resources.Load<AudioClip>(JoinPaths(soundsFolder, sound));
            return InstantiateSound(clip);
        }

        public Sound InstantiateSound(AudioClip clip)
        {
            Sound sound = Instantiate(SoundPrefab, transform).GetComponent<Sound>();
            sound.Initialize(clip);
            sound.gameObject.SetActive(true);
            return sound;
        }

        public void PlayMusicMain()
        {
            PlayMusic(mainMusic);
        }

        public void PlayMusic(string music, MusicType type)
        {
            PlayMusic(InstantiateMusic(music, type));
        }

        public void PlayMusic(AudioClip music, MusicType type)
        {
            PlayMusic(InstantiateMusic(music, type));
        }

        public void PlayMusic(Music music)
        {
            if (!music)
            {
                if (currentMusic)
                {
                    if (currentMusic.Type == MusicType.Main)
                    {
                        currentMusic.Pause();
                    }
                    else if (currentMusic.Type == MusicType.Secondary)
                    {
                        currentMusic.Destroy();
                    }
                    else
                    {
                        Destroy(currentMusic.gameObject);
                    }
                }
                currentMusic = music;
                return;
            }
            if (music == currentMusic)
            {
                if (currentMusic && !currentMusic.IsPlaying)
                {
                    currentMusic.Play();
                }
                return;
            }
            if (music.Type == MusicType.Main)
            {
                if (currentMusic) currentMusic.Destroy();
                if (music != mainMusic)
                {
                    if (mainMusic) Destroy(mainMusic.gameObject);
                    mainMusic = music;
                }
            }
            else if (music.Type == MusicType.Secondary)
            {
                if (currentMusic)
                {
                    if (currentMusic.Type == MusicType.Main)
                    {
                        currentMusic.Pause();
                    }
                    else if (currentMusic.Type == MusicType.Secondary)
                    {
                        currentMusic.Destroy();
                    }
                    else
                    {
                        Destroy(currentMusic.gameObject);
                    }
                }
            }
            currentMusic = music;
            currentMusic.Play();
        }

        public void PlaySound(string sound)
        {
            PlaySound(InstantiateSound(sound));
        }

        public void PlaySound(AudioClip sound)
        {
            PlaySound(InstantiateSound(sound));
        }

        public void PlaySound(Sound sound)
        {
            if (soundContainer.childCount < maxSoundInstances)
            {
                sound.transform.SetParent(soundContainer);
                sound.Play();
            }
            else
            {
                Destroy(sound.gameObject);
            }
        }

        public static string JoinPaths(string path1, string path2)
        {
            if (path1.Length == 0) return path2;
            if (path2.Length == 0) return path1;
            path1 = path1[^1] == '/' && path2[0] == '/' ? path1[..^1] : path1;
            return path1 + (path1[^1] != '/' && path2[0] != '/' ? "/" : "") + path2;
        }
    }
}