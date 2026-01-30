using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FPS.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] float pauseTimeScale = 0.2f;
        [SerializeField] Button restartButton;
        [SerializeField] Button quitButton;

        void OnEnable()
        {
            SetGameActive(false);
            restartButton.onClick.AddListener(ReloadScene);
            quitButton.onClick.AddListener(QuitGame);
        }

        void OnDisable()
        {
            SetGameActive(true);
            restartButton.onClick.RemoveListener(ReloadScene);
            quitButton.onClick.RemoveListener(QuitGame);
        }

        void SetGameActive(bool active)
        {
            Cursor.visible = !active;
            Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
            Time.timeScale = active ? 1f : pauseTimeScale;
        }

        void ReloadScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        void QuitGame()
        {
            # if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            # else
                Application.Quit();
            # endif
        }
    }
}