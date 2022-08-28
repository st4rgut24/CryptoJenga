/**
 * Initializes the contract
 */
import { readFile } from 'fs/promises';
import { EventsManager } from './EventsManager.js';

export class ContractManager { 
    web3;
    GameContractAbi;
    EventMgr;
    
     constructor(web3, _EventMgr){
      this.web3 = web3;
      this.EventMgr = _EventMgr;

      this.getContractAbi();
     }

     async getContractAbi() {
      this.GameContractAbi = JSON.parse(
        await readFile(
          new URL('../contracts/cryptoJenga_v6.json', import.meta.url)
        )
      ); 
     }

     // called after a game has been created or joined by a player
     // the address comes from a ws message received from unity client
     setContract(contractAddress) {
        const GameContractDeployed = new this.web3.eth.Contract(
          this.GameContractAbi,
          contractAddress
        );
        this.EventMgr.setContractListeners(GameContractDeployed);
      }     
 }