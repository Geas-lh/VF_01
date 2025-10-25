using UnityEngine;
using Vuforia;

public class TargetTrackerSimple : MonoBehaviour
{
    public bool isTracked { get; private set; }

    void Start()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnStatusChanged;
        }
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        isTracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;
    }
}
