using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

[ExecuteAlways] // Allows code to run during editor
[RequireComponent(typeof(Light2D))]
public class WorldLight : MonoBehaviour
{
    [SerializeField] private WorldTime worldTime;
    [SerializeField] private Gradient gradient;
    
    private Light2D _light2D;

    private void OnValidate()
    {
        RefreshLights();
    }
    
    private void Awake()
    {
        _light2D = GetComponent<Light2D>();
        
        // Only subscribe to events if the application is playing
        if (Application.isPlaying)
            EventBus.OnWorldTimeChanged += UpdateWorldLight;

    }

    private void OnDestroy()
    {
        if(Application.isPlaying)
            EventBus.OnWorldTimeChanged -= UpdateWorldLight;
    }
    
    public void RefreshLights()
    {
        if (_light2D == null) _light2D = GetComponent<Light2D>();
        if (worldTime == null) return;

        // Use the preview percentage if we aren't playing, 
        // otherwise it will be handled by the EventBus
        if (!Application.isPlaying)
        {
            float previewTime = worldTime.GetPreviewPercent();
            _light2D.color = gradient.Evaluate(previewTime);
        }
    }
    
    private void UpdateWorldLight(object sender, TimeSpan time)
    {
        _light2D.color = gradient.Evaluate(PercentOfDay(time));
    }

    /// <summary>
    /// Gets the time in THIS day taking into account total time (days / months passed)
    /// </summary>
    private float PercentOfDay(TimeSpan time)
    {
        return (float)time.TotalMinutes % WorldTimeConstants.MinutesInDay / WorldTimeConstants.MinutesInDay;
    }
}
