//using UnityEngine;
//using System.Collections;
//using UnityEngine.SocialPlatforms;
//
//#if UNITY_ANDROID
//using GooglePlayGames;
//#endif
//
//#if UNITY_IOS
//using UnityEngine.SocialPlatforms.GameCenter;
//#endif
//
//public class LeaderBoardMgr : SingletonMonoBehaviour<LeaderBoardMgr>
//{
//	#if UNITY_ANDROID
//
//	void Start ()
//	{
//		PlayGamesPlatform.Activate ();
//
//		//login
//		Social.localUser.Authenticate ((bool success) => {
//			if (success) {
//				Debug.Log ("You've successfully logged in");
//			} else {
//				Debug.Log ("Login failed for some reason");
//			}
//		});
//	}
//
//	public void PostHighScoreToLeaderBoardAndroid (int hiScore)
//	{
////        if (Social.localUser.authenticated)
////        {
////            Social.ReportScore(hiScore, GPGSIds.leaderboard_tap_to_merge, (bool success) =>
////            {
////                if (success)
////                {
////                    Debug.Log("post hi score sucess");
////                }
////                else
////                {
////                    Debug.Log("fail post hi score");
////                }
////            });
////        }
//	}
//
//	public void ShowLeaderBoardAndroid ()
//	{
//		if (Social.localUser.authenticated) {
////            PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_tap_to_merge);
//		}
//        else
//        {
////            DialogMessage.Instance.OpenDialog("Google Play Services", "Not available", null);
//        }
//	}
//
//	#endif
//
//	public void PostHighScoreToLeaderBoard (int hiScore)
//	{
//		#if UNITY_ANDROID
//		PostHighScoreToLeaderBoardAndroid (hiScore);
//		#endif
//		#if UNITY_IOS
//		ReportScoreIOS (hiScore);
//		#endif
//	}
//
//	public void ShowLeaderBoard ()
//	{
//		#if UNITY_ANDROID
//		ShowLeaderBoardAndroid ();
//		#endif
//		#if UNITY_IOS
//		ShowLeaderboardIOS ();
//		#endif
//	}
//
//	#if UNITY_IOS
//    string leaderboard_high_score = "lineballs_color";
//	void Start ()
//	{
//		AuthenticateToGameCenter ();
//	}
//
//	public void AuthenticateToGameCenter ()
//	{
//		Social.localUser.Authenticate (success => {
//			if (success) {
//				Debug.Log ("Authentication successful");
//			} else {
//				Debug.Log ("Authentication failed");
//			}
//		});
//	}
//
//	public void ReportScoreIOS (int hiScore)
//	{
//		Debug.Log ("Reporting score " + hiScore + " on leaderboard " + leaderboard_high_score);
//		Social.ReportScore (hiScore, leaderboard_high_score, success => {
//			if (success) {
//				Debug.Log ("Reported score successfully");
//			} else {
//				Debug.Log ("Failed to report score");
//			}
//		});
//	}
//
//	//call to show leaderboard
//	public void ShowLeaderboardIOS ()
//	{
//        if (Social.localUser.authenticated)
//        {
//            Social.ShowLeaderboardUI();
//        }
//        else
//        {
////    DialogMessage.Instance.OpenDialog("Game Center Unavailable", "Player is not signed in ", null);
//        }
//	}
//	#endif
//
//}