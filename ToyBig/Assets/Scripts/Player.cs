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

	public bool actionBuffer = false;
	public bool idleBuffer = false;
	public bool jumpBuffer = false;

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
			if (arg2.gameObject.name.StartsWith ("Enemy"))
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			else if (arg2.gameObject.tag == "Sword")
			{
				if (playerHand.transform.childCount == 0)
				{
					arg2.gameObject.GetComponent<Sword>().SetSwordParent(playerHand.transform);
					itemStatus = ItemStatus.SWORD;
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
		
		if (Input.GetMouseButtonDown (1))
			RightButtonPressed ();
		else if (Input.GetMouseButtonDown (0))
			LeftButtonPressed ();
		else if (Input.GetMouseButton (1))
			RightButtonDown ();


		if (isMoving) 
			UpdatePlayerPosition ();
		else if (isRotating) 
			UpdateArrowRotation ();
	}
	public void RightButtonDown()
	{
		idleBuffer = true;
	}

	public void LeftButtonPressed()
	{
		if (actionStatus != ActionStatus.NOTHING && actionBuffer)
			return;

		if (itemStatus == ItemStatus.SWORD) 
		{
			actionBuffer = true;
			actionStatus = ActionStatus.THROWING_SWORD;
			return;
		}

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
		jumpBuffer = true;
	}
	public void RightButtonPressed()
	{
		if (status == PlayerStatus.WALKING) 
			idleBuffer = true;
		else if (status == PlayerStatus.IDLE_BUFFER) 
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
			movimentManager.UpdateJumpPosition(moveTweenCount);
		else if (status == PlayerStatus.WALKING)
			movimentManager.UpdatePlayerPosition (moveTweenCount);
		else if (status == PlayerStatus.ACTION) 
		{
			if (actionStatus == ActionStatus.CLIMBING_UP_LADDER)
				movimentManager.UpdatePlayerPosition(moveTweenCount);
		}
		if (moveTweenCount >= 1f) 
			isMoving = false;
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
		Vector3 playerTargetPosition = playerGO.transform.localPosition + PlayerDirNormalized ();
		movimentManager.positions = new List<MovimentPosition> ();
		movimentManager.positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
		movimentManager.positions.Add (new MovimentPosition (playerTargetPosition + Vector3.up,0f));
		movimentManager.positions.Add (new MovimentPosition (playerTargetPosition + PlayerDirNormalized (),0f));
	}
	public void SetPlayerDestination()
	{
		movimentManager.CalcPlayerPath (PlayerDirNormalized());
		if (movimentManager.nextTile == PlayerMovimentManager.NextTile.NOTHING) 
		{
			idleBuffer = true;
			status = PlayerStatus.IDLE_BUFFER;
		} 
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
		if (actionBuffer || status == PlayerStatus.ACTION) 
		{
			status = PlayerStatus.ACTION;
			SetAction ();
		}
		if (jumpBuffer) 
		{
			status = PlayerStatus.JUMPING;
			idleBuffer = false;
			jumpBuffer = false;
			SetPlayerJump();
			return;
		}
		if (idleBuffer) 
		{
			status = PlayerStatus.IDLE_BUFFER;
			idleBuffer = false;
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
				status = PlayerStatus.IDLE;
			else if (actionStatus == ActionStatus.THROWING_SWORD) 
			{
				status = PlayerStatus.IDLE;
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
			movimentManager.CalcPlayerClimbUpLadder (PlayerDirNormalized());
		}
		else if (actionStatus == ActionStatus.THROWING_SWORD) 
		{
			playerHand.transform.GetChild (0).GetComponent<Sword> ().ThrowSword (playerDirection);
			itemStatus = ItemStatus.NOTHING;
		}
	}
}
