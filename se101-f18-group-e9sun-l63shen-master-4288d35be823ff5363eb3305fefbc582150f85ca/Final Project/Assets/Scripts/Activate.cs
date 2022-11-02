

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

    //variables for tweaking allowed press time for sending
    float currNoteSpeed;
    private double latePressTime;
    private double allowedDelay = 0.1;
    private bool watchLatePress = false;

    private void Awake()
    {
       sr = GetComponent<SpriteRenderer>(); 
       latePressTime = 0;
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
        if (Input.GetKeyDown(key))
        {
            buttonPressed = true;
            sr.color = new Color(255, 255, 255, (float)0.6);
            if(!sending){
                if(Active){
                    Destroy(note);
                    AddScore();
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

        // if (createMode)
        // {
        //     if (Input.GetKeyDown(key))
        //     {
        //         //Instantiate(n,transform.position, Quaternion.identity);  //ALT FOR DDR
        //         buttonPressed = true;
        //         StartCoroutine(Pressed());
        //     }
        // }
        // else
        // {
        //     if (Input.GetKeyDown(key))
        //     {
        //         StartCoroutine(Pressed());
        //     }
        //     if (Active && Input.GetKeyDown(key))
        //     {
        //         Destroy(note);
        //         AddScore();
        //        // Active = false;
        //     }
        // }
        
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
        //TODO does this trigger on destroy or just when it moves past?
    }

    void AddScore()
    {
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 100);
        PlayerPrefs.SetInt("Combo", PlayerPrefs.GetInt("Combo") + 1); 

        if (PlayerPrefs.GetInt("Combo") % 10 ==0)
        {
            Visible = true; 
        }
       // GetComponent<Text>().text = Score;
    }

    // IEnumerator Pressed()
    // {
    //     sr.color = new Color(255, 255, 255, (float)0.6);
    //     yield return new WaitForSeconds(0.05f);
    //     sr.color = old; 
    // }

    public void sendBeat(){
        if (buttonPressed)
        {
            // buttonPressed = false;
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

}

