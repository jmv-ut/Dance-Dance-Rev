using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kill : MonoBehaviour {

    public KeyCode key;
    public bool Visible = false;
    GameObject note;
    public Text opponentScoreText;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Note")
        {
            //destroy object
            Destroy(col.gameObject);
            addToOpponentScore();
            //KillCombo();
        }
    }

    private void addToOpponentScore(){
        int scoreBase = int.Parse(opponentScoreText.text);
        opponentScoreText.text = "" + (scoreBase + 1);
    }

    // void KillCombo()
    // {
    //     PlayerPrefs.SetInt("Combo", 0);
    //     if (PlayerPrefs.GetInt("Combo") % 10 == 0)
    //     {
    //         Visible = true;
    //     }
    // }
}
