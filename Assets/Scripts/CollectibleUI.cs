using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleUI : MonoBehaviour
{
  public GameObject Collectpanel;
  public TextMeshProUGUI textBox;

  public void Show(string message)
  {
    Collectpanel.SetActive(true);
    textBox.text = message;
    Time.timeScale = 0f;
  }

  public void Hide()
  {
    
    Collectpanel.SetActive(false);
    Time.timeScale = 1f;
  }


}
