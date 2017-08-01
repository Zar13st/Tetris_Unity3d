using UnityEngine;
using System.Collections.Generic;

public class Figure : MonoBehaviour {

    const int FigureArrSIZE = 4;
    const int STEP = 1;
    const float FALLACCELLERATE = 30f;

    public GameObject Cube;

    public bool FigureInGame { get; set; }

    Glass glass;
    GameController gameCtrl;

    List<GameObject> figureParts = new List<GameObject>();

    float dtMove = 0.2f;
    float moveTime = 0f;
    float dtMoveDown;
    float moveDownTime = 0f;
    float fallDownAccelerator = 1f;

    int[,,] figure;
    int rotation = 0;


    bool canControl = true;

    void Awake()
    {
        glass = FindObjectOfType<Glass>();
        gameCtrl = FindObjectOfType<GameController>();
        Create();
    }

    void Update()
    {
        if (FigureInGame)
        {
            if (canControl) Control();

            if (moveDownTime < Time.time)
            {
                if (CanMoveDown()) { MoveDown(); }
                else { FixInGlass(); }
            }
        }
    }
    //конструктор фигуры
    void Create()
    {
        figure = FindObjectOfType<FigureManager>().GetRandomFigure();
        var randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        dtMoveDown = gameCtrl.dtMoveDown;
        for (int row = 0; row < FigureArrSIZE; row++)
        {
            for (int col = 0; col < FigureArrSIZE; col++)
            {
                if (figure[rotation, row, col] == 1)
                {
                    var c = Instantiate(Cube);
                    c.transform.parent = this.transform;
                    c.GetComponent<Renderer>().material.color = randomColor;
                    c.transform.localPosition = new Vector3(col - 1, -row + 1, 0);
                    c.transform.rotation = Quaternion.identity;
                    figureParts.Add(c);
                }
            }
        }
    }
    // управление фигурой
    void Control()
    {
        if (Input.GetButtonDown("Rotate")) { Rotate(); }
        else
        {
            if (Input.GetButton("MoveRight") && CanMoveRight()) { HorizontalMove(STEP); }
            if (Input.GetButton("MoveLeft") && CanMoveLeft()) { HorizontalMove(-STEP); }
            if (Input.GetButtonDown("falldown")) { Falldown(); }
        }
    }
    // движение фигуры на дно стакана
    void MoveDown()
    {
        if (moveDownTime < Time.time)
        {
            this.transform.position -= new Vector3(0, STEP, 0);
            moveDownTime = Time.time + dtMoveDown / fallDownAccelerator;
        }
    }

    bool CanMoveDown()
    {
        foreach (var part in figureParts)
        {
            int M = GetM(part);
            int N = GetN(part);
            if (M <= 0 || (glass.cubeArr[M - 1, N] != null)) { return false; }
        }
        return true;
    }
 
    void FixInGlass()
    {
        foreach (var part in figureParts)
        {
            int M = GetM(part);
            int N = GetN(part);
            glass.cubeArr[M, N] = part;
            part.transform.parent = glass.transform;
        }
        glass.CheckFullRows();
        Destroy(this.gameObject);
    }

    void Rotate()
    {
        var oldPos = transform.position;
        List<Vector3> oldLocalPos = new List<Vector3>();
        foreach (var part in figureParts)
        {
            oldLocalPos.Add(part.transform.localPosition);
        }

        rotation++;
        int i = 0;
        for (int row = 0; row < FigureArrSIZE; row++)
        {
            for (int col = 0; col < FigureArrSIZE; col++)
            {
                if (figure[rotation % 4, row, col] == 1)
                {
                    figureParts[i].transform.localPosition = new Vector3(col - 1, -row + 1, 0);
                    // если кубик вышел за пределы стакана при вращении, сдвигаем всю фигуру внутрь стакана, а так же приподнимаем при коллизиях с дном.
                    SmartRotation(i);
                    i++;
                }
            }
        }
        // если после поворота фигура налезает на кубики в стакане, то отменяем поворот
        if (!rotationCorrect())
        {
            int j = 0;
            this.transform.position = oldPos;
            foreach (var part in figureParts)
            {
                part.transform.localPosition = oldLocalPos[j];
                j++;
            }
        }
    }

    void SmartRotation(int i)
    {
        int N = GetN(figureParts[i]);
        if (N > glass.maxN - 1) { transform.position -= new Vector3(N - (glass.maxN - 1), 0, 0); }
        if (N < 0) { transform.position += new Vector3(-N, 0, 0); }
        int M = GetM(figureParts[i]);
        if (M < 0) { transform.position += new Vector3(0, -M, 0); }
    }

    bool rotationCorrect()
    {
        foreach (var part in figureParts)
        {
            int M = GetM(part);
            int N = GetN(part);
            if (glass.cubeArr[M, N] != null) { return false; }
        }
        return true;
    }

    void HorizontalMove(int direction)
    {
        if (moveTime < Time.time)
        {
            this.transform.position += new Vector3(direction, 0, 0);
            moveTime = Time.time + dtMove;
        }
    }

    bool CanMoveRight()
    {
        foreach (var part in figureParts)
        {
            int M = GetM(part);
            int N = GetN(part);
            if (N >= (glass.maxN - 1) || (glass.cubeArr[M, N + 1] != null)) { return false; }
        }
        return true;
    }

    bool CanMoveLeft()
    {
        foreach (var part in figureParts)
        {
            int M = GetM(part);
            int N = GetN(part);
            if (N <= 0 || (glass.cubeArr[M, N - 1] != null)) { return false; }
        }
        return true;
    }

    void Falldown()
    {
        canControl = false;
        fallDownAccelerator = FALLACCELLERATE;
    }

    int GetN(GameObject c)
    {
        return (int)(c.transform.localPosition.x + this.transform.position.x - STEP / 2f) + glass.maxN / 2;
    }

    int GetM(GameObject c)
    {
        return (int)(c.transform.localPosition.y + this.transform.position.y - STEP / 2f) + glass.maxM / 2;
    }
}
