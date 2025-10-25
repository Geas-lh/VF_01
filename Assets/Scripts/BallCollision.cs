using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallCollision : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;  // Corregido aquí
    public Button attackButton;

    void Start()
    {
        // Asegurar que el texto empiece oculto (opcional)
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target2"))
        {
            if (gameOverText != null)
            {
                gameOverText.text = "¡Game Over!";
                gameOverText.gameObject.SetActive(true);
            }

            if (attackButton != null)
                attackButton.gameObject.SetActive(false);
        }
    }
}
