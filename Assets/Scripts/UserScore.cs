using UnityEngine;
using System.Collections;

public class UserScore : MonoBehaviour
{

	public string username;
	public string score;

	public UserScore(string name,string score){
		this.username=name;
		this.score=score;
	}
}

