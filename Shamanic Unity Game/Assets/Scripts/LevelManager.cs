using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class LevelManager : MonoBehaviour {

	public enum LvlVal {
		Start, Finish,
		NBlock, Empty,
		RBlock, RPotion,
		BBlock, BPotion,
		GBlock, GPotion,
		Nothing
	};

	public enum GameColors {
		Red, Green, Blue, Neutral
	}
	
	public GameObject Player;
	public GameObject End;

	public GameObject BlockPrefab;
	public GameObject PotionPrefab;

	//private LvlVal[][] SingleBlocklevel = new LvlVal[][]{ new LvlVal[] {LvlVal.Start} }; // single block

	/*private LvlVal[][] Testlevel = new LvlVal[][]{
		new LvlVal[] {LvlVal.Start, LvlVal.Empty, LvlVal.NBlock, LvlVal.NBlock, LvlVal.NBlock},
		new LvlVal[] {LvlVal.RBlock, LvlVal.NBlock, LvlVal.NBlock, LvlVal.NBlock},
		new LvlVal[] {LvlVal.NBlock, LvlVal.NBlock, LvlVal.NBlock, LvlVal.NBlock, LvlVal.NBlock},
		new LvlVal[] {LvlVal.NBlock, LvlVal.NBlock, LvlVal.NBlock, LvlVal.NBlock, LvlVal.NBlock}
	};*/

	private LvlVal[][] Testlevel = new LvlVal[][]{
		new LvlVal[] {LvlVal.Empty,LvlVal.RBlock,LvlVal.Empty,LvlVal.GBlock,LvlVal.Empty,LvlVal.BBlock,LvlVal.Empty},
		new LvlVal[] {LvlVal.Empty,LvlVal.RBlock,LvlVal.Empty,LvlVal.GBlock,LvlVal.Empty,LvlVal.BBlock,LvlVal.Empty},
		new LvlVal[] {LvlVal.Empty,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.Empty},
		new LvlVal[] {LvlVal.RPotion,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.Empty},
		new LvlVal[] {LvlVal.GPotion,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.Finish},
		new LvlVal[] {LvlVal.BPotion,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.Empty},
		new LvlVal[] {LvlVal.Empty,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.Empty},
		new LvlVal[] {LvlVal.Empty,LvlVal.Empty,LvlVal.Empty,LvlVal.Start,LvlVal.Empty,LvlVal.Empty,LvlVal.Empty}
	};

	private LvlVal[][] AnotherLever = new LvlVal[][]{
		new LvlVal[] {LvlVal.RPotion,LvlVal.NBlock,LvlVal.NBlock,LvlVal.Empty,LvlVal.Empty,LvlVal.Empty},
		new LvlVal[] {LvlVal.NBlock,LvlVal.Empty,LvlVal.NBlock,LvlVal.Empty,LvlVal.Empty,LvlVal.Empty},
		new LvlVal[] {LvlVal.NBlock,LvlVal.NBlock,LvlVal.NBlock,LvlVal.RBlock,LvlVal.NBlock,LvlVal.BPotion},
		new LvlVal[] {LvlVal.Empty,LvlVal.Empty,LvlVal.GBlock,LvlVal.Empty,LvlVal.BBlock,LvlVal.Empty},
		new LvlVal[] {LvlVal.Empty,LvlVal.Empty,LvlVal.GBlock,LvlVal.Empty,LvlVal.BBlock,LvlVal.Empty},
		new LvlVal[] {LvlVal.Empty,LvlVal.GPotion,LvlVal.NBlock,LvlVal.BBlock,LvlVal.NBlock,LvlVal.Finish},
		new LvlVal[] {LvlVal.Empty,LvlVal.Empty,LvlVal.Start,LvlVal.Empty,LvlVal.Empty,LvlVal.Empty}
	};

	private LvlVal[][] level;
	private GameObject[][] objLevel;
	private Vector3 correctBlockPosition;

	// Use this for initialization
	void Start() {
		level = AnotherLever;

		correctBlockPosition = new Vector3(-1.0f,0.0f,1.0f);
		correctBlockPosition += new Vector3(level.Max(element => element.Length), 0.0f, -level.Length);
		correctBlockPosition /= 2;

		CreateLevel();
	}

	void CreateLevel() {
		StartLevel();
		RemoveWalls();
		ColorWalls();
		CreatePotions();
		PlacePlayer();
		PlaceEnd();
	}


	void StartLevel() {
		objLevel = new GameObject[level.Length][];

		for(int i = 0; i < level.Length; i++) {
			objLevel[i] = new GameObject[level[i].Length];

			for(int j = 0; j < level[i].Length; j++) {
				if(level[i][j] != LvlVal.Empty) {
					Vector3 blockPos = new Vector3(j,0.0f,-i);
					blockPos -= correctBlockPosition;
					blockPos *= 5;

					GameObject newBlock = (GameObject)GameObject.Instantiate(BlockPrefab);
					newBlock.transform.parent = this.transform;
					newBlock.transform.position = blockPos;

					objLevel[i][j] = newBlock;
				}
			}
		}
	}

	bool IsInside(Vector3 position, Vector3 blockPos) {
		if(position.x > (blockPos.x + 2.5f) || position.x < (blockPos.x - 2.5f)) {
			return false;
		}
		if(position.z > (blockPos.z + 2.5f) || position.z < (blockPos.z - 2.5f)) {
			return false;
		}
		return true;
	}

	void RemoveWalls() {
		for(int i = 0; i < objLevel.Length; i++) {
			for(int j = 0; j < objLevel[i].Length; j++) {
				if(objLevel[i][j]) {
					Block block = (Block)objLevel[i][j].GetComponent(typeof(Block));
					block.ChangeWall(Block.Side.Up, !DoesExist(i-1,j));
					block.ChangeWall(Block.Side.Down, !DoesExist(i+1,j));
					block.ChangeWall(Block.Side.Right, !DoesExist(i,j+1));
					block.ChangeWall(Block.Side.Left, !DoesExist(i,j-1));
				}
			}
		}
	}

	void ColorWalls() {
		for(int i = 0; i < objLevel.Length; i++) {
			for(int j = 0; j < objLevel[i].Length; j++) {
				if(objLevel[i][j]) {
					Block block = (Block)objLevel[i][j].GetComponent(typeof(Block));
					switch(level[i][j]) {
					case LvlVal.RBlock:
						if(Game.numberOfColors >= 3) {
							block.FillColor(GameColors.Red);
						}
						break;
					case LvlVal.GBlock:
						if(Game.numberOfColors >= 2) {
							block.FillColor(GameColors.Green);
						}
						break;
					case LvlVal.BBlock:
						if(Game.numberOfColors >= 1) {
							block.FillColor(GameColors.Blue);
						}
						break;
					default:
						block.FillColor(GameColors.Neutral);
						break;
					}
				}
			}
		}
	}

	void CreatePotions()	{
		for(int i = 0; i < level.Length; i++) {
			for(int j = 0; j < level[i].Length; j++) {
				if(objLevel[i][j]) {
					Vector3 pos = objLevel[i][j].transform.position;
					pos.y = Player.transform.position.y;
					switch(level[i][j]) {
					case LvlVal.RPotion:
						if(Game.numberOfColors >= 3) {
							NewPotion(pos, GameColors.Red);
						}
						break;
					case LvlVal.GPotion:
						if(Game.numberOfColors >= 2) {
							NewPotion(pos, GameColors.Green);
						}
						break;
					case LvlVal.BPotion:
						if(Game.numberOfColors >= 1) {
							NewPotion(pos, GameColors.Blue);
						}
						break;
					default:
						break;
					}
				}
			}
		}
	}


	void NewPotion(Vector3 pos, GameColors color) {
		GameObject newPotion = (GameObject)GameObject.Instantiate(PotionPrefab);
		newPotion.transform.parent = this.transform;
		pos.y = newPotion.transform.position.y;
		newPotion.transform.position = pos;
		Potion potion = (Potion)newPotion.GetComponent(typeof(Potion));
		potion.color = color;
		potion.FillColor(color);
	}


	void PlacePlayer() {
		bool startFlag = false;
		for(int i = 0; i < level.Length; i++) {
			for(int j = 0; j < level[i].Length; j++) {
				if(level[i][j] == LvlVal.Start) {
					if(startFlag) { throw new NotImplementedException(); }
					Vector3 pos = objLevel[i][j].transform.position;
					pos.y = Player.transform.position.y;
					Player.transform.position = pos;
					startFlag = true;
				}
			}
		}
		if(!startFlag) { throw new NotImplementedException(); }
	}

	void PlaceEnd() {
		bool endFlag = false;
		for(int i = 0; i < level.Length; i++) {
			for(int j = 0; j < level[i].Length; j++) {
				if(level[i][j] == LvlVal.Finish) {
					if(endFlag) { throw new NotImplementedException(); }
					Vector3 pos = objLevel[i][j].transform.position;
					pos.y = End.transform.position.y;
					End.transform.position = pos;
					endFlag = true;
				}
			}
		}
		if(!endFlag) { throw new NotImplementedException(); }
	}

	// Update is called once per frame
	void Update () {

	}

	public LvlVal CheckPosition(Vector3 playerPos) {
		for(int i = 0; i < objLevel.Length; i++) {
			for(int j = 0; j < objLevel[i].Length; j++) {
				if(objLevel[i][j]) {
					Vector3 blockPos = objLevel[i][j].transform.position;
					if(IsInside (playerPos, blockPos)) {
						return level[i][j];
					}
				}
			}
		}
		return LvlVal.Nothing;
	}

	bool DoesExist(int i, int j) {
		if(i < 0 || i >= objLevel.Length) {	return false; }
		if(j < 0 || j >= objLevel[i].Length) { return false; }
		if(objLevel[i][j]) { return true; }
		return false;
	}

}
