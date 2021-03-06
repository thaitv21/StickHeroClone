﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {
    public int Score, LastScore;
    public GameObject EndUI;
    GameObject ScoreText,HighScore,NowScore;
	void Start () {
        ScoreText = GameObject.Find("ScoreText");
        
    }	
	void Update () {
        ScoreText.GetComponent<Text>().text = Score.ToString();
        LastScore = PlayerPrefs.GetInt("Score");
	}
    public void  AddScore()
    {
        Score++;
    }
    public void CalculateScore()
    {
        if(Score>LastScore)
        {
            PlayerPrefs.SetInt("Score", Score);
        }
        if (EndUI == null)
        {
            return;
        }
        EndUI.SetActive(true);
        NowScore = GameObject.Find("NowScore");
        HighScore = GameObject.Find("HighScore");
        NowScore.GetComponent<Text>().text = Score.ToString();
        HighScore.GetComponent<Text>().text = LastScore.ToString();
        GameObject.Find("Text").GetComponent<Text>().text = string.Format("Score\r\n{0}\n\nHigh score\n{1}", Score.ToString(), LastScore.ToString());
        if (Input.GetKey(KeyCode.Mouse0))
        {
            SceneManager.LoadScene("UI");
        }
    }
    public int getScore()
    {
        return this.Score;
    }
}
