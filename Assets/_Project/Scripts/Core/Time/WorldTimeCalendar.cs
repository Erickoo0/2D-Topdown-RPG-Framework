using UnityEngine;
using System;

public class WorldTimeCalendar : MonoBehaviour
{
    [Header("Month Names")]
    [SerializeField] private string[] monthNames = { "Spring", "Summer", "Fall", "Winter" };
    
    public int CurrentDay { get; private set; }
    public int CurrentMonth { get; private set; }
    public int CurrentYear { get; private set; }
    public string CurrentTime { get; private set; }

    private string CurrentMonthName
    {
        get
        {
            // 1. Safety Check
            if (monthNames == null || monthNames.Length == 0) return "Unknown Month";
            
            // 2. Index logic
            int index = (CurrentMonth - 1) % monthNames.Length;
            index = Mathf.Abs(index);
            return monthNames[index];
        }
    }
    private int _previousDay = -1;

    private void Awake() => EventBus.OnWorldTimeChanged += UpdateCalendar;
    private void OnDestroy() => EventBus.OnWorldTimeChanged -= UpdateCalendar;

    private void UpdateCalendar(object sender, TimeSpan time)
    {
        int totalDaysPassed = time.Days;
        
        // Calculate current date (Adding +1 so we start on Day 1, Month 1, Year 1)
        CurrentDay = (totalDaysPassed % WorldTimeConstants.DaysInMonth) + 1;
        CurrentMonth = ((totalDaysPassed / WorldTimeConstants.DaysInMonth) % WorldTimeConstants.MonthsInYear) + 1;
        CurrentYear = (totalDaysPassed / (WorldTimeConstants.DaysInMonth * WorldTimeConstants.MonthsInYear)) + 1;
        CurrentTime = time.ToString(@"hh\:mm");
        
        if (CurrentDay != _previousDay)
        {
            _previousDay = CurrentDay;
            OnNewDayStarted();
        }
    }

    private void OnNewDayStarted()
    {
        // New Day Started logic later
    }

    // Helper method to pass the date to other scripts
    public string GetDate()
    {
        return $"{CurrentMonthName}, {CurrentDay}, {CurrentYear}\n" +
               $"{CurrentTime}";    
    }

}
