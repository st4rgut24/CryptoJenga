import {getSigner, getAccountAddress} from "../accountsService";
import "dotenv/config";
import { Contract, ethers } from "ethers";
import * as cryptoJengaJson from "../../artifacts/contracts/cryptoJenga_v5.sol/cryptoJengaV5.json";
import {CryptoJengaV5} from "../../typechain-types";

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
      const jengaContractAddress: string = process.argv[2];
      const ownerSignerWallet = await getSigner(
        process.env.PRIVATE_KEY_2,
        process.env.MNEMONIC,
        networkName
      );
  
      console.log("jenga contract address is " + jengaContractAddress);
  
      const ownerSigner = ownerSignerWallet.connect(provider);
      const gameContractForOwner: CryptoJengaV5 = new Contract(
        jengaContractAddress,
        cryptoJengaJson.abi,
        ownerSigner
      ) as CryptoJengaV5;
  
      let tx = await gameContractForOwner.startGame()
      console.log(`Started game in transaction ${tx.hash}; waiting for confirmation.`)
      await tx.wait(1);
    }
}

main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
  });