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

contract cryptoJengaV6 is VRFConsumerBaseV2, KeeperCompatibleInterface, Ownable {
    VRFCoordinatorV2Interface COORDINATOR;

    uint256 public USDTicketPrice;
    address payable[] public players;
    uint256 public randomness;
    AggregatorV3Interface internal ethUsdPriceFeed;
    LinkTokenInterface LINKTOKEN;
    address link_token_contract = 0x326C977E6efc84E512bB9C30f76E30c160eD06FB;
    uint256 public RoundStartTime;
    uint public RoundDuration; // in seconds
    uint256 public TotalRounds;
    uint256 public CurrentRound;
    uint256 public MaxBets;
    uint256 public s_requestId;
    uint256 public count = 0;
    uint256 public gameId = 0;
    uint256 public vrfCalls = 0;
    uint256 public poolRewards;
    uint256 public finalBalance;
    uint256 public testUpkeepCounter = 0;
    // tolerance for determining if ticket purchase goes through 
    uint256 private slippage = 1000000;

    uint256 totalWinnings;
    uint256 highestBlocksWon;
    uint256[] public s_randomWords;
    address payable public gameWinner;

    using ECDSA for bytes32;
    mapping(uint256 => mapping(uint256 => mapping ( address => uint256))) betCounts; //bets[gameId][roundNumber][betCount per round]
    
    mapping(address=>uint256) public blocksWon;

    mapping(uint256 => uint256) public GameWinningPool; //gameId => amount
    mapping(uint256 => uint256) public GameFeePool; // gameId => amount

    address payable[] public participants;
    mapping(address => bool) participated;

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
    uint32 callbackGasLimit = 200000;

    event RequestedRandomness(uint256 requestId);
    event BetMade(address _player, uint256 _currentRoundBetCount, uint256 _remainingBets);
    event GameState(string _currentState);
    event RoundStarted(uint256 _currentRoundNumber);
    event RoundEnded(uint256 _currentRoundNumber);
    event RevealStarted(uint256 _currentRoundNumber);
    event RevealEnded(uint256 _currentRoundNumber);
    event PlayerJoined(address _joinedPlayer);
    event GameEnded(address _gameWinner, uint256 amountWon);
    event RoundWinner(address _roundWinner, uint256 _rewardAmount);
    event Test(address _testAddress);

    constructor(
        address _priceFeedAddress,
        address _vrfCoordinator,
        uint256 _fee,
        bytes32 _keyhash,
        uint256 _USDTicketPrice, // to 18 places
        uint256 _roundDuration,
        uint256 _totalRounds,
        uint256 _maxBets,
        uint64 _subscriptionId
    ) VRFConsumerBaseV2(_vrfCoordinator){
        USDTicketPrice = _USDTicketPrice;
        ethUsdPriceFeed = AggregatorV3Interface(_priceFeedAddress);
        game_state = GAME_STATE.INITIALIZED;
        LinkFee = _fee;
        MaxBets = _maxBets;
        keyhash = _keyhash;
        s_subscriptionId = _subscriptionId;
        LINKTOKEN = LinkTokenInterface(link_token_contract);

        RoundDuration = _roundDuration;
        TotalRounds = _totalRounds;
        COORDINATOR = VRFCoordinatorV2Interface(_vrfCoordinator);      
    }

    // get the state of the game
    function getUSDTicketPrice() public view returns(uint256) {
        return USDTicketPrice;
    }

    // get the state of the game
    function getGameState() public view returns(GAME_STATE) {
        return game_state;
    }

    function getTokensLeft(address playerAddress) public view returns(uint256){
        uint256 totalBets = getTotalBets(msg.sender);
        uint256 remainingBets = MaxBets - totalBets;
        return remainingBets;
    }

    // has player paid for joining the game yet?
    function isPlayerJoined(address playerAddress) public view returns(bool) {
        return participated[playerAddress];
    }

    // get the remaining number of times a player can bet in the game
    function getTotalBets(address playerAddr) internal view returns (uint256) {
        uint256 totalBets = 0;
        for (uint j=0;j<CurrentRound;j++){
            totalBets += betCounts[gameId][j][msg.sender];
        }
        return totalBets;
    }

    // before the game starts, pay admission fee to join
    function joinGame() public payable {
        require(game_state == GAME_STATE.INITIALIZED, "Can't join a game after it has started");
        require(msg.value >= (TicketPrice() - slippage), "Not enough ETH");
        if (!participated[msg.sender])
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
        uint256 totalBets = getTotalBets(msg.sender);
        uint256 remainingBets = MaxBets - totalBets - betSize;
        require(betSize >= 0, "You don't have enough bets left");

        uint256 currentRoundBetCount = betCounts[gameId][CurrentRound - 1][msg.sender];
        betCounts[gameId][CurrentRound - 1][msg.sender] = currentRoundBetCount + betSize;
        players.push(payable(msg.sender)); // if player makes more than 1 bet, name is entered multiple times like lottery tickets
        
        emit BetMade(msg.sender, currentRoundBetCount+betSize, remainingBets);
    }

    function startGame() public onlyOwner {
        require(game_state == GAME_STATE.INITIALIZED, "Can't start a new game");
        require(participants.length > 0, "There must be players in the game"); // <-- dont start game unless more than 1 player
        game_state = GAME_STATE.OPEN;
        RoundStartTime = block.timestamp;
        CurrentRound = 1;
        emit GameState("Open");
        emit RoundStarted(CurrentRound);
    }

    // for testing only. Reset the game state
    function resetGameState(uint256 _TotalRounds, uint256 _MaxBets, uint256 _RoundDuration, uint256 _USDTicketPrice) public {
        game_state = GAME_STATE.INITIALIZED;
        for (uint p=0;p<participants.length;p++){
            address payable participantAddress = participants[p];
            for (uint r=0;r<TotalRounds;r++){
                delete betCounts[gameId][r][participantAddress];
            }
            delete participated[participantAddress];
            delete blocksWon[participantAddress];
        }
        GameWinningPool[gameId] = 0;
        participants = new address payable[](0);
        players = new address payable[](0);   
        TotalRounds = _TotalRounds; 
        MaxBets = _MaxBets;
        RoundDuration = _RoundDuration;
        USDTicketPrice = _USDTicketPrice;
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

    function chooseWinnerForCurrentRound() public {        
        // Will revert if subscription is not set and funded.
        uint256 requestId = COORDINATOR.requestRandomWords(
        keyhash,
        s_subscriptionId,
        requestConfirmations,
        callbackGasLimit,
        numWords
        );
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
        require(players.length > 0 || CurrentRound == TotalRounds, "There must be bets in the round");
        uint256 _randomness = randomWords[0];

        if (players.length > 0){
            uint256 roundBetsLength = players.length; // 1
            uint256 indexOfWinner = _randomness % roundBetsLength; // 0 ... roundBetsLength - 1
            uint256 endIdx = roundBetsLength + indexOfWinner;
            
            uint256 rewardAmount = roundBetsLength;
            for (uint j=indexOfWinner;j<endIdx;j++){
                uint256 bettorIdx = j % roundBetsLength;
                blocksWon[players[bettorIdx]] += 1;
                emit RoundWinner(players[bettorIdx], rewardAmount);
                rewardAmount--;
            }            
        }

        // // Reset
        players = new address payable[](0);

        if ( CurrentRound == TotalRounds)
        {
            // End the game
            game_state = GAME_STATE.CLOSED;

            gameWinner = participants[0];
            // Commented code is causing the bug
            uint256 highestBlocksWon = blocksWon[participants[0]];
            totalWinnings = highestBlocksWon;
            
            for (uint i = 1; i < participants.length; i++)
            {
                // check if exists?
                address payable participantAddr = participants[i];
                uint256 b = blocksWon[participantAddr];
                if (b > highestBlocksWon) {
                    highestBlocksWon = b;
                    gameWinner = participantAddr;
                }
                totalWinnings += blocksWon[participantAddr];
            }

            totalWinnings -= highestBlocksWon; // dont include winners existing winngings in the tokens awarded
            poolRewards = GameWinningPool[gameId];
            GameWinningPool[gameId] = 0; // prevent re-entrancy


            (bool success, ) = gameWinner.call{value: poolRewards}("");
            emit RevealEnded(CurrentRound);
            emit GameState("Closed");
            emit GameEnded(gameWinner, totalWinnings);
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
            game_state = GAME_STATE.CHOOSEROUNDWINNER;
            chooseWinnerForCurrentRound();
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