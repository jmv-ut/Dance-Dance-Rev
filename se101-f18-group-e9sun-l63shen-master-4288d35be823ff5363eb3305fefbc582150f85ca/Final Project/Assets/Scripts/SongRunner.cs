using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongRunner : MonoBehaviour {
	//Data from GameMaster control
	//Song beats per minute- determined by the song you're trying to sync up to
    private float songBpm;
	//How long the song is, used to set switchback times
	// private float songLength;
    //an AudioSources that will play the music.
    private AudioSource runningSong;
	//The offset to the first beat of the song in seconds
    private float firstBeatOffset; //if song file has a delay at start.
	//the footpads
	private Activate topOne;
	private Activate topTwo;
	private Activate topThree;
	private Activate topFour;
	private Activate bottomOne;
	private Activate bottomTwo;
	private Activate bottomThree;
	private Activate bottomFour;
	
	
	//SongRunner Specific data
    private int beatCounter;
	//public SpawnerContoller spawnController; // ALT FOR DDR
	private double currTime;
	public SpriteRenderer background;
	private bool topSending;
	private bool bottomSending;
	private bool playing;
	//The number of seconds for each song beat
    private float secPerBeat;
	//Current song position, in seconds
    // private double songPosition;
	//Current song position, in beats
    private float songPositionInBeats;
    //How many seconds have passed since the song started
    private double dspSongTime;
	//What time song will end at
    private double dspEndTime;
	private float[] pulseGraph;

	private int transitionLength = 12; //in beats. TOdO will need to be a song-based variable
	private bool transitionDone = false;
	private int beatTransitionStart = 0;
	private int beatTransitionEnd = 0;
	
	// Use this for initialization
	void Start () {
		//Load the AudioSource attached to the Conductor GameObject
        // musicSource = GetComponent<AudioSource>();
        //Start the music
		playing = false;
		pulseGraph = new float[200];
		background.color = new Color(255, 255, 255, 0);
	}

	public void setActivators(Activate topOne, Activate topTwo, Activate topThree, Activate topFour, Activate bottomOne, Activate bottomTwo, Activate bottomThree, Activate bottomFour){
		this.topOne = topOne;
		this.topTwo = topTwo;
		this.topThree = topThree;
		this.topFour = topFour;
		this.bottomOne = bottomOne;
		this.bottomTwo = bottomTwo;
		this.bottomThree = bottomThree;
		this.bottomFour = bottomFour;
	}

	public void startGame(AudioSource newRunningSong, float newSongBpm, float newSongLength, float newFirstBeatOffset, float noteSpeed){
		runningSong = newRunningSong;
		songBpm = newSongBpm;
		transitionDone = false;
		// songLength = newSongLength;
		//TODO determine timing for stop and switch.
		firstBeatOffset = newFirstBeatOffset;
		beatCounter = 1;
		//Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;
		int numSongBeats = (int)(newSongLength/secPerBeat);
		beatTransitionStart = (numSongBeats - transitionLength) / 2;
		beatTransitionEnd = beatTransitionStart + transitionLength;

		setAllSpeeds(noteSpeed);

		//Record the time when the music starts
        dspSongTime = (double)AudioSettings.dspTime;
		dspEndTime = (double)(dspSongTime + newSongLength);
		playing = true;
		setTopSending(true);
		setBottomSending(false); //must be seperate for switch
		//Start the music
		//musicSource.Play();
		runningSong.Play();

		//calculate the undulation graph
		for(int i = 0; i < 200; i++){
			pulseGraph[i] = ((Mathf.Pow((Mathf.Abs(Mathf.Cos(Mathf.PI*((float)i/(float)200)))),10))/3);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//determine how many seconds since the song started
		if(playing){
			currTime = (double)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);
			// songPosition = currTime;
			//determine how many beats since the song started
			songPositionInBeats = (float)currTime / secPerBeat;

			//pulse background
			if(currTime > 0){ //prevent negative calls at start
				int pulseSample = ((int)((songPositionInBeats % 1)*(float)(200)))%200; //int from 0 to 199	// (currTime/secPerSample)
				background.color = new Color(255, 255, 255, pulseGraph[pulseSample]);
			}

			if(songPositionInBeats >= (float)beatCounter){
				beatCounter += 1;
				sendBeats();
				if(!transitionDone){
					if(beatCounter >= beatTransitionStart){
						transitionBeat(beatCounter - beatTransitionStart); //should feed 0 though transitionLength
						if(beatCounter >= beatTransitionEnd){
							endTransition();
							transitionDone = true;
						}
					}
				}
			}
		}
		
	}

	private void transitionBeat(int currBeat){ //start transition from top to bottom by stopping top from sending
		if(currBeat == 0){
			setTopSending(false);
		}
		if(currBeat == 5){
			//all notes off board, display transition effect
		}
	}

	private void endTransition(){ //end transition from top to bottom
		setBottomSending(true); //enable bottom player to send
		//disable any transition effects from transitionBeat method.
	}

	private void sendBeats(){
		if(topSending){
			topOne.sendBeat();
			topTwo.sendBeat();
			topThree.sendBeat();
			topFour.sendBeat();
		} else if(bottomSending){
			bottomOne.sendBeat();
			bottomTwo.sendBeat();
			bottomThree.sendBeat();
			bottomFour.sendBeat();
		}
	}

	private void setAllSpeeds(float inSpeed){
		topOne.setNoteSpeed(inSpeed);
		topTwo.setNoteSpeed(inSpeed);
		topThree.setNoteSpeed(inSpeed);
		topFour.setNoteSpeed(inSpeed);
		bottomOne.setNoteSpeed(-inSpeed);
		bottomTwo.setNoteSpeed(-inSpeed);
		bottomThree.setNoteSpeed(-inSpeed);
		bottomFour.setNoteSpeed(-inSpeed);
	}

	private void setTopSending(bool isTopSending){
		topSending = isTopSending;
		topOne.isSending(isTopSending);
		topTwo.isSending(isTopSending);
		topThree.isSending(isTopSending);
		topFour.isSending(isTopSending);
	}

	private void setBottomSending(bool isBottomSending){
		bottomSending = isBottomSending;
		bottomOne.isSending(isBottomSending);
		bottomTwo.isSending(isBottomSending);
		bottomThree.isSending(isBottomSending);
		bottomFour.isSending(isBottomSending);
	}
}
