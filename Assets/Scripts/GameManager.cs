using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RivalWarCard;
using Mirror;
using Mirror.Examples.Basic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SyncVar] public int attackHeroHealth = 20;
    [SyncVar] public int defendHeroHealth = 20;

    [SerializeField] public Canvas startCanvas;
    [SerializeField] public GameObject mainCanvas;


    private int playerHealth;
    private int playerXP;
    private int difficulty = 0;

    public OptionsManager OptionsManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public DeckManager DeckManager { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    [Command]
    public void CmdEndTurn() {
        NextPhase();
    }

    [Server]
    void NextPhase() {
        Debug.Log("Server resolving next phase...");
        WaitGame();
    }

    private void InitializeManagers()
    {
        OptionsManager = GetComponentInChildren<OptionsManager>();
        AudioManager = GetComponentInChildren<AudioManager>();
        DeckManager = GetComponentInChildren<DeckManager>();

        if (OptionsManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/OptionsManager");
            if (prefab == null)
            {
                Debug.Log($"OptionsManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                OptionsManager = GetComponentInChildren<OptionsManager>();
            }
        }
        if (AudioManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/AudioManager");
            if (prefab == null)
            {
                Debug.Log($"AudioManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                AudioManager = GetComponentInChildren<AudioManager>();
            }
        }
        if (DeckManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/DeckManager");
            if (prefab == null)
            {
                Debug.Log($"DeckManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                DeckManager = GetComponentInChildren<DeckManager>();
            }
        }
    }

    public int PlayerHealth
    {
        get { return playerHealth; }
        set { playerHealth = value; }
    }
    public int PlayerXP
    {
        get { return playerXP; }
        set { playerXP = value; }   
    }
    public int Difficulty
    {
        get { return difficulty; }
        set { difficulty = value; }
    }

    public void DamageHero(Team team, int dmg)
    {
        if (team == Team.Attack)
        {
            attackHeroHealth -= dmg;
            if (attackHeroHealth <= 0) EndGame(defenderWins:true);
        }
        else
        {
            defendHeroHealth -= dmg;
            if (defendHeroHealth <= 0) EndGame(defenderWins:false);
        }
    }

    private void EndGame(bool defenderWins)
    {
        Debug.Log(defenderWins ? "Defender wins!" : "Attacker wins!");
        // TODO: show UI, stop game loop
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ...existing code...
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        WaitGame();
    }
    public void WaitGame()
    {
        startCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);
    }
    public void StartGameAT()
    {
        var players = FindObjectsOfType<PlayerInfo>();
        var hostPlayer = players.First(p => p.isServer);
        hostPlayer.side = PlayerSide.Attack;
        if (players.Length == 2)
        {
            var clientPlayer = players.First(p => !p.isServer);
            clientPlayer.side = PlayerSide.Defense;
            RpcStartGame();
        }
        else
        {
            RpcStartGame();
            //StartCoroutine(WaitForClientAndStart(PlayerSide.Attack));
        }
    }
    public void StartGameDF()
    {
        var players = FindObjectsOfType<PlayerInfo>();
        var hostPlayer = players.First(p => p.isServer);
        hostPlayer.side = PlayerSide.Defense;
        if (players.Length == 2)
        {
            var clientPlayer = players.First(p => !p.isServer);
            clientPlayer.side = PlayerSide.Attack;
            RpcStartGame();
        }
        else
        {
            RpcStartGame();
            //StartCoroutine(WaitForClientAndStart(PlayerSide.Defense));
        }
    }
    IEnumerator WaitForClientAndStart(PlayerSide hostSide)
    {
        yield return new WaitUntil(() => FindObjectsOfType<PlayerInfo>().Length == 2);
        var players = FindObjectsOfType<PlayerInfo>();
        var clientPlayer = players.First(p => !p.isServer);
        clientPlayer.side = hostSide == PlayerSide.Attack ? PlayerSide.Defense : PlayerSide.Attack;
        RpcStartGame();
    }
    [ClientRpc]
    void RpcStartGame()
    {
        startCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
        DeckManager.DrawStartingHand(FindAnyObjectByType<HandManager>());
    }
    [TargetRpc]
    public void TargetDrawStartingHand(NetworkConnectionToClient target)
    {
        DeckManager.DrawStartingHand(FindAnyObjectByType<HandManager>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
