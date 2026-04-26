using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Lógica del minijuego Aplastar Bichos con sorting dinámico y gestión de contenedor de spawns.
 * Última modificación: 26/04/2026
 */

public class BichosLogic : MonoBehaviour
{
    [Header("Configuración de Bichos")]
    [SerializeField] private GameObject[] bichosPrefabs; 
    [SerializeField] private int totalBichos = 5;
    [SerializeField] private Transform contenedorPuntosSpawn; 

    [Header("Efectos y Sonidos")]
    [SerializeField] private AudioClip sonidoZasMatamoscas; // Sonido del golpe
    [SerializeField] private AudioClip sonidoAplastarBicho;  // Sonido orgánico del bicho
    [SerializeField] private float intensidadVibracion = 0.1f;
    [SerializeField] private float duracionVibracion = 0.1f;

    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;

    private float tiempoRestante;
    private int bichosAplastados = 0;
    private bool juegoTerminado = false;
    private AudioSource audioSource;
    private Vector3 posOriginalCamara;

    void Start()
    {
        tiempoRestante = tiempoLimite;
        audioSource = gameObject.AddComponent<AudioSource>();
        posOriginalCamara = Camera.main.transform.localPosition;

        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Confined;
        SpawnBichos();
    }

    void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            Debug.Log("SISTEMA: Tiempo agotado. ¡Has perdido!");
            FinalizarJuego(false);
        }
    }

    public void RegistrarBaja()
    {
        if (juegoTerminado) return;

        bichosAplastados++;
        Debug.Log($"SISTEMA: Bicho aplastado ({bichosAplastados}/{totalBichos})");

        // Reproducir sonidos
        if (sonidoZasMatamoscas != null) audioSource.PlayOneShot(sonidoZasMatamoscas);
        if (sonidoAplastarBicho != null) audioSource.PlayOneShot(sonidoAplastarBicho);
        
        StartCoroutine(VibrarCamara());

        if (bichosAplastados >= totalBichos)
        {
            Debug.Log("SISTEMA: ¡Victoria! Has exterminado a todos los bichos.");
            FinalizarJuego(true);
        }
    }

    private IEnumerator VibrarCamara()
    {
        float tiempoPasado = 0f;
        while (tiempoPasado < duracionVibracion)
        {
            Vector3 posRnd = posOriginalCamara + (Vector3)Random.insideUnitCircle * intensidadVibracion;
            Camera.main.transform.localPosition = new Vector3(posRnd.x, posRnd.y, posOriginalCamara.z);
            tiempoPasado += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = posOriginalCamara;
    }

    private void SpawnBichos()
    {
        List<Transform> spawnsLibres = new List<Transform>();
        foreach (Transform hijo in contenedorPuntosSpawn) spawnsLibres.Add(hijo);

        for (int i = 0; i < totalBichos; i++)
        {
            if (spawnsLibres.Count == 0) break;
            int indexBicho = (i < bichosPrefabs.Length) ? i : Random.Range(0, bichosPrefabs.Length);
            int indexSpawn = Random.Range(0, spawnsLibres.Count);
            Instantiate(bichosPrefabs[indexBicho], spawnsLibres[indexSpawn].position, Quaternion.identity);
            spawnsLibres.RemoveAt(indexSpawn);
        }
    }

    private void FinalizarJuego(bool victoria)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.instancia != null)
        {
            if (victoria) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }

    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    public bool EstaJuegoTerminado() => juegoTerminado;
}