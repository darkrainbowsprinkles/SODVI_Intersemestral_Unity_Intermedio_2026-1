using FPS.Core;
using UnityEngine;

namespace FPS.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground;
        Health playerHealth;

        void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            playerHealth = player.GetComponent<Health>();
        }

        void Update()
        {
            foreground.localScale = new Vector3(playerHealth.GetHealthPercentage(), 1f, 1f);
        }
    }
}
