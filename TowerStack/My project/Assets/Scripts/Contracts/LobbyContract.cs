using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;

public class LobbyContract : MonoBehaviour
{
    const int WEI_DECIMALS = 18;
    string WeiAdmissionPrice;

    async public Task<string> getUSDTicketPrice()
    {
        // smart contract method to call
        string method = "getUSDTicketPrice";

        // array of arguments for contract
        string args = "[]";

        try
        {
            string usdWeiTicketPrice = await EVM.Call(CryptoJenga.chain, CryptoJenga.network, CryptoJenga.contract, CryptoJenga.abi, method, args);
            Debug.Log("usd wei tikcet price " + usdWeiTicketPrice);
            return "$" + convertWeiStringToEth(usdWeiTicketPrice);

        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
            return ""; // closed game by default
        }
    }

    async public Task<string> getGameState()
    {
        // smart contract method to call
        string method = "getGameState";

        // array of arguments for contract
        string args = "[]";

        try
        {
            string gameState = await EVM.Call(CryptoJenga.chain, CryptoJenga.network, CryptoJenga.contract, CryptoJenga.abi, method, args);

            switch (gameState)
            {
                case "0":
                    return "Initialized";
                case "1":
                    return "Open";
                case "2":
                    return "Chooseroundwinner";
                case "3":
                    return "Calculating_winner";
                case "4":
                    return "Closed";
                default:
                    return "";
            }

        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
            return ""; // closed game by default
        }
    }

    async public Task<bool> isPlayerJoined(string playerAddress)
    {
        // smart contract method to call
        string method = "isPlayerJoined";

        // array of arguments for contract
        try
        {
            Debug.Log("player address " + playerAddress);
            string args = "[\"" + playerAddress + "\"]";
            string isJoined = await EVM.Call(CryptoJenga.chain, CryptoJenga.network, CryptoJenga.contract, CryptoJenga.abi, method, args);
            Debug.Log("is joined " + isJoined);
            return isJoined == "true";
        }
        catch (Exception e)
        {
            Debug.Log("error " + e.Message);
            return false;
        }
    }

    // get ticket price in ETH, the currency you will pay (not USD)
    async public Task<bool> getEthTicketPrice()
    {
        // smart contract method to call
        string method = "TicketPrice";

        // array of arguments for contract
        string args = "[]";

        try
        {
            WeiAdmissionPrice = await EVM.Call(CryptoJenga.chain, CryptoJenga.network, CryptoJenga.contract, CryptoJenga.abi, method, args);
            Debug.Log("Ticket price is " + WeiAdmissionPrice);

            // if # is less  than or equal to 18 places pad it with zeros in front to 18 places

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("error " + e.Message);
            return false;
        }
    }

    // moves decimal point 18 places to the left
    private string convertWeiStringToEth(string weiString)
    {
        string etherPriceText;
        if (weiString.Length <= WEI_DECIMALS)
        {
            int padding = WEI_DECIMALS - weiString.Length;
            etherPriceText = ".";
            for (int p = 0; p < padding; p++)
            {
                etherPriceText += "0";
            }
            etherPriceText += weiString;
        }
        // else put the decimal somewhere in the number
        else
        {
            int decimalPointIdx = weiString.Length - WEI_DECIMALS;
            Debug.Log("decimal point idx " + decimalPointIdx);
            etherPriceText = weiString.Substring(0, decimalPointIdx) + "." + weiString.Substring(decimalPointIdx, 3);
        }
        Debug.Log("ether price " + etherPriceText);
        return etherPriceText;
    }

    // user pays the admission fee in order to enter the game
    async public Task<bool> buyTicket()
    {
        Debug.Log("on buy ticket");
        // smart contract method to call
        string method = "joinGame";

        // array of arguments for contract
        // value in wei
        // gas price OPTIONAL

        string args = "[]";
        // value in wei
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";

        //Debug.Log("buy a ticket for the price " + WeiAdmissionPrice);
        //string response = await Web3GL.SendContract(method, CryptoJenga.abi, CryptoJenga.contract, args, "100000", gasLimit, gasPrice);
        try
        {
            string response = await Web3GL.SendContract(method, CryptoJenga.abi, CryptoJenga.contract, args, WeiAdmissionPrice, gasLimit, gasPrice);

            Debug.Log(response);
            return true;
        }
        catch(Exception e)
        {
            Debug.Log("error buying ticket " + e.Message);
            return false;
        }
    }
}
