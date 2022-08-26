import {getSigner, getAccountAddress} from "../accountsService";
import "dotenv/config";
import { Contract, ethers } from "ethers";
import * as cryptoJengaJson from "../../artifacts/contracts/cryptoJenga_v6.sol/cryptoJengaV6.json";
import {CryptoJengaV6} from "../../typechain-types";

import {deployCryptoJengaContract} from "./deploycryptoJenga_v6";
import { exit } from "process";

const priceFeedAddress = "0xD4a33860578De61DBAbDc8BFdb98FD742fA7028e";
const vrfCoordinator = "0x2Ca8E0C643bDe4C2E08ab1fA0da3401AdAD7734D";
const linkFee = 0.1;
const keyhash = "0x79d3d8832d904592c0bf9818b621522c988bb8b0c05cdc3b15aea1b6e8db0c15";
const ticketPriceInUSD = 1;
export const networkName = "goerli";

const provider = ethers.providers.getDefaultProvider(networkName);

async function main() 
{
    const ownerSignerWallet = await getSigner(
      process.env.PRIVATE_KEY_1 || "",
      process.env.MNEMONIC,
      networkName
    );

    const secondSignerWallet = await getSigner(
      process.env.PRIVATE_KEY_2 || "",
      process.env.MNEMONIC,
      networkName
    );
    
    const ballotContractAddress = await deployCryptoJengaContract(
      ownerSignerWallet, 
      priceFeedAddress,
      vrfCoordinator,
      linkFee,
      keyhash,
      ticketPriceInUSD
    );

    console.log("jenga contract address is " + ballotContractAddress);

    const ownerSigner = ownerSignerWallet.connect(provider);
    const gameContractForOwner: CryptoJengaV6 = new Contract(
      ballotContractAddress,
      cryptoJengaJson.abi,
      ownerSigner
    ) as CryptoJengaV6;

    // get the game state
    let gameState = await gameContractForOwner.game_state();
    console.log(`Game state ${gameState}`)

    console.log("Joining the game as owner ...");
    let tx = await gameContractForOwner.connect(ownerSignerWallet).joinGame({value:ethers.utils.parseEther("0.006")});
    await tx.wait(1);

    console.log("Joining the game as participant ...");
    tx = await gameContractForOwner.connect(secondSignerWallet).joinGame({value:ethers.utils.parseEther("0.006")});
    await tx.wait(1);

    // start the game
    console.log("Starting the game ...");
    tx = await gameContractForOwner.startGame();
    console.log(`Start game transaction ${tx.hash}; waiting for confirmation.`)
    await tx.wait(1);

    console.log("Fulfilling random words");
    tx = await gameContractForOwner.connect(ownerSignerWallet).fulfillRandomWordsTest(0, [0]);
    await tx.wait(1);
    console.log("transaction fulfilled");
    exit;
  }
  
  main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
  });