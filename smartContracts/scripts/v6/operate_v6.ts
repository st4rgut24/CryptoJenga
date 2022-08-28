import {getSigner, getAccountAddress} from "../accountsService";
import "dotenv/config";
import { Contract, ethers } from "ethers";
import * as cryptoJengaJson from "../../artifacts/contracts/cryptoJenga_v6.sol/cryptoJengaV6.json";
import {CryptoJengaV5} from "../../typechain-types";

import { exit } from "process";

const priceFeedAddress = "0xD4a33860578De61DBAbDc8BFdb98FD742fA7028e";
const vrfCoordinator = "0x2Ca8E0C643bDe4C2E08ab1fA0da3401AdAD7734D";
const linkFee = 0.1;
const keyhash = "0x79d3d8832d904592c0bf9818b621522c988bb8b0c05cdc3b15aea1b6e8db0c15";
const ticketPriceInUSD = 1;
const subscriptionId = 72; 
const maxBets = 5;
const roundDuration = 80;
const roundCount = 1;
export const networkName = "goerli";

const provider = ethers.providers.getDefaultProvider(networkName);

async function main() 
{
  if (process.env.PRIVATE_KEY_2) {
    const ownerSignerWallet = await getSigner(
      process.env.PRIVATE_KEY_2,
      process.env.MNEMONIC,
      networkName
    );

    const provider = ethers.providers.getDefaultProvider(networkName);
    const signer = ownerSignerWallet.connect(provider);
    const balanceBN = await signer.getBalance();
    const balance = Number(ethers.utils.formatEther(balanceBN));
    console.log(`Wallet balance ${balance}`);
    if (balance < 0.01) {
      throw new Error("Not enough ether");
    }
  
    console.log("");
    console.log("======Deploying CryptoJenga V6 contract======");
    console.log("");
    
    const cryptoJengaFactory = new ethers.ContractFactory(
      cryptoJengaJson.abi,
      cryptoJengaJson.bytecode, 
      signer
    );
  
    const cryptoJengaContract = (await cryptoJengaFactory.deploy(
      ethers.utils.parseEther(linkFee.toFixed(18)),
      keyhash,
      ethers.utils.parseEther(ticketPriceInUSD.toFixed(18)),
      roundDuration, // round duration 1 mins
      roundCount, // number of round
      maxBets
    )) as CryptoJengaV5;
    
    console.log("Awaiting confirmations");
    await cryptoJengaContract.deployed();
  
    console.log("Completed");
    console.log(`CryptoJenga Contract deployed at ${cryptoJengaContract.address}`);

  }  
    exit;

  }
  
  main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
  });