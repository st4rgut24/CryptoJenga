using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ContractForm : MonoBehaviour
{
    public GameObject LoadText;

    public InputField MaxBet;
    public InputField TotalRound;
    public InputField EntryFee;
    public InputField GameCode;
    public InputField RoundDuration;

    Connection connection;

    // Start is called before the first frame update
    void Start()
    {
        LoadText.SetActive(false);
        connection = GameObject.Find("Network").GetComponent<Connection>();
    }

    private bool isInputValid()
    {
        return MaxBet.text.Length != 0 && GameCode.text.Length != 0 && TotalRound.text.Length != 0 && EntryFee.text.Length != 0 && RoundDuration.text.Length != 0;
    }

    public async void onCreateContract()
    {
        if (isInputValid())
        {
            try
            {
                int maxBetCount = Int32.Parse(MaxBet.text);
                int totalRound = Int32.Parse(TotalRound.text);
                int roundDuration = Int32.Parse(RoundDuration.text);

                float entryFee = float.Parse(EntryFee.text);
                float decimals = 1000000000000000000; // 18 decimals
                float wei = entryFee * decimals;
                string USDToWei = Convert.ToDecimal(wei).ToString();
                Debug.Log("USD to wei is " + USDToWei);

                if (maxBetCount <= 0 && totalRound <= 0 && entryFee < 0)
                {
                    throw new FormatException("Invalid values");
                }
                if (connection != null)
                {
                    LoadText.SetActive(true);
                    StartCoroutine(CryptoJenga.createGame(connection, USDToWei, roundDuration, totalRound, maxBetCount, GameCode.text));
                }
            }
            catch (FormatException e)
            {
                Debug.Log("Format exception in input fields " + e.Message);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
