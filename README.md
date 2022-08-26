# CryptoJenga
This is a multiplayer betting game by allowing users to participate in round-by-round play where each round has specified duration (when game is created).

The front-end is built using Unity game engine and there is a backend monitoring the events emitted by the smart contract.

### Testing the game
Contract Address on Goerli Network: 0x0f24dB4c0490107027abf0C8e5042103E99e147e

Game Link: https://jacobjunior.itch.io/crypto-jenga

Chainlink VRF: https://vrf.chain.link/goerli/72

Chainlink Keeper: https://keepers.chain.link/goerli/44076444989087909005030241929667374833060791576656843945137326609272073049077

Latest Smart Contract Source Code File Path: CryptoJenga/smartContracts/contracts/cryptoJenga_v6.sol

1. Fund your Metmask with Goerli Ether (https://goerlifaucet.com/)
2. Access the game on Chrome on one device
3. Access the game on Chrome on another device (or invite a friend to join)
4. Load the smart contract in Remix and call `resetGameState` with the following parameters:
    - `_TotalRounds`: The number of of rounds users will player
    - `_MaxBets`: The number of bets users will be able to place per game
    - `_RoundDuration`: The duration of a round in seconds before when a player is able to bet
    - `_USDTicketPrice`: The price of entry to the game in USD to 18 decimal places (eg $1 would be entered as "1000000000000000000")

5. Press "play" to enter the lobby of the game
6. Press the ticket to purchase a ticket to play the game
7. Start the game from Remix by calling `StartGame`
8. Wait for the ticket text to change to "Admit" and click on the ticket again to enter the game
9. For every round the player can place bets. If a user places a bet after `_RoundDuration` the transaction will fail and the bet will not be placed
10. At the end of the last round the winner will receive the pooled funds
11. To start a new game repeat the steps from step 4.

### Testing the contract
how to test keeper + vrf

1. deploy contract on remix
2. call startGame()
3. joinGame()
4. add contract as consumer to VRF
5. register contract as a keeper

### Chainlink features used
- Price feed: the ticket price is in USD. The price feed is used for the conversion.
- VRF: used for choosing the winning answer and the winner
- Keeper: automatically move from round to round and end the game based on the duration defined.

```mermaid
sequenceDiagram
    participant FrontEnd
    participant SmartContract
    participant ChainLink_PriceFeed
    participant ChainLink_VRF
    participant ChainLink_Keeper

    FrontEnd->>+SmartContract: Start a game
    SmartContract->>SmartContract: constructor 

    Note right of SmartContract: address _priceFeedAddress <br/>address _vrfCoordinator <br>uint256 _linkFee<br>bytes32 _keyhash (VRF)<br>uint256 _ticketPrice<br>uint _roundDuration
   Note left of ChainLink_PriceFeed:   struct Bet {<br>uint256 betAmount<br>Signature betSignature<br>string betString<br>}
 
    SmartContract -->> -FrontEnd: Contract Deployed

    SmartContract -->> FrontEnd: Emit Round 1 started event

    FrontEnd ->> +SmartContract: place a bet
    Note right of SmartContract: uint Amout <br/>uint8 v<br>bytes32 r<br>bytes32 s;
    ChainLink_PriceFeed -->>SmartContract: USD to ETH conversion rate
    SmartContract ->>-SmartContract: determine if the bet can be taken
    Note right of SmartContract:mapping(uint256 => mapping(uint256 => mapping ( address => bool))) placedBet<br> mapping(uint256 => mapping(uint256 => mapping ( address => Bet))) bets

    ChainLink_Keeper-->>+SmartContract: RoundEnded
    SmartContract-->>FrontEnd: emit round end event
    SmartContract->>ChainLink_VRF: request random number
    ChainLink_VRF-->>SmartContract: request random number
    SmartContract ->>-SmartContract: determine the winning number

    FrontEnd ->> +SmartContract: reveal the bet
    Note right of SmartContract: string betString
    SmartContract ->>-SmartContract: save the betString if pass verification

    ChainLink_Keeper-->>+SmartContract: Reveal peroid end
    SmartContract->>-SmartContract: calculate the winner for the round
```