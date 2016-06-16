using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour 
{
	public List<Tile> tiles;
	// Use this for initialization
	void Start () 
	{
		tiles = new List<Tile>(FindObjectsOfType<Tile> ());
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
