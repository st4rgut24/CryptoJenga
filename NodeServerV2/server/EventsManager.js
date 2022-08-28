import { StackTowerWeb3 } from "./Main.js";

export class EventsManager {

    constructor() {
    }

    broadcastMessage(payload) {
        if (StackTowerWeb3.wss){
            console.log("broadcasting message", payload);
            StackTowerWeb3.wss.clients.forEach((client) => {
                client.send(JSON.stringify(payload));
              });
        }
        else {
            console.log('ws not available');
        }
    }

    /**
     * Listen for events received from the smart contract, 
     * if there are clients then forward these messages to them.
     * @param {*} contract 
     */
     setContractListeners(contract) {
        contract.events.RoundWinner({}, (function(error, events){
            let payload = {
                eventName: events.event,
                roundWinner: events.returnValues._roundWinner,
                rewardAmount: events.returnValues._rewardAmount
            }
            this.broadcastMessage(payload);
        }).bind(this));

        contract.events.BetMade({}, (function(error, events){
            let payload = {
                eventName: events.event,
                player: events.returnValues._player,
                currentRoundBetCount: events.returnValues._currentRoundBetCount,
                remainingBets: events.returnValues._remainingBets
            }
            this.broadcastMessage(payload);
        }).bind(this));

        contract.events.GameState({}, (function(error, events){
            let payload = {
                eventName: events.event,
                currentState: events.returnValues._currentState
            }
            this.broadcastMessage(payload);
        }).bind(this));

        contract.events.RoundStarted({}, (function(error, events){
            let payload = {
                eventName: events.event,
                currentRoundNumber: events.returnValues._currentRoundNumber
            }
            this.broadcastMessage(payload);
        }).bind(this));

        contract.events.RoundEnded({}, (function(error, events){
            let payload = {
                eventName: events.event
            }
            this.broadcastMessage(payload);
        }).bind(this));

        contract.events.GameEnded({}, (function(error, events){
            let payload = {
                eventName: events.event,
                winner: events.returnValues._gameWinner,
                amountWon: events.returnValues.amountWon
            }
            this.broadcastMessage(payload);
        }).bind(this));   

        contract.events.PlayerJoined({}, (function(error, events){
            let payload = {
                eventName: events.event,
                joinedPlayer: events.returnValues._joinedPlayer,
            }
            this.broadcastMessage(payload);
        }).bind(this));

        contract.events.Test({}, (function(error, events){
            let payload = {
                eventName: events.event,
                testAddress: events.returnValues._testAddress,
            }
            this.broadcastMessage(payload);
        }).bind(this));
    }
}