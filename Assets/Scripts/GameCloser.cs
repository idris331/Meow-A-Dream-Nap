using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCloser : MonoBehaviour
{
    private SceneLoader _sceneLoader;

    private bool _inMainMenu = false;

    private void Awake()
    {
        _inMainMenu = SceneManager.GetActiveScene().name == "MainMenu";

        _sceneLoader = FindObjectOfType<SceneLoader>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (_inMainMenu)
                Application.Quit();
            else
                _sceneLoader.SetSceneToLoad("MainMenu");
        }
    }

    public void Quit() => Application.Quit();
}
