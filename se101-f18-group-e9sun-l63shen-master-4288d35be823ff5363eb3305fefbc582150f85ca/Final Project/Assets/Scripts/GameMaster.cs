using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;

public class GameMaster : MonoBehaviour {

	public GameObject difficultySelect;
	public GameObject ReadyUp0;
	public GameObject ReadyUp1;
	public GameObject idleVideo;
	public GameObject shapeOutlines;
	public AudioSource menuMusic;
	public Activate topOne;
	public Activate topTwo;
	public Activate topThree;
	public Activate topFour;
	public Activate bottomOne;
	public Activate bottomTwo;
	public Activate bottomThree;
	public Activate bottomFour;
	private Activate[][] allActivators;
	private Activate[] activatorsLinearList;
	public SongRunner songRun;
	public AudioSource musicSource80;
	public AudioSource musicSource95;
	public AudioSource musicSource110;
	public AudioSource musicSource140;
	public AudioSource musicSource170;
	private float[] bpm = {140, 110, 95, 80}; // {170, 140, 110, 80};
	private float[] songLengthSeconds = {159, 164, 174, 159}; // {169, 159, 164, 159}; //{20, 20, 20, 20}; 
	private float[] firstBeatOffset = {(float).2, (float).32, (float).14, (float).08};// {(float).1, (float).2, (float).32, (float).08};
	private float[] noteSpeed =  {(float)7, (float)5.5, (float)4.75, (float)4}; // {(float)8.5, (float)7, (float)5.5, (float)4};
	private int[] pause_length = {8, 8, 4, 4}; //one pause unit, the time used to allow instructions and determine the middle break time
	private AudioSource[] allSongs;

	private bool songPlaying;
	private bool player1Registered;
	private int player2Needed;
	private int difficulty;
	private String playerInput;
	private char inChar;

	SerialPort stream = new SerialPort("\\\\.\\COM3", 9600);

	// Use this for initialization
	void Start () {
		songRun.setActivators(topOne, topTwo, topThree, topFour, bottomOne, bottomTwo, bottomThree, bottomFour);
		allActivators = new [] { new [] {topOne, topTwo, topThree, topFour}, new [] {bottomOne, bottomTwo, bottomThree, bottomFour}};
		activatorsLinearList = new [] {topOne, topTwo, topThree, topFour, bottomOne, bottomTwo, bottomThree, bottomFour};
		//allSongs = new [] {musicSource170, musicSource140, musicSource110, musicSource80};
		allSongs = new [] {musicSource140, musicSource110, musicSource95, musicSource80};

		resetGame();
		stream.ReadTimeout = 50;
		stream.Open();

	} 
	
	// Update is called once per frame
	void Update () {
		if(!songPlaying){
			checkForStartup();
		} else {
			if(songRun.getGameFinished()){
				resetGame();
			}
		}

		if(stream.IsOpen){
			playerInput = stream.ReadLine();
			if(playerInput.Length > 1){
				for(int i = 1; i < playerInput.Length; i++){
					inChar = playerInput[i];
					if(((int)inChar - 48) < 9)
						activatorsLinearList[(int)inChar - 49].buttonPressAlert();
				}
			}
		}
	}

	private void checkForStartup() {
		if(!player1Registered){
			for(int player = 0; player < 2; player++){
				for(int activator = 0; activator < 4; activator++){
					if(allActivators[player][activator].isPressed()){
						if(player == 0){
							player2Needed = 1;
							ReadyUp0.SetActive(true);
						} else {
							player2Needed = 0;
							ReadyUp1.SetActive(true);
						}
						player1Registered = true;
						difficulty = activator;
						difficultySelect.SetActive(false);						
					}
				}
			}
		} else {
			for(int activator = 0; activator < 4; activator++){
				if(allActivators[player2Needed][activator].isPressed()){
					startSong();
				}
			}
		}
	}

	private void startSong() {
		menuMusic.Stop();
		ReadyUp0.SetActive(false);
		ReadyUp1.SetActive(false);
		idleVideo.SetActive(false);
		shapeOutlines.SetActive(true);
		songRun.startGame(allSongs[difficulty], bpm[difficulty], songLengthSeconds[difficulty], firstBeatOffset[difficulty], noteSpeed[difficulty], pause_length[difficulty]);
		songPlaying = true;
	}

	private void resetGame(){
		songPlaying = false;
		player1Registered = false;
		menuMusic.Play();
		difficultySelect.SetActive(true);
		idleVideo.SetActive(true);
		ReadyUp0.SetActive(false);
		ReadyUp1.SetActive(false);
		shapeOutlines.SetActive(false);

	}





	// public void WriteToArduino(string message) {
    //     stream.WriteLine(message);
    //     stream.BaseStream.Flush();
    // }

    // public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity) {
	// 	DateTime initialTime = DateTime.Now;
	// 	DateTime nowTime;
	// 	TimeSpan diff = default(TimeSpan);

	// 	string dataString = null;

	// 	do {
	// 		try {
	// 			dataString = stream.ReadLine();
	// 		}
	// 		catch (TimeoutException) {
	// 			dataString = null;
	// 		}

	// 		if (dataString != null)
	// 		{
	// 			callback(dataString);
	// 			yield break; // Terminates the Coroutine
	// 		} else
	// 			yield return null; // Wait for next frame

	// 		nowTime = DateTime.Now;
	// 		diff = nowTime - initialTime;

	// 	} while (diff.Milliseconds < timeout);

	// 	if (fail != null)
	// 		fail();
	// 	yield return null;
	// }

}
