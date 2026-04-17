using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[System.Serializable]
public class DialogueLine
{
  public string speaker;
  [TextArea] public string text;
}

public class CollectibleUI : MonoBehaviour
{
  public GameObject Collectpanel;
  public TextMeshProUGUI textBox;

  public List<DialogueLine> dialogueLines;

  private int currentLine = 0;
  private int currentPage = 1;

  public void Show(List<DialogueLine> newDialogue)
  {
    dialogueLines = newDialogue;
    
    Collectpanel.SetActive(true);
    currentLine = 0;
    DisplayLine();

    Time.timeScale = 0f;
  }

  void DisplayLine()
  {
    string fullText = dialogueLines[currentLine].speaker + ": " + dialogueLines[currentLine].text;
    textBox.text = fullText;
    currentPage = 1;
    textBox.pageToDisplay = currentPage;


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
    if (currentLine < dialogueLines.Count - 1)
    {
      currentLine++;
      DisplayLine();
    }
    else
    {
         Hide();
    }
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
