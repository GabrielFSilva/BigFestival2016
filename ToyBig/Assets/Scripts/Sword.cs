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
	public bool isMoving = false;
	public float swordSpeed = 4f;
	void Start ()
	{
		if (swordGO == null)
			swordGO = gameObject;
	}
	void Update ()
	{
		if (isMoving) 
		{
			swordGO.transform.localPosition += direction * swordSpeed * Time.deltaTime;
			if (Mathf.Abs (direction.z) > 0.2f) 
				swordGO.transform.localRotation = Quaternion.Euler (new Vector3 (0f, -90f, 
					swordGO.transform.localRotation.eulerAngles.z + (direction.z * -400f * Time.deltaTime)));
			else
				swordGO.transform.localRotation = Quaternion.Euler (swordGO.transform.localRotation.eulerAngles +
					(Vector3.forward * direction.x * -400f * Time.deltaTime));
		}

		if (Vector3.Distance (swordGO.transform.localPosition, swordsContainer.transform.localPosition) > 100f)
			OnSwordLeaveWorld (this);
	}
	void OnCollisionEnter(Collision p_collision)
	{
		if (!isMoving)
			return;
		if (p_collision.gameObject.name == "Player")
			return;
		else if (p_collision.gameObject.tag == "Enemy") 
		{
			DropSword (p_collision.gameObject.transform.position);
			OnSwordHitEnemy (this, p_collision.gameObject.GetComponent<Enemy> ());
		}
		else
			DropSword (p_collision.gameObject);
	}
	public void DropSword(GameObject p_hit)
	{
		isMoving = false;
		swordGO.transform.localRotation = Quaternion.identity;
		Vector3 __tempPos = swordGO.transform.localPosition - (direction * swordSpeed * Time.deltaTime);
		swordGO.transform.localPosition = new Vector3 (Mathf.Round (__tempPos.x), 
			Mathf.Floor (__tempPos.y), Mathf.Round (__tempPos.z));
	
		if (p_hit.tag == "Stair")
			swordGO.transform.localPosition += Vector3.up * 0.6f;
	}
	public void DropSword(Vector3 p_enemyPosition)
	{
		isMoving = false;
		swordGO.transform.localRotation = Quaternion.identity;
		swordGO.transform.localPosition = new Vector3 (Mathf.Round (p_enemyPosition.x), 
			Mathf.Floor (p_enemyPosition.y), Mathf.Round (p_enemyPosition.z));
	}
	public void PlaceSwordOnPlayerHand(Transform p_playerHand)
	{
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
		isMoving = true;
	}
}
