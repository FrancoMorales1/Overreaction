using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "SystemDialogue/New Quest")]
public class DataDialogueScript : ScriptableObject
{
    public Sprite npcImage;
    public string npcName;
    [TextArea(3, 10)]
    public string[] dialogueLines;

    public string nameNextScene;
}
