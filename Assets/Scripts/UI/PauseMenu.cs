using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
   public GameObject pauseMenuPanel;
   

   void update()
   {
     if (Input.GetKeyDown(KeyCode.Escape))
     {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0;
     }
   }

   public void ResumeButton()
   {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;


   }

   public void MainMenuButton()
   {
    UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
   }

  
   
}
