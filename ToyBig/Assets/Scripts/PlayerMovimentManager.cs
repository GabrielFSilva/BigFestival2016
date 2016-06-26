using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovimentManager : MonoBehaviour 
{
	public GameObject playerGO;
	public List<MovimentPosition> positions;

	public NextTile currentTile = NextTile.GROUND;
	public NextTile nextTile = NextTile.GROUND;
	public enum NextTile
	{
		NOTHING,
		GROUND,
		STAIR_DOWN,
		STAIR_UP,
		WALL,
		LADDER_DOWN
	}
	public JumpType currentJumpType = JumpType.VERTICAL_JUMP;
	public enum JumpType
	{
		VERTICAL_JUMP,
		HALF_JUMP,
		LAND_ON_STAIR,
		FULL_JUMP
	}
	void Start () 
	{
		positions = new List<MovimentPosition> ();
	}
	public void UpdatePlayerPosition(float p_turnTimer)
	{
		if (positions.Count == 2)
			playerGO.transform.localPosition = Vector3.Lerp (positions [0].position, positions [1].position, p_turnTimer);
		else 
			for (int i = positions.Count - 1; i > 0; i--) 
				if (positions [i-1].timer <= p_turnTimer) 
				{
					playerGO.transform.localPosition = Vector3.Lerp (positions [i-1].position, positions [i].position,
						(p_turnTimer - positions [i-1].timer) / (positions [i].timer - positions [i-1].timer));
					return;
				}
	}
	public void UpdateJumpPosition(float p_jumpTimer)
	{
		float __angle = Mathf.Clamp (p_jumpTimer, 0f, 1f) * 180f * Mathf.Deg2Rad;
		if (currentJumpType == JumpType.FULL_JUMP || currentJumpType == JumpType.VERTICAL_JUMP)
			SetPlayerPositionJumpJumpSenoid (p_jumpTimer, __angle, 2);
		else if (currentJumpType == JumpType.HALF_JUMP) 
		{
			if (p_jumpTimer <= 0.5f)
				SetPlayerPositionJumpJumpSenoid (p_jumpTimer * 2f, __angle, 1);
			else
				playerGO.transform.localPosition = Vector3.Lerp (positions [1].position, 
					positions [2].position, (p_jumpTimer - 0.5f) / 0.5f);
		}
		else if (currentJumpType == JumpType.LAND_ON_STAIR) 
		{
			if (p_jumpTimer <= 0.5f)
				SetPlayerPositionJumpJumpSenoid (p_jumpTimer * 2f, __angle, 1);
			else
				playerGO.transform.localPosition = Vector3.Lerp (positions [1].position, 
					positions [2].position, (p_jumpTimer - 0.5f) / 0.5f);
		}
	}
	private void SetPlayerPositionJumpJumpSenoid(float p_jumpTimer, float p_angle, int p_targetPosition)
	{
		float __x = positions [0].position.x + 
			((positions [p_targetPosition].position.x - positions [0].position.x) * p_jumpTimer);
		float __y = positions [0].position.y + Mathf.Abs (Mathf.Sin (p_angle));
		float __z = positions [0].position.z + 
			((positions [p_targetPosition].position.z - positions [0].position.z) * p_jumpTimer);
		playerGO.transform.localPosition= new Vector3(__x,__y,__z);
	}
	public void CalcPlayerClimbUpLadder(Vector3 p_playerDir)
	{
		positions = new List<MovimentPosition> ();
		positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
		positions.Add (new MovimentPosition (playerGO.transform.localPosition + (Vector3.up * 2f),0.5f));
		positions.Add (new MovimentPosition (playerGO.transform.localPosition + (Vector3.up * 2f) + p_playerDir,1f));
	}
	public void CalcPlayerClimbDownLadder(Vector3 p_playerDir)
	{
		positions = new List<MovimentPosition> ();
		positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
		positions.Add (new MovimentPosition (playerGO.transform.localPosition + p_playerDir,0.5f));
		positions.Add (new MovimentPosition (playerGO.transform.localPosition - (Vector3.up * 2f) + p_playerDir,1f));
	}
	public void CalcPlayerPath(Vector3 p_playerDir)
	{
		Vector3 playerTargetPosition = playerGO.transform.localPosition + p_playerDir;
		CheckNextTile(p_playerDir);
		positions = new List<MovimentPosition> ();

		if (nextTile == NextTile.LADDER_DOWN) 
		{
			CalcPlayerClimbDownLadder (p_playerDir);
			nextTile = NextTile.GROUND;
		}
		else if (nextTile != NextTile.NOTHING) 
		{
			positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
			if (nextTile == NextTile.GROUND) 
			{
				if (currentTile == NextTile.GROUND) 
					positions.Add (new MovimentPosition (playerTargetPosition,1f));
				else if (currentTile == NextTile.STAIR_UP) 
				{
					positions.Add (new MovimentPosition (playerGO.transform.localPosition 
						+ Vector3.up * 0.5f + p_playerDir * 0.5f,0.5f));
					positions.Add (new MovimentPosition (playerTargetPosition + Vector3.up * 0.5f,1f));
				}
				else if (currentTile == NextTile.STAIR_DOWN) 
				{
					positions.Add (new MovimentPosition (playerGO.transform.localPosition 
						+ Vector3.down * 0.5f + p_playerDir * 0.5f,0.5f));
					positions.Add (new MovimentPosition (playerTargetPosition + Vector3.down * 0.5f,1f));
				}
			} 
			else if (nextTile == NextTile.STAIR_DOWN) 
			{
				if (currentTile == NextTile.GROUND || currentTile == NextTile.WALL)
				{
					positions.Add (new MovimentPosition (playerGO.transform.localPosition + p_playerDir * 0.5f,0.5f));
					positions.Add (new MovimentPosition (playerTargetPosition + Vector3.down * 0.5f,1f));
				}
				else if (currentTile == NextTile.STAIR_DOWN)
					positions.Add (new MovimentPosition (playerTargetPosition + Vector3.down,1f));

			}
			else if (nextTile == NextTile.STAIR_UP)
			{
				if (currentTile == NextTile.GROUND)
				{
					positions.Add (new MovimentPosition (playerGO.transform.localPosition + p_playerDir * 0.5f,0.5f));
					positions.Add (new MovimentPosition (playerTargetPosition + Vector3.up * 0.5f,1f));
				}
				else if (currentTile == NextTile.STAIR_UP)
					positions.Add (new MovimentPosition (playerTargetPosition + Vector3.up,1f));

			}
		}
		else if (nextTile == NextTile.NOTHING)
		{
			positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
			positions.Add (new MovimentPosition (playerTargetPosition,1f));
		}
	}
	public void CalcPlayerJumpPath(Vector3 p_playerDir)
	{
		Vector3 __targetPos = playerGO.transform.localPosition + p_playerDir;
		positions = new List<MovimentPosition> ();
		if (HasWallInPosition (__targetPos + Vector3.up, 0.3f)
		    || HasStairInPosition (__targetPos + Vector3.up, 0.3f))
		{
			currentJumpType = JumpType.VERTICAL_JUMP;
			positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
			positions.Add (new MovimentPosition (playerGO.transform.localPosition,1f));
			positions.Add (new MovimentPosition (playerGO.transform.localPosition,1f));
		}
		else if (HasWallInPosition (__targetPos + p_playerDir + Vector3.up, 0.3f))
		{
			currentJumpType = JumpType.HALF_JUMP;
			positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
			positions.Add (new MovimentPosition (__targetPos + Vector3.up,1f));
			positions.Add (new MovimentPosition (__targetPos,1f));
		}
		else if (HasStairInPosition (__targetPos + p_playerDir + (Vector3.up * 0.5f), 0.3f))
		{
			currentJumpType = JumpType.LAND_ON_STAIR;
			positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
			positions.Add (new MovimentPosition (__targetPos + Vector3.up,1f));
			positions.Add (new MovimentPosition (__targetPos + p_playerDir + (Vector3.up * 0.5f),1f));
			nextTile = NextTile.STAIR_UP;
		}
		else
		{
			currentJumpType = JumpType.FULL_JUMP;
			positions.Add (new MovimentPosition (playerGO.transform.localPosition,0f));
			positions.Add (new MovimentPosition (__targetPos + Vector3.up,1f));
			positions.Add (new MovimentPosition (__targetPos + p_playerDir,1f));
		}	
		Debug.Log (currentJumpType);
	}
	public void CheckNextTile(Vector3 p_playerDir)
	{
		currentTile = nextTile;
		Vector3 playerTargetPosition = playerGO.transform.localPosition + p_playerDir;
		Collider[] __collisions;

		//Check For Ladders
		if (currentTile == NextTile.GROUND) 
		{
			__collisions = Physics.OverlapSphere (playerGO.transform.localPosition + (p_playerDir * 0.5f)
				- (Vector3.up * 0.5f), 0.1f);

			if (HasColliderWithTag (__collisions, "Ladder")) 
			{
				nextTile = NextTile.LADDER_DOWN;
				return;
			}
		}
		//Going Down on Stairs
		if (currentTile == NextTile.STAIR_DOWN)
		{
			__collisions = Physics.OverlapSphere (playerTargetPosition - (Vector3.up * 1.5f), 0.2f);
			if (HasColliderWithTag (__collisions,"Stair")) 
			{
				nextTile = NextTile.STAIR_DOWN;
				return;
			}
			if (HasStairInPosition (playerTargetPosition + Vector3.up, 0.1f)) 
			{
				currentTile = NextTile.STAIR_UP;
				nextTile = NextTile.STAIR_UP;
				return;
			}
			if (!HasWallInPosition(playerTargetPosition, 0.05f))
			{
				Debug.Log ("down");
				currentTile = NextTile.STAIR_DOWN;
				nextTile = NextTile.GROUND;
				return;
			}
			else
			{
				Debug.Log ("up");
				currentTile = NextTile.STAIR_UP;
				nextTile = NextTile.GROUND;
				return;
			}
		}
		//Going Up on Stairs
		if (currentTile == NextTile.STAIR_UP)
		{
			__collisions = Physics.OverlapSphere (playerTargetPosition + (Vector3.up * 1.5f), 0.2f);
			if (HasColliderWithTag (__collisions,"Stair")) 
			{
				Debug.Log ("Here");
				nextTile = NextTile.STAIR_UP;
				return;
			}
			__collisions = Physics.OverlapSphere (playerTargetPosition + (Vector3.up * -1.0f), 0.2f);
			if (HasColliderWithTag (__collisions,"Stair")) 
			{
				Debug.Log ("Here2");
				nextTile = NextTile.STAIR_DOWN;
				return;
			}
			if (HasWallInPosition(playerTargetPosition + (Vector3.up * -1.0f), 0.2f)
				&& !HasWallInPosition(playerTargetPosition, 0.2f)) 
			{
				Debug.Log ("Here3");
				currentTile = NextTile.STAIR_DOWN;
				nextTile = NextTile.GROUND;
				return;
			}

		}
		//Try to find something in front of him, Checks for Walls, goes if finds Enemy or an Item
		__collisions = Physics.OverlapSphere (playerTargetPosition + (Vector3.up * 0.5f), 0.2f);
		if (__collisions.Length > 0) 
		{
			if (HasColliderWithTag (__collisions,"Stair")) 
			{
				nextTile = NextTile.STAIR_UP;
				return;
			}
			//Continue if Finds Item or Enemy
			else if (__collisions [0].name.StartsWith ("Enemy") || HasColliderWithTag(__collisions, "Sword")
				|| HasColliderWithTag(__collisions, "Dynamite")) 
			{
				nextTile = NextTile.GROUND;
				return;
			}
			else if (HasWallInPosition(playerTargetPosition + (Vector3.up * 0.5f), 0.2f)) 
			{
				if (currentTile == NextTile.STAIR_UP)
					nextTile = NextTile.GROUND;
				else
					nextTile = NextTile.WALL;
				return;
			}
			else
			{
				Debug.Log ("Should Not Be Here");
				nextTile = NextTile.WALL;
				return;
			}
		}
		//Try to find Ground on the next tile
		__collisions = Physics.OverlapSphere (playerTargetPosition - (Vector3.up * 0.5f), 0.2f);
		if (__collisions.Length > 0) 
		{
			if (HasColliderWithNameStart(__collisions,"P_Small") 
				|| HasColliderWithNameStart(__collisions,"P_Big")
				|| HasColliderWithTag(__collisions,"Elevator")) 
			{
				nextTile = NextTile.GROUND;
				return;
			}
			else if (HasColliderWithTag (__collisions,"Stair")) 
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
		__collisions = Physics.OverlapSphere (playerGO.transform.localPosition + (p_playerDir * 0.5f)
			- (Vector3.up * 0.5f), 0.1f);
		if (HasColliderWithTag(__collisions,"Ladder")) 
		{
			nextTile = NextTile.LADDER_DOWN;
			return;
		}
		else
			nextTile = NextTile.NOTHING;
	}
	public static bool HasColliderWithTag(Collider[] p_collisions, string p_tag)
	{
		if (p_collisions.Length == 0)
			return false;
		
		foreach (Collider __coll in p_collisions)
			if (p_tag == __coll.tag)
				return true;
		
		return false;
	}
	public static bool HasColliderWithNameStart(Collider[] p_collisions, string p_nameStart)
	{
		if (p_collisions.Length == 0)
			return false;

		foreach (Collider __coll in p_collisions)
			if (__coll.name.StartsWith(p_nameStart))
				return true;

		return false;
	}

	public bool IsPlayerOnStair()
	{
		Collider[] __collisions = Physics.OverlapSphere (playerGO.transform.localPosition + (Vector3.up * -0.5f), 0.5f);
		return HasColliderWithTag (__collisions, "Stair");
	}
	public static bool HasWallInPosition(Vector3 p_position, float p_radius)
	{
		Collider[] __collisions = Physics.OverlapSphere (p_position, p_radius);
		if (HasColliderWithNameStart (__collisions, "P_Small") || HasColliderWithNameStart (__collisions, "P_Big")
		    || HasColliderWithTag (__collisions, "Elevator") || HasColliderWithTag (__collisions, "Wall"))
			return true;
		return false;
	}
	public static bool HasStairInPosition(Vector3 p_position, float p_radius)
	{
		Collider[] __collisions = Physics.OverlapSphere (p_position, p_radius);
		if (HasColliderWithTag (__collisions, "Stair"))
			return true;
		return false;
	}
}
[System.Serializable]
public class MovimentPosition
{
	public Vector3 position;
	public float timer = 0f;

	public MovimentPosition(Vector3 p_pos, float p_timer)
	{
		position = p_pos;
		timer = p_timer;
	}

}
