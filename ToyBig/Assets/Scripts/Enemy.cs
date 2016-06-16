using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour 
{
	public GameObject enemyGO;
	public List<Player.PlayerDirection> moviments;
	public int movimentCount;
	public bool isMoving = false;
	public float moveTweenCount = 0f;
	public Vector3 moveStartPosition;
	public Vector3 moveEndPosition;

	void Update () 
	{
		if (isMoving) 
		{
			UpdateEnemyPosition ();
		}
	}
	public void PlayTurn()
	{
		isMoving = true;
		SetEnemyDestination ();
	}
	private void SetEnemyDestination()
	{
		isMoving = true;
		moveTweenCount = 0f;
		moveStartPosition = enemyGO.transform.localPosition;
		moveEndPosition = enemyGO.transform.localPosition + 
			new Vector3 (Mathf.Sin ((int)moviments[movimentCount] * -90f * Mathf.Deg2Rad),
			0f, Mathf.Cos ((int)moviments[movimentCount] * 90f * Mathf.Deg2Rad));
		
		movimentCount++;
		if (movimentCount == moviments.Count)
			movimentCount = 0;
	}
	private void UpdateEnemyPosition()
	{
		moveTweenCount += Time.deltaTime * GameSceneManager.gameSpeed;
		enemyGO.transform.localPosition = Vector3.Lerp (moveStartPosition, moveEndPosition, moveTweenCount);
		if (moveTweenCount >= 1f)
			isMoving = false;
	}
}
