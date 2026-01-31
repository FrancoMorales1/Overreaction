using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuestFlowManager : MonoBehaviour
{
    public static QuestFlowManager Instance;

    [System.Serializable]
    public class Mission
    {
        public string name;
        public DataDialogueScript dialogueIntro;
        public string levelSceneName;

        public DataDialogueScript GoodEnd;
        public DataDialogueScript OkEnd;
        public DataDialogueScript BadEnd;
    }

    public List<Mission> missionList;
    public int indexCurrentMission = 0;
    public int lastPointReached = 0;
    public bool questCompleted = false; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Mission GetCurrentMission()
    {
        if (indexCurrentMission >= 0 && indexCurrentMission < missionList.Count)
        {
            return missionList[indexCurrentMission];
        }
        return null;
    }

    public DataDialogueScript GetDialogueForCurrentMission()
    {
        Mission currentMission = GetCurrentMission();
        if (currentMission != null)
        {
            if (questCompleted)
            {
                if (lastPointReached >= 5)
                {
                    return currentMission.GoodEnd;
                }
                else if (lastPointReached >= 3)
                {
                    return currentMission.OkEnd;
                }
                else
                {
                    return currentMission.BadEnd;
                }
            }
            else
            {
                return currentMission.dialogueIntro;
            }
        }
        return null;
    }

    public void EndDialogueManager()
    {
        Mission current = GetCurrentMission();
        if (current == null) return;
        if (!questCompleted)
        {
            SceneManager.LoadScene(current.levelSceneName);
        }
        else
        {
            indexCurrentMission++;
            questCompleted = false;
            lastPointReached = 0;

            if (indexCurrentMission < missionList.Count)
            {
                SceneManager.LoadScene("QuestScene");
            }
            else
            {
                Debug.Log("All missions completed!");
                SceneManager.LoadScene("MainMenuScene");
            }
        }

    }
}
