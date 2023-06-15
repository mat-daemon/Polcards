using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameState
{
    public Tuple<int, int> location1Values;
    public Tuple<List<Card>, List<Card>> location1Cards;

    public Tuple<int, int> location2Values;
    public Tuple<List<Card>, List<Card>> location2Cards;

    public Tuple<int, int> location3Values;
    public Tuple<List<Card>, List<Card>> location3Cards;

    public List<Card> playerCards;
    public List<Card> opponentCards;

    public GameState(Tuple<int, int> location1Values, Tuple<int, int> location2Values, Tuple<int, int> location3Values, Tuple<List<Card>, List<Card>> location1Cards, Tuple<List<Card>, List<Card>> location2Cards, Tuple<List<Card>, List<Card>> location3Cards, List<Card> playerCards, List<Card> opponentCards)
    {
        this.location1Values = location1Values;
        this.location2Values = location2Values;
        this.location3Values = location3Values;
        this.location1Cards = location1Cards;
        this.location2Cards = location2Cards;
        this.location3Cards = location3Cards;
        this.playerCards = playerCards;
        this.opponentCards = opponentCards;
    }
    public GameState Clone()
    {
        GameState clonedState = new GameState(
            new Tuple<int, int>(location1Values.Item1, location1Values.Item2),
            new Tuple<int, int>(location2Values.Item1, location2Values.Item2),
            new Tuple<int, int>(location3Values.Item1, location3Values.Item2),
            new Tuple<List<Card>, List<Card>>(new List<Card>(location1Cards.Item1), new List<Card>(location1Cards.Item2)),
            new Tuple<List<Card>, List<Card>>(new List<Card>(location2Cards.Item1), new List<Card>(location2Cards.Item2)),
            new Tuple<List<Card>, List<Card>>(new List<Card>(location3Cards.Item1), new List<Card>(location3Cards.Item2)),
            new List<Card>(playerCards),
            new List<Card>(opponentCards)
        );

        return clonedState;
    }
}
public class GeneralTree<T> 
{
    private T root;
    public int value;
    public GameState gameState;

    private List<GeneralTree<T>> children { get; set; }

    public GeneralTree(T root)
    {
        this.root = root;
        children = new List<GeneralTree<T>>();
    }

    public List<GeneralTree<T>> getChildren()
    {
        return children;
    }

    public int childCount()
    {
        return children.Count;
    }
    public void addSubTree(GeneralTree<T> subtree)
    {
        children.Add(subtree);
    }

    public void addChildren(List<T> newChildren)
    {
        for (int i = 0; i < newChildren.Count; i++)
        {
            children.Add(new GeneralTree<T>(newChildren[i]));
        }
    }

    public GeneralTree<T> getChild(int i)
    {
        return children[i];
    }

    public int Hight()
    {
        throw new NotImplementedException();
    }

    public bool isLeaf()
    {
        return children.Count == 0;
    }

    public int numberSubTrees()
    {
        return children.Count;
    }

    public List<T> postorder()
    {
        return _postorder(new List<T>());
    }
    private List<T> _postorder(List<T> result)
    {
        for (int i = 0; i < numberSubTrees(); i++)
        {
            ((GeneralTree<T>)children[i])._postorder(result);
        }
        result.Add(root);
        return result;
    }
    public List<T> preorder()
    {
        return _preorder(new List<T>());
    }
    private List<T> _preorder(List<T> result)
    {
        result.Add(root);
        if (!isLeaf())
        {
            foreach (GeneralTree<T> subtree in children)
            {
                ((GeneralTree<T>)subtree)._preorder(result);
            }
        }
        return result;
    }
    public T Root()
    {
        return root;
    }
}

public class AIScript : MonoBehaviour
{
    public List<TMP_Text> playerPoints;
    public List<TMP_Text> opponentPoints;

    public GameController gameController;

    public GameObject player1Location;
    public GameObject player2Location;
    public GameObject player3Location;

    public GameObject opponent1Location;
    public GameObject opponent2Location;
    public GameObject opponent3Location;

    public int HeuristicOption;
    public int depth;
    public int winningBonus;
    public int smallWinningBonus;
    public int smallLost;

    private GeneralTree<Tuple<int, int>> tree;

    private GameState gameState;

    private List<Card> playerCards = new List<Card>(); //zawiera karty gracza
    private List<Card> opponentCards = new List<Card>(); //zawiera karty AI

    private Tuple<int, int> location1Values;//zawiera wynik gracza, AI
    private Tuple<List<Card>, List<Card>> location1Cards; //zawiera karty gracza, AI

    private Tuple<int, int> location2Values;//zawiera wynik gracza, AI
    private Tuple<List<Card>, List<Card>> location2Cards;//zawiera karty gracza, AI

    private Tuple<int, int> location3Values;//zawiera wynik gracza, AI
    private Tuple<List<Card>, List<Card>> location3Cards;//zawiera karty gracza, AI

    private List<string> locationsAbilities = new List<string>();//zawiera umiejêtnoœci lokacji
    private bool firstZeroPlayer;

   

    // Start is called before the first frame update
    void Start()
    {
        tree = new GeneralTree<Tuple<int, int>>(new(-1, -1));
    }

    public Tuple<int, int> GetBestMove() //zwraca numer karty i numer lokacji
    {
        UpdateStateOfGame();

        tree = MakeTree(depth, depth, new(-1, -1), gameState);
        Tuple<int, int> bestMove = new Tuple<int, int>(-1,-1);
        int bestVal = MinMax(depth, true, int.MinValue, int.MaxValue, tree);
        Debug.Log(bestMove);

        for (int i = 0; i < tree.childCount(); i++)
        {
            if (tree.getChild(i).value == bestVal)
            {
                bestMove = tree.getChild(i).Root();
            }
        }
        return bestMove;
    }

    public int MinMax(int depth, bool isMax, int alpha, int beta, GeneralTree<Tuple<int, int>> tree)
    {
        int playerNumber = 1;

        if (isMax)
        {
            playerNumber = 2;// ruch AI
        }

        if (GetMoves(tree.gameState, playerNumber).Count == 0 && GetMoves(tree.gameState, 3- playerNumber).Count == 0) //stan koñca gry
        {
            tree.value = PlayerEndScore(2, tree.gameState);
            return tree.value;
        }

        if (depth == 0)//maksymalna g³êbokoœæ sprawdzania
        {
            tree.value = Heuristic(HeuristicOption, tree.gameState, playerNumber);
            return tree.value;
        }

        if (isMax)
        {
            tree.value = int.MinValue;
            for (int i = 0; i < tree.childCount(); i++)
            {
                tree.value = Math.Max(tree.value, MinMax(depth - 1, false, alpha, beta, tree.getChild(i)));
                alpha = Math.Max(alpha, tree.value);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return tree.value;
        }
        else
        {
            tree.value = int.MaxValue;
            for (int i = 0; i < tree.childCount(); i++)
            {
                tree.value = Math.Min(tree.value, MinMax(depth - 1, true, alpha, beta, tree.getChild(i)));
                beta = Math.Min(beta, tree.value);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return tree.value;
        }
    }
    public int Heuristic(int option, GameState gameState, int playerNumber)
    {
        int playerScore = 0;
        if (option == 0)
        {
            gameState = EndTurnAbilitiesScore(gameState);
            playerScore = gameState.location1Values.Item2 + gameState.location2Values.Item2 + gameState.location3Values.Item2;
            if (gameState.location1Values.Item2 > gameState.location1Values.Item1)
            {
                playerScore += smallWinningBonus;
            }
            if (gameState.location2Values.Item2 > gameState.location2Values.Item1)
            {
                playerScore += smallWinningBonus;
            }
            if (gameState.location3Values.Item2 > gameState.location3Values.Item1)
            {
                playerScore += smallWinningBonus;
            }

            if (gameState.location1Values.Item1 - gameState.location1Values.Item2 < 15)
            {
                playerScore -= smallLost;
            }
            if (gameState.location1Values.Item1 - gameState.location1Values.Item2 < 15)
            {
                playerScore -= smallLost;
            }
            if (gameState.location1Values.Item1 - gameState.location1Values.Item2 < 15)
            {
                playerScore -= smallLost;
            }
        }
        return playerScore;
    }

    public int PlayerEndScore(int playerNumber, GameState gameState)
    {
        int playerScore = 0;

        gameState = EndTurnAbilitiesScore(gameState);

        playerScore = gameState.location1Values.Item2 + gameState.location2Values.Item2 + gameState.location3Values.Item2;

        if (gameState.location1Values.Item2 > gameState.location1Values.Item1)
        {
            playerScore += winningBonus;
        }
        if (gameState.location2Values.Item2 > gameState.location2Values.Item1)
        {
            playerScore += winningBonus;
        }
        if (gameState.location3Values.Item2 > gameState.location3Values.Item1)
        {
            playerScore += winningBonus;
        }

        return playerScore;
    }

    public GameState EndTurnAbilitiesScore(GameState gameState)
    {
        //lokacje gracza 
        foreach (var card in gameState.location1Cards.Item1)
        {
            foreach (var ability in card.abilities)
            {
                if (ability.StartsWith("SemOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location1Cards.Item2, gameState.location1Cards.Item2, details.Length == 3);


                    if (details.Length == 3)
                    {
                        //dodajemy punkty ujemne przeciwnikowi
                        gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + pointsForSymbols);

                    }
                    else
                    {
                        //dodajemy punkty graczowi
                        gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1 + pointsForSymbols, gameState.location1Values.Item2);

                    }
                }
                else if (ability.StartsWith("Sem"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location1Cards.Item1, gameState.location1Cards.Item2, details.Length == 3);


                    if (details.Length == 3)
                    {

                        gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + pointsForSymbols);

                    }
                    else
                    {
                        gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1 + pointsForSymbols, gameState.location1Values.Item2);

                    }
                }
            }
        }

        foreach (var card in gameState.location2Cards.Item1)
        {
            foreach (var ability in card.abilities)
            {
                if (ability.StartsWith("SemOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location2Cards.Item2, gameState.location2Cards.Item2, details.Length == 3);


                    if (details.Length == 3)
                    {
                        //dodajemy punkty ujemne przeciwnikowi
                        gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + pointsForSymbols);

                    }
                    else
                    {
                        //dodajemy punkty graczowi
                        gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + pointsForSymbols, gameState.location2Values.Item2);

                    }
                }
                else if (ability.StartsWith("Sem"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location2Cards.Item1, gameState.location2Cards.Item2, details.Length == 3);


                    if (details.Length == 3)
                    {

                        gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + pointsForSymbols);

                    }
                    else
                    {
                        gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + pointsForSymbols, gameState.location2Values.Item2);

                    }
                }
            }
        }

        foreach (var card in gameState.location3Cards.Item1)
        {
            foreach (var ability in card.abilities)
            {
                if (ability.StartsWith("SemOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location3Cards.Item2, gameState.location3Cards.Item2, details.Length == 3);


                    if (details.Length == 3)
                    {
                        //dodajemy punkty ujemne przeciwnikowi
                        gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + pointsForSymbols);

                    }
                    else
                    {
                        //dodajemy punkty graczowi
                        gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + pointsForSymbols, gameState.location3Values.Item2);

                    }
                }
                else if (ability.StartsWith("Sem"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location3Cards.Item1, gameState.location3Cards.Item2, details.Length == 3);


                    if (details.Length == 3)
                    {

                        gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + pointsForSymbols);

                    }
                    else
                    {
                        gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + pointsForSymbols, gameState.location3Values.Item2);

                    }
                }
            }
        }

        foreach (var card in gameState.location1Cards.Item2)
        {
            foreach (var ability in card.abilities)
            {
                if (ability.StartsWith("SemOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location1Cards.Item1, gameState.location1Cards.Item1, details.Length == 3);


                    if (details.Length == 3)
                    {

                       gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1 + pointsForSymbols, gameState.location1Values.Item2);


                    }
                    else
                    {

                       gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + pointsForSymbols);

                    }
                }
                else if (ability.StartsWith("Sem"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location1Cards.Item2, gameState.location1Cards.Item1, details.Length == 3);


                    if (details.Length == 3)
                    {
                       gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1 + pointsForSymbols, gameState.location1Values.Item2);

                    }
                    else
                    {
                       gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + pointsForSymbols);
  
                    }
                }
            }
        }

        foreach (var card in gameState.location2Cards.Item2)
        {
            foreach (var ability in card.abilities)
            {
                if (ability.StartsWith("SemOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location2Cards.Item1, gameState.location2Cards.Item1, details.Length == 3);


                    if (details.Length == 3)
                    {

                        gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + pointsForSymbols, gameState.location2Values.Item2);


                    }
                    else
                    {

                        gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + pointsForSymbols);

                    }
                }
                else if (ability.StartsWith("Sem"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location2Cards.Item2, gameState.location2Cards.Item1, details.Length == 3);


                    if (details.Length == 3)
                    {
                        gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + pointsForSymbols, gameState.location2Values.Item2);

                    }
                    else
                    {
                        gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + pointsForSymbols);

                    }
                }
            }
        }

        foreach (var card in gameState.location3Cards.Item2)
        {
            foreach (var ability in card.abilities)
            {
                if (ability.StartsWith("SemOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location3Cards.Item1, gameState.location3Cards.Item1, details.Length == 3);


                    if (details.Length == 3)
                    {

                        gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + pointsForSymbols, gameState.location3Values.Item2);


                    }
                    else
                    {

                        gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + pointsForSymbols);

                    }
                }
                else if (ability.StartsWith("Sem"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location3Cards.Item2, gameState.location3Cards.Item1, details.Length == 3);


                    if (details.Length == 3)
                    {
                        gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + pointsForSymbols, gameState.location3Values.Item2);

                    }
                    else
                    {
                        gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + pointsForSymbols);

                    }
                }
            }
        }

        return gameState;
    }

    public GeneralTree<Tuple<int, int>> MakeTree(int depth, int d, Tuple<int, int> posVal, GameState gameState)
    {
        var tree = new GeneralTree<Tuple<int, int>>(posVal);

        tree.gameState = gameState;

        int playerNumber;
        if ((depth - d) % 2 == 0)
        {
            playerNumber = 2;
        }
        else
        {
            playerNumber = 1;
        }

        List<Tuple<int, int>> moves = GetMoves(gameState, playerNumber);

        if (moves.Count == 0 && GetMoves(tree.gameState, 3 - playerNumber).Count == 0) //sprawdziæ zakoñczenie gry i jeœli tak to wartoœæ równa wartoœci na koniec gry
        {
            return tree;
        }

        if (d == 0)
        {
            //tree.value = Heuristic(HeuristicOption, cells, playerNumber); // jeœli okreœlona g³êbokoœæ u¿ywamy heurystyki
            return tree;
        }

        for (int i = 0; i < moves.Count; i++)
        {
            tree.addSubTree(MakeTree(depth, d - 1, moves[i], MakeMove(moves[i].Item1, moves[i].Item2, gameState.Clone(), playerNumber)));
        }

        return tree;
    }
    public GameState MakeMove(int cardNumber, int locationNumber, GameState gameState, int playerNumber)
    {
        if(playerNumber == 2)
        {
            var card = gameState.opponentCards[cardNumber];

            int power = card.power;
            if ((locationNumber == 0) && locationsAbilities[locationNumber] == "FirstZero" && gameState.location1Cards.Item2.Count == 0) //zdolnoœæ za 0 
            {
                power = 0;
                gameState.location1Cards.Item2.Add(card);
            }
            else if ((locationNumber == 1) && locationsAbilities[locationNumber] == "FirstZero" && gameState.location2Cards.Item2.Count == 0) //zdolnoœæ za 0 
            {
                power = 0;
                gameState.location2Cards.Item2.Add(card);
            }
            else if ((locationNumber == 2) && locationsAbilities[locationNumber] == "FirstZero" && gameState.location3Cards.Item2.Count == 0) //zdolnoœæ za 0 
            {
                power = 0;
                gameState.location3Cards.Item2.Add(card);
            }
            else
            {
                if (locationNumber == 0)
                {
                    gameState = CountStrength(cardNumber, locationNumber, gameState, playerNumber); //zmiana wyniku gry
                    gameState.location1Cards.Item2.Add(card);
                    gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + power);//dodanie si³y karty
                }
                else if (locationNumber == 1)
                {
                    gameState = CountStrength(cardNumber, locationNumber, gameState, playerNumber);
                    gameState.location2Cards.Item2.Add(card);
                    gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + power);
                }
                else
                {
                    gameState = CountStrength(cardNumber, locationNumber, gameState, playerNumber);
                    gameState.location3Cards.Item2.Add(card);
                    gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + power);
                }
            }

            gameState.opponentCards.Remove(card);
        }
        else
        {
            var card = gameState.playerCards[cardNumber];
            int power = card.power;

            if ((locationNumber == 0) && locationsAbilities[locationNumber] == "FirstZero" && gameState.location1Cards.Item1.Count == 0) //zdolnoœæ za 0 
            {
                power = 0;
                gameState.location1Cards.Item1.Add(card);
            }
            else if ((locationNumber == 1) && locationsAbilities[locationNumber] == "FirstZero" && gameState.location2Cards.Item1.Count == 0) //zdolnoœæ za 0 
            {
                power = 0;
                gameState.location2Cards.Item1.Add(card);
            }
            else if ((locationNumber == 2) && locationsAbilities[locationNumber] == "FirstZero" && gameState.location3Cards.Item1.Count == 0) //zdolnoœæ za 0 
            {
                power = 0;
                gameState.location3Cards.Item1.Add(card);
            }
            else
            {
                if (locationNumber == 0)
                {
                    gameState = CountStrength(cardNumber, locationNumber, gameState, playerNumber);
                    gameState.location1Cards.Item1.Add(card);
                    gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1 + power, gameState.location1Values.Item2);
                }
                else if (locationNumber == 1)
                {
                    gameState = CountStrength(cardNumber, locationNumber, gameState, playerNumber);
                    gameState.location2Cards.Item1.Add(card);
                    gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + power, gameState.location2Values.Item2);
                }
                else
                {
                    gameState = CountStrength(cardNumber, locationNumber, gameState, playerNumber);
                    gameState.location3Cards.Item1.Add(card);
                    gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + power, gameState.location3Values.Item2);
                }
            }
            gameState.playerCards.Remove(card);
        }

        return gameState;
    }
    public GameState CountStrength(int cardNumber, int locationNumber, GameState gameState, int playerNumber)
    {
        if (playerNumber == 2)
        {
            var card = gameState.opponentCards[cardNumber];
            foreach (var ability in card.abilities)
            {        
                //aded for oponent move
                if (ability.StartsWith("NatOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;

                    if (locationNumber == 0)
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location1Cards.Item1, gameState.location1Cards.Item1, details.Length == 3);
                    }
                    else if (locationNumber == 1)
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location2Cards.Item1, gameState.location2Cards.Item1, details.Length == 3);
                    }
                    else
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location3Cards.Item1, gameState.location3Cards.Item1, details.Length == 3);
                    }
                    
                    if (details.Length == 3)
                    {
                        if (locationNumber == 0)
                        {
                            gameState.location1Values = new Tuple<int,int>(gameState.location1Values.Item1 + pointsForSymbols, gameState.location1Values.Item2);
                        }
                        else if (locationNumber == 1)
                        {
                            gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + pointsForSymbols, gameState.location2Values.Item2);
                        }
                        else
                        {
                            gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + pointsForSymbols, gameState.location3Values.Item2);
                        }
                           
                    }
                    else
                    {
                        if (locationNumber == 0)
                        {
                            gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + pointsForSymbols);
                        }
                        else if (locationNumber == 1)
                        {
                            gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + pointsForSymbols);
                        }
                        else
                        {
                            gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + pointsForSymbols);
                        }
                    }
                }
                else if (ability.StartsWith("Nat"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;
                    if (locationNumber == 0)
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location1Cards.Item2, gameState.location1Cards.Item1, details.Length == 3);
                    }
                    else if (locationNumber == 1)
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location2Cards.Item2, gameState.location2Cards.Item1, details.Length == 3);
                    }
                    else
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location3Cards.Item2, gameState.location3Cards.Item1, details.Length == 3);
                    }

                    if (details.Length == 3)
                    {
                        if (locationNumber == 0)
                        {
                            gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1 + pointsForSymbols, gameState.location1Values.Item2);
                        }
                        else if (locationNumber == 1)
                        {
                            gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + pointsForSymbols, gameState.location2Values.Item2);
                        }
                        else
                        {
                            gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + pointsForSymbols, gameState.location3Values.Item2);
                        }
                    }
                    else
                    {
                        if (locationNumber == 0)
                        {
                            gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + pointsForSymbols);
                        }
                        else if (locationNumber == 1)
                        {
                            gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + pointsForSymbols);
                        }
                        else
                        {
                            gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + pointsForSymbols);
                        }
                    }
                }
            }
        }
        else
        {
            var card = gameState.playerCards[cardNumber];
            foreach (var ability in card.abilities)
            {
                if (ability.StartsWith("NatOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;
                    if (locationNumber == 0)
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location1Cards.Item2, gameState.location1Cards.Item2, details.Length == 3);
                    }
                    else if (locationNumber == 1)
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location2Cards.Item2, gameState.location2Cards.Item2, details.Length == 3);
                    }
                    else
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location3Cards.Item2, gameState.location3Cards.Item2, details.Length == 3);
                    }

                    if (details.Length == 3)
                    {
                        //dodajemy punkty ujemne przeciwnikowi
                        if (locationNumber == 0)
                        {
                            gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + pointsForSymbols);
                        }
                        else if (locationNumber == 1)
                        {
                            gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + pointsForSymbols);
                        }
                        else
                        {
                            gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + pointsForSymbols);
                        }
                    }
                    else
                    {
                        //dodajemy punkty graczowi
                        if (locationNumber == 0)
                        {
                            gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1 + pointsForSymbols, gameState.location1Values.Item2);
                        }
                        else if (locationNumber == 1)
                        {
                            gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + pointsForSymbols, gameState.location2Values.Item2);
                        }
                        else
                        {
                            gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + pointsForSymbols, gameState.location3Values.Item2);
                        }
                    }


                }
                else if (ability.StartsWith("Nat"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = 0;
                    if (locationNumber == 0)
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location1Cards.Item1, gameState.location1Cards.Item2, details.Length == 3);
                    }
                    else if (locationNumber == 1)
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location2Cards.Item1, gameState.location2Cards.Item2, details.Length == 3);
                    }
                    else
                    {
                        pointsForSymbols = countPointsForSymbols(symbol, wage, gameState.location3Cards.Item1, gameState.location3Cards.Item2, details.Length == 3);
                    }

                    if (details.Length == 3)
                    {
                        if (locationNumber == 0)
                        {
                            gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1, gameState.location1Values.Item2 + pointsForSymbols);
                        }
                        else if (locationNumber == 1)
                        {
                            gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1, gameState.location2Values.Item2 + pointsForSymbols);
                        }
                        else
                        {
                            gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1, gameState.location3Values.Item2 + pointsForSymbols);
                        }
                    }
                    else
                    {
                        if (locationNumber == 0)
                        {
                            gameState.location1Values = new Tuple<int, int>(gameState.location1Values.Item1 + pointsForSymbols, gameState.location1Values.Item2);
                        }
                        else if (locationNumber == 1)
                        {
                            gameState.location2Values = new Tuple<int, int>(gameState.location2Values.Item1 + pointsForSymbols, gameState.location2Values.Item2);
                        }
                        else
                        {
                            gameState.location3Values = new Tuple<int, int>(gameState.location3Values.Item1 + pointsForSymbols, gameState.location3Values.Item2);
                        }
                    }
                }
            }
        }
        return gameState;
    }
    private int countPointsForSymbols(int symbol, int wage, List<Card> cards, List<Card> oponentCards, bool oposite = false)
    {
        int points = 0;
        int symbolCounter = 0;
       
        // Additional symbol added
       // if (symbol == locationSymbol) symbolCounter++;

        if (oposite)
        {
            foreach (var cardInLocation in oponentCards)
            {
                foreach (int cardSymbol in cardInLocation.symbols)
                {
                    if (cardSymbol == symbol)
                    {
                        symbolCounter++;
                    }
                }
            }
        }
        else//dodano else ???
        {
            foreach (var cardInLocation in cards)
            {
                foreach (int cardSymbol in cardInLocation.symbols)
                {
                    if (cardSymbol == symbol)
                    {
                        symbolCounter++;
                    }
                }
            }
        }

        points += (symbolCounter * wage);
        return points;
    }

    public List<Tuple<int, int>> GetMoves(GameState gameState, int playerNumber)
    {
        List<Tuple<int, int>> moves = new List<Tuple<int, int>>(20);
        if(playerNumber == 2)
        {
            for(int i = 0; i < gameState.opponentCards.Count; i++)
            {
                if(gameState.location1Cards.Item2.Count != 4)
                {
                    moves.Add(new Tuple<int,int>(i, 0));
                }
                if (gameState.location2Cards.Item2.Count != 4)
                {
                    moves.Add(new Tuple<int, int>(i, 1));
                }
                if (gameState.location3Cards.Item2.Count != 4)
                {
                    moves.Add(new Tuple<int, int>(i, 2));
                }
            }
        }
        else
        {
            for (int i = 0; i < gameState.playerCards.Count; i++)
            {
                if (gameState.location1Cards.Item1.Count != 4)
                {
                    moves.Add(new Tuple<int, int>(i, 0));
                }
                if (gameState.location2Cards.Item1.Count != 4)
                {
                    moves.Add(new Tuple<int, int>(i, 1));
                }
                if (gameState.location3Cards.Item1.Count != 4)
                {
                    moves.Add(new Tuple<int, int>(i, 2));
                }
            }
        }
        //Debug.Log(moves);
        return moves;
    }

    void UpdateStateOfGame()
    {
        location1Values= new Tuple<int,int>(int.Parse(playerPoints[0].GetComponent<TextMeshProUGUI>().text), int.Parse(opponentPoints[0].GetComponent<TextMeshProUGUI>().text));
        location2Values = new Tuple<int, int>(int.Parse(playerPoints[1].GetComponent<TextMeshProUGUI>().text), int.Parse(opponentPoints[1].GetComponent<TextMeshProUGUI>().text));
        location3Values = new Tuple<int, int>(int.Parse(playerPoints[2].GetComponent<TextMeshProUGUI>().text), int.Parse(opponentPoints[2].GetComponent<TextMeshProUGUI>().text));

        location1Cards = new Tuple<List<Card>, List<Card>>(player1Location.GetComponent<CardSlotController>().carsInLocation, opponent1Location.GetComponent<OpponentCardController>().carsInLocation);
        location2Cards = new Tuple<List<Card>, List<Card>>(player2Location.GetComponent<CardSlotController>().carsInLocation, opponent2Location.GetComponent<OpponentCardController>().carsInLocation);
        location3Cards = new Tuple<List<Card>, List<Card>>(player3Location.GetComponent<CardSlotController>().carsInLocation, opponent3Location.GetComponent<OpponentCardController>().carsInLocation);

        locationsAbilities.Clear();
        locationsAbilities.Add(gameController.Locations[0].abilities[0]);
        locationsAbilities.Add(gameController.Locations[1].abilities[0]);
        locationsAbilities.Add(gameController.Locations[2].abilities[0]);

        //foreach (var ability in locationsAbilities)
        //{
        //    if (ability.StartsWith("NatSym"))
        //    {
        //        int symbolAdded = int.Parse(ability.Substring(6));
        //        locationSymbol = symbolAdded;

        //    }
        //    else if (ability.StartsWith("NatPlay"))
        //    {
        //        int strength = int.Parse(ability.Substring(7));
        //        additionalStrength = strength;
        //    }
        //    else if (ability.StartsWith("FirstZero"))
        //    {
        //        zeroOpponentCard = true;
        //        zeroPlayerCard = true;
        //    }
        //    else if (ability.StartsWith("SemMore"))
        //    {
        //        string[] strength = ability.Substring(7).Split(",");
        //        boostSymbolEndTurn = int.Parse(strength[0]);
        //        boostForSymbolEndTurn = 2;
        //    }
        //}

        playerCards.Clear();
        foreach (var card in gameController.cardsInHand)
        {
                playerCards.Add(card.GetComponent<Card>());
           
        }

        opponentCards.Clear();
        foreach (var card in gameController.OpponentCards)
        {
                opponentCards.Add(card.GetComponent<Card>());        
        }
        gameState = new GameState(location1Values, location2Values, location3Values, location1Cards, location2Cards, location3Cards, playerCards, opponentCards);
    }



    //bool EndOfGame()
    //{
    //    if ()
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
