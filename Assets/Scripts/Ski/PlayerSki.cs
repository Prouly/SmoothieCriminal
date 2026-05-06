using UnityEngine;

public class PlayerSki : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public GameManagerSki gameManager;
    private bool inGoodZone = false;
    private bool locked = false;

    void Update()
    {
        if (locked) return;
        if (inGoodZone)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x + 2f * Time.deltaTime, rb.linearVelocity.y);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inGoodZone)
            {
                // ACIERTO
                spriteRenderer.color = Color.green;
                locked = true;
                gameManager.Win();
                rb.AddForce(new Vector2(6f, 10f), ForceMode2D.Impulse);
                Debug.Log("Has ganado");
            }
            else
            {
                // FALLO
                spriteRenderer.color = Color.red;
                locked = true;
                gameManager.Lose();
                Debug.Log("Has perdido");
            }
        }
    }

    // Entra en zona buena
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "GoodCheck")
        {
            inGoodZone = true;
        }
    }

    // Sale de zona buena
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "GoodCheck")
        {
            inGoodZone = false;
        }
    }
}