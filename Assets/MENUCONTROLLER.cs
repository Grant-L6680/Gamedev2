using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    [Header("Menu Buttons")]
    public List<Button> menuButtons; // Assign buttons in inspector
    private int currentIndex = 0;

    [Header("Navigation Settings")]
    public float inputCooldown = 0.2f;
    private float nextInputTime = 0f;

#if ENABLE_INPUT_SYSTEM
    private Gamepad gamepad;
    private bool aButtonWasPressed = false;
#endif

    private void Start()
    {
        if (menuButtons == null || menuButtons.Count == 0)
        {
            Debug.LogError("Menu buttons not assigned in inspector!");
            return;
        }

        // Highlight the first button initially
        currentIndex = 0;
        HighlightButton(currentIndex);
    }

    private void Update()
    {
        if (menuButtons == null || menuButtons.Count == 0) return;

#if ENABLE_INPUT_SYSTEM
        gamepad = Gamepad.current;

        if (gamepad != null)
        {
            // Navigate vertically
            float vertical = gamepad.leftStick.ReadValue().y + (gamepad.dpad.up.isPressed ? 1 : 0) + (gamepad.dpad.down.isPressed ? -1 : 0);

            if (Time.time >= nextInputTime)
            {
                if (vertical > 0.5f)
                {
                    MoveSelection(-1);
                    nextInputTime = Time.time + inputCooldown;
                }
                else if (vertical < -0.5f)
                {
                    MoveSelection(1);
                    nextInputTime = Time.time + inputCooldown;
                }
            }

            // Press A / Button South
            if (gamepad.buttonSouth.isPressed && !aButtonWasPressed)
            {
                menuButtons[currentIndex].onClick.Invoke();
                aButtonWasPressed = true;
            }

            if (!gamepad.buttonSouth.isPressed)
            {
                aButtonWasPressed = false;
            }
        }
#endif
    }

    private void MoveSelection(int direction)
    {
        if (menuButtons.Count == 0) return;

        // Deselect current button
        menuButtons[currentIndex].OnDeselect(null);

        currentIndex += direction;

        // Wrap around safely
        if (currentIndex < 0) currentIndex = menuButtons.Count - 1;
        else if (currentIndex >= menuButtons.Count) currentIndex = 0;

        HighlightButton(currentIndex);
    }
    private Color normalColor = Color.red;
    private Color selectedColor = Color.yellow;






    private void HighlightButton(int index)
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            var colors = menuButtons[i].colors;
            colors.normalColor = (i == index) ? selectedColor : normalColor;
            menuButtons[i].colors = colors;

            if (i == index)
                menuButtons[i].Select(); // select current
            else
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); // deselect others
        }
    }







    // Public UI button methods
    public void StartGame() => SceneManager.LoadScene("loading");
    public void StartTutorial() => SceneManager.LoadScene("TUTORIAL");
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit button pressed");
    }
}
