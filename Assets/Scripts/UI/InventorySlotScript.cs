using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public string category;
    public Toggle checkToggle;
    public GameObject crossImage;
    public GameObject damageImage;

    public void CleanSlot()
    {
        checkToggle.isOn = false;
        crossImage.SetActive(false);
        damageImage.SetActive(false);
    }

    public void SetStatus(bool isCorrect, bool isDamaged)
    {
        if (isCorrect)
        {
            if (isDamaged)
            {
                checkToggle.isOn = false;
                crossImage.SetActive(false);
                damageImage.SetActive(true);
            }
            else
            {
                checkToggle.isOn = true;
                crossImage.SetActive(false);
                damageImage.SetActive(false);
            }
        }
        else
        {
            checkToggle.isOn = false;
            crossImage.SetActive(true);
            damageImage.SetActive(false);
        }
    }

}
