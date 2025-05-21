using UnityEngine;

public class Card : MonoBehaviour
{
    public enum Suit { Clubs, Diamonds, Hearts, Spades }
    public enum Rank {  Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 2, Queen = 3, King = 4, Ace = 11 }

    public Suit suit;
    public Rank rank;
    public SpriteRenderer spriteRenderer;
    
    public void SetCard(Suit newSuit, Rank newRank, Sprite cardSprite)
    {
        suit = newSuit;
        rank = newRank;
        spriteRenderer.sprite = cardSprite;
    }

    public int GetValue()
    {
        return (int)rank;
    }
}
