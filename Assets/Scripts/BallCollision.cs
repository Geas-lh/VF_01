using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallCollision : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public Button attackButton;

    void Start()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si la pelota choca con el modelo que tiene el tag "Target2"
        if (collision.gameObject.CompareTag("Target2"))
        {
            // Desactivar el modelo golpeado
            collision.gameObject.SetActive(false);

            // Mostrar mensaje de fin de juego
            if (gameOverText != null)
            {
                gameOverText.text = "¡Game Over!";
                gameOverText.gameObject.SetActive(true);
            }

            // Desactivar el botón de ataque
            if (attackButton != null)
                attackButton.gameObject.SetActive(false);

            // (Opcional) destruir la pelota también
            Destroy(gameObject);
        }
    }
}
