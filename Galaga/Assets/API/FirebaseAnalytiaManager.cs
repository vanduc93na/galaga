using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;

public class FirebaseAnalytiaManager : MonoBehaviour
{
    public static FirebaseAnalytiaManager Instance;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dependencyStatus = FirebaseApp.CheckDependencies();
        if (dependencyStatus != DependencyStatus.Available)
        {
            FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = FirebaseApp.CheckDependencies();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        else
        {
            InitializeFirebase();
        }

        AnalyticsLogin();

        LogOpen("user_open_game");
    }

    // Handle initialization of the necessary firebase modules:
    void InitializeFirebase()
    {
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        Debug.Log("Set user properties.");
        // Set the user's sign up method.
        FirebaseAnalytics.SetUserProperty(
            FirebaseAnalytics.UserPropertySignUpMethod,
            "Google");
        // Set the user ID.
        FirebaseAnalytics.SetUserId("uber_user_510");
    }

    public void LogOpen(string eventName)
    {
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen, eventName, 1);
    }

    public void LogEvent(string eventName, string parameterName, int id = 1)
    {
        //eventName = eventName.Replace(" ", "_");
        FirebaseAnalytics.LogEvent(eventName, parameterName, id);
    }

    //public void LogEvent(string eventName, string action)
    //{
    //    eventName = eventName.Replace(" ", "");
    //    FirebaseAnalytics.LogEvent(eventName, eventName, 0);
    //}

    public void LogPurchase(string eventName)
    {
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEcommercePurchase, eventName, 0);
    }

    public void AnalyticsLogin()
    {
        // Log an event with no parameters.
        Debug.Log("Logging a login event.");
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }

    public void AnalyticsProgress()
    {
        // Log an event with a float.
        Debug.Log("Logging a progress event.");
        FirebaseAnalytics.LogEvent("progress", "percent", 0.4f);
    }

    public void AnalyticsScore()
    {
        // Log an event with an int parameter.
        Debug.Log("Logging a post-score event.");
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventPostScore,
            FirebaseAnalytics.ParameterScore,
            42);
    }

    public void AnalyticsGroupJoin()
    {
        // Log an event with a string parameter.
        Debug.Log("Logging a group join event.");
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventJoinGroup, FirebaseAnalytics.ParameterGroupId,
            "spoon_welders");
    }

    public void AnalyticsLevelUp()
    {
        // Log an event with multiple parameters.
        Debug.Log("Logging a level up event.");
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventLevelUp,
            new Parameter(FirebaseAnalytics.ParameterLevel, 5),
            new Parameter(FirebaseAnalytics.ParameterCharacter, "mrspoon"),
            new Parameter("hit_accuracy", 3.14f));
    }
}