using UnityEngine;

namespace FPS.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        AudioSource audioSource;
        ParticleSystem particles;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            particles = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            bool particlesDone = particles == null || !particles.IsAlive();
            bool audioDone = audioSource == null || !audioSource.isPlaying;

            if (particlesDone && audioDone)
            {
                Destroy(gameObject);
            }
        }
    }
}