using UnityEngine;
using System.Collections;
using System;

public class CustomCollider : MonoBehaviour 
{
	public event Action <CustomCollider, Collision> onCollisionEnter;
	public event Action <CustomCollider, Collision> onCollisionExit;
	public event Action <CustomCollider, Collision> onCollisionStay;

	public event Action <CustomCollider, Collider> onTriggerEnter;
	public event Action <CustomCollider, Collider> onTriggerExit;
	public event Action <CustomCollider, Collider> onTriggerStay;

	void OnCollisionEnter(Collision collision) 
	{
		if (onCollisionEnter != null)
			onCollisionEnter(this, collision);		
	}
	void OnCollisionExit(Collision collision) 
	{
		if (onCollisionExit != null)
			onCollisionExit(this, collision);
	}
	void OnCollisionStay(Collision collision) 
	{
		if (onCollisionStay != null)
			onCollisionStay(this, collision);
	}
	void OnTriggerEnter(Collider collider) 
	{
		if (onTriggerEnter != null)
			onTriggerEnter(this, collider);
	}
	void OnTriggerExit(Collider collider) 
	{
		if (onTriggerExit != null)
			onTriggerExit(this, collider);
	}
	void OnTriggerStay(Collider collider) 
	{
		if (onTriggerStay != null)
			onTriggerStay(this, collider);
	}
}
