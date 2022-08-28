using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Connection : MonoBehaviour
{
    WebSocket websocket;
    public Text eventText;
    public string websocketAddress;

    // Start is called before the first frame update
    async void Start()
    {
        DontDestroyOnLoad(gameObject); // dont destroy the Connection object when loading another scene
        websocket = new WebSocket(websocketAddress);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
             // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);

            string currentSceneName = SceneManager.GetActiveScene().name;

            Debug.Log("Received message " + message);
            Debug.Log("Active scene is " + currentSceneName);

            var events = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
            Debug.Log("event deserialized JSON" + events);
            string eventName = events["eventName"];


            Debug.Log("Event name is " + eventName);
            if (currentSceneName == "Game") // test
            {
                GameManager GameManagerInst = GameObject.Find("GameManager").GetComponent<GameManager>();

                switch (eventName)
                {
                    case "BetMade":
                        GameManagerInst.onBetMade(events["player"], events["currentRoundBetCount"], events["remainingBets"]);
                        break;
                    case "RoundStarted":
                        int currentRound = int.Parse(events["currentRoundNumber"]);
                        GameManagerInst.onRoundStarted(currentRound);
                        break;
                    case "RoundEnded":
                        GameManagerInst.onRoundEnded();
                        break;
                    case "GameState":
                        GameManagerInst.onGameState(events["currentState"]);
                        break;
                    case "GameEnded":
                        int gameWinnings = int.Parse(events["amountWon"]);                        
                        GameManagerInst.onGameEnded(events["winner"], gameWinnings);
                        break;
                    case "RoundWinner":
                        int rewardAmount = int.Parse(events["rewardAmount"]);
                        GameManagerInst.onRoundWinningsAnnounced(events["roundWinner"], rewardAmount);
                        break;
                    case "PlayerJoined":
                        string joinedPlayer = events["joinedPlayer"];
                        GameManagerInst.onPlayerJoined(joinedPlayer);
                        break;
                }
            }
            else if (currentSceneName == "Lobby") // test
            {
                Lobby LobbyInst = GameObject.Find("BuyTicketScript").GetComponent<Lobby>();

                switch (eventName)
                {
                    case "GameState":
                        LobbyInst.onGameState(events["currentState"]);
                        break;
                }
            }
        };
        
        // Keep sending messages at every 0.3s
        InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
      websocket.DispatchMessageQueue();
#endif
    }

    // after a game has been created let ws server know so it can set up event listeners
    public void SendCreateGameMessage(string gameAddress)
    {
        if (websocket.State == WebSocketState.Open)
        {
            websocket.SendText(gameAddress);
        }
        else {
            Debug.Log("WARNING! Cannot send game address because the websocket is not open!");
        }
    }


    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText("plain text message");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}