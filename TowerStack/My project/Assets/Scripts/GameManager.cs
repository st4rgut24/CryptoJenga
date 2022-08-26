using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Text TokenCounter;

    public Transform CoinHolder;
    public Transform CoinTubeHolder;

    public GameObject Coin;
    public GameObject CoinTube;
    public GameObject PlayerContainer; // width is 55

    public Text GameStatusText;
    public Text CurrentRoundText;
    public Button BetButton;

    public GameContract GameContractInst;

    List<Player> PlayerList;

    public Canvas PlayerCanvas; // width is 640

    const float PlayerContainerWidth = 100;
    const float padding = 50; // If you make the padding smaller, neighboring coin tube colliders
                              // will overlap and prevent the coins from falling

    const float COIN_DROP_DELAY = 1.5f;
    const float END_GAME_COIN_DROP_DELAY = 3;

    const float CANVAS_OFFSET = 7f;
    int lastCoinDroppedTimestamp = 0;

    int currentRound;
    int gameId = 0;

    List<string> PlayerAddressList; // store the actual addresses instead of the shortened addresses
    int playerIdx = 0;

    List<int> Round1WinnerOrder = new List<int> { 0, 5, 3, 1, 4, 2, 6 };
    List<int> Round2WinnerOrder = new List<int> { 2,5,1,4,3,0, 6 };
    List<int> Round3WinnerOrder = new List<int> { 3,1,5,0,2,3, 6 };
    List<List<int>> RoundWinners = new List<List<int>>();


    void Start()
    {
        PlayerList = new List<Player>();
        currentRound = 0;
        getPlayers(); // Get the players who already paid the admission fee
        getTokensLeft();
    }

    // when user exits game load main screen
    public void onExit()
    {
        SceneManager.LoadScene("Lobby");
    }

    //event RequestedRandomness(bytes32 requestId);

    //event BetMade(address _player, uint256 _EthTicketPrice);
    public void onBetMade(string address, string currentRoundBetCount, string remainingBets)
    {
        // update the bet count UI underneath a person's stack
        Player player = GameManagerHelper.findPlayer(PlayerList, address);
        Transform WagerText = GameManagerHelper.RecursiveFindChild(player.ContainerInst.transform, "WagerText");
        Text WagerTextComponent = WagerText.gameObject.GetComponent<Text>();
        WagerTextComponent.text = "x" + currentRoundBetCount;
        Debug.Log("address received is " + address + " player prefs address is " + PlayerPrefs.GetString("Account"));   
        if (GameManagerHelper.IsAddressEqual(address, PlayerPrefs.GetString("Account")))
        {
            TokenCounter.text = remainingBets;
        }
    }

    public void onReceiveMessage(string test)
    {
    }

    //event GameState(string _currentState);
    public void onGameState(string currentState)
    {
        if (currentState == "Open")
        {
            GameStatusText.text = "Open For Bets";

            BetButton.interactable = true;
        }
        else if (currentState == "Calculating Winner")
        {
            GameStatusText.text = "Calculating Winner";
            BetButton.interactable = false;
        }
        // the game has ended
        else if (currentState == "Closed")
        {
            GameStatusText.text = "Game Over";
            BetButton.interactable = false;
        }
    }
    //event RoundStarted(uint256 _currentRoundNumber);
    public void onRoundStarted(int currentRoundNumber)
    {
        BetButton.interactable = true;
        currentRound = currentRoundNumber;
        CurrentRoundText.text = "Round " + currentRoundNumber;
        GameStatusText.text = "Open For Bets";
        PlayerList.ForEach((Player player) =>
        {
            Transform WagerText = GameManagerHelper.RecursiveFindChild(player.ContainerInst.transform, "WagerText");
            Text WagerTextComponent = WagerText.gameObject.GetComponent<Text>();
            WagerTextComponent.text = "x0"; // reset the bets at the start of the round
        });
    }

    public void onRoundEnded()
    {
        BetButton.interactable = false;
        GameStatusText.text = "Calculating Winnings";
    }

    // distributes the winnings in a round (excluding the last round)
    public void onRoundWinningsAnnounced(string address, int rewardAmount)
    {
        Debug.Log("round winner announce. Rewarding address " + address + " " + rewardAmount + " token");
        StartCoroutine(rewardPlayer(address, rewardAmount));
    }

    // when final round ends the game ended, and the winner is announced
    public void onGameEnded(string address, int totalWinnings)
    {
        StartCoroutine(endGame(address, totalWinnings));
    }

    // updates the number of players and positions the player UI in canvas
    // @playerCount total number of players in the game
    void initPlayerContainers(List<string> playerAddressList)
    {
        clearPlayerList();

        float totalWidth = playerAddressList.Count * PlayerContainerWidth + (playerAddressList.Count - 1) * padding;
        for (int i = 0; i < playerAddressList.Count; i++)
        {
            string playerAddress = playerAddressList[i];
            float offset = (PlayerContainerWidth + padding) * i + (PlayerContainerWidth/2);
            float centerOffset = totalWidth / 2;
            float xPos = offset - centerOffset;

            GameObject PlayerContainerInst = Instantiate(PlayerContainer, PlayerCanvas.transform);


            PlayerContainerInst.transform.localPosition = new Vector2(xPos, 0);
            GameObject CoinTubeInst = Instantiate(CoinTube, CoinTubeHolder);

            Transform addressTextTransform = PlayerContainerInst.transform.Find("AddressText");
            addressTextTransform.GetComponent<Text>().text = GameManagerHelper.shortenAddress(playerAddress);

            PlayerList.Add(new Player(PlayerContainerInst, playerAddress, CoinTubeInst));

            Vector3 uiWorldPos = PlayerContainerInst.transform.position;
            CoinTubeInst.transform.position = new Vector3(uiWorldPos.x, -20, uiWorldPos.z - CANVAS_OFFSET);

        }
    }

    // When adding a new player need to reinitialize the UIs associated with players, to redraw the screen
    void clearPlayerList()
    {
        PlayerList.ForEach((Player PlayerInst) =>
        {
            Destroy(PlayerInst.ContainerInst);
            Destroy(PlayerInst.CoinTubeInst);
        });
        PlayerList.Clear();
    }

    // TEST
    void testRewardAllPlayers(List<int>RoundWinner)
    {
        int reward = 1;

        RoundWinner.ForEach((int roundWinnerIdx) =>
        {
            string playerAddress = PlayerAddressList[roundWinnerIdx];
            StartCoroutine(rewardPlayer(playerAddress, 1));
        });
        Debug.Log("done testing?");
    }

    // TEST
    IEnumerator testPlayersJoin()
    {
        initPlayerContainers(PlayerAddressList);
        yield return new WaitForSeconds(5); // wait for all players to be rewarded before ending the game

        //PlayerAddressList.Add("0x5AD..D9C");
        PlayerAddressList.Add("0x41D..D92");
        PlayerAddressList.Add("0xAD2..N9G");
        initPlayerContainers(PlayerAddressList);
    }

    // TEST
    IEnumerator testEndGame()
    {
        Debug.Log("Test end game");
        RoundWinners.Add(Round1WinnerOrder);
        RoundWinners.Add(Round2WinnerOrder);
        RoundWinners.Add(Round3WinnerOrder);

        initPlayerContainers(PlayerAddressList);

        // test game
        for (int i = 0; i < 3; i++)
        {
            int currentRound = i + 1;
            Debug.Log("round is " + currentRound);
            CurrentRoundText.text = "Round " + currentRound.ToString();

            yield return new WaitForSeconds(15);
            GameStatusText.text = "Calculating Round Winner";
            //yield return new WaitForSeconds(15);
            BetButton.interactable = false;
            List<int> RoundWinner;
            if (i == 0)
            {
                RoundWinner = Round1WinnerOrder;
            }
            else if (i == 1)
            {
                RoundWinner = Round2WinnerOrder;
            }
            else
            {
                RoundWinner = Round3WinnerOrder;
            }
            //yield return new WaitForSeconds(15); // wait for all players to be rewarded before ending the game
            testRewardAllPlayers(RoundWinner);
            yield return new WaitForSeconds(10); // wait for all players to be rewarded before ending the game
            GameStatusText.text = "Open For Bets";
            // place bets here]

        }
        GameStatusText.text = "Decided Game Winner";
        string winnerAddress = PlayerAddressList[5];
        StartCoroutine(endGame(winnerAddress, 100));
    }

    // get total tokens belonging to players
    public async void getTokensLeft()
    {
        try
        {
            Debug.Log("Getting tokens left...");
            string tokensLeft = await GameContractInst.GetTokensLeft(PlayerPrefs.GetString("Account"));
            TokenCounter.text = tokensLeft;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    // calls smart contract to get a list of players already in the game when a player joins
    public async void getPlayers()
    {
        try
        {
            Debug.Log("Getting player addresses ...");
            PlayerAddressList = await GameContractInst.GetPlayerAddresses();
            // TODO: Parse response and get all player addresses here
            Debug.Log("init PlayerAddressList Here using response: " + PlayerAddressList);
            initPlayerContainers(PlayerAddressList);
            PlayerAddressList.ForEach((string playerAddr) => addPlayerToList(playerAddr));
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    // once inside the game, if a player joined event is received, reinitialize all the containers
    public void onPlayerJoined(string newPlayerAddr)
    {
        Debug.Log("on player joined " + newPlayerAddr);
        addPlayerToList(newPlayerAddr);
    }

    private void addPlayerToList(string newPlayerAddr)
    {
        if (!PlayerAddressList.Contains(newPlayerAddr))
        {
            PlayerAddressList.Add(newPlayerAddr);
            initPlayerContainers(PlayerAddressList);
        }
    }

    // when the user places a bet, they use one of pre-allocated bets. It is possible
    // to place multiple bets in one round to increase chance of winning. Until a
    // input field for number of bets is created, clicking the bet button will increment the bets by one
    public void placeBet() // TODO: param called betAmount
    {
        try
        {
            //if (float.TryParse(BetTextInput.text, out value))
            GameContractInst.Bet(1); // for testing
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    // when a player loses their tower of ocins will topple and be lost
    IEnumerator toppleTower(string playerAddress, bool isendgame)
    {
        Player PlayerInst = GameManagerHelper.findPlayer(PlayerList, playerAddress);
        if (PlayerInst.CoinTubeInst != null)
        {
            PlayerInst.CoinTubeInst.SetActive(false);
        }
        if (!isendgame)
        {
        }
        else
        {
            yield return new WaitForSeconds(15);
        }
        PlayerInst.CoinTower.ForEach((GameObject CoinInst) =>
        {
            Destroy(CoinInst);
        });
        PlayerInst.clearCoinTower();
    }

    // award players who won with coins
    IEnumerator rewardPlayer(string playerAddress, int winnings)
    {
        Player PlayerInst = GameManagerHelper.findPlayer(PlayerList, playerAddress);
        PlayerInst.CoinTubeInst.SetActive(true);

        for (int i = 0; i < winnings; i++)
        {
            int sec = PlayerInst.getTimeDiff();
            while (sec < COIN_DROP_DELAY)
            {
                 sec = PlayerInst.getTimeDiff();
                yield return new WaitForEndOfFrame();
            }
            PlayerInst.setTimestamp();
            GameObject CoinInst = Instantiate(Coin, CoinHolder);
            lastCoinDroppedTimestamp = GameManagerHelper.getCurrentTimestamp();
            Vector3 uiWorldPos = PlayerInst.ContainerInst.transform.position;
            CoinInst.transform.position = new Vector3(uiWorldPos.x, 70, uiWorldPos.z - CANVAS_OFFSET);
            PlayerInst.incrementToken(CoinInst);
        }
    }


    // ends the game in a winner-take-all scheme
    IEnumerator endGame(string winnerAddress, int totalWinnings)
    {
        List<string> LoserAddressList = new List<string>(PlayerAddressList);
        bool isWinnerRemoved = LoserAddressList.Remove(winnerAddress);
        Debug.Log("is winner removed " + isWinnerRemoved);
        // wait until the last player's coin dropped to show the end game animation
        while (GameManagerHelper.getCurrentTimestamp() - lastCoinDroppedTimestamp < END_GAME_COIN_DROP_DELAY)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("ready to begin winner animation");
        if (isWinnerRemoved)
        {
            List<Player> LosingPlayers = GameManagerHelper.getPlayerListFromAddresses(PlayerList, LoserAddressList);
            LosingPlayers.ForEach((Player PlayerInst) =>
            {
                StartCoroutine(toppleTower(PlayerInst.address, true));
            });
            StartCoroutine(rewardPlayer(winnerAddress, totalWinnings));
        }
    }
}
