using System.Collections.Generic;
using UnityEngine;

namespace RivalWarCard
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance; // singleton để gọi nhanh
        public List<Lane> lanes; // gán trong Inspector: 6 lane (4 Ground + 2 Water)

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public bool TryPlaceCard(CardDisplay cardDisplay, int laneIndex)
        {
            if (laneIndex < 0 || laneIndex >= lanes.Count) return false;

            Lane targetLane = lanes[laneIndex];

            if (targetLane.CanPlace(cardDisplay.card))
            {
                targetLane.PlaceCard(cardDisplay);
                return true;
            }

            return false;
        }
        public bool TryPlaceCard(CardDisplay cardDisplay, Lane targetLane)
        {
            if (cardDisplay == null || targetLane == null) return false;
            if (targetLane.CanPlace(cardDisplay.card))
            {
                targetLane.PlaceCard(cardDisplay);
                return true;
            }
            return false;
        }
    }
}
