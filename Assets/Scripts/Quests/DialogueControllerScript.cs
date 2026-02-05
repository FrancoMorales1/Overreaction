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

    private Queue<DataDialogueScript.DialogueEntry> dialogueQueue;

    void Start()
    {
        dialogueQueue = new Queue<DataDialogueScript.DialogueEntry>();
        if (QuestFlowManager.Instance != null)
        {
            currentQuest = QuestFlowManager.Instance.GetDialogueForCurrentMission();
        }
        if (currentQuest != null)
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        if (npcImageUI != null && currentQuest.npcImage != null)
        {
            npcImageUI.sprite = currentQuest.npcImage;
            npcImageUI.preserveAspect = true;
        }

        dialogueQueue.Clear();

        foreach (var entry in currentQuest.conversation)
        {
            dialogueQueue.Enqueue(entry);
        }

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Sacamos la siguiente entrada
        DataDialogueScript.DialogueEntry currentEntry = dialogueQueue.Dequeue();

        // 1. Ponemos el texto
        windowText.text = currentEntry.text;

        // 2. Decidimos el color según quién habla
        if (currentEntry.isPlayer)
        {
            // Es el jugador
            windowText.color = currentQuest.playerColor;
            
            // Opcional: Podrías oscurecer la imagen del NPC si habla el jugador
            // if(npcImageUI != null) npcImageUI.color = Color.gray;
        }
        else
        {
            // Es el NPC
            windowText.color = currentQuest.npcColor;
            
            // Opcional: Restaurar color normal
            // if(npcImageUI != null) npcImageUI.color = Color.white;
        }
    }

    void EndDialogue()
    {
        Debug.Log("Dialogue ended.");
        if (QuestFlowManager.Instance != null)
        {
            QuestFlowManager.Instance.EndDialogueManager();
        }
    }

    public void StartNewQuestFormManager()
    {
        if (dialogueQueue != null) dialogueQueue.Clear();
        if (QuestFlowManager.Instance != null)
        {
            currentQuest = QuestFlowManager.Instance.GetDialogueForCurrentMission();
        }

        StartDialogue();
    }
}
