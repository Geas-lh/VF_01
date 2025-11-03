using UnityEngine;
using Vuforia;

public class TargetStatusHandler : MonoBehaviour
{
    private ObserverBehaviour observer;
    public string targetName;

    void Start()
    {
        observer = GetComponent<ObserverBehaviour>();
        observer.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool visible = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;

        if (targetName == "Lanzador")
            GameManager.lanzadorVisible = visible;

        if (targetName == "Bateador")
            GameManager.bateadorVisible = visible;

        GameManager.Instance.CheckTargets();
    }
}
