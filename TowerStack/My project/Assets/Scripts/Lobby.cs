using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_WEBGL
public class Lobby : MonoBehaviour
{

    public string gameState;
    public bool isPlayerJoined;

    public GameObject AdmitButton;
    public LobbyContract lobbyContract;
    public Text PriceText;

    private async void Start()
    {
        PriceText.text = await lobbyContract.getUSDTicketPrice();
        gameState = await lobbyContract.getGameState();
        string account = PlayerPrefs.GetString("Account");
        isPlayerJoined = await lobbyContract.isPlayerJoined(account);
        if (isGameOpenForPlayer())
        {
            AdmitButton.SetActive(true);
        }
    }

    public void onGameState(string currentState)
    {
        Debug.Log("current game state " + currentState);
        gameState = currentState;
        // when the game has started, allow the player to join
        if (isGameOpenForPlayer())
        {
            AdmitButton.SetActive(true);
        }
    }

    // can user be addmitted to game
    private bool isGameOpenForPlayer()
    {
        return isPlayerJoined && (gameState == "Open" || gameState == "Chooseroundwinner" || gameState == "Calculating_winner");
    }

    public void onExit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void grantAdmission()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public async void onBuyTicket()
    {
        bool isPriceFetched = await lobbyContract.getEthTicketPrice(); // update ticket price
        if (isPriceFetched)
        {
            isPlayerJoined = await lobbyContract.buyTicket();
            if (isPlayerJoined && (gameState == "Open" || gameState == "Chooseroundwinner" || gameState == "Calculating_winner"))
            {
                AdmitButton.SetActive(true);
            }
            else
            {
                // TODO: warning message display

            }
        }
    }
}
#endif