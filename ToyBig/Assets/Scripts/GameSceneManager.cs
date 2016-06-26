using UnityEngine;
using UnityEngine.SceneManagement;
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
	public List<Dynamite> dynamites;
	public List<Wall> walls;

	public GameObject swordsContainer;
	public GameObject dynamitesContainer;
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
		foreach (Dynamite __dyn in dynamites) 
		{
			__dyn.dynamitesContainer = dynamitesContainer;
			__dyn.OnDynamiteExplosionHitPlayer += (Dynamite obj) =>
				RestartLevel ();
			__dyn.OnDynamiteExplosionHitWall += (Dynamite arg1, Wall arg2) =>
			{
				walls.Remove(arg2);
				Destroy(arg2.gameObject);
			};
			__dyn.OnDynamiteExploded += (Dynamite obj) => 
			{
				dynamites.Remove(obj);
				Destroy(obj.gameObject);
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

	public void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
