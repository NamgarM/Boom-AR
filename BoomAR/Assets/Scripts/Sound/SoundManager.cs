using System.Collections.Generic;
using UnityEngine;

namespace GrowAR.Sound
{
    public class SoundManager : MonoBehaviour
    {
        public List<SoundSettings> Sounds;

        // Start is called before the first frame update
        void Awake()
        {
            foreach (SoundSettings s in Sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.pitch = s.pitch;
                s.source.volume = s.volume;
            }
            Play("MainThemeBGM", 1f);
        }

        public void Play(string soundName, float delay)
        {
            foreach (SoundSettings sound in Sounds)
            {
                if (sound.name == soundName && !sound.source.isPlaying)
                    sound.source.PlayDelayed(delay);
            }
        }

        public void Stop(string soundName)
        {
            foreach (SoundSettings sound in Sounds)
            {
                if (sound.name == soundName)
                    sound.source.Stop();
            }
        }
    }
}
