using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManagerSimple : MonoBehaviour
{
    public TargetTrackerSimple target1;
    public TargetTrackerSimple target2;
    public Transform char1;
    public Transform char2;
    public GameObject ball;
    public Button attackButton;
    public TextMeshProUGUI gameOverText;
    public float ballSpeed = 2f;

    private bool battleReady = false;

    void Start()
    {
        attackButton.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        attackButton.onClick.AddListener(OnAttack);
    }

    void Update()
    {
        if (!battleReady && target1.isTracked && target2.isTracked)
        {
            battleReady = true;
            attackButton.gameObject.SetActive(true);

            // Hacen que ambos miren al otro
            char1.LookAt(char2.position);
            char2.LookAt(char1.position);
        }
    }

    void OnAttack()
    {
        // Lanza la bola en dirección al objetivo
        Vector3 direction = (char2.position - ball.transform.position).normalized;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = direction * ballSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == char2.gameObject)
        {
            gameOverText.text = "¡Game Over!";
            gameOverText.gameObject.SetActive(true);
            attackButton.gameObject.SetActive(false);
        }
    }
}
