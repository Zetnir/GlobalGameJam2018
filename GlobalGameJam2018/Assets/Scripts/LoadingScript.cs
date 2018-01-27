﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour {
    public Image imgPourcent;
    public Text textPourcent;
    public GameManager gameManager;
    public int buildIndex;
    public Scene LoadingScene;

    AsyncOperation synchScene;
     
	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        switch (gameManager.currentPhase)
        {
            case GameManager.GamePhase.Menu:
                break;
            case GameManager.GamePhase.RoomMenu:
                break;
            case GameManager.GamePhase.InGame:
                break;
            case GameManager.GamePhase.Pause:
                break;
            case GameManager.GamePhase.EndGame:
                break;
        }

        if(imgPourcent)
        {
            synchScene = SceneManager.LoadSceneAsync(buildIndex);
        }
        else
        {
            synchScene = SceneManager.LoadSceneAsync(LoadingScene.buildIndex);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (textPourcent)
        {
            textPourcent.text = (synchScene.progress * 100 + 10).ToString() + "%";
        }

		if(imgPourcent)
        {
            imgPourcent.fillAmount = synchScene.progress + 0.1f;
        }
	}
}