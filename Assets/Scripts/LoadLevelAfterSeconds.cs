using System.Collections;
using UnityEngine;

public class LoadLevelAfterSeconds : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    private void Start()
    {
        StartCoroutine(WaitToLoadNextLevel());
    }

    private IEnumerator WaitToLoadNextLevel()
    {
        yield return new WaitForSeconds(2f);

        FindObjectOfType<SceneLoader>().SetSceneToLoad(_sceneName);
    }
}
