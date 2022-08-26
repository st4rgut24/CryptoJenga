using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<GameObject> CoinTower { get; private set; }
    public GameObject ContainerInst { get; private set; }
    public GameObject CoinTubeInst { get; private set; }

    public string address { get; private set; }
    // how many tokens a player has left in the game
    public int tokenCount { get; private set; }
    // how many tokens a player bet in a round
    public int betCount { get; private set; }
    public int coinDropTimestamp = 0; // seconds since UNIX time

    public Player(GameObject PlayerContainer, string address, GameObject CoinTubeHolder)
    {
        ContainerInst = PlayerContainer;
        this.address = address;
        CoinTower = new List<GameObject>();
        CoinTubeInst = CoinTubeHolder;
    }

    public void setTimestamp()
    {
        coinDropTimestamp = GameManagerHelper.getCurrentTimestamp();
    }

    // get time diff in seconds
    public int getTimeDiff()
    {
        return GameManagerHelper.getCurrentTimestamp() - coinDropTimestamp;
    }

    public void clearCoinTower()
    {
        this.CoinTower = new List<GameObject>();
    }

    public void incrementToken(GameObject token)
    {
        CoinTower.Add(token);
    }
}
