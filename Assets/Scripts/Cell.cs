using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {
	public bool eating;

	public delegate void OnCellDestroyed();
	public static event OnCellDestroyed onCellDestroyed;

	void OnMouseDown(){
		print ("click on cell ");
		Destroy (gameObject);
	}

	void OnClick(){
		print ("On click");
		if (gameObject.tag.Equals ("cell")) {
			if (GameHandler.isReadyUseErase&& !eating && GameHandler.getEraseQuantity()>0){
				eating=true;
				gameObject.GetComponent<TweenAlpha> ().PlayForward ();
				gameObject.GetComponent<TweenAlpha> ().enabled = true;
				gameObject.GetComponent<TweenAlpha> ().AddOnFinished (removeObject);
				GameHandler.useErase();
				gameObject.GetComponent<AudioSource>().Play();
			}

		}


	}
	void removeObject(){

		NGUITools.DestroyImmediate (gameObject);
		//Invoke ("onCellDestroyed",0.5f);
		onCellDestroyed ();
	}

}
