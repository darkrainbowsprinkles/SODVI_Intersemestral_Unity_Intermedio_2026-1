using UnityEngine;

namespace FPS.Audio
{
    public class SFXRandomizer : MonoBehaviour
    {
        [SerializeField] AudioClip[] soundEffects;

        // Called in Unity Events
        public void PlayRandom()
        {
            int randomIndex = Random.Range(0, soundEffects.Length);
            GetComponent<AudioSource>().PlayOneShot(soundEffects[randomIndex]);
        }
    }
}
