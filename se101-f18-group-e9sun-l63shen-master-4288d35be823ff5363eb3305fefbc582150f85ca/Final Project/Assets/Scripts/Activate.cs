

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Activate : MonoBehaviour
{
    SpriteRenderer sr; 
    public KeyCode key;
    bool Active = false;
    public bool Visible = false;
    // public string Score = "0000";
    GameObject note;
    Color old;
    public bool createMode;
    public Note n;
    private bool buttonPressed;
    private bool sending = false;
    public Text playerScoreText;
    public Text opponentScoreText;

    //variables for tweaking allowed press time for sending
    float currNoteSpeed;
    private double latePressTime;
    private double stepPressTime;
    private double allowedDelay = 0.25;
    private double allowedDelayStep = 0.05;
    private bool watchLatePress = false;

    private void Awake()
    {
       sr = GetComponent<SpriteRenderer>(); 
       latePressTime = 0;
       stepPressTime = 0;
       currNoteSpeed = (float)0;
    }
    // Use this for initialization
    void Start()
    {
        old = sr.color;
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Combo", 0); 
    }

    // Update is called once per frame
    void Update()
    {
        getInput();   
    }

    private void getInput(){
        if(buttonPressed){
            double elapsedStep = AudioSettings.dspTime - stepPressTime;
            if(elapsedStep >= allowedDelayStep){ //allow button to be pressed for allowerDelayStep before counting as deactivated.
                buttonPressed = false;
            }
        
            if(!sending){
            if(Active){
                Destroy(note);
                addToPlayerScore();
                Active = false; // will exit code be called for this instead?
            }
        }
        }

    
    
        /*
        if (Input.GetKeyDown(key))
        {
            buttonPressed = true;
            sr.color = new Color(255, 255, 255, (float)0.6);
            if(!sending){
                if(Active){
                    Destroy(note);
                    addToPlayerScore();
                    Active = false; // will exit code be called for this instead?
                }
            }
            else if(watchLatePress){
                double elapsed = AudioSettings.dspTime - latePressTime;
                if(elapsed <= allowedDelay){
                    Vector3 positionVector = transform.position + new Vector3(0, currNoteSpeed*(float)elapsed*(float)(-1), 0);
                    Instantiate(n,positionVector, Quaternion.identity); //must instantiate with delayed offset to appear correct despite delay.
                }
                watchLatePress = false; //if pressed, will always end.
            }
        } else if(Input.GetKeyUp(key)){
            buttonPressed = false;
            sr.color = new Color(255, 255, 255, 1);
        }
        */
    }

    public void buttonPressAlert(){ //allows user to press any time between 
        if(watchLatePress){
            double elapsed = AudioSettings.dspTime - latePressTime;
            if(elapsed <= allowedDelay){
                Vector3 positionVector = transform.position + new Vector3(0, currNoteSpeed*(float)elapsed*(float)(-1), 0);
                Instantiate(n,positionVector, Quaternion.identity); //must instantiate with delayed offset to appear correct despite delay.
            }
            watchLatePress = false; //if pressed, will always end.
            buttonPressed = false;
        } else {
            buttonPressed = true;
            stepPressTime = AudioSettings.dspTime;
        }
    }

    public void setNoteSpeed(float speedIn){
        n.setSpeed(speedIn);
        currNoteSpeed = speedIn;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!sending){
            Active = true;
            if (col.gameObject.tag == "Note")
            {
                note = col.gameObject;
                //Active = true;
                //Destroy(col.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Active = false;
        //addToOpponentScore();
        //TODO does this trigger on destroy or just when it moves past?
    }

    // void AddScore()
    // {
    //     PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 100);
    //     PlayerPrefs.SetInt("Combo", PlayerPrefs.GetInt("Combo") + 1); 

    //     if (PlayerPrefs.GetInt("Combo") % 10 ==0)
    //     {
    //         Visible = true; 
    //     }
    //    // GetComponent<Text>().text = Score;
    // }

    // IEnumerator Pressed()
    // {
    //     sr.color = new Color(255, 255, 255, (float)0.6);
    //     yield return new WaitForSeconds(0.05f);
    //     sr.color = old; 
    // }

    public void sendBeat(){
        if (buttonPressed)
        {
            buttonPressed = false;
            Instantiate(n,transform.position, Quaternion.identity);
        }
        else{
            watchLatePress = true;
            latePressTime = AudioSettings.dspTime;
        }
    }

    public bool isPressed(){
        return buttonPressed;
    }

    public void isSending(bool isSending){
        sending = isSending;
    }

    private void addToPlayerScore(){
        int scoreBase = int.Parse(playerScoreText.text);
        playerScoreText.text = "" + (scoreBase + 1);
    }

    private void addToOpponentScore(){
        int scoreBase = int.Parse(opponentScoreText.text);
        opponentScoreText.text = "" + (scoreBase + 1);
    }

    public void resetPlayerScores(){
        playerScoreText.text = "0";
        opponentScoreText.text = "0";
    }
}

