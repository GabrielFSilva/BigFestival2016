using UnityEngine;
using System.Collections;
using System;

public class Sword : MonoBehaviour 
{
	public event Action <Sword> OnSwordLeaveWorld;
	public event Action <Sword, Enemy> OnSwordHitEnemy;
	public GameObject swordsContainer;
	public GameObject swordGO;

	public Vector3 direction;
	public float swordSpeed = 4f;

	public SwordState swordState = SwordState.IDLE;
	public enum SwordState
	{
		IDLE,
		ON_HOLD,
		MOVING,
		FALLING
	}
	void Start ()
	{
		if (swordGO == null)
			swordGO = gameObject;
	}
	void Update ()
	{
		if (swordState == SwordState.MOVING) 
		{
			swordGO.transform.localPosition += direction * swordSpeed * Time.deltaTime;
			if (Mathf.Abs (direction.z) > 0.2f) 
				swordGO.transform.localRotation = Quaternion.Euler (new Vector3 (0f, -90f, 
					swordGO.transform.localRotation.eulerAngles.z + (direction.z * -400f * Time.deltaTime)));
			else
				swordGO.transform.localRotation = Quaternion.Euler (swordGO.transform.localRotation.eulerAngles +
					(Vector3.forward * direction.x * -400f * Time.deltaTime));
		}
		else if (swordState == SwordState.FALLING)
			swordGO.transform.localPosition += Vector3.down * Time.deltaTime * swordSpeed;
		else if (swordState == SwordState.IDLE)
		{
			if (!PlayerMovimentManager.HasWallInPosition (swordGO.transform.localPosition + Vector3.up * -0.2f, 0.1f) 
				&& !PlayerMovimentManager.HasStairInPosition (swordGO.transform.localPosition + Vector3.up * -0.2f, 0.1f))
				swordState = SwordState.FALLING;
		}	
		if (Vector3.Distance (swordGO.transform.localPosition, swordsContainer.transform.localPosition) > 100f)
			OnSwordLeaveWorld (this);
	}
	void OnCollisionEnter(Collision p_collision)
	{
		if (swordState == SwordState.IDLE || swordState == SwordState.ON_HOLD)
			return;
		
		if (p_collision.gameObject.name == "Player")
			return;
		else if (p_collision.gameObject.tag == "Enemy") 
		{
			DropSword (p_collision.gameObject.transform.position);
			OnSwordHitEnemy (this, p_collision.gameObject.GetComponent<Enemy> ());
		}
		else if (swordState == SwordState.MOVING)
			DropSword (p_collision.gameObject);
		else
		{
			swordState = SwordState.IDLE;
			swordGO.transform.localRotation = Quaternion.identity;
			if (p_collision.gameObject.tag == "Stair")
				swordGO.transform.localPosition = p_collision.transform.position + (Vector3.up * 0.6f);
			else
				swordGO.transform.localPosition = p_collision.transform.position;
		}	
	}
	public void DropSword(GameObject p_hit)
	{
		swordState = SwordState.IDLE;
		swordGO.transform.localRotation = Quaternion.identity;
		Vector3 __tempPos = swordGO.transform.localPosition - (direction * swordSpeed * Time.deltaTime);
		swordGO.transform.localPosition = new Vector3 (Mathf.Round (__tempPos.x), 
			Mathf.Floor (__tempPos.y), Mathf.Round (__tempPos.z));
	
		if (p_hit.tag == "Stair")
			swordGO.transform.localPosition += Vector3.up * 0.6f;
		else if (!PlayerMovimentManager.HasWallInPosition (swordGO.transform.localPosition
		         + Vector3.up * -0.2f, 0.1f))
			swordState = SwordState.FALLING;
	}
	public void DropSword(Vector3 p_enemyPosition)
	{
		swordState = SwordState.IDLE;
		swordGO.transform.localRotation = Quaternion.identity;
		swordGO.transform.localPosition = new Vector3 (Mathf.Round (p_enemyPosition.x), 
			Mathf.Floor (p_enemyPosition.y), Mathf.Round (p_enemyPosition.z));
	}
	public void PlaceSwordOnPlayerHand(Transform p_playerHand)
	{
		swordState = SwordState.ON_HOLD;
		swordGO.transform.parent = p_playerHand;
		swordGO.transform.localPosition = Vector3.zero;
		swordGO.transform.localRotation = Quaternion.identity;
		swordGO.transform.localScale = Vector3.one;
	}

	public void ThrowSword(Player.PlayerDirection p_direction)
	{
		direction = new Vector3 (Mathf.Sin ((int)p_direction * -90f * Mathf.Deg2Rad),
			0f, Mathf.Cos ((int)p_direction * 90f * Mathf.Deg2Rad));
		
		swordGO.transform.parent = swordsContainer.transform;
		swordGO.transform.localRotation = Quaternion.identity;
		swordGO.transform.localScale = Vector3.one;
		swordState = SwordState.MOVING;
	}
}
