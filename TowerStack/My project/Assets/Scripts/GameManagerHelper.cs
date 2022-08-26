using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerHelper : MonoBehaviour
{
    public static Player findPlayer(List<Player> PlayerList, string address)
    {
        return PlayerList.Find((Player PlayerInst) => PlayerInst.address == address);
    }

    // given a list of addresses, find the corresponding list of players
    public static List<Player> getPlayerListFromAddresses(List<Player> PlayerList, List<string> addressList)
    {
        List<Player> MatchingPlayerList = new List<Player>();
        addressList.ForEach((string address) =>
        {
            Player MatchingPlayer = PlayerList.Find((Player player) => player.address == address);
            MatchingPlayerList.Add(MatchingPlayer);
        });
        return MatchingPlayerList;
    }

    public static int getCurrentTimestamp()
    {
        return (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
    }

    // Shorten an ethereum address (40 chars long)
    // @addr     the users ethereum address eg 5ADB78276219bAf90577453A9BdDd5f200452D9C 40 chars long
    public static string shortenAddress(string addr)
    {
        string addrBegin = addr.Substring(0, 5);
        string addrEnding = addr.Substring(38);
        return addrBegin + "..." + addrEnding;
    }

    public static bool IsAddressEqual(string address1, string address2)
    {
        return address1.ToLower().Equals(address2.ToLower());
    }

    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }
}
