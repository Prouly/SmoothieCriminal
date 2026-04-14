using UnityEngine;

public class Meta : MonoBehaviour
{
    public bool jugadorGano = false;
    public Sprite spritePerdedor;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Mostramos el tag del objeto que entró
        Debug.Log("Objeto entró al collider con tag: " + other.tag);

        //Marcamos si el jugador fue el primero
        if (other.CompareTag("Player"))
        {
            jugadorGano = true;
            Debug.Log("¡El Player llegó primero!");
            GameObject competidor = GameObject.FindGameObjectWithTag("NPC");
            if (competidor != null) competidor.GetComponent<SpriteRenderer>().sprite = spritePerdedor;
            
            GameManager.instancia.Ganar();
        }
        else
        {
            GameObject competidor = GameObject.FindGameObjectWithTag("Player");
            if (competidor != null) competidor.GetComponent<SpriteRenderer>().sprite = spritePerdedor;
            GameManager.instancia.Perder();
        }

        //Desactivamos el collider para que no se vuelva a activar
        GetComponent<Collider2D>().enabled = false;
    }
}
