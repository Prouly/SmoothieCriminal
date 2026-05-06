using UnityEngine;

public class PlataformaPlayer : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false; // Evita doble salto inmediato
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Recorremos los puntos de contacto
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Si el contacto viene desde abajo (suelo)
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Cuando deja de tocar algo, podría estar en el aire
        isGrounded = false;
    }
}