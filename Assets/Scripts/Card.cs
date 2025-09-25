using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Timeline.Actions;

namespace RivalWarCard
{
    [CreateAssetMenu(fileName ="New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public int id;
        public string cardName;//tên thẻ
        public Properties properties; //loại thẻ vd: Fighter(Đấu sĩ), Assassin(Sát thủ), Tank (Đỡ đòn), Support(Hỗ trợ)
        public string image;//ảnh của thẻ
        public int health;//máu thẻ
        public int strengh;//sức tấn công
        public string description;//mô tả khả năng vd: Triệu hồi: Gây 1AT cho kẻ địch, tiêu diệt nếu kẻ địch đó dưới 2AT
        public List<Attribute> attribute;//thuộc tính thẻ vd: Bơi, Úp, Săn,...
        public Type type;//Bên tấn công có 4 type Demon,History, Army, Natural Bên phòng thử có 4 type Kinght, Beast, Fantasy, Future   
        public Team team;//Đội Tấn công và Đội Phòng thủ
        public int cost;//giá của thẻ
        public Rarity rarity;//độ hiếm của thẻ

    }

    public enum Type
    {
        // Attack types
        Demon,
        History,
        Army,
        Natural,

        // Defend types
        Knight,
        Beast,
        Fantasy,
        Future
        
    }

    public enum Properties
    {
        Knight,
        Assassin,
        Tank,
        Support
    }
    public enum Attribute
    {
        Swim,
        Flip,
        Trick,
        Hunt,
        Aim,
        Resist,
        Block1,
        Block2,
        Slay,
        Pierce,
        Spy,

    }
    public enum Team
    {
        Attack,
        Defend
    }
    public enum Rarity
    {
        Basic,
        Rare,
        SuppperRare,
        Legendary
    }
}
