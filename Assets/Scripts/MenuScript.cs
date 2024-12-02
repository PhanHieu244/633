using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {
	 static bool adOpened=false;
	public static bool firstGameStart=true;
	public GameObject dialogAd,dialogShop,dialogSetting,dialogLoading,toastMessage,buttonRemoveAd,buttonErase,dialogRanking,itemPrefab;
	int soundOn;
	public static string playerName;
	void Start(){
		
		checkSoundState();
		Application.targetFrameRate=60;
	//	if (PlayerPrefs.GetInt("remove_ads",0)==0){
		
			Debug.Log("admobload");

	//	}
	
		//RequestInterstitial();
	//	StartCoroutine(ServerComunication.getGameVersion());

		int showPopUpDialog = Random.Range (0, 100);
		if (showPopUpDialog < 30) {
			int showAdPercent = Random.Range (0, 100);
			
			if (showAdPercent<60){
				openAds(); // 60% show ad
			}else if (showAdPercent>=60 && showAdPercent<=80 ){
				if (PlayerPrefs.GetInt("rategame",0)==0)
				{
					if (!firstGameStart) ;
					//dialogRate.SetActive(true); // 20% show rate 

				} else 	openAds(); 
			}else if (showAdPercent>80  ){
				
				StartCoroutine(ServerComunication.getGameVersion()); // 20% show game has new version
				
			}	
		}
		buttonErase.transform.Find("quantity").GetComponent<UILabel>().text=""+GameHandler.getEraseQuantity();
		if (PlayerPrefs.GetInt("remove_ads",0)!=1)
			buttonRemoveAd.SetActive(true);	
		else 	buttonRemoveAd.SetActive(false);

		gameObject.GetComponents<AudioSource>()[1].Play();

		if (!PlayerPrefs.GetString("playername","").Equals("")){
			StartCoroutine(ServerComunication.setHighScore(PlayerPrefs.GetString("playername",""),""+getHighScore()));
		}



	}
	int getHighScore ()
	{
		return PlayerPrefs.GetInt ("highscore", 0);
	}

		//Connect success
	public void HandleConnectSuccess()
	{
		Debug.Log ("Connect Success");
	
		//Now that we are connected we can start preloading our placements
	//	TJPlacement p = TJPlacement.CreatePlacement("my_placement");
	//	p.RequestContent();
	}
	void openAds()
	{
		return;
		if (PlayerPrefs.GetInt("remove_ads",0)==0)
			if (!adOpened) {
				adOpened=true;
				StartCoroutine(ServerComunication.getAds());
			}
	}


	void OnEnable(){
		print ("enable");
		ServerComunication.onNewVersion += newVersion;
		ServerComunication.onAdRecieved += adRecieved;

		ServerComunication.onSetUserName+=onSetUserName;
		ServerComunication.onSetUserNameFailed+=onSetUsernameFailed;
		ServerComunication.onGetTop10+=loadRankingSuccess;
		ServerComunication.onGetCurrentRank+=setCurrentRank;
		ServerComunication.onSetHighscore+=setHighscoreSuccess;
	}

	void OnDisable(){
		ServerComunication.onNewVersion -= newVersion;
		ServerComunication.onAdRecieved -= adRecieved;

		ServerComunication.onSetUserName-=onSetUserName;
		ServerComunication.onSetUserNameFailed-=onSetUsernameFailed;
		ServerComunication.onGetTop10-=loadRankingSuccess;
		ServerComunication.onGetCurrentRank-=setCurrentRank;
		ServerComunication.onSetHighscore-=setHighscoreSuccess;
	}
	void setHighscoreSuccess(){
		StartCoroutine(ServerComunication.getTop10());

	}
	void setCurrentRank(){
		dialogRanking.transform.Find("playerName").GetComponent<UILabel>().text=ServerComunication.userRanking+". "+playerName;
		dialogRanking.transform.Find("playerScore").GetComponent<UILabel>().text=""+PlayerPrefs.GetInt ("highscore", 0);

	}
	void onSetUsernameFailed(){
		closeLoadingDialog();
		toastMessage.GetComponent<ToastMessage>().show("Set user name fail");
	}
	void onSetUserName(){

		closeLoadingDialog();
		toastMessage.GetComponent<ToastMessage>().show("Set user name success");
		PlayerPrefs.SetString("playername",dialogRanking.transform.Find("Container").Find("input").GetComponent<UILabel>().text); 
		if (!PlayerPrefs.GetString("playername","").Equals("")){
			dialogRanking.transform.Find("buttonOk").gameObject.SetActive(false);
			dialogRanking.transform.Find("Container").gameObject.GetComponent<BoxCollider>().enabled=false;
			StartCoroutine(ServerComunication.setHighScore(PlayerPrefs.GetString("playername",""),""+getHighScore()));
		}

	
	}
	void rewardViewVideo(){
		toastMessage.GetComponent<ToastMessage>().show("Reward recieved.");
		GameHandler.addErase(1);
		buttonErase.transform.Find("quantity").GetComponent<UILabel>().text= ""+GameHandler.getEraseQuantity();
		saveTimeViewVideo();
	}

	void saveTimeViewVideo(){
		PlayerPrefs.SetString("lastViewVideo",System.DateTime.Now.ToString());
		print("vungle time "+		PlayerPrefs.GetString("lastViewVideo",System.DateTime.Now.ToString()));
	}
	void restorePurchase(){
		toastMessage.GetComponent<ToastMessage>().show("Restore successed.");
		buttonRemoveAd.SetActive(false);
	}
	 void purchasePack(){
		toastMessage.GetComponent<ToastMessage>().show("Purchase successed.");
		buttonErase.transform.Find("quantity").GetComponent<UILabel>().text=""+GameHandler.getEraseQuantity();
	}
	public void removeAd(){
		toastMessage.GetComponent<ToastMessage>().show("Purchase successed.");
		buttonRemoveAd.SetActive(false);
	}

	void onAwardCurrencyResponse(string currencyName, int amount){
		print ("onAwardCurrencyResponse " +currencyName +"  "+ amount);
	}
	void tapjoyViewDidClose(int viewType){
		print ("tapjoyViewDidClose " );
	}
	void onCachedAdAvailableEvent()
	{	
		//Vungle.playAd();
		Debug.Log( "onCachedAdAvailableEvent" );
	//	Tapjoy.GetCurrencyBalance();
	}

	public void openVungleVideo(){
	//	string dateString = PlayerPrefs.GetString ("lasetViewVideo",System.DateTime.Now.ToString());
	//	System.DateTime	dateTimeLast = System.DateTime.Parse (dateString);
	//	int minute = (int )(System.DateTime.Now -dateTimeLast).TotalMinutes;
	//	print("vungle time "+minute);
	
	} 
	public void openTabJoyOfferWall(){
	}
	void newVersion(){
		print ("newVersion");
	}
	void adRecieved(Texture2D text){
		print ("adRecieved");
		dialogAd.transform.Find("Texture").GetComponent<UITexture>().mainTexture= text;
		dialogAd.GetComponent<UIScaleAnimation>().open();

	}

	public void openStoreAd(){
		Application.OpenURL(ServerComunication.adStoreLink);
	}
	public void closeAd(){
		dialogAd.GetComponent<UIScaleAnimation>().close();
	}
	public void playGame(){
		Application.LoadLevel ("GameScreen");
	}
	public void goToStore(){

		PlayerPrefs.SetInt ("rategame",1);
		//dialogRate.GetComponent<UIScaleAnimation> ().close ();
		Application.OpenURL ("https://play.google.com/store/apps/details?id=com.arrowhitech.letsget45");
	}
	public void closeDialogRate(){
		PlayerPrefs.SetInt ("rategame",1);
		//dialogRate.GetComponent<UIScaleAnimation> ().close ();
	}
	public void openRanking(){
		StartCoroutine(ServerComunication.getTop10());
		openLoadingDialog();

	}
	public void loadRankingSuccess(){
		playerName =PlayerPrefs.GetString("playername","");
		dialogRanking.GetComponent<UIScaleAnimation>().open();
		if (!playerName.Equals("")){
			dialogRanking.transform.Find("Container").Find("input").GetComponent<UILabel>().text=playerName;
			dialogRanking.transform.Find("playerName").GetComponent<UILabel>().text=playerName;
			dialogRanking.transform.Find("playerScore").GetComponent<UILabel>().text=""+PlayerPrefs.GetInt ("highscore", 0);
			StartCoroutine(ServerComunication.getCurrentRank());
			dialogRanking.transform.Find("buttonOk").gameObject.SetActive(false);
			dialogRanking.transform.Find("Container").gameObject.GetComponent<BoxCollider>().enabled=false;
		} else {
			dialogRanking.transform.Find("buttonOk").gameObject.SetActive(true);
			dialogRanking.transform.Find("Container").gameObject.GetComponent<BoxCollider>().enabled=true;
		}
		UIGrid grid = dialogRanking.transform.Find("Scroll View").Find("Grid").GetComponent<UIGrid>();
		while (grid.gameObject.transform.childCount > 0){
			//	print("clear grid");
			NGUITools.Destroy(grid.GetChild(0).gameObject);
		}
		int count =1;
		if (ServerComunication.userList!=null){
			foreach (UserScore user in ServerComunication.userList){
				GameObject item= NGUITools.AddChild(dialogRanking.transform.Find ("Scroll View").gameObject,itemPrefab);
				item.transform.Find("name").GetComponent<UILabel>().text=count+ ". "+user.username;
				item.transform.Find("score").GetComponent<UILabel>().text=""+user.score;	
				grid.AddChild(item.transform);
				item.transform.localScale= new Vector3(1,1,1);
				grid.Reposition ();
				count++;
			}
		}
		closeLoadingDialog();
	}
	public void askLater(){
		//dialogRate.GetComponent<UIScaleAnimation> ().close ();
	}
	void openLoadingDialog(){
		dialogLoading.SetActive(true);
		Invoke("closeLoadingDialog",15f);
	}
	void closeLoadingDialog(){
		dialogLoading.SetActive(false);
	}
	public void openShop(){

		dialogShop.GetComponent<UIScaleAnimation>().open();

	}
	public void openSetting(){
		dialogSetting.GetComponent<UIScaleAnimation>().open();
		checkSoundState();
	}
	public void closeShop(){
		dialogShop.GetComponent<UIScaleAnimation>().close();
	}
	public void closeSetting(){
		dialogSetting.GetComponent<UIScaleAnimation>().close();
	}
	public void playClickSound(){
		gameObject.GetComponent<AudioSource>().Play();
	}
	public void openHelp(){
		Application.LoadLevel("HelpScreen");
	}
	public void checkSoundState(){
		soundOn = PlayerPrefs.GetInt ("soundOn",1);
		if (soundOn == 0) {
			AudioListener.volume = 0;
			dialogSetting.transform.Find("sound").gameObject.GetComponent<UISprite>().spriteName="sound 2";
			dialogSetting.transform.Find("sound").gameObject.GetComponent<UIButton>().normalSprite="sound 2";
		} else {
			AudioListener.volume = 1;
			dialogSetting.transform.Find("sound").gameObject.GetComponent<UISprite>().spriteName="sound 1";
			dialogSetting.transform.Find("sound").gameObject.GetComponent<UIButton>().normalSprite="sound 1";
		}
		
	}

	public void changeSoundState(){
		soundOn = PlayerPrefs.GetInt ("soundOn",1);
		if (soundOn == 1) {
			AudioListener.volume = 0;
			PlayerPrefs.SetInt("soundOn",0);
			dialogSetting.transform.Find("sound").gameObject.GetComponent<UISprite>().spriteName="sound 2";
			dialogSetting.transform.Find("sound").gameObject.GetComponent<UIButton>().normalSprite="sound 2";
		} else {
			AudioListener.volume = 1;
			PlayerPrefs.SetInt("soundOn",1);
			dialogSetting.transform.Find("sound").gameObject.GetComponent<UISprite>().spriteName="sound 1";
			dialogSetting.transform.Find("sound").gameObject.GetComponent<UIButton>().normalSprite="sound 1";
		}
		
	}
	public void setUserName(){
		playerName = dialogRanking.transform.Find("Container").Find("input").GetComponent<UILabel>().text;
		StartCoroutine(ServerComunication.setUserName(playerName));
		openLoadingDialog();
	}
	#region Currency Delegate Handlers
	public void HandleAwardCurrencyResponse(string currencyName, int balance) {
		Debug.Log("C#: HandleAwardCurrencySucceeded: currencyName: " + currencyName + ", balance: " + balance);

	}
	
	public void HandleAwardCurrencyResponseFailure(string error) {
		Debug.Log("C#: HandleAwardCurrencyResponseFailure: " + error);
	}
	
	public void HandleGetCurrencyBalanceResponse(string currencyName, int balance) {
		Debug.Log("C#: HandleGetCurrencyBalanceResponse: currencyName: " + currencyName + ", balance: " + balance);
		if (balance>0){
			GameHandler.addErase(balance);
			buttonErase.transform.Find("quantity").GetComponent<UILabel>().text=""+GameHandler.getEraseQuantity();
			toastMessage.GetComponent<ToastMessage>().show(""+balance+" Eraser(s) added");
		}
	}
	
	public void HandleGetCurrencyBalanceResponseFailure(string error) {
		Debug.Log("C#: HandleGetCurrencyBalanceResponseFailure: " + error);
	}
	
	public void HandleSpendCurrencyResponse(string currencyName, int balance) {
		Debug.Log("C#: HandleSpendCurrencyResponse: currencyName: " + currencyName + ", balance: " + balance);
	
	}
	
	public void HandleSpendCurrencyResponseFailure(string error) {
		Debug.Log("C#: HandleSpendCurrencyResponseFailure: " + error);
	}
	
	public void HandleEarnedCurrency(string currencyName, int amount) {
		Debug.Log("C#: HandleEarnedCurrency: currencyName: " + currencyName + ", amount: " + amount);

		
		//Tapjoy.ShowDefaultEarnedCurrencyAlert();
	}
	#endregion
	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
		
		}
		else
		{
			
		}
	}
}
