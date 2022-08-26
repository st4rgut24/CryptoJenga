using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

#if UNITY_WEBGL
public class CryptoJenga: MonoBehaviour
{
    [SerializeField]
    InputField ABIInputField;

    [SerializeField]
    InputField ContractAddressField;

    // REPLACE ME!!!
    //public const string abi = "[ { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"_priceFeedAddress\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"_vrfCoordinator\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"_fee\", \"type\": \"uint256\" }, { \"internalType\": \"bytes32\", \"name\": \"_keyhash\", \"type\": \"bytes32\" }, { \"internalType\": \"uint256\", \"name\": \"_USDTicketPrice\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_roundDuration\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_totalRounds\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_maxBets\", \"type\": \"uint256\" }, { \"internalType\": \"uint64\", \"name\": \"_subscriptionId\", \"type\": \"uint64\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"have\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"want\", \"type\": \"address\" } ], \"name\": \"OnlyCoordinatorCanFulfill\", \"type\": \"error\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_player\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_betCount\", \"type\": \"uint256\" } ], \"name\": \"BetMade\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_gameWinner\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"amountWon\", \"type\": \"uint256\" } ], \"name\": \"GameEnded\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"string\", \"name\": \"_currentState\", \"type\": \"string\" } ], \"name\": \"GameState\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"previousOwner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" } ], \"name\": \"OwnershipTransferred\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_joinedPlayer\", \"type\": \"address\" } ], \"name\": \"PlayerJoined\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"requestId\", \"type\": \"uint256\" } ], \"name\": \"RequestedRandomness\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundNumber\", \"type\": \"uint256\" } ], \"name\": \"RevealEnded\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundNumber\", \"type\": \"uint256\" } ], \"name\": \"RevealStarted\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundNumber\", \"type\": \"uint256\" } ], \"name\": \"RoundEnded\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundNumber\", \"type\": \"uint256\" } ], \"name\": \"RoundStarted\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_roundWinner\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_amountWon\", \"type\": \"uint256\" } ], \"name\": \"RoundWinner\", \"type\": \"event\" }, { \"inputs\": [], \"name\": \"LinkFee\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"RoundStartTime\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"TicketPrice\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"USDTicketPrice\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"betSize\", \"type\": \"uint256\" } ], \"name\": \"bet\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes\", \"name\": \"\", \"type\": \"bytes\" } ], \"name\": \"checkUpkeep\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"upkeepNeeded\", \"type\": \"bool\" }, { \"internalType\": \"bytes\", \"name\": \"\", \"type\": \"bytes\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"choiceWinnerForCurrentRound\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"count\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"gameId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"gameWinner\", \"outputs\": [ { \"internalType\": \"address payable\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"game_state\", \"outputs\": [ { \"internalType\": \"enum cryptoJengaV5.GAME_STATE\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getContractBalance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getLinkBalance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getNumberofPlayers\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getPlayerAddresses\", \"outputs\": [ { \"internalType\": \"address payable[]\", \"name\": \"\", \"type\": \"address[]\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getRoundRemainingTime\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"joinGame\", \"outputs\": [], \"stateMutability\": \"payable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"keyhash\", \"outputs\": [ { \"internalType\": \"bytes32\", \"name\": \"\", \"type\": \"bytes32\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"owner\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes\", \"name\": \"\", \"type\": \"bytes\" } ], \"name\": \"performUpkeep\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"randomness\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"requestId\", \"type\": \"uint256\" }, { \"internalType\": \"uint256[]\", \"name\": \"randomWords\", \"type\": \"uint256[]\" } ], \"name\": \"rawFulfillRandomWords\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"renounceOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"requestRandomWords\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"s_randomWords\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"s_requestId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"startGame\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" } ], \"name\": \"transferOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"withdrawEth\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" } ], \"name\": \"withdrawLINK\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";

    // TEST
    public static string abi = "[ { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"_priceFeedAddress\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"_vrfCoordinator\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"_fee\", \"type\": \"uint256\" }, { \"internalType\": \"bytes32\", \"name\": \"_keyhash\", \"type\": \"bytes32\" }, { \"internalType\": \"uint256\", \"name\": \"_USDTicketPrice\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_roundDuration\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_totalRounds\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_maxBets\", \"type\": \"uint256\" }, { \"internalType\": \"uint64\", \"name\": \"_subscriptionId\", \"type\": \"uint64\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"have\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"want\", \"type\": \"address\" } ], \"name\": \"OnlyCoordinatorCanFulfill\", \"type\": \"error\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_player\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundBetCount\", \"type\": \"uint256\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_remainingBets\", \"type\": \"uint256\" } ], \"name\": \"BetMade\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_gameWinner\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"amountWon\", \"type\": \"uint256\" } ], \"name\": \"GameEnded\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"string\", \"name\": \"_currentState\", \"type\": \"string\" } ], \"name\": \"GameState\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"previousOwner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" } ], \"name\": \"OwnershipTransferred\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_joinedPlayer\", \"type\": \"address\" } ], \"name\": \"PlayerJoined\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"requestId\", \"type\": \"uint256\" } ], \"name\": \"RequestedRandomness\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundNumber\", \"type\": \"uint256\" } ], \"name\": \"RevealEnded\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundNumber\", \"type\": \"uint256\" } ], \"name\": \"RevealStarted\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundNumber\", \"type\": \"uint256\" } ], \"name\": \"RoundEnded\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"_currentRoundNumber\", \"type\": \"uint256\" } ], \"name\": \"RoundStarted\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_roundWinner\", \"type\": \"address\" } ], \"name\": \"RoundWinner\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"address\", \"name\": \"_testAddress\", \"type\": \"address\" } ], \"name\": \"Test\", \"type\": \"event\" }, { \"inputs\": [], \"name\": \"CurrentRound\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"LinkFee\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"MaxBets\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"RoundDuration\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"RoundStartTime\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"TicketPrice\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"TotalRounds\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"USDTicketPrice\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"betSize\", \"type\": \"uint256\" } ], \"name\": \"bet\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes\", \"name\": \"\", \"type\": \"bytes\" } ], \"name\": \"checkUpkeep\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"upkeepNeeded\", \"type\": \"bool\" }, { \"internalType\": \"bytes\", \"name\": \"\", \"type\": \"bytes\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"chooseWinnerForCurrentRound\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"count\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"gameId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"gameWinner\", \"outputs\": [ { \"internalType\": \"address payable\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"game_state\", \"outputs\": [ { \"internalType\": \"enum cryptoJengaV6.GAME_STATE\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getContractBalance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getGameState\", \"outputs\": [ { \"internalType\": \"enum cryptoJengaV6.GAME_STATE\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getLinkBalance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getNumberofPlayers\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getPlayerAddresses\", \"outputs\": [ { \"internalType\": \"address payable[]\", \"name\": \"\", \"type\": \"address[]\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getRoundRemainingTime\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"playerAddress\", \"type\": \"address\" } ], \"name\": \"getTokensLeft\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"getUSDTicketPrice\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"isGameStateOpen\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"playerAddress\", \"type\": \"address\" } ], \"name\": \"isPlayerJoined\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"joinGame\", \"outputs\": [], \"stateMutability\": \"payable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"keyhash\", \"outputs\": [ { \"internalType\": \"bytes32\", \"name\": \"\", \"type\": \"bytes32\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"mockFulfillRandomWords\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"owner\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"participants\", \"outputs\": [ { \"internalType\": \"address payable\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes\", \"name\": \"\", \"type\": \"bytes\" } ], \"name\": \"performUpkeep\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"players\", \"outputs\": [ { \"internalType\": \"address payable\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"randomness\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"requestId\", \"type\": \"uint256\" }, { \"internalType\": \"uint256[]\", \"name\": \"randomWords\", \"type\": \"uint256[]\" } ], \"name\": \"rawFulfillRandomWords\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"renounceOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"resetGameState\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"s_randomWords\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"s_requestId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"startGame\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"testUpkeepCounter\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" } ], \"name\": \"transferOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"vrfCalls\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"winningPlayers\", \"outputs\": [ { \"internalType\": \"address payable\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"withdrawEth\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" } ], \"name\": \"withdrawLINK\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";

    // REPLACE ME !!!
    //public const string contract = "0x568914BFe6de276a4451478dBEf09D6636A48D0F";

    // TEST
    public static string contract = "0x0f24dB4c0490107027abf0C8e5042103E99e147e";

    // set chain: ethereum, moonbeam, polygon etc
    public const string chain = "ethereum";
    // set network mainnet, testnet
    public const string network = "goerli";

    public void onSetAddressField()
    {
        contract = ContractAddressField.text;
    }

    public void onABIInputField()
    {
        abi = ABIInputField.text;
    }
}
#endif