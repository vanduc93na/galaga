using UnityEngine;
using System.Collections;

public class UtilitiesGameTool : MonoBehaviour {

    public static bool IsInternetAvailable()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ||
            (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork))
        {
            return true;
        }
        return false;
    }
}
