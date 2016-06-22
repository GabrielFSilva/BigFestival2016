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
		float __x =  positions [0].position.x + ((positions [2].position.x - positions [0].position.x) * p_jumpTimer);
		float __y =  positions [0].position.y + Mathf.Abs (Mathf.Sin (__angle));
		float __z =  positions [0].position.z + ((positions [2].position.z - positions [0].position.z) * p_jumpTimer);
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
				if (currentTile == NextTile.GROUND)
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
	}
	public void CheckNextTile(Vector3 p_playerDir)
	{
		currentTile = nextTile;
		Vector3 playerTargetPosition = playerGO.transform.localPosition + p_playerDir;
		Collider[] __collisions;
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
		if (currentTile == NextTile.STAIR_DOWN)
		{
			__collisions = Physics.OverlapSphere (playerTargetPosition - (Vector3.up * 1.5f), 0.2f);
			if (HasColliderWithTag (__collisions,"Stair")) 
			{
				nextTile = NextTile.STAIR_DOWN;
				return;
			}
		}
		if (currentTile == NextTile.STAIR_UP)
		{
			__collisions = Physics.OverlapSphere (playerTargetPosition + (Vector3.up * 1.5f), 0.2f);
			if (HasColliderWithTag (__collisions,"Stair")) 
			{
				nextTile = NextTile.STAIR_UP;
				return;
			}

		}
		//Try to fing Ground, Checks for Walls, goes if finds Enemy or an Item
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
			else if (HasColliderWithNameStart(__collisions,"P_Small") 
				|| HasColliderWithNameStart(__collisions,"P_Big")
				|| HasColliderWithTag(__collisions,"Elevator")) 
			{
				nextTile = NextTile.GROUND;
				return;
			}
			else
			{
				Debug.Log (__collisions[0].name);
				nextTile = NextTile.WALL;
				return;
			}
		}
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
