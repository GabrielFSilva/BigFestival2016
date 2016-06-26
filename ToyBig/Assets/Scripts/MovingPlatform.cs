using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour 
{
	public GameObject platformGO;
	public Player.PlayerDirection direction;
	public bool isMoving = false;
	public float moveTweenCount = 0f;
	public Vector3 moveStartPosition;
	public Vector3 moveEndPosition;
	public bool skipTurn = false;
	void Update () 
	{
		if (isMoving) 
		{
			UpdatePlatformPosition ();
		}
	}
	public void PlayTurn()
	{
		
		Collider[] __collisions = Physics.OverlapSphere (platformGO.transform.localPosition 
			+ PlatformDirNormalized() - (Vector3.up * 0.5f), 0.2f);
		
		int __count = 0;
		foreach (Collider __coll in __collisions)
			if (__coll.name != "Player")
				__count++;
		if (__count > 0)
			InvertDirection ();
		
		if (skipTurn) 
		{
			skipTurn = false;
			return;
		}
		isMoving = true;
		SetPlatformDestination ();
	}
	private void SetPlatformDestination()
	{
		isMoving = true;
		moveTweenCount = 0f;
		moveStartPosition = platformGO.transform.localPosition;
		moveEndPosition = platformGO.transform.localPosition + PlatformDirNormalized();


	}
	private void InvertDirection()
	{
		direction += 2;
		if ((int)direction >= 4)
			direction -= 4;
		skipTurn = true;
	}
	public Vector3 PlatformDirNormalized()
	{
		return new Vector3 (Mathf.Sin ((int)direction * -90f * Mathf.Deg2Rad),
			0f, Mathf.Cos ((int)direction * 90f * Mathf.Deg2Rad));
	}
	private void UpdatePlatformPosition()
	{
		moveTweenCount += Time.deltaTime * GameSceneManager.gameSpeed;
		platformGO.transform.localPosition = Vector3.Lerp (moveStartPosition, moveEndPosition, moveTweenCount);
		if (moveTweenCount >= 1f)
			isMoving = false;
	}
}
