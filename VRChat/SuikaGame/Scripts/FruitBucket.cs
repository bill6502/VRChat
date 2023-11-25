
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class FruitBucket : UdonSharpBehaviour
{
    public fruit[] fruits;

    public Transform startPosition;
    public Transform localScale;
    public Transform spawnPosition;

    private VRCPlayerApi LocalPlayer;

    private void Start()
    {
        LocalPlayer = Networking.LocalPlayer;
    }

    public fruit getFruit()
    {

        foreach (var fruit in fruits)
        {
            if (fruit.index == -1)
            {
                Networking.SetOwner(LocalPlayer, fruit.gameObject);
                fruit.transform.position = spawnPosition.position;
                return fruit;
            }
        }
        return null;
    }

    public void ResetFruits()
    {
        foreach (var fruit in fruits)
        {
            Networking.SetOwner(LocalPlayer, fruit.gameObject);
            fruit.Reset();
        }
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        //getting all of furits
        foreach (var fruit in fruits)
        {
            Networking.SetOwner(LocalPlayer, fruit.gameObject);
        }
    }
}
