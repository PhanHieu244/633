using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
public class GameHandler : MonoBehaviour
{
	
	
	
	public Camera cam;
	
	public const int DEFAULT_ERASE_QUANTITY= 1;
	public GameObject boardTittleTransform, newCellPrefab, boardBg;
	public static int BOARD_SIZE = 5;
	public int CELL_BG_WIDTH = 107;
	public int CELL_BG_HEIGHT = 129;
	public static int loseTime = 0;
	UIGrid boardGrid ;
	List<GameObject> addedCell;
	GameObject[,] boardObjects;
	public static int currentSide;
	Vector3[] topPosArr, rightPosArr, bottomPosArr, leftPosArr;
	GameObject itemObj;
	bool moveCellInBoard = false;
	int boardNumber;
	int addNumber ;
	int cellPosition;
	int cellRow, cellColumn;
	int score;
	public static bool isReadyUseErase;
	public UIFont point9font, numberfont;
	public UILabel scoreText;
	GameObject objectToDestroy;
	public GameObject dialogLose, dialogScore, dialogHighScore, buttonErase, buttonHighScore,tutorialFinger,tutorialSwipe,comboIcon,tutorialFinger2,tutorialSwipe2;
	public GameObject nomoveleftText,dialogPause,dialogHelp,scrollView;
	bool blockWhenMoving,isNoMoveLeft,isPause,eating;
	public GameObject[] helppage;
	int soundOn;
	//    public GameObject bgTittle;
	// Use this for initialization
	
	
	
	void Start ()
	{
		isPause=false;
		isNoMoveLeft=false;
		MenuScript.firstGameStart=false;
		buttonHighScore.transform.Find ("score").GetComponent<UILabel> ().text = "" + getHighScore ();
		score = 0;
		blockWhenMoving = false;
		isReadyUseErase = false;
		currentSide = 0;
		initBoardBg ();
		initCellDefaultPos ();
		addNewCell ();
		if (PlayerPrefs.GetInt("remove_ads",0)==0){
		}
		
		
		checkSoundState();
		
	}
	
	void initCellDefaultPos ()
	{
		
		boardObjects = new GameObject[5, 5];
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				boardObjects [i, j] = null;
				//print("" +boardObjects[i,j]);
				
			}
		}
		
		topPosArr = new Vector3[5];
		rightPosArr = new Vector3[5];
		bottomPosArr = new Vector3[5];
		leftPosArr = new Vector3[5];
		addedCell = new List<GameObject> ();
		for (int i=0; i<5; i++) {
			topPosArr [i] = boardGrid.GetChild (i).localPosition + new Vector3 (0, CELL_BG_HEIGHT, 0);
			rightPosArr [i] = boardGrid.GetChild (i * 5 + 4).localPosition + new Vector3 (CELL_BG_WIDTH, 0, 0);
			bottomPosArr [i] = boardGrid.GetChild (20 + i).localPosition + new Vector3 (0, -CELL_BG_HEIGHT, 0);
			leftPosArr [i] = boardGrid.GetChild (i * 5).localPosition + new Vector3 (-CELL_BG_WIDTH, 0, 0);
		}
		//test ();
		//        testLose ();
		//testErase ();
		//PlayerPrefs.DeleteAll();
		
	}
	void showComboIcon(){
		comboIcon.GetComponent<TweenScale> ().from = new Vector3(1f,1f,1f);
		comboIcon.GetComponent<TweenScale> ().to = new Vector3(1.7f,1.7f,1f);
		comboIcon.GetComponent<TweenScale> ().duration = 0.2f;
		comboIcon.GetComponent<TweenScale> ().PlayForward ();
		comboIcon.GetComponent<TweenScale> ().enabled = true;
		comboIcon.GetComponent<TweenScale> ().ResetToBeginning ();
		comboIcon.SetActive(true);
		Invoke("removeComboIcon",2.5f);
		
	}
	void removeComboIcon(){
		comboIcon.SetActive(false);
		
	}
	void testErase ()
	{
		PlayerPrefs.SetInt ("erase", 10);
	}
	
	void testLose ()
	{
		//PlayerPrefs.DeleteAll ();
		for (int i=0; i<5; i++) {
			GameObject obj;
			obj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
			obj.transform.localPosition = boardGrid.GetChild (i).localPosition;
			obj.GetComponent<UISprite> ().spriteName = "o so 1";
			obj.transform.Find ("number").GetComponent<UILabel> ().text = "8";
			if (i==1||i==2) obj.transform.Find ("number").GetComponent<UILabel> ().text = "9";
			obj.tag = "cell";
			boardObjects [0, i] = obj;
		}
	}
	
	void test ()
	{
		GameObject obj;
		obj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
		obj.transform.localPosition = boardGrid.GetChild (0).localPosition;
		obj.GetComponent<UISprite> ().spriteName = "o 9 diem b";
		obj.transform.Find ("number").GetComponent<UILabel> ().text = "9";
		boardObjects [0, 0] = obj;
		
		obj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
		obj.transform.localPosition = boardGrid.GetChild (1).localPosition;
		obj.GetComponent<UISprite> ().spriteName = "o 9 diem b";
		obj.transform.Find ("number").GetComponent<UILabel> ().text = "9";
		boardObjects [0, 1] = obj;
		
		obj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
		obj.transform.localPosition = boardGrid.GetChild (2).localPosition;
		obj.GetComponent<UISprite> ().spriteName = "o 9 diem b";
		obj.transform.Find ("number").GetComponent<UILabel> ().text = "9";
		boardObjects [0, 2] = obj;
		
		obj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
		obj.transform.localPosition = boardGrid.GetChild (6).localPosition;
		obj.GetComponent<UISprite> ().spriteName = "o 9 diem b";
		obj.transform.Find ("number").GetComponent<UILabel> ().text = "9";
		boardObjects [1, 1] = obj;
		
		obj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
		obj.transform.localPosition = boardGrid.GetChild (11).localPosition;
		obj.GetComponent<UISprite> ().spriteName = "o 9 diem b";
		obj.transform.Find ("number").GetComponent<UILabel> ().text = "9";
		boardObjects [2, 1] = obj;
		
	}
	
	void OnEnable ()
	{
		print ("enable");
		Cell.onCellDestroyed += destroyCell;
	}
	
	void OnDisable ()
	{
		Cell.onCellDestroyed -= destroyCell;
	}
	
	void destroyCell ()
	{
		print ("ON CEL Destroyed");
		//    Invoke ("showHighLight", 0.2f);
		isReadyUseErase = false;
		buttonErase.GetComponent<UIButton> ().normalSprite = "button erase 1";
		buttonErase.GetComponent<UIButton> ().pressedSprite = "button erase 2";
		showHighLight ();
		
	}
	
	void addNewCell ()
	{
		//    itemObj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
		//    itemObj.transform.localPosition = topPosArr[2];
		
		//addedCell.Add (itemObj);
		//    currentSide=2;
		//currentSide++;
		eating = false;
		currentSide++;
		if (currentSide > 4)
			currentSide = 1;
		switch (currentSide) {
		case 1:
			itemObj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
			itemObj.transform.localPosition = topPosArr [2];
			break;
		case 2:
			itemObj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
			itemObj.transform.localPosition = rightPosArr [2];
			break;
		case 3:
			itemObj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
			itemObj.transform.localPosition = bottomPosArr [2];
			break;
		case 4:
			itemObj = NGUITools.AddChild (boardBg.gameObject, newCellPrefab);
			itemObj.transform.localPosition = leftPosArr [2];
			break;
			
		}
		addedCell.Add (itemObj);
		itemObj.GetComponent<UISprite> ().spriteName = "o ngoai";
		itemObj.transform.Find ("number").GetComponent<UILabel> ().text = "" + Random.Range (1, 6);
		itemObj.transform.Find ("number").GetComponent<UILabel> ().bitmapFont = numberfont;
		
		checkNoMoreMove ();
		showHighLight ();
		
	}
	
	int  getCurrentIndex ()
	{
		switch (currentSide) {
		case 1:
			for (int i=0; i<topPosArr.Length; i++) {
				if (topPosArr [i] == itemObj.transform.localPosition)
					return i;
			}
			return -1;
		case 2:
			for (int i=0; i<rightPosArr.Length; i++) {
				if (rightPosArr [i] == itemObj.transform.localPosition)
					return i;
			}
			return -1;
		case 3:
			for (int i=0; i<bottomPosArr.Length; i++) {
				if (bottomPosArr [i] == itemObj.transform.localPosition)
					return i;
			}
			return -1;
		case 4:
			for (int i=0; i<leftPosArr.Length; i++) {
				if (leftPosArr [i] == itemObj.transform.localPosition)
					return i;
			}
			return -1;
		default:
			return -1;
		}
	}
	
	public void swipeUp ()
	{
		if (blockWhenMoving)
			return;
		if (eating) return;
		switch (currentSide) {
		case 1:
			break;
		case 2:
			moveCellTo (rightPosArr [0], false);
			break;
		case 3:
			for (int i=4; i>=0; i--) {
				print ("check " + i);
				if (boardObjects [i, getCurrentIndex ()] != null) {
					boardNumber = int.Parse (boardObjects [i, getCurrentIndex ()].transform.Find ("number").GetComponent<UILabel> ().text);
					addNumber = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					print ("board number : " + boardNumber + "addnumber: " + addNumber);
					print ("board row " + i + " / board column: " + getCurrentIndex ());
					if (boardNumber + addNumber <= 9) {
						cellRow = i;
						cellColumn = getCurrentIndex ();
						cellPosition = (i * 5) + getCurrentIndex ();
						
						print ("move down to " + cellPosition);
						moveCellInBoard = true;
						objectToDestroy = boardObjects [i, getCurrentIndex ()];
						moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, true);
						
					} else {
						if (i < 4) {
							
							print ("can not merge");
							cellRow = i;
							cellColumn = getCurrentIndex ();
							cellPosition = (i * 5) + 5 + getCurrentIndex ();
							boardObjects [cellRow + 1, cellColumn] = itemObj;
							moveCellInBoard = true;
							moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, false);
						}
						
						
					}
					break;
				} else if (i == 0) {
					cellPosition = (i * 5) + getCurrentIndex ();
					cellRow = i;
					cellColumn = getCurrentIndex ();
					boardObjects [i, getCurrentIndex ()] = itemObj;
					moveCellInBoard = true;
					moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, false);
				}
			}
			break;
		case 4:
			moveCellTo (leftPosArr [0], false);
			break;
		}
	}
	
	public void swipeDown ()
	{
		if (blockWhenMoving)
			return;
		if (eating) return;
		/*if (tutorial==0) return;
        if (tutorial==1) {
            tutorialSwipe.SetActive(false);
            tutorialFinger2.SetActive(true);
            PlayerPrefs.SetInt("tutorial",2);
            tutorial=2;
        }*/
		
		switch (currentSide) {
		case 1:
			
			for (int i=0; i<BOARD_SIZE; i++) {
				print ("check " + i);
				if (boardObjects [i, getCurrentIndex ()] != null) {
					boardNumber = int.Parse (boardObjects [i, getCurrentIndex ()].transform.Find ("number").GetComponent<UILabel> ().text);
					addNumber = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					print ("board number : " + boardNumber + "addnumber: " + addNumber);
					print ("board row " + i + " / board column: " + getCurrentIndex ());
					if (boardNumber + addNumber <= 9) {
						cellRow = i;
						cellColumn = getCurrentIndex ();
						cellPosition = (i * 5) + getCurrentIndex ();
						
						print ("move down to " + cellPosition);
						moveCellInBoard = true;
						objectToDestroy = boardObjects [i, getCurrentIndex ()];
						moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, true);
						
					} else {
						if (i > 0) {
							
							print ("can not merge");
							cellRow = i;
							cellColumn = getCurrentIndex ();
							cellPosition = (i * 5) - 5 + getCurrentIndex ();
							boardObjects [cellRow - 1, cellColumn] = itemObj;
							moveCellInBoard = true;
							moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, false);
						}
						
						
					}
					break;
				} else if (i == 4) {
					cellPosition = (i * 5) + getCurrentIndex ();
					cellRow = i;
					cellColumn = getCurrentIndex ();
					boardObjects [i, getCurrentIndex ()] = itemObj;
					moveCellInBoard = true;
					moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, false);
				}
			}
			break;
		case 2:
			moveCellTo (rightPosArr [4], false);
			break;
		case 3:
			break;
			
		case 4:
			moveCellTo (leftPosArr [4], false);
			break;
		}
	}
	
	public void swipeLeft ()
	{
		/*
        if (tutorial==2) return;
        if (tutorial==3) {

            tutorialSwipe2.SetActive(false);
            PlayerPrefs.SetInt("tutorial",4);
            tutorial=4;
        }*/
		if (blockWhenMoving)
			return;
		if (eating) return;
		switch (currentSide) {
		case 1:
			//itemObj.GetComponent<TweenPosition>().enabled=false;
			
			
			moveCellTo (topPosArr [0], false);
			print ("left 1");
			break;
		case 2:
			for (int i=4; i>=0; i--) {
				print ("check " + i);
				if (boardObjects [getCurrentIndex (), i] != null) {
					
					boardNumber = int.Parse (boardObjects [getCurrentIndex (), i].transform.Find ("number").GetComponent<UILabel> ().text);
					addNumber = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					print ("board number : " + boardNumber + "addnumber: " + addNumber);
					print ("board row " + getCurrentIndex () + " / board column: " + i);
					if (boardNumber + addNumber <= 9) {
						cellRow = getCurrentIndex ();
						cellColumn = i;
						cellPosition = (cellRow * 5) + i;
						
						print ("move down to " + cellPosition);
						moveCellInBoard = true;
						objectToDestroy = boardObjects [getCurrentIndex (), i];
						moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, true);
						
					} else {
						if (i < 4) {
							//boardObjects[getCurrentIndex(),i]=itemObj;
							print ("can not merge");
							cellRow = getCurrentIndex ();
							cellColumn = i;
							cellPosition = (cellRow * 5) + 1 + i;
							boardObjects [cellRow, cellColumn + 1] = itemObj;
							moveCellInBoard = true;
							moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, false);
						}
						
						
						
						
					}
					break;
				} else if (i == 0) {
					cellPosition = getCurrentIndex () * 5;
					cellRow = getCurrentIndex ();
					cellColumn = i;
					boardObjects [getCurrentIndex (), i] = itemObj;
					moveCellInBoard = true;
					moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, false);
				}
			}
			break;
		case 3:
			moveCellTo (bottomPosArr [0], false);
			break;
		}
	}
	
	public void swipeRight ()
	{
		if (blockWhenMoving)
			return;
		if (eating) return;
		switch (currentSide) {
		case 1:
			//    itemObj.GetComponent<TweenPosition>().enabled=false;
			
			moveCellTo (topPosArr [4], false);
			print ("right 1");
			break;
			
		case 3:
			moveCellTo (bottomPosArr [4], false);
			break;
		case 4:
			for (int i=0; i<BOARD_SIZE; i++) {
				print ("check " + i);
				if (boardObjects [getCurrentIndex (), i] != null) {
					boardNumber = int.Parse (boardObjects [getCurrentIndex (), i].transform.Find ("number").GetComponent<UILabel> ().text);
					addNumber = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					print ("board number : " + boardNumber + "addnumber: " + addNumber);
					print ("board row " + getCurrentIndex () + " / board column: " + i);
					if (boardNumber + addNumber <= 9) {
						cellRow = getCurrentIndex ();
						cellColumn = i;
						cellPosition = (cellRow * 5) + i;
						
						print ("move down to " + cellPosition);
						moveCellInBoard = true;
						objectToDestroy = boardObjects [getCurrentIndex (), i];
						moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, true);
						
					} else {
						if (i > 0) {
							
							print ("can not merge");
							cellRow = getCurrentIndex ();
							cellColumn = i;
							cellPosition = (cellRow * 5) + i - 1;
							boardObjects [cellRow, cellColumn - 1] = itemObj;
							moveCellInBoard = true;
							moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, false);
						}
						
						
						
						
					}
					break;
				} else if (i == 4) {
					print ("end row");
					cellPosition = getCurrentIndex () * 5 + i;
					cellRow = getCurrentIndex ();
					cellColumn = i;
					boardObjects [getCurrentIndex (), i] = itemObj;
					moveCellInBoard = true;
					moveCellTo (boardGrid.GetChild (cellPosition).transform.localPosition, false);
				}
			}
			break;
		}
		
	}
	
	void moveCellTo (Vector3 pos, bool isMerge)
	{
		blockWhenMoving = true;
		itemObj.GetComponent<TweenPosition> ().from = itemObj.transform.localPosition;
		itemObj.GetComponent<TweenPosition> ().to = pos;
		itemObj.GetComponent<TweenPosition> ().duration = 0.2f;
		itemObj.GetComponent<TweenPosition> ().PlayForward ();
		itemObj.GetComponent<TweenPosition> ().enabled = true;
		itemObj.GetComponent<TweenPosition> ().ResetToBeginning ();
		//    itemObj.GetComponent<TweenPosition> ().onFinished = new List<EventDelegate>();
		if (moveCellInBoard) {
			itemObj.tag = "cell";
			if (!isMerge) {
				print ("is mearge " + isMerge);
				itemObj.GetComponent<TweenPosition> ().AddOnFinished (onFinishMoving);
			} else {
				print ("is mearge " + isMerge);
				itemObj.GetComponent<TweenPosition> ().AddOnFinished (onFinishMovingAndMerge);
			}
		} else {
			print ("move cell");
			//        itemObj.GetComponent<TweenPosition> ().AddOnFinished (showHighLight);
			
			EventDelegate del = new EventDelegate (this, "showHighLight");
			//    del.parameters[0].value = obj;
			//EventDelegate.Set( obj.GetComponents<TweenScale> () [1].onFinished, del ); 
			EventDelegate.Add (itemObj.GetComponent<TweenPosition> ().onFinished, del, true);
			
		}
		for (int i=0; i<boardGrid.GetChildList().Count; i++) {
			boardGrid.GetChild (i).transform.Find ("line").gameObject.SetActive (false);
		}
		
		
		
		
	}
	
	public void showHighLight ()
	{
		blockWhenMoving = false;
		print ("show highlight");
		switch (currentSide) {
		case 1:
			for (int i=0; i<BOARD_SIZE; i++) {
				
				if (boardObjects [i, getCurrentIndex ()] == null) {
					boardGrid.GetChild (i * 5 + getCurrentIndex ()).transform.Find ("line").gameObject.SetActive (true);
					
				} else {
					int boardNumber = int.Parse (boardObjects [i, getCurrentIndex ()].transform.Find ("number").GetComponent<UILabel> ().text);
					int addNumber = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					
					if (boardNumber + addNumber <= 9) {
						boardGrid.GetChild (i * 5 + getCurrentIndex ()).transform.Find ("line").gameObject.SetActive (true);
					}
					return;
				}
				
			}
			break;
		case 2:
			for (int j=4; j>=0; j--) {
				
				if (boardObjects [getCurrentIndex (), j] == null) {
					boardGrid.GetChild (getCurrentIndex () * 5 + j).transform.Find ("line").gameObject.SetActive (true);
					
				} else {
					int boardNumber = int.Parse (boardObjects [getCurrentIndex (), j].transform.Find ("number").GetComponent<UILabel> ().text);
					int addNumber = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					
					if (boardNumber + addNumber <= 9) {
						boardGrid.GetChild (getCurrentIndex () * 5 + j).transform.Find ("line").gameObject.SetActive (true);
					}
					return;
				}
				
			}
			break;
		case 3:
			for (int i=4; i>=0; i--) {
				
				if (boardObjects [i, getCurrentIndex ()] == null) {
					boardGrid.GetChild (i * 5 + getCurrentIndex ()).transform.Find ("line").gameObject.SetActive (true);
					
				} else {
					int boardNumber = int.Parse (boardObjects [i, getCurrentIndex ()].transform.Find ("number").GetComponent<UILabel> ().text);
					int addNumber = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					
					if (boardNumber + addNumber <= 9) {
						boardGrid.GetChild (i * 5 + getCurrentIndex ()).transform.Find ("line").gameObject.SetActive (true);
					}
					return;
				}
				
			}
			break;
		case 4:
			for (int j=0; j<5; j++) {
				
				if (boardObjects [getCurrentIndex (), j] == null) {
					boardGrid.GetChild (getCurrentIndex () * 5 + j).transform.Find ("line").gameObject.SetActive (true);
					
				} else {
					int boardNumber = int.Parse (boardObjects [getCurrentIndex (), j].transform.Find ("number").GetComponent<UILabel> ().text);
					int addNumber = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					
					if (boardNumber + addNumber <= 9) {
						boardGrid.GetChild (getCurrentIndex () * 5 + j).transform.Find ("line").gameObject.SetActive (true);
					}
					return;
				}
				
			}
			break;
			
			
		}
	}
	
	public void onFinishMovingAndMerge ()
	{
		print ("finish moving and merge");    
		blockWhenMoving = false;
		
		if (moveCellInBoard) {
			moveCellInBoard = false;
			itemObj.transform.Find ("number").GetComponent<UILabel> ().text = "" + (boardNumber + addNumber);
			if (boardNumber + addNumber == 9) {
				addScore (18);
				itemObj.GetComponent<UISprite> ().spriteName = "o 9 diem b";
				itemObj.transform.Find ("number").GetComponent<UILabel> ().bitmapFont = point9font;
				gameObject.GetComponents<AudioSource> () [0].Play ();
			} else {
				addScore (2);
				switch(boardNumber + addNumber){
				case 1:
					itemObj.GetComponent<UISprite> ().spriteName = "o so 1";
					break;
				case 2:
					itemObj.GetComponent<UISprite> ().spriteName = "o so 2";
					break;
				case 3:
					itemObj.GetComponent<UISprite> ().spriteName = "o so 3";
					break;
				case 4:
					itemObj.GetComponent<UISprite> ().spriteName = "o so 4";
					break;
				case 5:
					itemObj.GetComponent<UISprite> ().spriteName = "o so 5";
					break;
				case 6:
					itemObj.GetComponent<UISprite> ().spriteName = "o so 6";
					break;
				case 7:
					itemObj.GetComponent<UISprite> ().spriteName = "o so 7";
					break;
				case 8:
					itemObj.GetComponent<UISprite> ().spriteName = "o so 8";
					break;
					
				}
				
				gameObject.GetComponents<AudioSource> () [3].Play ();
			}
			
			boardObjects [cellRow, cellColumn] = itemObj;
			NGUITools.DestroyImmediate (objectToDestroy);
			checkBoard ();
			if (!eating)
				addNewCell ();
			else {
				Invoke("addNewCell",0.5f);
			}
		}
	}
	
	public void onFinishMoving ()
	{
		print ("finish moving");    
		blockWhenMoving = false;
		if (moveCellInBoard) {
			moveCellInBoard = false;
			//    boardObjects [cellRow,cellColumn] = itemObj;
			//    print("add number "+addNumber);
			switch(int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text)){
			case 1:
				itemObj.GetComponent<UISprite> ().spriteName = "o so 1";
				break;
			case 2:
				itemObj.GetComponent<UISprite> ().spriteName = "o so 2";
				break;
			case 3:
				itemObj.GetComponent<UISprite> ().spriteName = "o so 3";
				break;
			case 4:
				itemObj.GetComponent<UISprite> ().spriteName = "o so 4";
				break;
			case 5:
				itemObj.GetComponent<UISprite> ().spriteName = "o so 5";
				break;
				
			}
			
			gameObject.GetComponents<AudioSource>()[5].Play();
			addScore (1);
			addNewCell ();
		}
		
		
	}
	
	void checkNoMoreMove ()
	{
		switch (currentSide) {
		case 1:
			
			for (int i=0; i<BOARD_SIZE; i++) {
				if (boardObjects [0, i] != null) {
					if (boardObjects [0, i].gameObject.GetComponent<Cell>().eating) return;
					int inboard = int.Parse (boardObjects [0, i].transform.Find ("number").GetComponent<UILabel> ().text);
					int outboard = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					if (inboard + outboard <= 9)
						return;
					
					
					
					
				} else
					return;
			}
			print ("NO MORE MOVE");
			loseGame ();
			break;
		case 2:
			for (int i=0; i<BOARD_SIZE; i++) {
				if (boardObjects [i, 4] != null) {
					if (boardObjects [i, 4].gameObject.GetComponent<Cell>().eating) return;
					int inboard = int.Parse (boardObjects [i, 4].transform.Find ("number").GetComponent<UILabel> ().text);
					int outboard = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					if (inboard + outboard <= 9)
						return;
					
					
					
				} else
					return;
				
			}
			print ("NO MORE MOVE");
			loseGame ();
			break;
		case 3:
			for (int i=0; i<BOARD_SIZE; i++) {
				if (boardObjects [4, i] != null) {
					if (boardObjects [4, i].gameObject.GetComponent<Cell>().eating) return;
					int inboard = int.Parse (boardObjects [4, i].transform.Find ("number").GetComponent<UILabel> ().text);
					int outboard = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					if (inboard + outboard <= 9)
						return;
					
					
					
				} else
					return;
				
			}
			print ("NO MORE MOVE");
			loseGame ();
			break;
		case 4:
			for (int i=0; i<BOARD_SIZE; i++) {
				if (boardObjects [i, 0] != null) {
					if (boardObjects [i, 0].gameObject.GetComponent<Cell>().eating) return;
					int inboard = int.Parse (boardObjects [i, 0].transform.Find ("number").GetComponent<UILabel> ().text);
					int outboard = int.Parse (itemObj.transform.Find ("number").GetComponent<UILabel> ().text);
					if (inboard + outboard <= 9)
						return;
					
					
					
				} else
					return;
			}
			print ("NO MORE MOVE");
			loseGame ();
			break;
			
		}
	}
	
	void loseGame ()
	{
		//    gameObject.GetComponents<AudioSource>()[2].Play();
		isNoMoveLeft=true;
		
		nomoveleftText.GetComponent<TweenScale> ().from = new Vector3(0,0,0);
		nomoveleftText.GetComponent<TweenScale> ().to =new Vector3(1,1,1);
		nomoveleftText.GetComponent<TweenScale> ().duration = 1f;
		nomoveleftText.GetComponent<TweenScale> ().PlayForward ();
		nomoveleftText.GetComponent<TweenScale> ().enabled = true;
		nomoveleftText.GetComponent<TweenScale> ().ResetToBeginning ();
		
		nomoveleftText.GetComponent<TweenAlpha> ().from = 1f;
		nomoveleftText.GetComponent<TweenAlpha> ().to =0f;
		nomoveleftText.GetComponent<TweenAlpha> ().duration = 1f;
		nomoveleftText.GetComponent<TweenAlpha> ().delay = 1f;
		nomoveleftText.GetComponent<TweenAlpha> ().enabled = false;
		nomoveleftText.GetComponent<TweenAlpha> ().ResetToBeginning ();
		
		nomoveleftText.SetActive(true);
		
		
	}
	public void showLoseDialog(){
		
		nomoveleftText.SetActive(false);
		if (getEraseQuantity () > 0) {
			if (score > getHighScore ()) {
				setHighScore ();
			}
			dialogLose.GetComponent<UIScaleAnimation> ().open ();
			dialogLose.transform.Find ("lblScore").GetComponent<UILabel> ().text = "" + score;
			dialogLose.transform.Find ("lblHScore").GetComponent<UILabel> ().text = "BEST ; " + getHighScore ();
			dialogLose.transform.Find ("ButtonErase").Find ("quantity").GetComponent<UILabel> ().text = "" + getEraseQuantity ();
			dialogLose.transform.Find ("erasetext").GetComponent<UILabel> ().text = "You have "+getEraseQuantity()+" Eraser(s) \n You can erase any number by Eraser";

		} else {
			if (score <= getHighScore ()) {
				dialogScore.GetComponent<UIScaleAnimation> ().open ();
				dialogScore.transform.Find ("lblScore").GetComponent<UILabel> ().text = "" + score;
				dialogScore.transform.Find ("lblHScore").GetComponent<UILabel> ().text = "BEST ; " + getHighScore ();
			} else {
				setHighScore ();
				dialogHighScore.GetComponent<UIScaleAnimation> ().open ();
				dialogHighScore.transform.Find ("lblScore").GetComponent<UILabel> ().text = "" + score;
				dialogHighScore.transform.Find ("lblHScore").GetComponent<UILabel> ().text = "BEST ; " + getHighScore ();
			}
		}
	}
	
	
	
	int getHighScore ()
	{
		return PlayerPrefs.GetInt ("highscore", 0);
	}
	
	void setHighScore ()
	{
		PlayerPrefs.SetInt ("highscore", score);
		
	}
	public void pauseGame(){
		isPause=true;
		dialogPause.SetActive(true);
		checkSoundState();
	}
	public void resumeGame(){
		isPause=false;
		dialogPause.SetActive(false);
	}
	public void toMainMenu ()
	{
		Application.LoadLevel ("MainMenu");
			
	}
	
	public void replay ()
	{
		Application.LoadLevel ("GameScreen");
	}
	int currentPagePos=1;
	// Update is called once per frame
	void checkCenterHelpPage(){
		switch (currentPagePos){
		case 1:
			if (helppage[0].transform.position.x<0.6){
				Vector3 newPos = scrollView.transform.worldToLocalMatrix.MultiplyPoint3x4(helppage[1].transform.position);
				SpringPanel.Begin(scrollView, -newPos-new Vector3(0,97f,0), 8f);
				currentPagePos ++;
			}
			break;
		case 2:
			if (helppage[1].transform.position.x<0.6){
				Vector3 newPos = scrollView.transform.worldToLocalMatrix.MultiplyPoint3x4(helppage[2].transform.position);
				SpringPanel.Begin(scrollView, -newPos-new Vector3(0,97f,0), 8f);
				currentPagePos ++;
			}
			if (helppage[1].transform.position.x>0){
				Vector3 newPos = scrollView.transform.worldToLocalMatrix.MultiplyPoint3x4(helppage[0].transform.position);
				SpringPanel.Begin(scrollView, -newPos-new Vector3(0,97f,0), 8f);
				currentPagePos --;
			}
			break;
		case 3:
			if (helppage[2].transform.position.x>0){
				Vector3 newPos = scrollView.transform.worldToLocalMatrix.MultiplyPoint3x4(helppage[1].transform.position);
				SpringPanel.Begin(scrollView, -newPos-new Vector3(0,97f,0), 8f);
				currentPagePos --;
			}
			break;
		}
	}
	void Update ()
	{
		buttonErase.transform.Find ("quantity").GetComponent<UILabel> ().text = "" + getEraseQuantity ();
		
		
		
		if (!isNoMoveLeft&& !isPause){
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				
				swipeUp ();
			}
			
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				swipeDown ();
				
			}
			
			
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				swipeLeft ();
			}
			
			
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				swipeRight ();
			}
			
			#if UNITY_EDITOR
			if     (Input.GetMouseButtonDown(0)){
				Vector3 vec3 = Input.mousePosition;
				vec3.z=-1;    
				checkClickInside(boardBg.transform.InverseTransformPoint( cam.ScreenToWorldPoint(vec3)));
			}
			if     (Input.GetMouseButtonUp(0)){
				print("pos "+ transform.position);
				checkCenterHelpPage();
				
			}
			
			#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
			
			
			int nbTouches = Input.touchCount;
			if(nbTouches > 0)
			{
				for (int i = 0; i < nbTouches; i++)
				{
					Touch touch = Input.GetTouch(i);
					if(touch.phase == TouchPhase.Began)
					{
						
						print("touch " + touch.position.x +"/ "+ touch.position.y);
						print("touch world" + cam.WorldToScreenPoint(touch.position).x +"/ "+ cam.WorldToScreenPoint(touch.position).y);
						Vector3 vec3 =  touch.position;
						vec3.z=-1;    
						checkClickInside(boardBg.transform.InverseTransformPoint( cam.ScreenToWorldPoint(vec3)));
						
					}
					if (touch.phase == TouchPhase.Ended){
						print("check input up");
						checkCenterHelpPage();
					}
				}
			}
			
			#endif
			
		} else if( isPause) {
			#if UNITY_EDITOR
			if     (Input.GetMouseButtonUp(0)){
				print("pos "+ transform.position);
				checkCenterHelpPage();
				
			}
			#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
			int nbTouches = Input.touchCount;
			if(nbTouches > 0)
			{
				for (int i = 0; i < nbTouches; i++)
				{
					Touch touch = Input.GetTouch(i);
					
					if (touch.phase == TouchPhase.Ended){
						print("check input up");
						checkCenterHelpPage();
					}
				}
			}
			#endif
			
		}
		
		
	}
	
	void checkClickInside (Vector3 clickPos)
	{
		if (blockWhenMoving)
			return;
		if (eating) return;
		switch (currentSide) {
		case 1:
			for (int i=0; i<topPosArr.Length; i++) {
				if (clickPos.x < topPosArr [i].x + CELL_BG_WIDTH / 2  
				    && clickPos.x > topPosArr [i].x - CELL_BG_WIDTH / 2 
				    && clickPos.y < topPosArr [i].y + CELL_BG_HEIGHT / 2 
				    && clickPos.y > topPosArr [i].y - CELL_BG_HEIGHT / 2) {
					print ("click inside " + topPosArr [i].x);
					disableFinger();
					if (i!=getCurrentIndex())
						moveCellTo (topPosArr [i], false);
				}
				
			}
			
			break;
		case 2:
			for (int i=0; i<rightPosArr.Length; i++) {
				if (clickPos.x < rightPosArr [i].x + CELL_BG_WIDTH / 2  
				    && clickPos.x > rightPosArr [i].x - CELL_BG_WIDTH / 2 
				    && clickPos.y < rightPosArr [i].y + CELL_BG_HEIGHT / 2 
				    && clickPos.y > rightPosArr [i].y - CELL_BG_HEIGHT / 2) {
					print ("click inside " + rightPosArr [i].x);
					disableFinger();
					if (i!=getCurrentIndex())
						moveCellTo (rightPosArr [i], false);
				}
				
			}
			
			break;
		case 3:
			for (int i=0; i<bottomPosArr.Length; i++) {
				if (clickPos.x < bottomPosArr [i].x + CELL_BG_WIDTH / 2  
				    && clickPos.x > bottomPosArr [i].x - CELL_BG_WIDTH / 2 
				    && clickPos.y < bottomPosArr [i].y + CELL_BG_HEIGHT / 2 
				    && clickPos.y > bottomPosArr [i].y - CELL_BG_HEIGHT / 2) {
					print ("click inside " + bottomPosArr [i].x);
					disableFinger();
					if (i!=getCurrentIndex())
						moveCellTo (bottomPosArr [i], false);
				}
				
			}
			
			break;
		case 4:
			for (int i=0; i<leftPosArr.Length; i++) {
				if (clickPos.x < leftPosArr [i].x + CELL_BG_WIDTH / 2  
				    && clickPos.x > leftPosArr [i].x - CELL_BG_WIDTH / 2 
				    && clickPos.y < leftPosArr [i].y + CELL_BG_HEIGHT / 2 
				    && clickPos.y > leftPosArr [i].y - CELL_BG_HEIGHT / 2) {
					print ("click inside " + leftPosArr [i].x);
					disableFinger();
					if (i!=getCurrentIndex())
						moveCellTo (leftPosArr [i], false);
				}
				
			}
			
			break;
		}
		
	}
	void disableFinger(){
		/*
        if (tutorial==0){
            tutorialFinger.SetActive(false);
            tutorialSwipe.SetActive(true);
            PlayerPrefs.SetInt("tutorial",1);
            tutorial=1;
        } else if (tutorial ==2){
            tutorialFinger2.SetActive(false);
            tutorialSwipe2.SetActive(true);
            PlayerPrefs.SetInt("tutorial",3);
            tutorial=3;
        }*/
		
	}
	void addScore (int addScore)
	{
		score = score + addScore;
		scoreText.text = "" + score;
	}
	
	void initBoardBg ()
	{
		boardGrid = boardBg.gameObject.transform.Find ("Grid").GetComponent<UIGrid> ();
		for (int i=0; i<BOARD_SIZE; i++) {
			for (int j=0; j<BOARD_SIZE; j++) {
				GameObject itemObj = NGUITools.AddChild (boardBg.gameObject, boardTittleTransform);
				//    itemObj.transform.Find("line").gameObject.SetActive(true);
				boardGrid.AddChild (itemObj.transform);
				itemObj.transform.localScale = new Vector3 (1, 1, 1);
				boardGrid.Reposition ();
			}
		}
	}
	
	string getNumber (GameObject obj)
	{
		return obj.transform.Find ("number").GetComponent<UILabel> ().text;
	}
	
	bool isEqual9 (GameObject obj)
	{
		if (obj != null)
			return obj.transform.Find ("number").GetComponent<UILabel> ().text.Equals ("9") && !obj.GetComponent<Cell> ().eating;
		else
			return false;
	}
	
	void removeSelf (GameObject obj)
	{
		NGUITools.DestroyImmediate (obj);
		//Invoke("showHighLight",0.2f);
		//    showHighLight ();
	}
	public void playClickSound(){
		gameObject.GetComponents<AudioSource>()[6].Play();
	}
	void removingEatedCell (GameObject obj)
	{
		eating = true;
		obj.GetComponent<Cell> ().eating = true;
		obj.GetComponents<TweenScale> () [1].PlayForward ();
		obj.GetComponents<TweenScale> () [1].enabled = true;
		obj.GetComponent<TweenRotation> ().PlayForward ();
		obj.GetComponent<TweenRotation> ().enabled = true;
		
		EventDelegate del = new EventDelegate (this, "removeSelf");
		del.parameters [0].value = obj;
		//EventDelegate.Set( obj.GetComponents<TweenScale> () [1].onFinished, del ); 
		EventDelegate.Add (obj.GetComponents<TweenScale> () [1].onFinished, del);
		
	}
	
	public void continuePlay ()
	{
		isNoMoveLeft=false;
		dialogLose.GetComponent<UIScaleAnimation> ().close ();
		buttonHighScore.transform.Find ("score").GetComponent<UILabel> ().text = "" + getHighScore ();
		isReadyUseErase = true;
		buttonErase.GetComponent<UIButton> ().normalSprite = "red erase";
		buttonErase.GetComponent<UIButton> ().pressedSprite = "red erase";
	}
	
	public void readyToUseErase ()
	{
		
		if (getEraseQuantity () > 0) {
			if (!isReadyUseErase) {
				isReadyUseErase = true;
				buttonErase.GetComponent<UIButton> ().normalSprite = "red erase";
				buttonErase.GetComponent<UIButton> ().pressedSprite = "red erase";
			} else {
				isReadyUseErase = false;
				buttonErase.GetComponent<UIButton> ().normalSprite = "button erase 1";
				buttonErase.GetComponent<UIButton> ().pressedSprite = "button erase 2";
			}
			
		}
		
	}
	
	public static int getEraseQuantity ()
	{
		return PlayerPrefs.GetInt ("erase", DEFAULT_ERASE_QUANTITY);
	}
	
	public static void useErase ()
	{
		int eraseQuantity = PlayerPrefs.GetInt ("erase", DEFAULT_ERASE_QUANTITY);
		eraseQuantity--;
		PlayerPrefs.SetInt ("erase", eraseQuantity);
		
	}
	
	public static void addErase (int add)
	{
		int eraseQuantity = PlayerPrefs.GetInt ("erase", DEFAULT_ERASE_QUANTITY);
		eraseQuantity += add;
		PlayerPrefs.SetInt ("erase", eraseQuantity);
		
	}
	public void shareScreenShot(){
		ScreenCapture.CaptureScreenshot("share.png");
		string pathToImage = Application.persistentDataPath + "/" + "share.png";
		print("image path "+ pathToImage);
		#if UNITY_ANDROID
		
		//    byte[] bytes = new byte[];
		
		//    File.WriteAllBytes(pathToImage, bytes);
		
		//instantiate the class Intent
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		
		//instantiate the object Intent
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		
		//call setAction setting ACTION_SEND as parameter
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		
		//instantiate the class Uri
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		
		//instantiate the object Uri with the parse of the url's file
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://"+pathToImage);
		
		//call putExtra with the uri object of the file
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		
		//set the type of file
		intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
		
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Let's get 45");
		
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Do you dare to get throught me? https://play.google.com/store/apps/details?id=com.arrowhitech.letsget45  #letsget45");
		//instantiate the class UnityPlayer
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		
		//instantiate the object currentActivity
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		
		//call the activity with our Intent
		currentActivity.Call("startActivity", intentObject);
		#elif UNITY_IOS
		GeneralSharingiOSBridge.ShareTextWithImage(pathToImage, "Do you dare to get through me? https://itunes.apple.com/us/app/lets-get-45/id1046445808?ls=1&mt=8 #letsget45");
		
		#endif
		
		
		
	}
	void checkBoard ()
	{
		checkT51 ();
		checkT52 ();
		checkT53 ();
		checkT54 ();
		checkL51 ();
		checkL52 ();
		checkL53 ();
		checkL54 ();
		checkI51 ();
		checkI52 ();
		checkI41 ();
		checkI42 ();
		checkL41 ();
		checkL42 ();
		checkL43 ();
		checkL44 ();
		checkL4R1 ();
		checkL4R2 ();
		checkL4R3 ();
		checkL4R4 ();
		checkSquare4 ();
		check31 ();
		check32 ();
	}
	//T Doc
	void checkT51 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 3 && j > 0 && j < 4)
						if (isEqual9 (boardObjects [i, j - 1]) 
						    && isEqual9 (boardObjects [i, j + 1])
						    && isEqual9 (boardObjects [i + 1, j])
						   && isEqual9 (boardObjects [i + 2, j])) {
						print ("T51 COMPLETE, START REMOVING");
						showComboIcon();
						addScore (9 * 5);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i + 2, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
	}
	//T Ngang Trai
	void checkT52 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 4 && j > 1 && i > 0)
						if (isEqual9 (boardObjects [i + 1, j]) 
						    && isEqual9 (boardObjects [i, j - 1])
						    && isEqual9 (boardObjects [i, j - 2])
						   && isEqual9 (boardObjects [i - 1, j])) {
						print ("T52 COMPLETE, START REMOVING");
						showComboIcon();
						addScore (9 * 5);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j - 2]);
						removingEatedCell (boardObjects [i - 1, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
	}
	//T Nguoc 
	void checkT53 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (j > 0 && i > 1 && j < 4)
						if (isEqual9 (boardObjects [i, j - 1]) 
						    && isEqual9 (boardObjects [i, j + 1])
						    && isEqual9 (boardObjects [i - 1, j])
						   && isEqual9 (boardObjects [i - 2, j])) {
						print ("T53 COMPLETE, START REMOVING");
						
						showComboIcon();
						addScore (9 * 5);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i - 2, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
	}
	
	//T xoay phai 
	void checkT54 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i > 0 && j < 3 && i < 4)
						if (isEqual9 (boardObjects [i + 1, j]) 
						    && isEqual9 (boardObjects [i - 1, j])
						    && isEqual9 (boardObjects [i, j + 1])
						   && isEqual9 (boardObjects [i, j + 2])) {
						print ("T54 COMPLETE, START REMOVING");
						showComboIcon();
						addScore (9 * 5);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i, j + 2]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
	}
	
	void checkL51 ()
	{
		
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i > 1 && j < 3)
						if (isEqual9 (boardObjects [i, j + 1]) 
						    && isEqual9 (boardObjects [i, j + 2])
						    && isEqual9 (boardObjects [i - 1, j])
						   && isEqual9 (boardObjects [i - 2, j])) {
						print ("checkL51 COMPLETE, START REMOVING");
						addScore (9 * 5);
						showComboIcon();
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i, j + 2]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i - 2, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
		
		
		
	}
	
	void checkL52 ()
	{
		
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 3 && j < 3)
						if (isEqual9 (boardObjects [i, j + 1]) 
						    && isEqual9 (boardObjects [i, j + 2])
						    && isEqual9 (boardObjects [i + 1, j])
						   && isEqual9 (boardObjects [i + 2, j])) {
						print ("checkL52 COMPLETE, START REMOVING");
						showComboIcon();
						addScore (9 * 5);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i, j + 2]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i + 2, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
		
		
		
	}
	
	void checkL53 ()
	{
		
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 3 && j > 1)
						if (isEqual9 (boardObjects [i + 1, j]) 
						    && isEqual9 (boardObjects [i + 2, j])
						    && isEqual9 (boardObjects [i, j - 1])
						   && isEqual9 (boardObjects [i, j - 2])) {
						print ("checkL53 COMPLETE, START REMOVING");
						showComboIcon();
						addScore (9 * 5);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i + 2, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j - 2]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
		
		
		
	}
	
	void checkL54 ()
	{
		
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i > 1 && j > 1)
						if (isEqual9 (boardObjects [i, j - 1]) 
						    && isEqual9 (boardObjects [i, j - 2])
						    && isEqual9 (boardObjects [i - 1, j])
						   && isEqual9 (boardObjects [i - 2, j])) {
						print ("checkL54 COMPLETE, START REMOVING");
						showComboIcon();
						addScore (9 * 5);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j - 2]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i - 2, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
		
		
		
	}
	
	void checkI51 ()
	{
		
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i == 2)
						if (isEqual9 (boardObjects [i + 1, j]) 
						    && isEqual9 (boardObjects [i + 2, j])
						    && isEqual9 (boardObjects [i - 1, j])
						   && isEqual9 (boardObjects [i - 2, j])) {
						print ("checkI51 COMPLETE, START REMOVING");
						showComboIcon();
						addScore (9 * 5);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i + 2, j]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i - 2, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}    
	}
	
	void checkI52 ()
	{
		
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (j == 2)
						if (isEqual9 (boardObjects [i, j + 1]) 
						    && isEqual9 (boardObjects [i, j + 2])
						    && isEqual9 (boardObjects [i, j - 1])
						   && isEqual9 (boardObjects [i, j - 2])) {
						print ("checkI52 COMPLETE, START REMOVING");
						addScore (9 * 5);
						showComboIcon();
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i, j + 2]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j - 2]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}    
	}
	// I doc
	void checkI41 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 3 && i > 0)
						if (isEqual9 (boardObjects [i + 1, j]) 
						    && isEqual9 (boardObjects [i + 2, j])
						   && isEqual9 (boardObjects [i - 1, j])) {
						print ("checkI41 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i + 2, j]);
						removingEatedCell (boardObjects [i - 1, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// I ngang
	void checkI42 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (j < 3 && j > 0)
						if (isEqual9 (boardObjects [i, j - 1]) 
						    && isEqual9 (boardObjects [i, j + 1])
						   && isEqual9 (boardObjects [i, j + 2])) {
						print ("checkI42 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i, j + 2]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// L xuoi
	void checkL41 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i > 1 && j < 4)
						if (isEqual9 (boardObjects [i, j + 1]) 
						    && isEqual9 (boardObjects [i - 1, j])
						   && isEqual9 (boardObjects [i - 2, j])) {
						print ("checkL41 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i - 2, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// L do phai
	void checkL42 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 4 && j < 3)
						if (isEqual9 (boardObjects [i + 1, j]) 
						    && isEqual9 (boardObjects [i, j + 1])
						   && isEqual9 (boardObjects [i, j + 2])) {
						print ("checkL42 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i, j + 2]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// L do nguoc
	void checkL43 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 3 && j > 0)
						if (isEqual9 (boardObjects [i + 1, j]) 
						    && isEqual9 (boardObjects [i + 2, j])
						   && isEqual9 (boardObjects [i, j - 1])) {
						print ("checkL43 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i + 2, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// L do trai
	void checkL44 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i > 0 && j > 1)
						if (isEqual9 (boardObjects [i, j - 1]) 
						    && isEqual9 (boardObjects [i, j - 2])
						   && isEqual9 (boardObjects [i - 1, j])) {
						print ("checkL44 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j - 2]);
						removingEatedCell (boardObjects [i - 1, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// L xuoi trai
	void checkL4R1 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i > 1 && j > 0)
						if (isEqual9 (boardObjects [i, j - 1]) 
						    && isEqual9 (boardObjects [i - 1, j])
						   && isEqual9 (boardObjects [i - 2, j])) {
						print ("checkL4R1 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i - 2, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// L do phai trai
	void checkL4R2 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i > 0 && j < 3)
						if (isEqual9 (boardObjects [i - 1, j]) 
						    && isEqual9 (boardObjects [i, j + 1])
						   && isEqual9 (boardObjects [i, j + 2])) {
						print ("checkL4R2 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i, j + 2]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// L do nguoc
	void checkL4R3 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 3 && j < 4)
						if (isEqual9 (boardObjects [i + 1, j]) 
						    && isEqual9 (boardObjects [i + 2, j])
						   && isEqual9 (boardObjects [i, j + 1])) {
						print ("checkL4R3 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						removingEatedCell (boardObjects [i + 2, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// L do trai
	void checkL4R4 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 4 && j > 1)
						if (isEqual9 (boardObjects [i, j - 1]) 
						    && isEqual9 (boardObjects [i, j - 2])
						   && isEqual9 (boardObjects [i + 1, j])) {
						print ("checkL4R4 COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j - 2]);
						removingEatedCell (boardObjects [i + 1, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// hinh vuong
	void checkSquare4 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i < 4 && j < 4)
						if (isEqual9 (boardObjects [i, j + 1]) 
						    && isEqual9 (boardObjects [i + 1, j + 1])
						   && isEqual9 (boardObjects [i + 1, j])) {
						print ("checksquare COMPLETE, START REMOVING");
						addScore (9 * 4);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j + 1]);
						removingEatedCell (boardObjects [i + 1, j + 1]);
						removingEatedCell (boardObjects [i + 1, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
						
					}
					
				}
			}
		}    
		
	}
	// 3 doc
	void check31 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (i > 0 && i < 4)
					if (isEqual9 (boardObjects [i - 1, j]) && isEqual9 (boardObjects [i + 1, j])) {
						print ("3 1 COMPLETE, START REMOVING");
						addScore (9 * 3);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i - 1, j]);
						removingEatedCell (boardObjects [i + 1, j]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
	}
	// 3 ngang
	void check32 ()
	{
		for (int i=0; i<5; i++) {
			for (int j=0; j<5; j++) {
				if (boardObjects [i, j] != null)
				if (isEqual9 (boardObjects [i, j])) {
					if (j > 0 && j < 4)
					if (isEqual9 (boardObjects [i, j - 1]) && isEqual9 (boardObjects [i, j + 1])) {
						print ("3 2 COMPLETE, START REMOVING");
						addScore (9 * 3);
						removingEatedCell (boardObjects [i, j]);
						removingEatedCell (boardObjects [i, j - 1]);
						removingEatedCell (boardObjects [i, j + 1]);
						gameObject.GetComponents<AudioSource> () [4].Play ();
					}
					
				}
			}
		}
	}
	
	public void checkSoundState(){
		soundOn = PlayerPrefs.GetInt ("soundOn",1);
		if (soundOn == 0) {
			AudioListener.volume = 0;
			dialogPause.transform.Find("buttonSound").gameObject.GetComponent<UISprite>().spriteName="sound p2";
			dialogPause.transform.Find("buttonSound").gameObject.GetComponent<UIButton>().normalSprite="sound p2";
		} else {
			AudioListener.volume = 1;
			dialogPause.transform.Find("buttonSound").gameObject.GetComponent<UISprite>().spriteName="sound p1";
			dialogPause.transform.Find("buttonSound").gameObject.GetComponent<UIButton>().normalSprite="sound p1";
		}
		
	}
	public void openHelp(){
		isPause=true;
		dialogHelp.SetActive(true);
	}
	public void closeHelp(){
		isPause=false;
		dialogHelp.SetActive(false);
	}
	public void changeSoundState(){
		soundOn = PlayerPrefs.GetInt ("soundOn",1);
		if (soundOn == 1) {
			AudioListener.volume = 0;
			PlayerPrefs.SetInt("soundOn",0);
			dialogPause.transform.Find("buttonSound").gameObject.GetComponent<UISprite>().spriteName="sound p2";
			dialogPause.transform.Find("buttonSound").gameObject.GetComponent<UIButton>().normalSprite="sound p2";
		} else {
			AudioListener.volume = 1;
			PlayerPrefs.SetInt("soundOn",1);
			dialogPause.transform.Find("buttonSound").gameObject.GetComponent<UISprite>().spriteName="sound p1";
			dialogPause.transform.Find("buttonSound").gameObject.GetComponent<UIButton>().normalSprite="sound p1";
		}
		
	}
}


