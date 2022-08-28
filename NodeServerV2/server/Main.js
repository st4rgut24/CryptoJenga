import Web3 from 'web3';
import { ContractManager } from './ContractManager.js';
import { EventsManager } from './EventsManager.js';
const INFURA_API_URL = "wss://goerli.infura.io/ws/v3/481c7a826bc445ccb7b417ce9a6096c7";

export class StackTowerWeb3 {

    static wss;
    static ContractMgr;

    constructor(wss){
        var web3 = new Web3(INFURA_API_URL);
        new EventsManager();
        StackTowerWeb3.ContractMgr = new ContractManager(web3);
    }

    setWebSocket(wss) {
        StackTowerWeb3.wss = wss;
    }
}