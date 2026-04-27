using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

public class LightProp : MonoBehaviour
{
    [SerializeField] private int turnOffHour = 6;
    [SerializeField] private int turnOnHour = 20;
    private Light2D _light;
    
    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }
    
    
    private void OnEnable()
    {
        // Subscribe to the time update event
        EventBus.OnWorldTimeChanged += HandleTimeUpdate;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        EventBus.OnWorldTimeChanged -= HandleTimeUpdate;
    }

    private void HandleTimeUpdate(object sender, TimeSpan currentTime)
    {
        // Connvert current time into hours
        int currentHour = currentTime.Hours;
        
        bool isDayTime = currentHour >= turnOnHour && currentHour < turnOffHour;
        _light.enabled = isDayTime;
    }
}
