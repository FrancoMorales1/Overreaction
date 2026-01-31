using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TempData", menuName = "Juego/DatosJugador")]
public class TempData : ScriptableObject
{
    public float totalTime;
    public float timeLeft;
    private bool isRestarting = false;

    public void SetIsRestarting(bool value)
    {
        isRestarting = value;
    }

    public bool GetIsRestarting()
    {
        return isRestarting;
    }

    public void SetTimeLeft(float amount)
    {
        timeLeft = amount;
    }

    public void Reset(float time)
    {
        SetTimeLeft(time);
        isRestarting = true;
        Debug.Log($"Tiempo reseteado a: {time}s");
    }

    public void FullReset()
    {
        SetTimeLeft(totalTime);
    }
}