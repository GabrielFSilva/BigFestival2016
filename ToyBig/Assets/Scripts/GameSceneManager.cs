using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSceneManager : MonoBehaviour 
{
	public static float gameSpeed;
	public float gameSpeedLocal;

	public Player player;
	public List<Enemy> enemies;

	public float turnCount = 0f;

	// Use this for initialization
	void Start () 
	{
		gameSpeed = gameSpeedLocal;
	}
	
	// Update is called once per frame
	void Update () 
	{
		turnCount += Time.deltaTime * gameSpeed;
		if (turnCount >= 1f) 
		{
			PlayTurns ();
			turnCount = 0f;
		}
	}
	public void PlayTurns()
	{
		foreach (Enemy __enemy in enemies) 
			__enemy.PlayTurn();
		player.PlayTurn ();
	}
}
