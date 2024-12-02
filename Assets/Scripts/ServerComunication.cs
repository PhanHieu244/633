using UnityEngine;
using System.Collections;
using  System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
public class ServerComunication : MonoBehaviour {
	public static string GAME_VERSION= "1.0.1";
	public static string adStoreLink = "";
	public static string serverURL= "http://www.iphone-vietnam.com/";
	public static string userRanking;

	public delegate void OnNewVersion();
	public static event  OnNewVersion onNewVersion;

	public delegate void OnSetUserName();
	public static event  OnSetUserName onSetUserName;

	
	public delegate void OnSetHighscore();
	public static event  OnSetHighscore onSetHighscore;

	public delegate void OnSetUserNameFailed();
	public static event  OnSetUserName onSetUserNameFailed;

	public delegate void OnGetTop10();
	public static event  OnGetTop10 onGetTop10;

	public delegate void OnGetCurrentRank();
	public static event  OnGetCurrentRank onGetCurrentRank;

	public delegate void OnAdRecieved(Texture2D texture);
	public static event  OnAdRecieved onAdRecieved;

	public static List<UserScore> userList;

	public static IEnumerator getAds(){
		
		WWW w = new WWW(serverURL+"ahtservices?method=getAdsApp&device=google_play&name_app=mylovepath");
		Debug.Log("request url "+w.url);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
		}
		else {
			JSONObject json= new JSONObject(w.text);

		
			if (json.HasField("content")){
				string link= json["url_store"].ToString().Replace("\"","");
				string imageLink= json["content"].ToString().Replace("\"","");
				
				Debug.Log("getAds "+link);
				Debug.Log("getAds "+imageLink);
			
				link= link.Replace("\"","");
				link = link.Replace("\\","");

				imageLink=imageLink.Replace("\"","");
				imageLink=imageLink.Replace("\\","");
			//	print ("loadImage "+imageLink);
				WWW www = new WWW( imageLink );
				yield return www;
				print ("loadImage "+imageLink);
				adStoreLink=link;
				onAdRecieved(www.texture);

			}
			
		}
	}


	public static IEnumerator  getGameVersion(){
		
		WWW w = new WWW(serverURL+"ahtservices?method=getAppVersion&app_name=mylovepath&type=android");
		Debug.Log("request url "+w.url);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
		}
		else {
			JSONObject json= new JSONObject(w.text);
			Debug.Log("getGameVersion "+json["id"]);
			Debug.Log("getGameVersion "+json["version"]);
			string gameversion= json["version"].ToString().Replace("\"","");
			Debug.Log("getGameVersion "+gameversion);
			if (!gameversion.Equals(GAME_VERSION)){
				onNewVersion();
			} 

		}
	}

	public static IEnumerator  setUserName(string userName){
		
		WWW w = new WWW(serverURL+ "ahtservices?method=setUserName&username="+userName+"&gamename=letsget45");
		Debug.Log("request url "+w.url);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
		}
		else {
			JSONObject json= new JSONObject(w.text);
			Debug.Log("setUserName "+json);
			if (json["result"].ToString().Contains("SUCCESS")){
				onSetUserName();
			} else {
				onSetUserNameFailed();
			}

		
			
		}
	}

	public static IEnumerator  setHighScore(string userName,string score){
		
		WWW w = new WWW(serverURL+ "ahtservices?method=setHighScore&username="+userName+"&score="+score+"&gamename=letsget45");
		Debug.Log("request url "+w.url);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
		}
		else {
			JSONObject json= new JSONObject(w.text);
			Debug.Log("setHighScore "+json);
			
			if (json["result"].ToString().Contains("SUCCESS")){
				onSetHighscore();
			}
				
			
		}
	}

	public static IEnumerator  getCurrentRank(){
		
		WWW w = new WWW(serverURL+ "ahtservices?method=getCurrentRank&gamename=letsget45&username="+MenuScript.playerName);
		Debug.Log("request url "+w.url);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
		}else {
			JSONObject json= new JSONObject(w.text);
			Debug.Log("getCurrentRank "+json);

			if (json.HasField("rank")){
				userRanking= json["rank"].ToString();
				onGetCurrentRank();
			}
		}
	}


	public static IEnumerator  getTop10(){
		
		WWW w = new WWW(serverURL+ "ahtservices?method=getTop&gamename=letsget45&top=10");
		Debug.Log("request url "+w.url);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
		}
		else {
			JSONObject json= new JSONObject(w.text);
			Debug.Log("getTop10 "+json);
			
			userList= new List<UserScore>();
			userList.Clear();

			if (json["result"].ToString().Contains("SUCCESS")){
			
				for (int i=0;i<json["topscore"].list.Count;i++){
					string username =json["topscore"][i]["username"].ToString().Replace("\"","");
					string score = json["topscore"][i]["score"].ToString().Replace("\"","");
					UserScore user= new UserScore(username,score);
					userList.Add(user);
				//	print(""+user.username);
				//	print(""+user.score);
				}


			}
			onGetTop10();

			
		}
	}

	public static string DecodeFromUtf8(string utf8String)
	{
		// copy the string as UTF-8 bytes.
		byte[] utf8Bytes = new byte[utf8String.Length];
		for (int i=0;i<utf8String.Length;++i) {
			Debug.Assert( 0 <= utf8String[i] && utf8String[i] <= 255, "the char must be in byte's range");
			utf8Bytes[i] = (byte)utf8String[i];
		}
		print ("DecodeFromUtf8  " + Encoding.UTF8.GetString (utf8Bytes, 0, utf8Bytes.Length));
		return Encoding.UTF8.GetString(utf8Bytes,0,utf8Bytes.Length);
	}
	public static string UTF8toASCII(string text)
	{
		System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
		byte[] encodedBytes = utf8.GetBytes(text);
		byte[] convertedBytes =
			Encoding.Convert(Encoding.UTF8, Encoding.ASCII, encodedBytes);
		System.Text.Encoding ascii = System.Text.Encoding.ASCII;
		
		return ascii.GetString(convertedBytes);
	}


}
