using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   public GameObject mainMenuPanel;
   public GameObject optionsMenuPanel;

   public Toggle fullscreenToggle;

  

   public void Play()
   {
    SceneManager.LoadScene("Game");
   }

   public void OpenOptions()
   {
      mainMenuPanel.SetActive(false);
      optionsMenuPanel.SetActive(true);
      fullscreenToggle.isOn = Screen.fullScreen;
   }

   public void CloseOptions()
   {
      optionsMenuPanel.SetActive(false);
      mainMenuPanel.SetActive(true);

   }

   public void ToggleFullscreen(bool isFullscreen)
   {
      Screen.fullScreen = isFullscreen;
   }


   public void Exit()
   {
      Application.Quit();
   }
}
