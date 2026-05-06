using UnityEngine;

public class LoseTrigger : MonoBehaviour
{
    public PlataformaGameManager gameManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlataformaPlayer>() != null)
        {
            gameManager.Lose();
        }
    }
}