using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public string category;
    public Toggle checkToggle;
    public GameObject crossImage;

    public void CleanSlot()
    {
        checkToggle.isOn = false;
        crossImage.SetActive(false);
    }

    public void SetStatus(bool isCorrect)
    {
        if (isCorrect)
        {
            checkToggle.isOn = true;
            crossImage.SetActive(false);
        }
        else
        {
            checkToggle.isOn = false;
            crossImage.SetActive(true);
        }
    }

}
