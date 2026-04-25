using System;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    [SerializeField] private float dayLengthInMinutes = 10f;
    [SerializeField, HideInInspector] private float dayLength; // Seconds for a full day
    
    [Header("Starting Date & Time")] 
    [SerializeField, Range(0, 23)] private int startHour = 8;
    [SerializeField] private int startDay = 1;
    [SerializeField] private int startMonth = 1;
    [SerializeField] private int startYear = 2026;

    private TimeSpan _currentTime;
    
    // This tracks "fractional" minutes so we don't lose time between frames
    private float _minuteAccumulator;

    
    // Helper for Editor previews
    public float GetPreviewPercent()
    {
        return (float)startHour / 24f;
    }
    // OnValidate() is called when the inspector is changed
    private void OnValidate()
    {
        dayLength = dayLengthInMinutes * 60f;
        // Tell the light system to refresh if it exists in the scene
        var lightSystem = FindFirstObjectByType<WorldLight>();
        if (lightSystem != null)
        {
            lightSystem.RefreshLights();
        }
    }

    private void Start()
    {
        SetupTime();
    }

    private void Update()
    {
        // 1. Calculate how many real-world seconds passed this frame
        float secondsPassedThisFrame = Time.deltaTime;

        // 2. Convert those real seconds into game minutes
        // Ratio: (Minutes in a Day / Real seconds in a Day)
        float gameMinutesPassed = secondsPassedThisFrame * (WorldTimeConstants.MinutesInDay / dayLength);

        _minuteAccumulator += gameMinutesPassed;

        // 3. If we've accumulated at least 1 full minute, update the clock
        if (_minuteAccumulator >= 1f)
        {
            int minutesToAdd = Mathf.FloorToInt(_minuteAccumulator);
            _minuteAccumulator -= minutesToAdd; // Keep the remainder

            _currentTime += TimeSpan.FromMinutes(minutesToAdd);
            EventBus.RequestUpdateWorldTime(this, _currentTime);
        }
    }

    private void SetupTime()
    {
        int totalDays = ((startYear - 1) * (WorldTimeConstants.MonthsInYear * WorldTimeConstants.DaysInMonth)) + 
                        ((startMonth - 1) * WorldTimeConstants.DaysInMonth) + 
                        (startDay - 1);

        _currentTime = TimeSpan.FromDays(totalDays) + TimeSpan.FromHours(startHour);
        EventBus.RequestUpdateWorldTime(this, _currentTime);  
    }
}