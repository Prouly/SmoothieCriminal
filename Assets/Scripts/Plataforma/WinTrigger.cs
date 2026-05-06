using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public PlataformaGameManager gameManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlataformaPlayer>() != null)
        {
            gameManager.Win();
        }
    }
}