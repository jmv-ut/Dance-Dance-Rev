using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
	public float speed;
	public float bpm;
	public GameObject note;
	private float beatTime;
	private float beatTimer;

	private void Awake()
    {
		//speed is in units per second, so (BPM/60) = BPSec. Place spawner 10*BPs
		float vertical = (bpm/60)*10;
        transform.Translate (0f, vertical, 0f);
    }

	// Use this for initialization
	void Start () {
		beatTime = bpm/60;
		beatTimer = beatTime;
	}
	
	// Update is called once per frame
	void Update () {
		targetTime -= Time.deltaTime;
 
		if (beatTimer <= 0.0f)
		{
			beatTimer = beatTime;
			Instantiate(note,transform.position, Quaternion.identity);
		}		
	}
}
