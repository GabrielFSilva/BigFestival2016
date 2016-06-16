using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovimentManager : MonoBehaviour 
{
	public GameObject playerGO;
	public List<MovimentPosition> positions;

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
