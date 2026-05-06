using UnityEngine;

public class PelotaColision : MonoBehaviour
{
    public PenaltiMinijuego minijuego;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Portero"))
        {
            minijuego.Parada();
        }

        if (col.CompareTag("Porteria"))
        {
            minijuego.Gol();
        }
    }
}