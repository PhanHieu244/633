using UnityEngine;
using System.Collections;

public class ToastMessage : MonoBehaviour {
	public GameObject msg;
	// Use this for initialization
	public void show(string text){
		msg.GetComponent<UILabel> ().text = text;
		GetComponent<UIScaleAnimation> ().open ();
		CancelInvoke ("closeToast");
		Invoke ("closeToast",3f);
	}
	void Start () {
		//Invoke ("closeToast",3f);
	}
	public void closeToast(){
		GetComponent<UIScaleAnimation> ().close ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
