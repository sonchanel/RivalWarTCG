using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

namespace RivalWarCard
{
    public enum Phase
    {
        AttackPhase,    // Attacker summons Units
        DefensePhase,   // Defender summons Units + uses tricks
        TrickPhase,     // Attacker plays tricks
        CombatPhase     // Resolve combat
    }

    public class TurnManager : MonoBehaviour
    {
        [SerializeField] public TMP_Text phaseATText;
        [SerializeField] public TMP_Text phaseDFText;
        [SerializeField] public TMP_Text coinATmainText;
        [SerializeField] public TMP_Text coinATText;
        [SerializeField] public TMP_Text coinDFmainText;
        [SerializeField] public TMP_Text coinDFText;
        [SerializeField] public Button buttonATNextPhase;
        [SerializeField] public Button buttonDFNextPhase;


        public static TurnManager Instance;

        public Phase currentPhase = Phase.AttackPhase;
        public int turnNumber = 1;

        [Header("Coins (resource)")]
        public int baseCoins = 1;
        public int coinsPerTurn = 0; // tăng coins theo turn
        public int attackerCoins;
        public int defenderCoins;

        [Header("Config")]
        public Team attackerTeam = Team.Attack; // cố định
        public Team defenderTeam = Team.Defend;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            StartNewTurn();
        }
        private void Update()
        {
            // update UI
            // phaseATText.text = $"{currentPhase}";
            // phaseDFText.text = $"{currentPhase}";
            // if (coinATText != null) coinATText.text = $"{attackerCoins}";
            // if (coinATText != null) coinATmainText.text = $"{attackerCoins}";
            // if (coinATText != null) coinDFmainText.text = $"{defenderCoins}";
            //if (coinDFText != null) coinDFText.text = $"{defenderCoins}";
        }

        public void StartNewTurn()
        {
            // update turn & coins
            if (turnNumber == 1)
            {
                attackerCoins = baseCoins;
                defenderCoins = baseCoins;
            }
            else
            {
                attackerCoins = turnNumber + coinsPerTurn;
                defenderCoins = turnNumber + coinsPerTurn;
            }

            currentPhase = Phase.AttackPhase;
            Debug.Log($"Turn {turnNumber} started. Phase: {currentPhase}. Coins A:{attackerCoins} D:{defenderCoins}");
        }

        // call to progress to next phase (UI button calls)
        public void NextPhase()
        {
            switch (currentPhase)
            {
                case Phase.AttackPhase:
                    currentPhase = Phase.DefensePhase;
                    break;
                case Phase.DefensePhase:
                    currentPhase = Phase.TrickPhase;
                    break;
                case Phase.TrickPhase:
                    currentPhase = Phase.CombatPhase;
                    StartCoroutine(ResolveCombatThenEndTurn());
                    return;
                case Phase.CombatPhase:
                    currentPhase = Phase.AttackPhase;
                    break;
            }
            Debug.Log($"Phase -> {currentPhase}");
        }

        private IEnumerator ResolveCombatThenEndTurn()
        {
            yield return ResolveCombat();
            // end of turn: increase turn
            turnNumber++;
            StartNewTurn();
        }

        // play a card request from UI/CardMovement:
        // returns true if played
        public bool PlayCard(CardDisplay cardDisplay, int laneIndex)
        {
            if (cardDisplay == null || cardDisplay.cardData == null) return false;
            Console.Write("PlayCard requested: " + cardDisplay.cardData.cardName + " to lane " + laneIndex);
            Card c = cardDisplay.cardData;
            Team cardTeam = c.team;

            // check phase rules
            if (!IsPlayAllowedThisPhase(c)) return false;

            // check coins
            if (cardTeam == attackerTeam && c.cost > attackerCoins) return false;
            if (cardTeam == defenderTeam && c.cost > defenderCoins) return false;

            // If trick: apply effect immediately (no placement) - except if Trick is "environment" or special
            if (c.category == CardCategory.Trick)
            {
                // deduct coins
                if (cardTeam == attackerTeam) attackerCoins -= c.cost; else defenderCoins -= c.cost;
                //ApplyTrickEffect(c, laneIndex, cardTeam);
                Destroy(cardDisplay.gameObject); // consumed
                return true;
            }

            // Unit: place on board
            bool placed = BoardManager.Instance.TryPlaceCard(cardDisplay, laneIndex);
            if (!placed) return false;

            // deduct coins
            if (cardTeam == attackerTeam) attackerCoins -= c.cost; else defenderCoins -= c.cost;

            // remove from hand
            var hand = UnityEngine.Object.FindFirstObjectByType<HandManager>();
            if (hand != null) hand.RemoveCardFromHand(cardDisplay.gameObject);

            return true;
        }

        // phase rules: who can play what
        private bool IsPlayAllowedThisPhase(Card c)
        {
            if (currentPhase == Phase.AttackPhase)
            {
                return c.team == attackerTeam && c.category == CardCategory.Unit;
            }
            else if (currentPhase == Phase.DefensePhase)
            {
                return c.team == defenderTeam && (c.category == CardCategory.Unit || c.category == CardCategory.Trick);
            }
            else if (currentPhase == Phase.TrickPhase)
            {
                return c.team == attackerTeam && c.category == CardCategory.Trick;
            }
            return false;
        }

        // small trick handler: very simple effects (can be extended)
        // private void ApplyTrickEffect(Card trickCard, int laneIndex, Team playingTeam)
        // {
        //     if (trickCard.trickEffect == TrickEffect.DamageUnit)
        //     {
        //         var lane = BoardManager.Instance.GetLane(laneIndex);
        //         if (lane == null) return;

        //         // target opposing unit first if exists
        //         CardDisplay target = (playingTeam == attackerTeam) ? lane.defendCard : lane.attackCard;
        //         if (target != null)
        //             target.TakeDamage(trickCard.effectValue);
        //     }
        //     else if (trickCard.trickEffect == TrickEffect.DamageHero)
        //     {
        //         // damage opposite hero
        //         if (playingTeam == attackerTeam) GameManager.Instance.DamageHero(defenderTeam, trickCard.effectValue);
        //         else GameManager.Instance.DamageHero(attackerTeam, trickCard.effectValue);
        //     }
        //     else if (trickCard.trickEffect == TrickEffect.RemoveUnit)
        //     {
        //         var lane = BoardManager.Instance.GetLane(laneIndex);
        //         if (lane == null) return;
        //         if (playingTeam == attackerTeam) lane.RemoveDefendCard();
        //         else lane.RemoveAttackCard();
        //     }
        //     // add other effects as needed
        // }

        // Combat resolution coroutine (left → right)
        private IEnumerator ResolveCombat()
        {
            int lanesCount = BoardManager.Instance.totalLanes;
            Debug.Log("Combat phase starting...");
            for (int i = 0; i < lanesCount; i++)
            {
                Lane lane = BoardManager.Instance.GetLane(i);
                if (lane == null) continue;

                CardDisplay attacker = lane.playerCell != null ? GetCurrentCard(lane.playerCell) : null;
                CardDisplay defender = lane.enemyCell != null ? GetCurrentCard(lane.enemyCell) : null;

                // attacker attacks first (design decision consistent with attacker-first)
                if (attacker != null && defender != null)
                {
                    // attacker hits defender
                    defender.TakeDamage(attacker.cardData.strengh);
                    yield return new WaitForSeconds(0.15f);
                    // if defender still alive, counterattack
                    if (defender != null && defender.currentHealth > 0)
                    {
                        attacker.TakeDamage(defender.cardData.strengh);
                        yield return new WaitForSeconds(0.15f);
                    }
                }
                else if (attacker != null && defender == null)
                {
                    // deal damage to defender hero
                    GameManager.Instance.DamageHero(defenderTeam, attacker.cardData.strengh);
                    yield return new WaitForSeconds(0.15f);
                }
                else if (defender != null && attacker == null)
                {
                    // defender attacks attacker's hero
                    GameManager.Instance.DamageHero(attackerTeam, defender.cardData.strengh);
                    yield return new WaitForSeconds(0.15f);
                }
            }
            Debug.Log("Combat phase ended.");
            yield return null;
        }

        // Helper để lấy currentCard từ GridCell
        private CardDisplay GetCurrentCard(GridCell cell)
        {
            var field = cell.GetType().GetField("currentCard", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field != null ? field.GetValue(cell) as CardDisplay : null;
        }

    }
}
