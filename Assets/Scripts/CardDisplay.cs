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

    private void Start()
    {
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
}
