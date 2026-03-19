using System;
using UnityEngine;

public class ClickableMole : MonoBehaviour
{
    public bool isClickable = false;

    private SpriteRenderer spriteRenderer;
    public Game1 gameManager;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isClickable && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("¡Mole clickeado!");

                // Cambiar color a rojo
                spriteRenderer.color = Color.red;

                // Desactivar clicks futuros
                isClickable = false;
                if (gameManager != null) gameManager.MoleClicked();
                
            }
        }
    }
}
