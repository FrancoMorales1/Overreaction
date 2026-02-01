using UnityEngine;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    [Header("UI References")]
    public Image finalImage;

    [Header("EndingImages")]
    public Sprite goodEndingImage;
    public Sprite badEndingImage;

    void Start()
    {
        if (QuestFlowManager.Instance != null)
        {
            if (QuestFlowManager.Instance.currentReputation >= 50)
            {
                finalImage.sprite = goodEndingImage;
            }
            else
            {
                finalImage.sprite = badEndingImage;
            }
        }
    }
    
    public void ReturnToMainMenu()
        {
            if (QuestFlowManager.Instance != null)
            {
                QuestFlowManager.Instance.currentReputation = 50;
                QuestFlowManager.Instance.lastReputation = 50;
                QuestFlowManager.Instance.indexCurrentMission = 0;
                QuestFlowManager.Instance.ResetFace();
                QuestFlowManager.Instance.questCompleted = false;
            }
            if (TransitionManager.Instance != null)
            {
                TransitionManager.Instance.LoadSceneWithTransition("MainMenuScene");
            }
        }    
}

