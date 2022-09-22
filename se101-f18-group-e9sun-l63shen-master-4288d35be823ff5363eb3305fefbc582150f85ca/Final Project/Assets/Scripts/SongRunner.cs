using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongRunner : MonoBehaviour {
	//Song beats per minute- determined by the song you're trying to sync up to
    public float songBpm;
    //The number of seconds for each song beat
    private float secPerBeat;
    //Current song position, in seconds
    private float songPosition;
    //Current song position, in beats
    private float songPositionInBeats;
    //How many seconds have passed since the song started
    private float dspSongTime;
    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource;
    //The offset to the first beat of the song in seconds
    public float firstBeatOffset;
    private float beatCounter;
	public SpawnerContoller spawnController;

	// Use this for initialization
	void Start () {
		//Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();
        //Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;
        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;
        //Start the music
        musicSource.Play();
        beatCounter = 1;
	}
	
	// Update is called once per frame
	void Update () {
		//determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);
        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;
        
        if(songPositionInBeats >= beatCounter){
            beatCounter += 1;
            spawnController.beat();
        }
	}
}
