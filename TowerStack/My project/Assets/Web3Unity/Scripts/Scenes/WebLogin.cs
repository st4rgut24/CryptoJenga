using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

#if UNITY_WEBGL
public class WebLogin : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    private string account;

    public InputField GameCodeInput;

    public async void OnPlay()
    {
        Debug.Log("Attempt to nter Game with code " + GameCodeInput.text);

        string gameAddress = await CryptoJenga.joinGame(GameCodeInput.text);
        if (gameAddress != null)
        {
            await ConnectToWallet();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public async void OnCreateGame()
    {
        await ConnectToWallet();
        SceneManager.LoadScene("ContractForm");
    }

    async private Task<bool> ConnectToWallet()
    {
        string accountInfo = PlayerPrefs.GetString("Account", null);
        Debug.Log("account info " + accountInfo);
        //if (accountInfo == null)
        //{
            Web3Connect();
            await OnConnected();
        //}
        Debug.Log("after on connected");
        return true;
    }

    async private Task<bool> OnConnected()
    {
        Debug.Log("attempt to connect");

        account = ConnectAccount();
        Debug.Log("attempt to connect 2");
        while (account == "")
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
            Debug.Log("attempt to connect3");
        };
        // save account for next scene
        PlayerPrefs.SetString("Account", account);
        // reset login message
        SetConnectAccount("");
        // load next scene
        return true;
    }


    public void OnExit()
    {
        Application.Quit();
    }

    public void OnSkip()
    {
        // burner account for skipped sign in screen
        PlayerPrefs.SetString("Account", "");
        // move to next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
#endif
