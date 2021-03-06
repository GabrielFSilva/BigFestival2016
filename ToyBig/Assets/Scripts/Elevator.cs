﻿using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour 
{
	public GameObject elevatorGO;

	public Vector3 startPosition;
	public Vector3 endPosition;

	public float moveTimer;
	public float moveTimerCount = 0f;
	public bool isMoving = false;

	public bool isMovingFoward = false;
	void Start () 
	{
		if (elevatorGO == null)
			elevatorGO = gameObject;

		isMovingFoward = false;

		startPosition = elevatorGO.transform.localPosition;
		moveTimer = Vector3.Magnitude(endPosition - startPosition);
	}

	void Update () 
	{
		if (isMoving) 
		{
			moveTimerCount += Time.deltaTime * GameSceneManager.gameSpeed;
			if (isMovingFoward)
				elevatorGO.transform.localPosition = Vector3.Lerp (startPosition, 
					endPosition, moveTimerCount / moveTimer);
			else
				elevatorGO.transform.localPosition = Vector3.Lerp (endPosition, 
					startPosition, moveTimerCount / moveTimer);
			if (moveTimerCount >= moveTimer)
				isMoving = false;
		}
	}

	public void StartMoving()
	{
		isMovingFoward = !isMovingFoward;
		isMoving = true;
		moveTimerCount = 0f;
	}
}
