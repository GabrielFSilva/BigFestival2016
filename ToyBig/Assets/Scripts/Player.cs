using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class Player : MonoBehaviour 
{
	public PlayerMovimentManager movManager;
	public CustomCollider playerCollider;


	//Move Tween Info
	public GameObject playerGO;
	public bool isMoving = false;
	public float moveTweenCount = 0f;
	public Elevator elevatorGO;
	public MovingPlatform platformGO;
	//Arrow Rot Tween Info
	public GameObject arrowGO;
	public bool isRotating = false;
	public float rotTweenCount = 0f;
	public Quaternion arrowStartRotation;
	public Quaternion arrowEndRotation;

	public bool actionBuffer = false;
	public bool idleBuffer = false;
	public bool jumpBuffer = false;

	public bool rightButtonPressed = false;
	//Items Info
	public GameObject playerHand;

	public PlayerDirection	playerDirection = PlayerDirection.UP;
	public enum PlayerDirection
	{
		UP,
		LEFT,
		DOWN,
		RIGHT
	}
	public PlayerStatus status = PlayerStatus.WALKING;
	public enum PlayerStatus
	{
		WALKING,
		JUMPING,
		IDLE,
		IDLE_BUFFER,
		ACTION,
		TAKING_ELEVATOR,
		TAKING_PLATFORM,
		FALLING
	}
	public ActionStatus actionStatus = ActionStatus.NOTHING;
	public enum ActionStatus
	{
		NOTHING,
		CLIMBING_UP_LADDER,
		THROWING_SWORD,
		PLACING_DYNAMITE
	}

	public ItemStatus itemStatus = ItemStatus.NOTHING;
	public enum ItemStatus
	{
		NOTHING,
		SWORD,
		DYNAMITE
	}
	void Start()
	{
		arrowGO.transform.localRotation = Quaternion.Euler(new Vector3(0f, (int)playerDirection * -90f - 180f,0f));
		playerCollider.onCollisionEnter += (CustomCollider arg1, Collision arg2) => 
		{
			if (arg2.gameObject.tag ==  "Enemy")
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			
			if (status == PlayerStatus.FALLING)
				CheckLanding(arg2);
			
			if (arg2.gameObject.tag == "Sword")
			{
				if (playerHand.transform.childCount == 0)
				{
					arg2.gameObject.GetComponent<Sword>().PlaceSwordOnPlayerHand(playerHand.transform);
					itemStatus = ItemStatus.SWORD;
					jumpBuffer = false;
					actionBuffer = false;
				}
			}
			else if (arg2.gameObject.tag == "Dynamite")
			{
				if (playerHand.transform.childCount == 0)
				{
					arg2.gameObject.GetComponent<Dynamite>().PlaceynamiteOnPlayerHand(playerHand.transform);
					itemStatus = ItemStatus.DYNAMITE;
					jumpBuffer = false;
					actionBuffer = false;
				}
			}
		};
	}
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Q))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		if (playerGO.transform.position.y <= -10f)
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		if (rightButtonPressed)
			RightButtonDown ();
		
		/*if (Input.GetMouseButtonDown (0))
			LeftButtonPressed ();
		if (Input.GetMouseButtonDown (1))
			RightButtonPressed ();
		if (Input.GetMouseButton (1))
			RightButtonDown();*/
		
		if (status == PlayerStatus.TAKING_ELEVATOR) 
		{
			playerGO.transform.position = elevatorGO.transform.position + (Vector3.up * 2f);
			if (elevatorGO.isMoving == false) 
			{
				status = PlayerStatus.IDLE_BUFFER;
				jumpBuffer = false;
				actionBuffer = false;
			}
		}
		else if (status == PlayerStatus.TAKING_PLATFORM)
			playerGO.transform.position = platformGO.transform.position;
		else if (status == PlayerStatus.FALLING)
			playerGO.transform.position += Vector3.up * -4f * Time.deltaTime * GameSceneManager.gameSpeed;
		else if (isMoving) 
			UpdatePlayerPosition ();
		if (isRotating) 
			UpdateArrowRotation ();
	}
	private void CheckLanding(Collision p_coll)
	{
		if (p_coll.gameObject.tag == "Stair") 
		{
			playerGO.transform.position = p_coll.transform.position + (Vector3.up * 0.5f);
			status = PlayerStatus.IDLE;
			idleBuffer = false;
			jumpBuffer = false;
			movManager.nextTile = PlayerMovimentManager.NextTile.STAIR_DOWN;
		}
		else if (p_coll.gameObject.tag == "Elevator")
		{
			playerGO.transform.position = p_coll.transform.position + (Vector3.one * 2.0f);
			status = PlayerStatus.IDLE_BUFFER;
			idleBuffer = true;
			jumpBuffer = false;
			movManager.nextTile = PlayerMovimentManager.NextTile.GROUND;
		}
		else
		{
			playerGO.transform.position = p_coll.transform.position;
			status = PlayerStatus.IDLE_BUFFER;
			idleBuffer = true;
			jumpBuffer = false;
			movManager.nextTile = PlayerMovimentManager.NextTile.GROUND;
		}
		CheckElevatorButtons ();
	}
	public void RightButtonDown()
	{
		idleBuffer = true;
	}
	public void DirectionButtonDown()
	{
		rightButtonPressed = true;
		RightButtonPressed ();
	}
	public void DirectionButtonUp()
	{
		rightButtonPressed = false;
	}
	public void ActionButtonDown()
	{
		LeftButtonPressed ();
	}
	public void LeftButtonPressed()
	{
		if (actionStatus != ActionStatus.NOTHING && actionBuffer)
			return;
		if (status == PlayerStatus.TAKING_ELEVATOR)
			return;
		Vector3 __targetPos = playerGO.transform.localPosition + PlayerDirNormalized(); 
		Collider[] __collisions;
		__collisions = Physics.OverlapSphere (playerGO.transform.localPosition + (PlayerDirNormalized() * 0.5f)
			+ (Vector3.up * 0.5f), 0.1f);
		if (__collisions.Length > 0) 
		{
			if (PlayerMovimentManager.HasColliderWithTag (__collisions, "Ladder")) 
			{
				actionBuffer = true;
				actionStatus = ActionStatus.CLIMBING_UP_LADDER;
				return;
			}
		}
		if (itemStatus != ItemStatus.NOTHING 
			&& !PlayerMovimentManager.HasWallInPosition (__targetPos + (Vector3.up * 0.5f), 0.3f)
			&& !PlayerMovimentManager.HasStairInPosition (__targetPos + (Vector3.up * 0.5f), 0.3f))
		{
			if (itemStatus == ItemStatus.SWORD) 
				actionStatus = ActionStatus.THROWING_SWORD;
			else if (itemStatus == ItemStatus.DYNAMITE) 
				actionStatus = ActionStatus.PLACING_DYNAMITE;
			
			actionBuffer = true;
			return;
		}
		else if (itemStatus == ItemStatus.NOTHING)
		{
			if (PlayerMovimentManager.HasWallInPosition (playerGO.transform.localPosition + (Vector3.up * 1.5f), 0.2f))
				Debug.Log ("Blocked Up");
			else if (PlayerMovimentManager.HasStairInPosition (__targetPos + (Vector3.up * 0.5f), 0.3f))
				Debug.Log ("Blocked Stair");
			else if (PlayerMovimentManager.HasStairInPosition (playerGO.transform.localPosition, 0.3f))
				Debug.Log ("Blocked Stair Beneath");
			else
				jumpBuffer = true;
		}
	}

	public void RightButtonPressed()
	{
		if (status == PlayerStatus.WALKING)
			idleBuffer = true;
		else if (status == PlayerStatus.IDLE_BUFFER || status == PlayerStatus.TAKING_ELEVATOR
			|| status == PlayerStatus.TAKING_PLATFORM) 
		{
			if (!isRotating) 
			{
				idleBuffer = true;
				rotTweenCount = 0f;
				IncreaseArrowRotation ();
			}
		}
	}
	private void IncreaseArrowRotation()
	{
		isRotating = true;
		playerDirection += 1;
		if ((int)playerDirection == 4)
			playerDirection -= 4;
		SetPlayerArrow ();
	}
	private void UpdatePlayerPosition()
	{
		moveTweenCount += Time.deltaTime * GameSceneManager.gameSpeed;
		if (status == PlayerStatus.JUMPING)
			movManager.UpdateJumpPosition(moveTweenCount);
		else if (status == PlayerStatus.WALKING)
			movManager.UpdatePlayerPosition (moveTweenCount);
		else if (status == PlayerStatus.ACTION) 
		{
			if (actionStatus == ActionStatus.CLIMBING_UP_LADDER)
				movManager.UpdatePlayerPosition(moveTweenCount);
		}
		if (moveTweenCount >= 1f) 
		{
			isMoving = false;
			if (status == PlayerStatus.JUMPING) 
			{
				CheckElevatorButtons ();
				CheckMovingPlatform ();
				if (movManager.currentJumpType != PlayerMovimentManager.JumpType.LAND_ON_STAIR)
					if (!PlayerMovimentManager.HasWallInPosition (playerGO.transform.localPosition, 0.05f)
						&& status != PlayerStatus.TAKING_PLATFORM)
						status = PlayerStatus.FALLING;
			}
		}
	}
	private void CheckMovingPlatform()
	{
		Collider[] __collisions;
		__collisions = Physics.OverlapSphere (playerGO.transform.localPosition, 0.1f);

		foreach (Collider __coll in __collisions)
			if (__coll.tag == "MovingPlatform") 
			{
				Debug.Log ("HEre");
				platformGO = __coll.GetComponent<MovingPlatform>();
				status = PlayerStatus.TAKING_PLATFORM;
			}
	}
	private void CheckElevatorButtons()
	{
		Collider[] __collisions;
		__collisions = Physics.OverlapSphere (playerGO.transform.localPosition, 0.1f);

		foreach (Collider __coll in __collisions)
			if (__coll.tag == "ElevatorButton")
				__coll.GetComponent<ElevatorButton> ().SetElevatorMoving ();
		foreach (Collider __coll in __collisions)
			if (__coll.tag == "Elevator") 
			{
				elevatorGO = __coll.GetComponent<Elevator>();
				status = PlayerStatus.TAKING_ELEVATOR;
			}
	}
	private void UpdateArrowRotation()
	{
		rotTweenCount += Time.deltaTime * GameSceneManager.gameSpeed * 3f;
		arrowGO.transform.localRotation = Quaternion.Lerp (arrowStartRotation, arrowEndRotation, rotTweenCount);
		if (rotTweenCount >= 1f)
			isRotating = false;
	}
	private void SetPlayerArrow()
	{
		rotTweenCount = 0f;
		arrowStartRotation = arrowGO.transform.localRotation;
		arrowEndRotation = Quaternion.Euler(new Vector3(0f, (int)playerDirection * -90f - 180f,0f));
	}
	private void SetPlayerJump()
	{
		isMoving = true;
		moveTweenCount = 0f;
		movManager.CalcPlayerJumpPath (PlayerDirNormalized ());
	}
	public void SetPlayerDestination()
	{
		movManager.CalcPlayerPath (PlayerDirNormalized());
		if (movManager.currentTile == PlayerMovimentManager.NextTile.NOTHING) 
			status = PlayerStatus.FALLING;
		else
		{
			isMoving = true;
			moveTweenCount = 0f;
		}
	}
	public Vector3 PlayerDirNormalized()
	{
		return new Vector3 (Mathf.Sin ((int)playerDirection * -90f * Mathf.Deg2Rad),
			0f, Mathf.Cos ((int)playerDirection * 90f * Mathf.Deg2Rad));
	}

	public void PlayTurn()
	{
		if (status == PlayerStatus.TAKING_ELEVATOR)
			return;
		if (status == PlayerStatus.FALLING)
			return;
		if (actionBuffer || status == PlayerStatus.ACTION) 
		{
			status = PlayerStatus.ACTION;
			SetAction ();
			return;
		}
		if (jumpBuffer) 
		{
			status = PlayerStatus.JUMPING;
			idleBuffer = false;
			jumpBuffer = false;
			SetPlayerJump();
			return;
		}
		if (status == PlayerStatus.TAKING_PLATFORM)
			return;
		if (idleBuffer) 
		{
			status = PlayerStatus.IDLE_BUFFER;
			idleBuffer = false;
			if (!PlayerMovimentManager.HasWallInPosition (playerGO.transform.localPosition, 0.05f)
				&& !PlayerMovimentManager.HasStairInPosition (playerGO.transform.localPosition, 0.05f))
				status = PlayerStatus.FALLING;
			
			return;
		}

		if (status == PlayerStatus.WALKING)
		{
			SetPlayerDestination ();
		}
		else if (status == PlayerStatus.JUMPING)
		{
			status = PlayerStatus.IDLE;
			idleBuffer = false;
			jumpBuffer = false;
		}	
		else if (status == PlayerStatus.IDLE)
		{
			status = PlayerStatus.WALKING;
			SetPlayerDestination ();
		}
		else if (status == PlayerStatus.IDLE_BUFFER)
		{
			status = PlayerStatus.IDLE;
		}
	}
	public void SetAction()
	{
		if (!actionBuffer) 
		{
			if (actionStatus == ActionStatus.CLIMBING_UP_LADDER) 
			{
				status = PlayerStatus.WALKING;
				idleBuffer = false;
			}
			else if (actionStatus == ActionStatus.THROWING_SWORD) 
			{
				status = PlayerStatus.IDLE;
				itemStatus = ItemStatus.NOTHING;
			}
			else if (actionStatus == ActionStatus.PLACING_DYNAMITE) 
			{
				status = PlayerStatus.IDLE;
				if (playerHand.transform.childCount == 0) 
					itemStatus = ItemStatus.NOTHING;
			}
			actionStatus = ActionStatus.NOTHING;
			return;
		}
		actionBuffer = false;
		if (actionStatus == ActionStatus.CLIMBING_UP_LADDER) 
		{
			isMoving = true;
			moveTweenCount = 0f;
			movManager.CalcPlayerClimbUpLadder (PlayerDirNormalized());
		}
		else if (actionStatus == ActionStatus.THROWING_SWORD) 
		{
			playerHand.transform.GetChild (0).GetComponent<Sword> ().ThrowSword (playerDirection);
			itemStatus = ItemStatus.NOTHING;
		}
		else if (actionStatus == ActionStatus.PLACING_DYNAMITE) 
		{
			if (PlayerMovimentManager.HasStairInPosition (playerGO.transform.localPosition, 0.1f))
				return;
			playerHand.transform.GetChild (0).GetComponent<Dynamite> ().
				PlaceDynamiteOnGround (playerGO.transform.localPosition + PlayerDirNormalized());
			if(playerHand.transform.childCount == 0)
				itemStatus = ItemStatus.NOTHING;
		}
	}
}
