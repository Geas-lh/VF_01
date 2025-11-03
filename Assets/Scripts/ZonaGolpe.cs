using UnityEngine;
using System.Collections;

public class ZonaGolpe : MonoBehaviour
{
    public static ZonaGolpe Instance;

    private bool enDesafio = false;
    private GameObject pelotaActual;
    private int toquesRealizados;
    private int toquesObjetivo;
    private float tiempoRestante;
    private bool desafioActivo = false;

    void Awake()
    {
        Instance = this;
    }

    // ===========================================================
    // 🧩 Detección de pelota en zona de golpe
    // ===========================================================
    void OnTriggerEnter(Collider other)
    {
        // Detectar pelota en movimiento (tiro normal)
        if (!enDesafio && other.CompareTag("Pelota"))
        {
            Debug.Log("🎯 Pelota detectada en zona de golpe (normal)");
            pelotaActual = other.gameObject;
        }
    }

    // ===========================================================
    // 👊 Este método lo llama el botón de golpe del jugador
    // ===========================================================
    public void BotonGolpe()
    {
        if (pelotaActual != null)
        {
            // Si estamos en desafío → manejar minijuego
            if (enDesafio)
            {
                toquesRealizados++;
                Debug.Log($"Toques: {toquesRealizados}/{toquesObjetivo}");

                if (toquesRealizados >= toquesObjetivo)
                {
                    TerminarDesafio(true);
                }
            }
            else
            {
                // Si es tiro normal → golpe directo
                GolpearPelota(false);
            }
        }
    }

    // ===========================================================
    // 💥 Simular golpe físico de la pelota
    // ===========================================================
    void GolpearPelota(bool perfecto)
    {
        if (pelotaActual == null) return;

        // Animar bateador
        GameManager.Instance.bateadorAnim.SetTrigger("Golpear");

        Rigidbody rb = pelotaActual.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            // Calcular dirección opuesta al lanzamiento (desde bateador hacia lanzador)
            Vector3 direccionBase = (GameManager.Instance.posicionLanzador.position - GameManager.Instance.objetivoBateador.position).normalized;

            // Variación aleatoria (ángulo y rotación)
            Vector3 variacion = new Vector3(
                Random.Range(-0.3f, 0.3f),   // desviación horizontal
                Random.Range(0.2f, 0.6f),    // ángulo vertical
                Random.Range(-0.3f, 0.3f)    // leve variación de profundidad
            );

            Vector3 direccionFinal = (direccionBase + variacion).normalized;

            // Fuerza base (más fuerte si es perfecto)
            float fuerzaGolpe = perfecto ? Random.Range(7f, 9f) : Random.Range(4f, 6f);

            // Aplicar fuerza y rotación
            rb.AddForce(direccionFinal * fuerzaGolpe, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

            // (Opcional) Activar rastro si la pelota tiene TrailRenderer
            var trail = pelotaActual.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.Clear();
                trail.emitting = true;
            }
        }

        // Registrar puntuación
        GameManager.Instance.Golpe(perfecto);

        // Destruir pelota luego de unos segundos
        Destroy(pelotaActual, 3f);
        pelotaActual = null;
    }

    // ===========================================================
    // 🕹️ Iniciar desafío (llamado desde GameManager)
    // ===========================================================
    public IEnumerator IniciarDesafio(GameObject pelota, int toques, float tiempo)
    {
        if (desafioActivo) yield break;
        desafioActivo = true;
        enDesafio = true;
        pelotaActual = pelota;
        toquesObjetivo = toques;
        toquesRealizados = 0;
        tiempoRestante = tiempo;

        Debug.Log($"🔥 Iniciando desafío: {toquesObjetivo} toques en {tiempoRestante} segundos.");

        // Loop del desafío (espera toques o fin del tiempo)
        while (tiempoRestante > 0f && toquesRealizados < toquesObjetivo)
        {
            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        // Resultado del desafío
        if (toquesRealizados >= toquesObjetivo)
        {
            TerminarDesafio(true);
        }
        else
        {
            TerminarDesafio(false);
        }
    }

    // ===========================================================
    // 🧮 Finalizar desafío
    // ===========================================================
    void TerminarDesafio(bool exito)
    {
        enDesafio = false;
        desafioActivo = false;

        if (pelotaActual != null)
        {
            if (exito)
            {
                Debug.Log("✅ Desafío completado!");
                GolpearPelota(true);
            }
            else
            {
                Debug.Log("❌ Desafío fallido. Pelota destruida.");
                Destroy(pelotaActual);
            }
        }
    }
}
