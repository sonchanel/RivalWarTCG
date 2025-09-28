using System;
using UnityEngine;
using RivalWarCard;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int attackHeroHealth = 20;
    public int defendHeroHealth = 20;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
