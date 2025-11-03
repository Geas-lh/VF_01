using UnityEngine;
using TMPro; // 👈 necesario para TextMeshPro

public class ScoreUI_TMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bonusText;

    void Start()
    {
        bonusText.gameObject.SetActive(false);
    }

    void Update()
    {
        scoreText.text = $"Puntos: {GameManager.Instance.score}";
        bestScoreText.text = $"Mejor: {GameManager.Instance.bestScore}";

        // Mostrar el mensaje de bonus activo
        if (GameManager.Instance.BonusActivo)
        {
            bonusText.text = "BONUS x2 ACTIVO!";
            bonusText.gameObject.SetActive(true);
        }
        else
        {
            bonusText.gameObject.SetActive(false);
        }
    }
}
