using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RivalWarCard;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card cardData;
    public Card card => cardData;
    public Image cardImage;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text strenghText;
    public TMP_Text costText;
    public TMP_Text rarityText;
    public TMP_Text typeText;
    public TMP_Text attributeText;
    public TMP_Text descriptionText;
    public Image[] propertyImage;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int currentStrength;

    private void Start()
    {
        InitializeRuntime();
        UpdateCardDisplay();
    }
    public void UpdateCardDisplay()
    {
        nameText.text = cardData.cardName;
        healthText.text = cardData.health.ToString();
        strenghText.text = cardData.strengh.ToString();
        costText.text = cardData.cost.ToString();
        rarityText.text = cardData.rarity.ToString();

        foreach (var img in propertyImage)
        {
            img.gameObject.SetActive(false);
        }

        // Hiện đúng hình dựa trên property của card
        int index = (int)cardData.properties;  // ép enum sang int để lấy index
        propertyImage[index].gameObject.SetActive(true);
    }

    public void InitializeRuntime()
    {
        if (cardData != null)
        {
            currentHealth = cardData.health;
            currentStrength = cardData.strengh;
        }
    }

    // --- NEW ---
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        // update health text if you want
        if (healthText != null)
            healthText.text = currentHealth.ToString();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- NEW ---
    public void Die()
    {
        // Inform parent GridCell to clear slot (GridCell handles destroying/unparenting)
        var parentCell = GetComponentInParent<GridCell>();
        if (parentCell != null)
        {
            // remove for the correct team
            if (cardData != null && cardData.team == Team.Attack)
                parentCell.RemoveAttackCard();
            else
                parentCell.RemoveDefendCard();
        }
        else
        {
            // fallback
            Destroy(gameObject);
        }
    }
}
