using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BlackjackGame : MonoBehaviour
{
    [Header("Префабы и спрайты")]
    public GameObject cardPrefab;
    public Sprite[] cardSprites; // Массив со спрайтами карт

    [Header("Позиции карт")]
    public Transform playerCardsParent; // Точка для карт игрока
    public Transform dealerCardsParent; // Точка для карт дилера
    public float cardOffset = 0.5f; // Расстояние между картами

    [Header("UI элементы")]
    public Text playerScoreText;
    public Text dealerScoreText;
    public Button hitButton;
    public Button standButton;
    public Button playAgainButton;
    
    [Header("Результат игры")]
    public GameObject winObject;
    public GameObject loseObject;

    private List<Card> deck = new List<Card>();
    private List<Card> playerCards = new List<Card>();
    private List<Card> dealerCards = new List<Card>();
    private int playerScore = 0;
    private int dealerScore = 0;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        // Скрываем объекты результата
        if (winObject != null) winObject.SetActive(false);
        if (loseObject != null) loseObject.SetActive(false);
        
        // Очищаем старые карты
        ClearCards();
        
        // Создаем новую колоду
        CreateDeck();
        
        // Перемешиваем колоду
        ShuffleDeck();
        
        // Раздаем начальные карты
        StartCoroutine(DealInitialCards());

        // Настраиваем кнопки
        hitButton.interactable = true;
        standButton.interactable = true;
        playAgainButton.gameObject.SetActive(false);
    }

    void CreateDeck()
    {
        Debug.Log("Создание колоды...");
        
        // Создаем словарь для правильного сопоставления спрайтов с картами
        Dictionary<string, int> cardToSpriteIndex = new Dictionary<string, int>();
        
        // Заполняем словарь. Для каждой масти идут 9 карт по порядку
        int index = 0;
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
        {
            // Создаем ключи в формате "Масть_Ранг"
            
            cardToSpriteIndex[$"{suit}_Six"] = index;
            cardToSpriteIndex[$"{suit}_Seven"] = index + 1;
            cardToSpriteIndex[$"{suit}_Eight"] = index + 2;
            cardToSpriteIndex[$"{suit}_Nine"] = index + 3;
            cardToSpriteIndex[$"{suit}_Ten"] = index + 4;
            cardToSpriteIndex[$"{suit}_Jack"] = index + 5;
            cardToSpriteIndex[$"{suit}_Queen"] = index + 6;
            cardToSpriteIndex[$"{suit}_King"] = index + 7;
            cardToSpriteIndex[$"{suit}_Ace"] = index + 8;
            index += 9; // Переходим к следующей масти
        }

        // Создаем карты
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
        {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
            {
                GameObject cardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                cardObj.SetActive(false);
                Card card = cardObj.GetComponent<Card>();
                
                // Получаем правильный индекс спрайта для данной комбинации масти и ранга
                string key = $"{suit}_{rank}";
                int spriteIndex = cardToSpriteIndex[key];
                
                card.SetCard(suit, rank, cardSprites[spriteIndex]);
                Debug.Log($"Создана карта: {suit} {rank} со значением {card.GetValue()} (Sprite Index: {spriteIndex})");
                deck.Add(card);
            }
        }
    }

    void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    IEnumerator DealInitialCards()
    {
        // Раздаем 2 карты игроку
        yield return StartCoroutine(DealCard(true));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DealCard(true));

        // Раздаем 1 карту дилеру
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DealCard(false));

        UpdateScores();
    }

    IEnumerator DealCard(bool toPlayer)
    {
        if (deck.Count == 0) yield break;

        Card card = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);

        card.gameObject.SetActive(true);
        
        // Определяем позицию для новой карты
        Transform parent = toPlayer ? playerCardsParent : dealerCardsParent;
        List<Card> targetList = toPlayer ? playerCards : dealerCards;
        
        Vector3 targetPosition = parent.position + new Vector3(targetList.Count * cardOffset, 0, 0);
        card.transform.position = targetPosition + Vector3.up * 5; // Начальная позиция выше

        // Устанавливаем Order in Layer, чтобы новая карта была сверху
        SpriteRenderer cardRenderer = card.GetComponent<SpriteRenderer>();
        cardRenderer.sortingOrder = targetList.Count; // Каждая следующая карта будет иметь больший Order in Layer

        // Анимация появления карты
        float duration = 0.5f;
        float elapsed = 0;
        Vector3 startPos = card.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            card.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        card.transform.position = targetPosition;
        
        if (toPlayer)
            playerCards.Add(card);
        else
            dealerCards.Add(card);

        UpdateScores();
    }

    void UpdateScores()
    {
        playerScore = CalculateScore(playerCards);
        dealerScore = CalculateScore(dealerCards);

        Debug.Log($"Карты игрока:");
        foreach (Card card in playerCards)
        {
            Debug.Log($"- {card.suit} {card.rank} = {card.GetValue()}");
        }
        Debug.Log($"Общий счет игрока: {playerScore}");

        Debug.Log($"Карты дилера:");
        foreach (Card card in dealerCards)
        {
            Debug.Log($"- {card.suit} {card.rank} = {card.GetValue()}");
        }
        Debug.Log($"Общий счет дилера: {dealerScore}");

        playerScoreText.text = "" + playerScore;
        dealerScoreText.text = "" + dealerScore;

        if (playerScore > 21)
        {
            EndGame(false);
        }
    }

    int CalculateScore(List<Card> cards)
    {
        int score = 0;
        foreach (Card card in cards)
        {
            score += card.GetValue();
            Debug.Log($"Добавлено {card.GetValue()} очков от карты {card.suit} {card.rank}");
        }
        return score;
    }

    public void OnHitButton()
    {
        StartCoroutine(DealCard(true));
    }

    public void OnStandButton()
    {
        StartCoroutine(DealerTurn());
    }

    IEnumerator DealerTurn()
    {
        hitButton.interactable = false;
        standButton.interactable = false;

        while (dealerScore < 17)
        {
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(DealCard(false));
        }

        EndGame(playerScore <= 21 && (playerScore > dealerScore || dealerScore > 21));
    }

    void EndGame(bool playerWins)
    {
        if (playerScore > 21)
        {
            if (loseObject != null) loseObject.SetActive(true);
            if (winObject != null) winObject.SetActive(false);
        }
        else if (dealerScore > 21)
        {
            if (winObject != null) winObject.SetActive(true);
            if (loseObject != null) loseObject.SetActive(false);
        }
        else if (playerWins)
        {
            if (winObject != null) winObject.SetActive(true);
            if (loseObject != null) loseObject.SetActive(false);
        }
        else if (playerScore == dealerScore)
        {
            if (winObject != null) winObject.SetActive(false);
            if (loseObject != null) loseObject.SetActive(false);
        }
        else
        {
            if (loseObject != null) loseObject.SetActive(true);
            if (winObject != null) winObject.SetActive(false);
        }

        hitButton.interactable = false;
        standButton.interactable = false;
        playAgainButton.gameObject.SetActive(true);
    }

    public void OnPlayAgainButton()
    {
        InitializeGame();
    }

    void ClearCards()
    {
        foreach (Card card in playerCards)
            Destroy(card.gameObject);
        foreach (Card card in dealerCards)
            Destroy(card.gameObject);
        foreach (Card card in deck)
            Destroy(card.gameObject);

        playerCards.Clear();
        dealerCards.Clear();
        deck.Clear();
    }
}
