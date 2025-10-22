using UnityEngine;
using TMPro;
using Vuforia;

public class MostrarTextoAlDetectar : MonoBehaviour
{
    public GameObject texto3D; // asigna aquí tu objeto de texto

    void Start()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
            observer.OnTargetStatusChanged += OnStatusChanged;

        texto3D.SetActive(false);
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
            texto3D.SetActive(true);   // mostrar texto
        else
            texto3D.SetActive(false);  // ocultar texto
    }
}
