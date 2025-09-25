using UnityEngine;
using System.Linq; // để dùng Contains

namespace RivalWarCard
{
    public enum LaneKind
    {
        Ground,
        Water
    }

    public class Lane : MonoBehaviour
    {
        public LaneKind kind;
        public CardDisplay currentCardDisplay; // card đang đứng trên lane (nếu có)

        public bool CanPlace(Card card)
        {
            if (currentCardDisplay != null) return false; // đã có card

            if (kind == LaneKind.Ground)
                return true; // làn đất nhận mọi card

            if (kind == LaneKind.Water)
                return card.attribute != null && card.attribute.Contains(Attribute.Swim);

            return false;
        }

        public void PlaceCard(CardDisplay cardDisplay)
        {
            currentCardDisplay = cardDisplay;

            // nếu card UI là RectTransform (UI), dùng SetParent với worldPositionStays = false để local pos = zero
            if (cardDisplay.transform is RectTransform)
            {
                cardDisplay.transform.SetParent(transform, false);
                (cardDisplay.transform as RectTransform).anchoredPosition = Vector2.zero;
            }
            else
            {
                cardDisplay.transform.SetParent(transform, false);
                cardDisplay.transform.localPosition = Vector3.zero;
            }

            // If card had a canvasGroup or drag script, disable raycasts or interactions if desired
            var cg = cardDisplay.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = false;
        }

        // --- NEW: Remove card from lane (destroy or unparent depending on logic)
        public void RemoveCard(bool destroy = true)
        {
            if (currentCardDisplay != null)
            {
                if (destroy)
                    Destroy(currentCardDisplay.gameObject);
                else
                    currentCardDisplay.transform.SetParent(null);
            }
            currentCardDisplay = null;
        }
    }
}
