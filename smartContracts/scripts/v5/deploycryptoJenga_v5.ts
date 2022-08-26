import { ethers, Signer } from "ethers";
import "dotenv/config";
import * as cryptoJengaJson from "../../artifacts/contracts/cryptoJenga_v5.sol/cryptoJengaV5.json";
import {CryptoJengaV4} from "../../typechain-types";
import { networkName } from "../v4/operate_v4";

function convertStringArrayToBytes32(array: string[]) {
  const bytes32Array = [];
  for (let index = 0; index < array.length; index++) {
    bytes32Array.push(ethers.utils.formatBytes32String(array[index]));
  }
  return bytes32Array;
}

async function  deployCryptoJengaContract( 
  signerWallet: ethers.Wallet,
  priceFeedAddress: string,
  vrfCoordinator : string,
  linkFee: number,
  keyhash: string,
  ticketPriceInUSD: number,
  subscriptionId: number
) 
{
  const GAS_OPTIONS = {
    maxFeePerGas: 60 * 10 ** 9,
    maxPriorityFeePerGas: 60 * 10 ** 9,
  };



  return cryptoJengaContract.address;
}

export {deployCryptoJengaContract};
