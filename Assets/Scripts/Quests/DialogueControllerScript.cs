using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueControllerScript : MonoBehaviour
{
    public TextMeshProUGUI windowText;
    public Image npcImageUI;

    public DataDialogueScript currentQuest;

    private Queue<string> dialogueLines;

    void Start()
    {
        dialogueLines = new Queue<string>();
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (npcImageUI != null && currentQuest.npcImage != null)
        {
            npcImageUI.sprite = currentQuest.npcImage;
            npcImageUI.preserveAspect = true;
        }

        dialogueLines.Clear();

        foreach (string line in currentQuest.dialogueLines)
        {
            dialogueLines.Enqueue(line);
        }

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = dialogueLines.Dequeue();
        windowText.text = line;
    }

    void EndDialogue()
    {
       Debug.Log("Dialogue ended.");
       if (!string.IsNullOrEmpty(currentQuest.nameNextScene))
       {
           UnityEngine.SceneManagement.SceneManager.LoadScene(currentQuest.nameNextScene);
       }
       else 
       {
           Debug.Log("No next scene specified.");//esto revisar si vale la pena
       }
    }
}
