using UnityEngine;

public class NPC : MonoBehaviour
{
    // Velocidad a la que se moverá el objeto
    public float velocidad = 5f;

    void Update()
    {
        // Mover el objeto hacia la derecha
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }
}