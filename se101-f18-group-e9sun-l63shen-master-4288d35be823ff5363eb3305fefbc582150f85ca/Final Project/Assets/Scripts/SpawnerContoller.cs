using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnerContoller : MonoBehaviour {
	public float speed;
	public Spawner spawner1;
	public Spawner spawner2;
	public Spawner spawner3;
	public Spawner spawner4;
	public TextAsset csvFile;
	private Queue<int[]> songNoteList;

	// Use this for initialization
	void Awake () {
		songNoteList = new Queue<int[]>();
		readCSV();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}

	public void beat () {
		//check song beat list and print out.
		if (songNoteList.Count != 0){
			int[] beatNotes = songNoteList.Dequeue();
			//Debug.Log(beatNotes[0] + ", " + beatNotes[1] + ", " + beatNotes[2] + ", " + beatNotes[3]);
			if(beatNotes[0] == 1){
				spawner1.sendNote();
			}
			if(beatNotes[1] == 1){
				spawner2.sendNote();
			}
			if(beatNotes[2] == 1){
				spawner3.sendNote();
			}
			if(beatNotes[3] == 1){
				spawner4.sendNote();
			}
		}
	}

	private void readCSV(){
		int[] beatNotes;
		string[] data = csvFile.text.Split(new string[] {",", "\n"}, StringSplitOptions.None);

		int tableSize = data.Length / 4;
		
		for(int i = 0; i < tableSize; i++){
			beatNotes = new int[4];

			beatNotes[0] = int.Parse(data[4*i]);
			beatNotes[1] = int.Parse(data[4*i + 1]);
			beatNotes[2] = int.Parse(data[4*i + 2]);
			beatNotes[3] = int.Parse(data[4*i + 3]);
			//Debug.Log(beatNotes[0] + ", " + beatNotes[1] + ", " + beatNotes[2] + ", " + beatNotes[3]);
			songNoteList.Enqueue(beatNotes);
		}		
	}
}
