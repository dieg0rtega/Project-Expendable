using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleUI : MonoBehaviour
{
  public GameObject Collectpanel;
  public TextMeshProUGUI textBox;

  private int currentPage = 1;

  public void Show(string message)
  {
    Collectpanel.SetActive(true);
    textBox.text = message;


    currentPage = 1;
    textBox.pageToDisplay = currentPage;

    Time.timeScale = 0f;
  }

  public void NextPage()
  {
    if ( currentPage < textBox.textInfo.pageCount)
    {
      currentPage++;
      textBox.pageToDisplay = currentPage;
    }
    else
    {
      Hide();
    }


  }

  public void PreviousPage()
  {
    if (currentPage > 1)
    {
      currentPage--;
      textBox.pageToDisplay = currentPage;
    }
  }

  public void Hide()
  {
    
    Collectpanel.SetActive(false);
    Time.timeScale = 1f;
  }


}
