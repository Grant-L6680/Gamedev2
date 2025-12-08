using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Called when the Start button is pressed
    public void StartGame()
    {
        // Replace "OGScene" with the exact name of your gameplay scene
        SceneManager.LoadScene("OG");
    }

    // Called when the Quit button is pressed
    public void QuitGame()
    {
        Application.Quit(); // Only works in build, not editor
        Debug.Log("Quit button pressed"); // Shows in editor
    }
}
