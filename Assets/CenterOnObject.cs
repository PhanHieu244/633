using UnityEngine;
using System.Collections;


public class CenterOnObject : MonoBehaviour
{
	public UIScrollView dragPanel;
	
	void OnClick ()
	{
	//	Vector3 newPos = dragPanel.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
		//SpringPanel.Begin(dragPanel.gameObject, -newPos, 8f);
	
	}
	void Update(){
		if 	(Input.GetMouseButtonUp(0)){
			print("pos "+ transform.position);
			if (transform.position.x<0.6f &&transform.position.x >-0.6f ){
			//	Vector3 newPos = dragPanel.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
			//	SpringPanel.Begin(dragPanel.gameObject, -newPos, 8f);
			}
		
		}

	}
	void OnMouseUp() {

	}

}