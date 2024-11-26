using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;

    [SerializeField] private string _nextLevel;

    private Color _color;
    private float _currentAlpha = 0;

    private string _sceneToLoad;

    public enum LoadState { None, In, Out }
    public static LoadState loadState { get; private set; } = LoadState.None;

    private int _levelNumber = 0;

    private void Awake()
    {
        _color = _fadeImage.color;

        _levelNumber = SceneManager.GetActiveScene().buildIndex;

        _fadeImage.color = new Color(_color.r, _color.g, _color.b, 1);
        _currentAlpha = 1;
        loadState = LoadState.In;
    }

    private void OnEnable()
    {
        PlayerController.onReachLastGoal += LoadNextLevel;
    }

    private void OnDisable()
    {
        PlayerController.onReachLastGoal -= LoadNextLevel;
    }

    private void Update()
    {
        switch (loadState)
        {
            case LoadState.In:

                _currentAlpha = Mathf.MoveTowards(_currentAlpha, 0, 2 * Time.deltaTime);

                if (_currentAlpha <= 0)
                {
                    _currentAlpha = 0;
                    loadState = LoadState.None;
                }

                _fadeImage.color = new Color(_color.r, _color.g, _color.b, _currentAlpha);

                break;

            case LoadState.Out:

                _currentAlpha = Mathf.MoveTowards(_currentAlpha, 1, 2 * Time.deltaTime);

                if (_currentAlpha >= 1)
                {
                    loadState = LoadState.None;
                    _currentAlpha = 1;
                    SceneManager.LoadScene(_sceneToLoad);
                }

                _fadeImage.color = new Color(_color.r, _color.g, _color.b, _currentAlpha);

                break;
        }
    }

    public void SetSceneToLoad(string sceneName)
    {
        if (loadState != LoadState.None)
            return;

        _sceneToLoad = sceneName;

        loadState = LoadState.Out;
    }

    public void LoadNextLevel()
    {
        if (_nextLevel == "")
        {
            SetSceneToLoad("Main Menu");
            return;
        }

        SetSceneToLoad(_nextLevel);
    }
}
