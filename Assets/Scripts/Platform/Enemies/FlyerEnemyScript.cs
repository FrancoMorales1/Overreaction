using UnityEngine;

public class FlyerEnemyScript : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 3f;
    private int direction = 1;

    void Update()
    {
        transform.Translate(Vector3.up * direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        direction *= -1;
    }
}