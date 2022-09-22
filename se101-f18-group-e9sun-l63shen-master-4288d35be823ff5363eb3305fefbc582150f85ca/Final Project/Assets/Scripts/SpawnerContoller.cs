using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerContoller : MonoBehaviour {
	public float speed;
	public Spawner spawner1;
	public Spawner spawner2;
	public Spawner spawner3;
	public Spawner spawner4;
	//array of arrays for songs to pull from

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}

	public void beat () {
		//check song beat list and print out.
		spawner1.sendNote();
		spawner2.sendNote();
		spawner3.sendNote();
		spawner4.sendNote();
	}
}
