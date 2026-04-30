using UnityEngine;

public class BalonBasketColision : MonoBehaviour
{
    public BasketMinijuego minijuego;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Canasta"))
        {
            minijuego.Canasta();
        }
    }
}