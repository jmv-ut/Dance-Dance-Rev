using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMasterBackup : MonoBehaviour {

	public GameObject difficultySelect;
	public GameObject ReadyUp0;
	public GameObject ReadyUp1;
	public GameObject idleVideo;
	public GameObject shapeOutlines;
	public Activate topOne;
	public Activate topTwo;
	public Activate topThree;
	public Activate topFour;
	public Activate bottomOne;
	public Activate bottomTwo;
	public Activate bottomThree;
	public Activate bottomFour;
	private Activate[][] allActivators;
	public SongRunner songRun;
	public AudioSource musicSource80;
	public AudioSource musicSource110;
	public AudioSource musicSource140;
	public AudioSource musicSource170;
	private float[] bpm = {80, 110, 140, 170};
	private float[] songLengthSeconds = {159, 164, 159, 169}; //TODO update to songs
	private float[] firstBeatOffset = {(float).1, (float).15, (float).1, (float).1};
	private float[] noteSpeed = {(float)4, (float)5.5, (float)7, (float)8.5};
	private int[] pause_length = {4, 4, 8, 8}; //one pause unit, the time used to allow instructions and determine the middle break time
	private AudioSource[] allSongs;

	private bool songPlaying;
	private bool player1Registered;
	private int player2Needed;
	private int difficulty;

	// Use this for initialization
	void Start () {
		difficultySelect.SetActive(true);
		idleVideo.SetActive(true);
		ReadyUp0.SetActive(false);
		ReadyUp1.SetActive(false);
		shapeOutlines.SetActive(false);
		songRun.setActivators(topOne, topTwo, topThree, topFour, bottomOne, bottomTwo, bottomThree, bottomFour);
		songPlaying = false;

		allActivators = new [] { new [] {topOne, topTwo, topThree, topFour}, new [] {bottomOne, bottomTwo, bottomThree, bottomFour}};
		allSongs = new [] {musicSource80, musicSource110, musicSource140, musicSource170};
	} 
	
	// Update is called once per frame
	void Update () {
		if(!songPlaying){
			checkForStartup();
		} else {
			if(!songRun.getGameFinished()){
				songPlaying = false;
			}
		}
	}

	private void checkForStartup() {
		if(!player1Registered){
			for(int player = 0; player < 2; player++){
				for(int activator = 0; activator < 4; activator++){
					if(allActivators[player][activator].isPressed()){
						//get player 1 and level select
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
		ReadyUp0.SetActive(false);
		ReadyUp1.SetActive(false);
		idleVideo.SetActive(false);
		shapeOutlines.SetActive(true);
		songRun.startGame(allSongs[difficulty], bpm[difficulty], songLengthSeconds[difficulty], firstBeatOffset[difficulty], noteSpeed[difficulty], pause_length[difficulty]);
		songPlaying = true;
	}

}
