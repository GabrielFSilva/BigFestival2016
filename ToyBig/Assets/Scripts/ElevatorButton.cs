using UnityEngine;
using System.Collections;

public class ElevatorButton : MonoBehaviour 
{
	public Elevator	elevatorGO;

	public void SetElevatorMoving()
	{
		elevatorGO.StartMoving ();
	}
}
