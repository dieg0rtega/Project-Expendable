using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
   public GameObject pauseMenuPanel;
   public GameObject optionsMenuPanel;

   private bool isPaused = false;
   

   void Update()
   {
     if (Input.GetKeyDown(KeyCode.Escape))
     {
      if (optionsMenuPanel.activeSelf)
      {
         CloseOptions();
      }
      else if (isPaused)
      {
         Resume();
      }
      else
      {
         Pause();
      }

      
     }
        
   }

   public void Resume()
   {
      pauseMenuPanel.SetActive(false);
      Time.timeScale = 1f;
      isPaused = false;
   }

   void Pause()
   {
      pauseMenuPanel.SetActive(true);
      Time.timeScale = 0f;
      isPaused = true; 
   }

   public void GotoMain()
   {
      Time.timeScale = 1f;
      SceneManager.LoadScene("Main Menu");
   }

   public void OpenOptions()
   {
      pauseMenuPanel.SetActive(false);
      optionsMenuPanel.SetActive(true);
      Time.timeScale = 0f;
   }

   public void CloseOptions()
   {
      optionsMenuPanel.SetActive(false);
      pauseMenuPanel.SetActive(true);
   }



   public void QuitGame()
   {
      Application.Quit();
      Debug.Log("Game Exited");
   }

  

  
   
}
