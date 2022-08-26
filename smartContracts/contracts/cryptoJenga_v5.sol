// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@chainlink/contracts/src/v0.8/interfaces/LinkTokenInterface.sol";
import "@chainlink/contracts/src/v0.8/interfaces/AggregatorV3Interface.sol";
import "@chainlink/contracts/src/v0.8/interfaces/VRFCoordinatorV2Interface.sol";
import "@chainlink/contracts/src/v0.8/VRFConsumerBaseV2.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/cryptography/ECDSA.sol";
import "@openzeppelin/contracts/utils/Counters.sol";
import "@chainlink/contracts/src/v0.8/interfaces/KeeperCompatibleInterface.sol";

contract cryptoJengaV5 is VRFConsumerBaseV2, KeeperCompatibleInterface, Ownable {
    VRFCoordinatorV2Interface COORDINATOR;

    uint256 public USDTicketPrice; // 18 places of precision
    address payable[] players;
    address payable[] winningPlayers;
    uint256 public randomness;
    AggregatorV3Interface internal ethUsdPriceFeed;
    LinkTokenInterface LINKTOKEN;
    address link_token_contract = 0x326C977E6efc84E512bB9C30f76E30c160eD06FB;
    uint256 public RoundStartTime;
    uint public immutable RoundDuration = 60; // in seconds
    uint256 TotalRounds;
    uint256 CurrentRound;
    uint256 MaxBets;
    uint256 public s_requestId;
    uint256 public count = 0;
    uint256 public gameId = 0;

    uint256 public testUpkeepCounter = 0;
    // tolerance for determining if ticket purchase goes through 
    uint256 private slippage = 1000000;

    uint256[] public s_randomWords;

    using ECDSA for bytes32;
    mapping(uint256 => mapping(uint256 => mapping ( address => bool))) placedBet; 
    mapping(uint256 => mapping(uint256 => mapping ( address => Bet[]))) bets; //bets[gameId][roundNumber][playerAddress]
    mapping(address=>uint256) blocksWon;

    mapping(uint256 => uint256) GameWinningPool; //gameId => amount
    mapping(uint256 => uint256) GameFeePool; // gameId => amount

    address payable [] participants;
    mapping(address => bool) participated;
    address payable public gameWinner;

    struct Bet {
        uint256 betAmount;
    }

    enum GAME_STATE {
        INITIALIZED,
        OPEN,
        CHOOSEROUNDWINNER,
        CALCULATING_WINNER,
        CLOSED
    }

    GAME_STATE public game_state;
    uint256 public LinkFee;
    uint64 s_subscriptionId;
    bytes32 public keyhash;

    uint16 requestConfirmations = 3;
    uint32 numWords =  2;
    uint32 callbackGasLimit = 100000;

    event RequestedRandomness(uint256 requestId);
    event BetMade(address _player, uint256 _betCount);
    event GameState(string _currentState);
    event RoundStarted(uint256 _currentRoundNumber);
    event RoundEnded(uint256 _currentRoundNumber);
    event RevealStarted(uint256 _currentRoundNumber);
    event RevealEnded(uint256 _currentRoundNumber);
    event PlayerJoined(address _joinedPlayer);
    event GameEnded(address _gameWinner, uint256 amountWon);
    event RoundWinner(address _roundWinner, uint256 _amountWon);
    event Test();

    constructor(
        // address _priceFeedAddress,
        address _vrfCoordinator
        // uint256 _fee,
        // bytes32 _keyhash,
        // uint256 _USDTicketPrice, // to 18 places
        // uint256 _roundDuration,
        // uint256 _totalRounds,
        // uint256 _maxBets,
        // uint64 _subscriptionId
    ) VRFConsumerBaseV2(_vrfCoordinator){

        
        // USDTicketPrice = _USDTicketPrice;
        // ethUsdPriceFeed = AggregatorV3Interface(_priceFeedAddress);
        // game_state = GAME_STATE.INITIALIZED;
        // LinkFee = _fee;
        // MaxBets = _maxBets;
        // keyhash = _keyhash;
        // s_subscriptionId = _subscriptionId;
        // LINKTOKEN = LinkTokenInterface(link_token_contract);

        // RoundDuration = _roundDuration;
        // TotalRounds = _totalRounds;
        // COORDINATOR = VRFCoordinatorV2Interface(_vrfCoordinator);

        USDTicketPrice = 10;
        ethUsdPriceFeed = AggregatorV3Interface(0xD4a33860578De61DBAbDc8BFdb98FD742fA7028e);
        game_state = GAME_STATE.INITIALIZED;
        LinkFee = 100000000000000000;
        MaxBets = 5;
        keyhash = 0x79d3d8832d904592c0bf9818b621522c988bb8b0c05cdc3b15aea1b6e8db0c15;
        s_subscriptionId = 72;
        LINKTOKEN = LinkTokenInterface(link_token_contract);
        TotalRounds = 3;
        
        COORDINATOR = VRFCoordinatorV2Interface(_vrfCoordinator);        
    }
    
    // get the remaining number of times a player can bet in the game
    function getRemainingBets(address playerAddr) internal view returns (uint256) {
        Bet[] memory playerBets = bets[gameId][CurrentRound][playerAddr];
        uint256 totalBets = 0;
        for (uint i=0;i<playerBets.length;i++){
            totalBets += playerBets[i].betAmount;
        }
        return MaxBets - totalBets;
    }

    // before the game starts, pay admission fee to join
    function joinGame() public payable {
        require(game_state == GAME_STATE.INITIALIZED, "Can't join a game after it has started");
        require(msg.value >= (TicketPrice() - slippage), "Not enough ETH");
        if ( ! participated[msg.sender])
        {
            participated[msg.sender] = true;
            participants.push(payable(msg.sender));
        }        
        uint256 amountGotoWinningPool = msg.value * 90 / 100;
        GameWinningPool[gameId] += amountGotoWinningPool;
        GameFeePool[gameId] += (msg.value - amountGotoWinningPool);     
        emit PlayerJoined(msg.sender);   
    }

    function bet(        
        uint256 betSize
    ) public {
        require(game_state==GAME_STATE.OPEN);
        require( (block.timestamp - RoundStartTime) < RoundDuration, "You are too late for this round");
        uint256 remainingBets = getRemainingBets(msg.sender);
        require(betSize <= remainingBets, "You don't have enough bets left");
        placedBet[gameId][CurrentRound - 1][msg.sender] = true;
        (bets[gameId][CurrentRound - 1][msg.sender]).push(Bet({betAmount: betSize}));

        players.push(payable(msg.sender)); // if player makes more than 1 bet, name is entered multiple times like lottery tickets
        emit BetMade(msg.sender, betSize);
    }

    function testEmit() public onlyOwner {
        emit Test();
    }

    function startGame() public onlyOwner {
        require(game_state == GAME_STATE.INITIALIZED, "Can't start a new game");
        game_state = GAME_STATE.OPEN;
        RoundStartTime = block.timestamp;
        CurrentRound = 1;
        emit GameState("Open");
        emit RoundStarted(CurrentRound);
    }

    function isGameStateOpen() public returns (bool) {
        return game_state == GAME_STATE.OPEN;
    }

    function TicketPrice() public view returns (uint256){
        (, int256 price, , , ) = ethUsdPriceFeed.latestRoundData();
        uint256 adjustedUSDPrice = uint256(price); // USD price for 1 eth to 8 places of precision
        uint256 preciseUSDTicketPrice = USDTicketPrice * 10**26;
        uint256 weiCostToEnter = preciseUSDTicketPrice / (adjustedUSDPrice * 10**18); // cost in ETH to 10 places of precision
        return weiCostToEnter;
    }

    // Assumes the subscription is funded sufficiently.
    function requestRandomWords() external onlyOwner {
        requestRandomWordsInternal();
    }

    // Assumes the subscription is funded sufficiently.
    function requestRandomWordsInternal() internal onlyOwner returns (uint256 requestId) {
        // Will revert if subscription is not set and funded.
        COORDINATOR.requestRandomWords(
        keyhash,
        s_subscriptionId,
        requestConfirmations,
        callbackGasLimit,
        numWords
        );
    }    

    function choiceWinnerForCurrentRound() public {
        require(
            game_state == GAME_STATE.CHOOSEROUNDWINNER,
            "You aren't there yet!"
        );
        
        uint256 requestId = requestRandomWordsInternal();
        emit RequestedRandomness(requestId);
        emit GameState("Calculating winner");
        emit RoundEnded(1);
        emit RevealStarted(1);
    }

    function fulfillRandomWords(
      uint256, /* requestId */
      uint256[] memory randomWords
    ) internal override {
        require(
            game_state == GAME_STATE.CHOOSEROUNDWINNER,
            "You aren't there yet!"
        );
        require(randomWords.length > 0, "random words not found");
        uint256 _randomness = randomWords[0];

        uint numberOfPlayers = players.length;
        uint256 indexOfWinner = _randomness % numberOfPlayers;

        for (uint i=0; i < numberOfPlayers; i++)
        {
            Bet[] memory betsForPlayer = bets[gameId][CurrentRound - 1][players[i]];
            for (uint j = 0; j < betsForPlayer.length; j ++)
            {
                uint256 amountWon = numberOfPlayers - i;
                uint256 winnerIdx = (indexOfWinner + j) % numberOfPlayers;
                address winnerAddress = players[winnerIdx];
                blocksWon[players[winnerIdx]] += amountWon;
                emit RoundWinner(winnerAddress, amountWon);
            }
        }

        // Reset
        players = new address payable[](0);

        if ( CurrentRound == TotalRounds)
        {
            // End the game
            game_state = GAME_STATE.CLOSED;
            randomness = _randomness;

            gameWinner = participants[0];
            uint256 highestBlocksWon = blocksWon[participants[0]];
            for (uint i = 1; i < participants.length; i++)
            {
                if (blocksWon[participants[i]] > highestBlocksWon) {
                    highestBlocksWon = blocksWon[participants[i]];
                    gameWinner = participants[i];
                }

            }
            
            gameWinner.transfer(GameWinningPool[1]);

            emit RevealEnded(CurrentRound);
            emit GameState("Closed");
            emit GameEnded(gameWinner, GameWinningPool[1]);
        } 
        else
        {
            // start new round
            CurrentRound += 1;
            RoundStartTime = block.timestamp;
            game_state = GAME_STATE.OPEN;
            emit RoundStarted(CurrentRound);
        } 
    }

    function getLinkBalance() external view returns (uint256){
        return LINKTOKEN.balanceOf(address(this));
    }

    function withdrawEth() external onlyOwner{
        payable(msg.sender).transfer(address(this).balance);
    }

    function withdrawLINK(uint256 amount, address to) external onlyOwner {
        LINKTOKEN.transfer(to, amount);
    }
    function getContractBalance() external view returns(uint256){
        return address(this).balance;
    }
    function getNumberofPlayers() external view returns (uint256){
        return players.length;
    }

    function checkUpkeep(bytes calldata /* checkData */) external view override returns (bool upkeepNeeded, bytes memory /* performData */) {
        upkeepNeeded = (game_state == GAME_STATE.OPEN) && (block.timestamp - RoundStartTime) > RoundDuration;
        // We don't use the checkData in this example. The checkData is defined when the Upkeep was registered.
    }    

    function performUpkeep(bytes calldata /* performData */) external override{
        //We highly recommend revalidating the upkeep in the performUpkeep function
        if (game_state == GAME_STATE.OPEN && (block.timestamp - RoundStartTime) > RoundDuration ) {
            RoundStartTime = block.timestamp;
            game_state = GAME_STATE.CHOOSEROUNDWINNER;
            choiceWinnerForCurrentRound();
        }
    }

    function getRoundRemainingTime() external view returns (uint256){
        if (( RoundStartTime + RoundDuration) < block.timestamp) 
        {
            return 0;
        }
        return ( RoundStartTime + RoundDuration) - block.timestamp;
    }

    function getPlayerAddresses()public view returns( address payable [] memory){
        return participants;
    }
}
