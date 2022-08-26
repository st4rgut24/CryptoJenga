import {getSigner, getAccountAddress} from "../accountsService";
import "dotenv/config";
import { Contract, ethers } from "ethers";
import * as testJson from "../../artifacts/contracts/test.sol/VRFv2Consumer.json";
import {VRFv2Consumer} from "../../typechain-types";
import { exit } from "process";

function convertStringArrayToBytes32(array: string[]) {
  const bytes32Array = [];
  for (let index = 0; index < array.length; index++) {
    bytes32Array.push(ethers.utils.formatBytes32String(array[index]));
  }
  return bytes32Array;
}

const priceFeedAddress = "0xb4c4a493AB6356497713A78FFA6c60FB53517c63";
const vrfCoordinator = "0x2Ca8E0C643bDe4C2E08ab1fA0da3401AdAD7734D";
const linkFee = 0.1;
const keyhash = "0x79d3d8832d904592c0bf9818b621522c988bb8b0c05cdc3b15aea1b6e8db0c15";
const ticketPriceInUSD = 1;
const subscriptionId = 58;
export const networkName = "goerli";

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
    console.log("======Deploying TEST contract======");
    console.log("");
    
    const cryptoJengaFactory = new ethers.ContractFactory(
      testJson.abi,
      testJson.bytecode, 
      signer
    );
  
    const testContract = (await cryptoJengaFactory.deploy(
      subscriptionId
    )) as VRFv2Consumer;
    
    console.log("Awaiting confirmations");
    await testContract.deployed();
  
    console.log("Completed");
    console.log(`CryptoJenga Contract deployed at ${testContract.address}`);
  
    return testContract.address;
  }


    exit;
  }
  
  main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
  });