﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
public class timerScript : uLink.MonoBehaviour {

	public float timer = 12*60;
	float timerOrig;
	public Text displayText;

	public bool runEndGameOnce = false;
	
	bool gamePaused = false;
	float timeWhenPaused = 0;

	float gameEndTime = 0;
	// Use this for initialization
	 void Start () {
		timerOrig = timer;

	}
	public void restart()
	{
		runEndGameOnce=false;
		
		if(uLink.Network.isServer==true)
		{
			gameEndTime = (float)uLink.NetworkTime.rawServerTime;
			Debug.Log(gameEndTime);
			
			transform.uLinkNetworkView().RPC("initalServerTimeOffset", uLink.RPCMode.OthersBuffered, gameEndTime);
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(gamePaused==false)
		{
			timer = timerOrig-(float)uLink.NetworkTime.rawServerTime+gameEndTime;
			countDown();
		}

	}

	void countDown()
	{

			


		int minsDisplay = (int)timer/60;
		int secsDisplay = (int)timer;

		if(minsDisplay<10)
		{
			if ( ((int)timer -  minsDisplay * 60) >= 10 ) {
				secsDisplay = (int)timer - minsDisplay* 60;
				displayText.text = "0" +minsDisplay.ToString() + ":" + secsDisplay.ToString();
			} 
			else {
				secsDisplay =  (int)timer - minsDisplay * 60;
				displayText.text = "0" +minsDisplay.ToString() + ":" +"0" + secsDisplay.ToString();
			}
		}
		else
		{

			if ( ((int)timer -  minsDisplay * 60) >= 10 ) {
				secsDisplay = (int)timer - minsDisplay* 60;
				displayText.text = minsDisplay.ToString() + ":" + secsDisplay.ToString();
			} 
			else {
				secsDisplay =  (int)timer - minsDisplay * 60;
				displayText.text = minsDisplay.ToString() + ":" +"0" + secsDisplay.ToString();
			}
		}

		if(timer<=0)
		{
			if(uLink.Network.isServer ==true && runEndGameOnce==false)//only run this on server
			{
				endGameTime();
			}
		}

	}

	public void pauseTimer()
	{
		gamePaused = true;
		timeWhenPaused = (float)uLink.NetworkTime.rawServerTime;
		transform.uLinkNetworkView().RPC("clientPauseTime", uLink.RPCMode.OthersBuffered,timer);
	}

	public void unPauseTimer()
	{
		gamePaused = false;
		gameEndTime=gameEndTime-timeWhenPaused+(float)uLink.NetworkTime.rawServerTime;
		transform.uLinkNetworkView().RPC("clientUnPauseTime", uLink.RPCMode.OthersBuffered, gameEndTime);
	}

	void endGameTime()
	{

		//so this function only runs once
		runEndGameOnce=true;

		//run end game function
		if(GameObject.FindGameObjectWithTag("Player")!=null)
		{
		serverBallScript serverScript = GameObject.FindGameObjectWithTag("Player").GetComponent<serverBallScript>();
		serverScript.restartGame(2);


		}
		else{
			serverStart serverScript = GameObject.FindGameObjectWithTag("server").GetComponent<serverStart>();
			serverScript.restartGame();


		}



	}



	void uLink_OnConnectedToServer()
	{
		GetComponent<Text>().enabled=true;

		GameObject.FindGameObjectWithTag("redScoreText").GetComponent<Text>().enabled=true;
		GameObject.FindGameObjectWithTag("blueScoreText").GetComponent<Text>().enabled=true;
	}
 
	[RPC]
	void initalServerTimeOffset(float initalTime)
	{
		gameEndTime = initalTime;
	}

	[RPC]
	void clientPauseTime(float pauseTime)
	{
		gamePaused=true;
		timer = pauseTime;
		countDown();
	}
	[RPC]
	void clientUnPauseTime(float timeDifference)
	{
		gamePaused=false;
		gameEndTime=timeDifference;
	}



}
