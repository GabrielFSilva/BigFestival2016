using UnityEngine;
using System.Collections;
using System;

public class Dynamite : MonoBehaviour 
{
	public event Action <Dynamite, Wall> OnDynamiteExplosionHitWall;
	public event Action <Dynamite> OnDynamiteExplosionHitPlayer;
	public event Action <Dynamite> OnDynamiteExploded;
	public GameObject dynamitesContainer;
	public GameObject dynamiteGO;
	public BoxCollider dynamiteCollider;
	public bool isCounting = false;
	public float explosionTimerCount = 0f;
	public float explosionTimer = 7f;
	// Use this for initialization
	void Start () 
	{
		dynamiteCollider = GetComponent<BoxCollider> ();
		explosionTimer = 7f;
		if (dynamiteGO == null)
			dynamiteGO = gameObject;
	}
	

	void Update () 
	{
		if (isCounting) 
		{
			explosionTimerCount += Time.deltaTime;
			if (explosionTimerCount >= explosionTimer)
				Explode ();
		}
	}
	public void Explode()
	{
		isCounting = false;
		Collider[] __collisions = Physics.OverlapSphere (dynamiteGO.transform.localPosition, 1.75f);
		foreach (Collider __coll in __collisions) 
		{
			if (__coll.tag == "Wall")
				OnDynamiteExplosionHitWall (this, __coll.GetComponent<Wall>());
			else if (__coll.name.StartsWith ("Player"))
				OnDynamiteExplosionHitPlayer (this);
		}
		OnDynamiteExploded (this);
	}

	public void PlaceynamiteOnPlayerHand(Transform p_playerHand)
	{
		dynamiteCollider.enabled = false;
		isCounting = false;
		dynamiteGO.transform.parent = p_playerHand;
		dynamiteGO.transform.rotation = Quaternion.identity;
		dynamiteGO.transform.localScale = Vector3.one;
		dynamiteGO.transform.localPosition = new Vector3(0.15f,0.3f, 0f);
	}
	public void PlaceDynamiteOnGround(Vector3 p_playerTargetPosition)
	{
		Debug.Log ("Placing");
		Collider[] __collisions = Physics.OverlapSphere (p_playerTargetPosition + (Vector3.up * -0.3f), 0.2f);

		if (PlayerMovimentManager.HasColliderWithNameStart (__collisions, "P_Small")
		    || PlayerMovimentManager.HasColliderWithNameStart (__collisions, "P_Big")) 
		{
			dynamiteCollider.enabled = true;
			dynamiteGO.transform.parent = dynamitesContainer.transform;
			dynamiteGO.transform.rotation = Quaternion.identity;
			dynamiteGO.transform.localScale = Vector3.one;
			dynamiteGO.transform.localPosition = p_playerTargetPosition;
			isCounting = true;
			explosionTimerCount = 0f;
		}
		else
			Debug.Log ("Nop");
	}
}
