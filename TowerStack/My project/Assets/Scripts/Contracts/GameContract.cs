using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameContract : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    async public void Bet(int betAmount)
    {
        // smart contract method to call
        string method = "bet";

        // array of arguments for contract
        // args address _player, uint256 _EthTicketPrice
        string args = "[" + betAmount + "]"; // TODO: Change this when calling the real contract
        // value in wei
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";

        string value = "0";

        string response = await Web3GL.SendContract(method, CryptoJenga.gameAbi, CryptoJenga.gameAddress, args, value, gasLimit, gasPrice);
        Debug.Log(response);
    }

    public async Task<string> GetTokensLeft(string address)
    {
        string method = "getTokensLeft";
        // array of arguments for contract
        string args = "[\""+address+"\"]";
        // connects to user's browser wallet to call a transaction
        string response = await EVM.Call(CryptoJenga.chain, CryptoJenga.network, CryptoJenga.gameAddress, CryptoJenga.gameAbi, method, args);
        // display response in game
        print("Tokens left " + response);
        return response;
    }

    public async Task<List<string>> GetPlayerAddresses()
    {
        string method = "getPlayerAddresses";
        // array of arguments for contract
        string args = "[]";
        // connects to user's browser wallet to call a transaction
        string response = await EVM.Call(CryptoJenga.chain, CryptoJenga.network, CryptoJenga.gameAddress, CryptoJenga.gameAbi, method, args);
        // display response in game
        print("Player addresses " + response);
        List<string> playerAddresses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response);
        print("player addresses " + playerAddresses);
        return playerAddresses;
    }
}
