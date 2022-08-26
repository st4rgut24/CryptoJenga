import {getSigner, getAccountAddress} from "../accountsService";
import "dotenv/config";
import { Contract, ethers } from "ethers";
import * as testJson from "../../artifacts/contracts/test.sol/VRFv2Consumer.json";
import {VRFv2Consumer} from "../../typechain-types";

function convertStringArrayToBytes32(array: string[]) {
  const bytes32Array = [];
  for (let index = 0; index < array.length; index++) {
    bytes32Array.push(ethers.utils.formatBytes32String(array[index]));
  }
  return bytes32Array;
}

export const networkName = "goerli";

const provider = ethers.providers.getDefaultProvider(networkName);

async function main() 
{
    if (process.argv.length < 3) {
        throw new Error("Does not include the jenga contract's address");
    }
    if (process.env.PRIVATE_KEY_2) {
      const testContractAddress: string = process.argv[2];
      const ownerSignerWallet = await getSigner(
        process.env.PRIVATE_KEY_2,
        process.env.MNEMONIC,
        networkName
      );
  
      console.log("test contract address is " + testContractAddress);
  
      const ownerSigner = ownerSignerWallet.connect(provider);
      const gameContractForOwner: VRFv2Consumer = new Contract(
        testContractAddress,
        testJson.abi,
        ownerSigner
      ) as VRFv2Consumer;
  
      const res = await gameContractForOwner.requestRandomWords()
  
      console.log("requested random words. response is", res);
    }
}

main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
  });