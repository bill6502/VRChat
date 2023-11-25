
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Game : UdonSharpBehaviour
{
    [SerializeField]
    private FruitBucket Bucket;
    [SerializeField]
    private int randomSpawnRange = 5;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Color scoreColor;

    //index of fruit
    [HideInInspector, UdonSynced]
    public int index = 0;
    [HideInInspector, UdonSynced]
    public int nextFruit = 0;

    [HideInInspector, UdonSynced]
    public bool gameState = false;
    [HideInInspector, UdonSynced]
    public int score = 0;

    public int[] points;
    //scales have to as long as materials
    public float[] scales;
    public Material[] materials;
    public float mass = 100f;

    private VRCPlayerApi LocalPlayer;

    private void Start()
    {
        LocalPlayer = Networking.LocalPlayer;
    }

    public void GameStart()
    {
        //checking
        if (scales.Length == 0 || materials.Length == 0 || scales.Length != materials.Length || gameState || scoreText.color == Color.red) return;

        //getting owner
        Networking.SetOwner(LocalPlayer, gameObject);

        //start game
        gameState = true;
        RequestSerialization();

        fruit fruit = Bucket.getFruit();
        if (fruit)
            fruit.setFruit();
    }

    public void GameOver()
    {
        gameState = false;
        RequestSerialization();
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(gameOverText));
    }

    public void GameReset()
    {
        //checking
        if (scales.Length == 0 || materials.Length == 0 || scales.Length != materials.Length) return;

        bool isOwner = Networking.IsOwner(LocalPlayer, gameObject);
        bool isMaster = Networking.IsMaster;

        if (gameState && !(isOwner || isMaster)) return;

        //getting owner
        Networking.SetOwner(LocalPlayer, gameObject);

        //resetting
        score = 0;
        index = 0;
        gameState = false;
        updateScore();
        getNextRandomFruit();
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(gameStartText));

        // Debug.Log(name + " : GameReset() called." + index);
        Bucket.ResetFruits();
    }

    public void previewFruit()
    {
        fruit fruit = Bucket.getFruit();

        //getting index of fruit
        index++;
        getNextRandomFruit();

        if (fruit)
            fruit.setFruit();
        else
            GameOver();
        // Debug.Log($"{fruit.name} : index={fruit.index} ,Material={_meshRenderer.material.name}");
    }

    public void getNextRandomFruit()
    {
        nextFruit = Random.Range(0, randomSpawnRange);
        RequestSerialization();
    }

    public void updateScore()
    {
        scoreText.text = score.ToString();
    }

    public void gameOverText()
    {
        scoreText.color = Color.red;
    }

    public void gameStartText()
    {
        scoreText.color = scoreColor;
    }

    private void OnTriggerExit(Collider other)
    {
        bool isOwner = Networking.IsOwner(LocalPlayer, gameObject);
        fruit _fruit = other.GetComponent<fruit>();
        if (!isOwner || !_fruit || !gameState) return;

        var FruitIndex = _fruit.index;
        var _pickup = other.GetComponent<VRC_Pickup>();
        var isHeld = _pickup.IsHeld;
        // Debug.Log($"exited,{isHeld} ,{other.name} {FruitIndex} {index}");
        if (!isHeld && FruitIndex != -1 && !_pickup.pickupable)
            GameOver();
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        Networking.SetOwner(LocalPlayer, Bucket.gameObject);
    }
}
