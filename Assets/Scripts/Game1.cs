using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.UI;

public class Game1 : MonoBehaviour
{
    [SerializeField] private GameObject[] squares;
    [SerializeField] private float number;
    private List<GameObject> seleccionados;

    [SerializeField] private float timer;
    [SerializeField] private Sprite newSprite; 
    
    private int clickeadosCount = 0;
    private bool gameFinished = false;
    
    [SerializeField] private RawImage winImage;
    [SerializeField] private RawImage loseImage;
    
    private void Start()
    {
        seleccionados = new List<GameObject>();
        List<GameObject> copia = new List<GameObject>(squares);

        for (int i = 0; i < number && copia.Count > 0; i++)
        {
            int index = Random.Range(0, copia.Count);
            seleccionados.Add(copia[index]);
            copia.RemoveAt(index); // evita repetir
        }
        
        StartCoroutine(CambiarSprites());
        StartCoroutine(TimerCoroutine());
    }
    
    private IEnumerator CambiarSprites()
    {
        // Calculamos el intervalo entre cambios
        float intervalo = timer / number;

        foreach (GameObject obj in seleccionados)
        {
            // Cambiar el sprite del objeto
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = newSprite;
                ClickableMole mole = obj.GetComponent<ClickableMole>();
                if (mole != null)
                {
                    mole.isClickable = true;
                }
            }
            

            // Esperar el intervalo antes de cambiar el siguiente
            yield return new WaitForSeconds(intervalo);
        }
    }

    public void MoleClicked()
    {
        if (gameFinished) return;  // evita contar clicks después de terminar

        clickeadosCount++;

        if (clickeadosCount >= number)
        {
            if (winImage != null) winImage.gameObject.SetActive(true);
            gameFinished = true;
        }
    }
    
    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(timer);

        if (!gameFinished)
        {
            if (loseImage != null) loseImage.gameObject.SetActive(true);
            gameFinished = true;
        }
    }
}
