using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject timer; //Objeto que quieres mantener (canvas, panel, etc.)

    private void Awake()
    {
        //Singleton básico
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Activar UI
    public void MostrarUI()
    {
        if (timer != null) timer.SetActive(true);
    }

    //Desactivar UI
    public void OcultarUI()
    {
        if (timer != null) timer.SetActive(false);
    }
    
}