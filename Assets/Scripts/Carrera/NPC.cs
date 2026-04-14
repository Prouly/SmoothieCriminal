<<<<<<< HEAD
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
=======
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
>>>>>>> 6582b7c27b7bb627f7c1a50f0e38a056e4d593a0
}