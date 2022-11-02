using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

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
	private float[] songLengthSeconds = {20, 159, 159, 159}; //TODO update to songs
	private float[] firstBeatOffset = {(float).1, (float).15, (float).1, (float).1};
	private float[] noteSpeed = {(float)4, (float)5.5, (float)7, (float)8.5};
	private AudioSource[] allSongs;

	private bool songPlaying;
	private bool player1Registered;
	private int player2Needed;
	private int difficulty;

	// Use this for initialization
	void Start () {
		songRun.setActivators(topOne, topTwo, topThree, topFour, bottomOne, bottomTwo, bottomThree, bottomFour);
		songPlaying = false;

		allActivators = new [] { new [] {topOne, topTwo, topThree, topFour}, new [] {bottomOne, bottomTwo, bottomThree, bottomFour}};
		allSongs = new [] {musicSource80, musicSource110, musicSource140, musicSource170};
	} 
	
	// Update is called once per frame
	void Update () {
		if(!songPlaying){
			checkForStartup();
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
						} else {
							player2Needed = 0;
						}
						player1Registered = true;
						difficulty = activator;
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
		songRun.startGame(allSongs[difficulty], bpm[difficulty], songLengthSeconds[difficulty], firstBeatOffset[difficulty], noteSpeed[difficulty]);
		songPlaying = true;
	}

}
