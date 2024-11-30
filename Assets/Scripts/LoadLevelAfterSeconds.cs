using System.Collections;
using UnityEngine;

public class LoadLevelAfterSeconds : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(WaitToLoadNextLevel());
    }

    private IEnumerator WaitToLoadNextLevel()
    {
        yield return new WaitForSeconds(2f);

        FindObjectOfType<SceneLoader>().LoadNextLevel();
    }
}
