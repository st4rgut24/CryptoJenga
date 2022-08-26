/**
 * Initializes the contract
 */
import { readFile } from 'fs/promises';
import { EventsManager } from './EventsManager.js';

export class ContractManager { 
     constructor(web3){
      const contract = this.setContract(web3);
     }
 
     async setContract(web3, wss) {
        const GameContract = JSON.parse(
            await readFile(
              new URL('../contracts/cryptoJenga_v6.json', import.meta.url)
            )
          );
        const GameContractDeployed = new web3.eth.Contract(
          GameContract.abi,
          "0x0f24dB4c0490107027abf0C8e5042103E99e147e"
        );
        console.log(`address deployed to ${GameContractDeployed.options.address}`);
        const EventsMgr = new EventsManager(GameContractDeployed);
      }     
 }