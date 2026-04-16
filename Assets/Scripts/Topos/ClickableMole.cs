using System;
using System.Collections;
using UnityEngine;

public class ClickableMole : MonoBehaviour
{
    public bool isClickable = false;

    private SpriteRenderer spriteRenderer;
    public Topos gameManager;
    [SerializeField] private Sprite[] sprites;
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

                //Cambiar sprite dañado
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];

                //Desactivar clicks futuros
                isClickable = false;
                if (gameManager != null) gameManager.MoleClicked();
                //Esto se deberia cambiar por un animation
                StartCoroutine(CambiarSpriteDespues());
            }
        }
    }
    
    private IEnumerator CambiarSpriteDespues()
    {
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.sprite = sprites[1];
    }
}
