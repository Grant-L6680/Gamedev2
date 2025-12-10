using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // <-- Add this using directive

public class LoadingSceneController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadAfterDelay());
    }

    IEnumerator LoadAfterDelay()
    {
        // Wait 5 seconds
        yield return new WaitForSeconds(5f);

        // Load the main scene
        SceneManager.LoadScene("OG");
    }
}
