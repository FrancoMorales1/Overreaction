using UnityEngine;
using UnityEngine.UI;

public class FaceCompositeUI : MonoBehaviour
{
    [Header("UIReferences")]
    public Image imgBrows;
    public Image imgEyes;
    public Image imgMouth;

    [Header("Containter")]
    public GameObject fullpanel;

    void Start()
    {
        if (QuestFlowManager.Instance == null) return;

        bool showFace = QuestFlowManager.Instance.questCompleted && QuestFlowManager.Instance.faceBrows != null;

        if(showFace)
        {
            fullpanel.SetActive(true);

            imgBrows.sprite = QuestFlowManager.Instance.faceBrows;
            imgEyes.sprite = QuestFlowManager.Instance.faceEyes;
            imgMouth.sprite = QuestFlowManager.Instance.faceMouth;

            imgBrows.preserveAspect = true;
            imgEyes.preserveAspect = true;
            imgMouth.preserveAspect = true;

        }
        else
        {
            fullpanel.SetActive(false);
        }
    }
}
