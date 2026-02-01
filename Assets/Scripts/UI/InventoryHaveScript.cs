using UnityEngine;
using UnityEngine.UI;

public class InventoryHaveScript : MonoBehaviour
{
    public string category;
    public Image itemImage;

    void Awake()
    {
        CleanSlot();
    }

    public void SetItem(Sprite icon)
    {
        if (itemImage != null && icon != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = true; // Lo mostramos
        }
    }

    public void CleanSlot()
    {
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false; // Lo ocultamos si no hay nada
        }
    }
}