using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   public GameObject mainMenuPanel;
   public GameObject optionsMenuPanel;
   public GameObject messagePanel; 
   public GameObject attributionPanel;
   

   public Toggle fullscreenToggle;

   public void Play()
   {
    SceneManager.LoadScene(1);
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

   public void ToggleMessage()
   {
      if (messagePanel != null)
      {
      messagePanel.SetActive(!messagePanel.activeSelf);
      }
   }

   public void CloseCredits()
   {
      messagePanel.SetActive(false);
      mainMenuPanel.SetActive(true);
   }

   public void openAttributes()
   {
      attributionPanel.SetActive(true);
      messagePanel.SetActive(false);
   }

   public void closeAttributes()
   {
      attributionPanel.SetActive(false);
      messagePanel.SetActive(true);
   }
   



   public void Exit()
   {
      Application.Quit();
   }
}
