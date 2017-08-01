using UnityEngine;
using System.Collections.Generic;

public class Glass : MonoBehaviour {
    int deleteRowBelow;

    const int EXTRAM = 3;

    public GameObject explosion;
    public GameObject[,] cubeArr;

    public int maxN = 10;
    public int maxM = 20;

    GameController gameCtrl;

    List<int> deleteRowArr;
	
	void Awake () {
        gameCtrl = FindObjectOfType<GameController>();
        cubeArr = new GameObject[maxM + EXTRAM, maxN];
	}
    // перед новой игрой очищаем игровое поле (стакан)
    public void ClearGlass()
    {
        for (int row = 0; row < maxM + EXTRAM; row++)
        {
            for (int col = 0; col < maxN; col++)
            {
                if (cubeArr[row,col] != null)
                {
                    Destroy(cubeArr[row, col].gameObject);
                    cubeArr[row, col] = null;
                }
            }
        }
    }
	// проверяем ряды на заполненность...
    public void CheckFullRows()
    {
        deleteRowArr = new List<int>();
        int firstFreeRow = maxM;
        for (int row = 0; row < maxM; row++)
        {
            int cubesInRow = 0;
            for (int col = 0; col < maxN; col++)
            {
                if (cubeArr[row, col] != null) { cubesInRow++; }
            }

            if (cubesInRow == maxN) { deleteRowArr.Add(row); }
            if (cubesInRow == 0) { firstFreeRow = row; break; }
        }

        int deleteRowCount = deleteRowArr.Count;
        gameCtrl.Score += deleteRowCount;

        if (deleteRowCount != 0)
        {
            DeleteFullRows();
            ShiftDownRows();
        }

        if (firstFreeRow == maxM) { gameCtrl.GameOver(); }
        else { gameCtrl.StartNewGameCycle(); }
    }
    //... если нашли заполненные - удаляем их ...
    void DeleteFullRows()
    {
        foreach (var row in deleteRowArr)
        {
            for (int col = 0; col < maxN; col++)
            {
                 var expl = Instantiate(explosion);
                 expl.transform.position = cubeArr[row, col].transform.position;
                 expl.transform.parent = this.transform;
                 Destroy(expl.gameObject, 1.0f);
                 
                 Destroy(cubeArr[row, col].gameObject);
                 cubeArr[row, col] = null;

            }
        }
    }
    // ... спускаем оставшиемя ряды на место удалённых
    void ShiftDownRows()
    {
        deleteRowBelow = 0;
        for (int row = 0; row < maxM; row++)
        {
            if (deleteRowArr.Contains(row)) { deleteRowBelow++; }
            else 
            {
                if (deleteRowBelow != 0)
                {
                    for (int col = 0; col < maxN; col++)
                    {
                        if (cubeArr[row, col] != null)
                        {
                            cubeArr[row, col].transform.localPosition -= new Vector3(0, deleteRowBelow, 0);
                            cubeArr[row - deleteRowBelow, col] = cubeArr[row, col];
                            cubeArr[row, col] = null;
                        }
                    }
                }
            }
        }
    }
}
