using UnityEngine;
using System.Collections;

public class ElevatorButton : MonoBehaviour 
{
	public Elevator	elevatorGO;

	public bool pressed = false;
	public void SetElevatorMoving()
	{
		if (pressed)
			return;

		pressed = true;
		elevatorGO.StartMoving ();
	}
}
