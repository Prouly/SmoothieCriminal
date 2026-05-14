using UnityEngine;

public class PlataformaPlayer : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private PlataformaGameManager plataformaLogic;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        plataformaLogic = Object.FindFirstObjectByType<PlataformaGameManager>();
    }

    void Update()
    {
        // BLOQUEO: Si no hay manager o la intro no ha terminado, no puede saltar
        if (plataformaLogic == null || !plataformaLogic.ObtenerIntroFinalizada()) return;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (plataformaLogic == null || !plataformaLogic.ObtenerIntroFinalizada())
        {
            // SOLUCIÓN DEFINITIVA: 
            // 1. Ponemos velocidad a 0
            rb.linearVelocity = Vector2.zero;
            // 2. Congelamos la posición para que el Physics Material no lo deslice
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            return;
        }

        // Cuando la intro termina, restauramos las restricciones (solo congelamos rotación)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Movimiento normal
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