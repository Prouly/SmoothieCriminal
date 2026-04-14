<<<<<<< HEAD
using UnityEngine;

public class PasoSimpleJugador : MonoBehaviour
{
    [SerializeField] private float distanciaPaso = 0.5f;   //Cuánto avanza por paso
    private bool esperaD = true;//Empieza esperando D

    void Update()
    {
        //Alterna entre presionar D y A para avanzar
        if (esperaD && Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(distanciaPaso, 0, 0);
            esperaD = false; // Ahora espera A
        }
        else if (!esperaD && Input.GetKeyDown(KeyCode.A))
        {
            transform.position += new Vector3(distanciaPaso, 0, 0);
            esperaD = true; //Ahora espera D
        }
    }
=======
using UnityEngine;

public class PasoSimpleJugador : MonoBehaviour
{
    [SerializeField] private float distanciaPaso = 0.5f;   //Cuánto avanza por paso
    private bool esperaD = true;//Empieza esperando D

    void Update()
    {
        //Alterna entre presionar D y A para avanzar
        if (esperaD && Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(distanciaPaso, 0, 0);
            esperaD = false; // Ahora espera A
        }
        else if (!esperaD && Input.GetKeyDown(KeyCode.A))
        {
            transform.position += new Vector3(distanciaPaso, 0, 0);
            esperaD = true; //Ahora espera D
        }
    }
>>>>>>> 6582b7c27b7bb627f7c1a50f0e38a056e4d593a0
}