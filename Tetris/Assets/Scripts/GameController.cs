using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public int Score { get; set; }
    public float dtMoveDown
    {
        get
        {
            if (gameLevel <= 12) { return 0.525f - 0.025f * gameLevel; }
            else { return 0.2f; }
        }
    }

    public GameObject Figure;
    public GameObject MainMenu;
    public GameObject GameOverText;

    public Text ScoreText;
    public Text SpeedText;
    
    public Transform CurrentFigureSpawn;
    public Transform NextFigureSpawn;

    GameObject nextFigure;

    float _speed;

    int gameLevel = 1;
    int figuresPlayed;
    int fuguresPerLevel = 10;

    public void StartGame()
    {
        Reload();
        RefreshUI();
        MainMenu.SetActive(false);
        SetFirstCurrentFigure();
        SetNextFigure();
    }
    // главный игровой цикл: следующую фигруру делаем текущей и в ней происходит Update()
    public void StartNewGameCycle()
    {
        figuresPlayed++;
        nextFigure.transform.position = CurrentFigureSpawn.position;
        nextFigure.GetComponent<Figure>().FigureInGame = true;
        SetNextFigure();
        if ((figuresPlayed % fuguresPerLevel) == 0) { gameLevel++; }
        RefreshUI();
    }

    public void GameOver()
    {
        MainMenu.SetActive(true);
        GameOverText.SetActive(true);
    }

    void Reload()
    {
        Score = 0;
        gameLevel = 0;
        FindObjectOfType<Glass>().ClearGlass();
        if (nextFigure != null)
        {
            Destroy(nextFigure.gameObject);
        }
    }

    void SetFirstCurrentFigure()
    {
        figuresPlayed++;
        var currFigure = Instantiate(Figure);
        currFigure.transform.position = CurrentFigureSpawn.position;
        currFigure.GetComponent<Figure>().FigureInGame = true;
    }

    void SetNextFigure()
    {
        nextFigure = Instantiate(Figure);
        nextFigure.transform.localPosition = NextFigureSpawn.position;
    }

    void RefreshUI()
    {
        ScoreText.text = "Score: " + Score.ToString();
        SpeedText.text = "Level: " + gameLevel.ToString();
    }
}
