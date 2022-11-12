using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongRunner : MonoBehaviour {

	public GameObject PlayInstrFirst; //instructions on canvases.
	public GameObject PlayInstrReverse;
	public GameObject scoreBoard;
	public GameObject BlueWinScreen;
	public GameObject PinkWinScreen;
	public GameObject TieScreen;
	public Text BlueScore;
    public Text PinkScore;
	public Text FinalBlueScore;
    public Text FinalPinkScore;
	public AudioSource endScreenMusic;

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
	private double beggining_of_end;
	public double endScreenLengthSeconds; //seconds to sit on endscreen

	private bool isGameFinished = true;
	private int transitionLength = 12; //in beats. TOdO will need to be a song-based variable
	private bool transitionDone = false;
	private int beatTransitionStart = 0;
	private int beatTransitionEnd = 0;
	private int init_pause_stop = 0;
	private int beatToStop = 0;
	private int songBeatLength = 0;
	
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

	public void startGame(AudioSource newRunningSong, float newSongBpm, float newSongLength, float newFirstBeatOffset, float noteSpeed, int pause_unit){
		runningSong = newRunningSong;
		songBpm = newSongBpm;
		transitionDone = false;
		PlayInstrFirst.SetActive(true);
		// songLength = newSongLength;
		//TODO determine timing for stop and switch.
		firstBeatOffset = newFirstBeatOffset;
		beatCounter = 1;
		//Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;
		int numSongBeats = (int)(newSongLength/secPerBeat);
		beatTransitionStart = (numSongBeats - transitionLength) / 2;
		beatTransitionEnd = beatTransitionStart + transitionLength;
		init_pause_stop = pause_unit + 1;
		transitionLength = pause_unit*3;
		songBeatLength = (int)(newSongLength/secPerBeat);
		beatToStop = songBeatLength - 5; //must stop 5 early to clear board for ending.
		setAllSpeeds(noteSpeed);

		//Record the time when the music starts
        dspSongTime = (double)AudioSettings.dspTime;
		dspEndTime = (double)(dspSongTime + newSongLength);
		
		setTopSending(false); //start false, trigger after correct num of songUnits.
		setBottomSending(false); //must be seperate for switch
		//Start the music
		//musicSource.Play();
		runningSong.Play();
		isGameFinished = false;
		playing = true;

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
				if(beatCounter == init_pause_stop){
					setTopSending(true);
					PlayInstrFirst.SetActive(false);
				} else if (beatCounter >= beatToStop){
					if(beatCounter == beatToStop){
						setBottomSending(false);
					}
					if(beatCounter > songBeatLength){
						endSong();
					}
				} 
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
		} else if(!isGameFinished){ //if done playing but game not finished, must run the ending animations
			currTime = (double)(AudioSettings.dspTime);
			if(currTime > (beggining_of_end + endScreenLengthSeconds)){
				resetGame();
			}
		}
	}

	private void transitionBeat(int currBeat){ //start transition from top to bottom by stopping top from sending
		if(currBeat == 0){
			setTopSending(false);
		}
		if(currBeat == 5){
			//all notes off board, display transition effect
			PlayInstrReverse.SetActive(true);
		}
	}

	private void endTransition(){ //end transition from top to bottom
		setBottomSending(true); //enable bottom player to send
		PlayInstrReverse.SetActive(false);
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

	private void endSong(){
		playing = false;
		beggining_of_end = (double)(AudioSettings.dspTime);
        int blueFinalScore = int.Parse(BlueScore.text);
        FinalBlueScore.text = "" + blueFinalScore;
		int pinkFinalScore = int.Parse(PinkScore.text);
        FinalPinkScore.text = "" + pinkFinalScore;
		scoreBoard.SetActive(false);
		runningSong.Stop();
		endScreenMusic.Play();
		if(pinkFinalScore > blueFinalScore){
			//pink
			PinkWinScreen.SetActive(true);
		} else if(blueFinalScore > pinkFinalScore){
			//blue
			BlueWinScreen.SetActive(true);
		}else{
			//tie
			TieScreen.SetActive(true);
		}
		//fun score results
	}

	private void resetGame(){
		FinalBlueScore.text = "";
        FinalPinkScore.text = "";
		endScreenMusic.Stop();
		scoreBoard.SetActive(true);
		topOne.resetPlayerScores(); //set scoreboard to 0-0
		PinkWinScreen.SetActive(false);
		BlueWinScreen.SetActive(false);
		TieScreen.SetActive(false);
		isGameFinished = true;
	}

	public bool getGameFinished(){
		//return playing;
		return isGameFinished;
	}
}
