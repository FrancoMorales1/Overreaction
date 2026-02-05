using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "ScriptableObjects/DataDialogueScript", order = 1)]
public class DataDialogueScript : ScriptableObject
{
    [Header("Configuración Visual")]
    public Sprite npcImage;
    public Color npcColor = Color.white;   // Color para el NPC
    public Color playerColor = Color.yellow; // Color para el Player (ej: amarillo)

    [System.Serializable]
    public class DialogueEntry
    {
        [TextArea(3, 10)]
        public string text;
        public bool isPlayer; // Casilla: Si la marcas, habla el Player. Si no, el NPC.
    }

    [Header("Conversación")]
    public List<DialogueEntry> conversation; // Usamos la nueva clase en lugar de String[]
}