using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class Player : MonoBehaviour 
{
	public PlayerMovimentManager movimentManager;
	public CustomCollider playerCollider;
	//Move Tween Info
	public GameObject playerGO;
	public bool isMoving = false;
	public float moveTweenCount = 0f;
	//Arrow Rot Tween Info
	public GameObject arrowGO;
	public bool isRotating = false;
	public float rotTweenCount = 0f;
	public Quaternion arrowStartRotation;
	public Quaternion arrowEndRotation;

	public bool idleBuffer = false;

	public PlayerDirection	playerDirection = PlayerDirection.UP;
	public enum PlayerDirection
	{
		UP,
		LEFT,
		DOWN,
		RIGHT
	}
	public PlayerStatus status = PlayerStatus.MOVING;
	public enum PlayerStatus
	{
		MOVING,
		IDLE,
		ROTATING,
		ROTATING_BUFFER
	}
	public NextTile currentTile = NextTile.GROUND;
	public NextTile nextTile = NextTile.GROUND;
	public enum NextTile
	{
		NOTHING,
		GROUND,
		STAIR_DOWN,
		STAIR_UP,
		WALL
	}
	void Start()
	{
		playerCollider.onCollisionEnter += (CustomCollider arg1, Collision arg2) => 
		{
			if (arg2.gameObject.name.StartsWith ("Enemy"))
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		};
	}
	void Update () 
	{
		if (Input.GetMouseButtonDown (1))
			RightButtonPressed ();
		else if (Input.GetMouseButton (1))
			RightButtonDown ();
		else if (!Input.GetMouseButton (1) && status == PlayerStatus.IDLE)
			idleBuffer = false;
		if (isMoving) 
			UpdatePlayerPosition ();
		else if (isRotating) 
			UpdateArrowRotation ();
	}
	public void RightButtonDown()
	{
		if (status == PlayerStatus.MOVING || status == PlayerStatus.IDLE) 
			idleBuffer = true;
	}
	public void RightButtonPressed()
	{
		if (status == PlayerStatus.MOVING) 
		{
			idleBuffer = true;
			return;
		} 
		else if (status == PlayerStatus.IDLE) 
		{
			rotTweenCount = 0f;
			IncreaseArrowRotation ();
		}
		else if (status == PlayerStatus.ROTATING && rotTweenCount >= 0.75f) 
		{
			IncreaseArrowRotation ();
		}
	}
	private void IncreaseArrowRotation()
	{
		if (status == PlayerStatus.IDLE)
			status = PlayerStatus.ROTATING;
		else if (status == PlayerStatus.ROTATING)
			status = PlayerStatus.ROTATING_BUFFER;
		
		idleBuffer = true;
		playerDirection += 1;
		if ((int)playerDirection == 4)
			playerDirection -= 4;
	}
	private void UpdatePlayerPosition()
	{
		moveTweenCount += Time.deltaTime * GameSceneManager.gameSpeed;
		movimentManager.UpdatePlayerPosition (moveTweenCount);
		if (moveTweenCount >= 1f) 
			isMoving = false;
	}
	private void UpdateArrowRotation()
	{
		rotTweenCount += Time.deltaTime * GameSceneManager.gameSpeed;
		arrowGO.transform.localRotation = Quaternion.Lerp (arrowStartRotation, arrowEndRotation, rotTweenCount);
		if (rotTweenCount >= 1f) 
		{
			isRotating = false;
			if (status == PlayerStatus.ROTATING)
				status = PlayerStatus.IDLE;
			else if (status == PlayerStatus.ROTATING_BUFFER) 
			{
				rotTweenCount = 0f;
				status = PlayerStatus.ROTATING;
			}
		}
	}
	private void SetPlayerArrow()
	{
		isRotating = true;
		rotTweenCount = 0f;
		arrowStartRotation = arrowGO.transform.localRotation;
		arrowEndRotation = Quaternion.Euler(new Vector3(0f, (int)playerDirection * -90f - 180f,0f));
	}
	public void SetPlayerDestination()
	{
		Vector3 playerTargetPosition = playerGO.transform.localPosition + PlayerDirNormalized ();
		CheckNextTile();
		movimentManager.positions = new List<MovimentPosition> ();
		if (nextTile != NextTile.NOTHING) 
		{
			isMoving = true;
			moveTweenCount = 0f;
			movimentManager.positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
			if (nextTile == NextTile.GROUND) 
			{
				if (currentTile == NextTile.GROUND) 
					movimentManager.positions.Add (new MovimentPosition (playerTargetPosition,1f));
				else if (currentTile == NextTile.STAIR_UP) 
				{
					movimentManager.positions.Add (new MovimentPosition (playerGO.transform.localPosition 
						+ Vector3.up * 0.5f + PlayerDirNormalized() * 0.5f,0.5f));
					movimentManager.positions.Add (new MovimentPosition (playerTargetPosition + Vector3.up * 0.5f,1f));
				}
				else if (currentTile == NextTile.STAIR_DOWN) 
				{
					movimentManager.positions.Add (new MovimentPosition (playerGO.transform.localPosition 
						+ Vector3.down * 0.5f + PlayerDirNormalized() * 0.5f,0.5f));
					movimentManager.positions.Add (new MovimentPosition (playerTargetPosition + Vector3.down * 0.5f,1f));
				}
			} 
			else if (nextTile == NextTile.STAIR_DOWN) 
			{
				if (currentTile == NextTile.GROUND)
				{
					movimentManager.positions.Add (new MovimentPosition (playerGO.transform.localPosition + PlayerDirNormalized () * 0.5f,0.5f));
					movimentManager.positions.Add (new MovimentPosition (playerTargetPosition + Vector3.down * 0.5f,1f));
				}
				else if (currentTile == NextTile.STAIR_DOWN)
					movimentManager.positions.Add (new MovimentPosition (playerTargetPosition + Vector3.down,1f));

			}
			else if (nextTile == NextTile.STAIR_UP)
			{
				if (currentTile == NextTile.GROUND)
				{
					movimentManager.positions.Add (new MovimentPosition (playerGO.transform.localPosition + PlayerDirNormalized () * 0.5f,0.5f));
					movimentManager.positions.Add (new MovimentPosition (playerTargetPosition + Vector3.up * 0.5f,1f));
				}
				else if (currentTile == NextTile.STAIR_UP)
					movimentManager.positions.Add (new MovimentPosition (playerTargetPosition + Vector3.up,1f));

			}
		}
	}
	public Vector3 PlayerDirNormalized()
	{
		return new Vector3 (Mathf.Sin ((int)playerDirection * -90f * Mathf.Deg2Rad),
			0f, Mathf.Cos ((int)playerDirection * 90f * Mathf.Deg2Rad));
	}
	public void CheckNextTile()
	{
		currentTile = nextTile;
		Vector3 playerTargetPosition = playerGO.transform.localPosition + PlayerDirNormalized();
		Collider[] __collisions;

		if (currentTile == NextTile.STAIR_DOWN)
		{
			__collisions = Physics.OverlapSphere (playerTargetPosition - (Vector3.up * 1.5f), 0.2f);
			if (__collisions.Length > 0 && __collisions [0].name.StartsWith ("P_Ladder")) 
			{
				nextTile = NextTile.STAIR_DOWN;
				return;
			}
		}
		if (currentTile == NextTile.STAIR_UP)
		{
			__collisions = Physics.OverlapSphere (playerTargetPosition + (Vector3.up * 1.5f), 0.2f);
			if (__collisions.Length > 0 && __collisions [0].name.StartsWith ("P_Ladder")) 
			{
				Debug.Log ("here");
				nextTile = NextTile.STAIR_UP;
				return;
			}

		}
		__collisions = Physics.OverlapSphere (playerTargetPosition + (Vector3.up * 0.5f), 0.2f);
		if (__collisions.Length > 0) 
		{
			if (__collisions [0].name.StartsWith ("P_Ladder")) 
			{
				nextTile = NextTile.STAIR_UP;
				return;
			}
			else if (__collisions [0].name.StartsWith ("P_Small") || __collisions [0].name.StartsWith ("P_Big")
				|| __collisions [0].name.StartsWith ("Enemy")) 
			{
				nextTile = NextTile.GROUND;
				return;
			}
			else
			{
				nextTile = NextTile.WALL;
				return;
			}
		}
		__collisions = Physics.OverlapSphere (playerTargetPosition - (Vector3.up * 0.5f), 0.2f);
		if (__collisions.Length > 0) 
		{
			if (__collisions [0].name.StartsWith ("P_Small") 
				|| __collisions [0].name.StartsWith ("P_Big")) 
			{
				nextTile = NextTile.GROUND;
				return;
			}
			else if (__collisions [0].name.StartsWith ("P_Ladder")) 
			{
				nextTile = NextTile.STAIR_DOWN;
				return;
			}
			else
			{
				nextTile = NextTile.WALL;
				return;
			}
		}
		else
			nextTile = NextTile.NOTHING;
			
	}
	public void PlayTurn()
	{
		
		if (status == PlayerStatus.MOVING)
		{
			if (idleBuffer) 
			{
				status = PlayerStatus.IDLE;
				idleBuffer = false;
			}
			else
				SetPlayerDestination ();
		}
		else if (status == PlayerStatus.IDLE)
		{
			if (idleBuffer)
				idleBuffer = false;
			else 
			{
				status = PlayerStatus.MOVING;
				SetPlayerDestination ();
			}
		}
		else if (status == PlayerStatus.ROTATING)
			SetPlayerArrow ();
		else if (status == PlayerStatus.ROTATING_BUFFER)
			SetPlayerArrow ();
	}
}
