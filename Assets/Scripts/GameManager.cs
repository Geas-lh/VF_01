using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool lanzadorVisible = false;
    public static bool bateadorVisible = false;

    [Header("Animadores")]
    public Animator lanzadorAnim;
    public Animator bateadorAnim;

    [Header("Pelota")]
    public GameObject pelotaPrefab;
    public Transform posicionLanzador;
    public Transform objetivoBateador;
    public float velocidadPelota = 5f;

    [Header("Control de lanzamiento")]
    public float tiempoEntreLanzamientos = 4f; // Ajusta según animaciones
    private bool lanzando = false;
    private int lanzamientosContados = 0;
    private bool modoDesafio = false;

    [Header("Puntuación")]
    public int score = 0;
    public int bestScore = 0;
    private int golpesPerfectos = 0;
    private bool bonusActivo = false;

    [Header("Desafío")]
    private int toquesRequeridos = 3;
    private float tiempoDesafio = 2f;

    // Propiedad pública para UI
    public bool BonusActivo => bonusActivo;

    void Awake()
    {
        Instance = this;
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    // ===========================================================
    // ✅ Verifica si ambos targets están visibles
    // ===========================================================
    public void CheckTargets()
    {
        if (lanzadorVisible && bateadorVisible && !lanzando)
        {
            StartCoroutine(InicioJuego());
        }
    }

    // ===========================================================
    // 🕒 Cuenta regresiva inicial
    // ===========================================================
    IEnumerator InicioJuego()
    {
        Debug.Log("Ambos targets detectados. Empezando en 5 segundos...");
        yield return new WaitForSeconds(5f);
        StartCoroutine(LanzarPelota());
    }

    // ===========================================================
    // ⚾ Lógica de lanzamiento de pelota + desafío
    // ===========================================================
    IEnumerator LanzarPelota()
    {
        if (lanzando) yield break;
        lanzando = true;

        // Animación del lanzador
        lanzadorAnim.SetTrigger("Lanzar");
        yield return new WaitForSeconds(1f); // Espera a sincronizar con animación

        lanzamientosContados++;

        // Crear pelota
        GameObject pelota = Instantiate(pelotaPrefab, posicionLanzador.position, Quaternion.identity);
        Rigidbody rb = pelota.GetComponent<Rigidbody>();

        // Tamaño base
        pelota.transform.localScale = Vector3.one * 0.02f;

        // Activar modo desafío cada 3 lanzamientos
        if (lanzamientosContados % 3 == 0)
        {
            modoDesafio = true;
            pelota.transform.localScale = Vector3.one * 0.035f;
            Debug.Log("🔥 Desafío activado: pelota más grande.");
        }
        else
        {
            modoDesafio = false;
        }

        // Calcular dirección hacia el bateador
        Vector3 direccion = (objetivoBateador.position - posicionLanzador.position).normalized;
        rb.isKinematic = false;
        rb.linearVelocity = direccion * velocidadPelota;

        // Esperar a que la pelota llegue al bateador (con seguridad por si se destruye)
        yield return new WaitUntil(() =>
            pelota != null &&
            Vector3.Distance(pelota.transform.position, objetivoBateador.position) < 0.25f
        );

        if (pelota == null)
        {
            lanzando = false;
            yield break;
        }

        // Si es desafío: detener la pelota y lanzar minijuego
        if (modoDesafio)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            pelota.transform.position = objetivoBateador.position; // asegurar posición exacta

            Debug.Log("🎯 Pelota detenida en el bateador - Iniciando desafío");
            StartCoroutine(ZonaGolpe.Instance.IniciarDesafio(pelota, toquesRequeridos, tiempoDesafio));
        }
        else
        {
            // Movimiento normal — permitir que siga unos segundos y desaparezca
            yield return new WaitForSeconds(0.5f);
            if (pelota != null)
                Destroy(pelota, 1.5f);
        }

        // Esperar un poco antes del siguiente lanzamiento
        yield return new WaitForSeconds(tiempoEntreLanzamientos);
        lanzando = false;

        StartCoroutine(LanzarPelota());
    }

    // ===========================================================
    // 💥 Golpe exitoso (llamado desde ZonaGolpe)
    // ===========================================================
    public void Golpe(bool perfecto)
    {
        int puntos = perfecto ? 100 : 50;

        if (perfecto)
        {
            golpesPerfectos++;
            if (golpesPerfectos % 3 == 0)
            {
                bonusActivo = true;
                Debug.Log("🔥 BONUS ACTIVADO: doble puntuación en el siguiente golpe!");
            }
        }

        if (bonusActivo)
        {
            puntos *= 2;
            bonusActivo = false; // Se consume el bonus
        }

        score += puntos;

        // Aumentar dificultad progresiva
        if (score >= 900)
        {
            toquesRequeridos = 6;
            tiempoDesafio = 0.8f;
        }
        else if (score >= 600)
        {
            toquesRequeridos = 5;
            tiempoDesafio = 1f;
        }
        else if (score >= 300)
        {
            toquesRequeridos = 4;
            tiempoDesafio = 1.5f;
        }

        // Guardar mejor puntuación
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }

        Debug.Log($"Golpe {(perfecto ? "Perfecto" : "Normal")} | Puntos: {puntos} | Total: {score}");
    }
}
