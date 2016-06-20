using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSceneManager : MonoBehaviour 
{
	public static float gameSpeed;
	public float gameSpeedLocal;

	public Player player;
	public List<Enemy> enemies;
	public List<MovingPlatform> platforms;
	public List<Sword> swords;

	public GameObject swordsContainer;
	public float turnCount = 0f;

	void Start () 
	{
		gameSpeed = gameSpeedLocal;
		foreach (Sword __sword in swords) 
		{
			__sword.swordsContainer = swordsContainer;
			__sword.OnSwordHitEnemy += (Sword p_sword, Enemy p_enemy) => 
			{
				Debug.Log("Enemy Hit");
				enemies.Remove(p_enemy);
				Destroy(p_enemy.gameObject);
			};
			__sword.OnSwordLeaveWorld += (Sword p_sword) => 
			{
				Debug.Log("Leave World");
				swords.Remove(p_sword);
				Destroy(p_sword.gameObject);
			};
		}
	}

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
		foreach (MovingPlatform __plat in platforms) 
			__plat.PlayTurn();
		player.PlayTurn ();
	}
}
