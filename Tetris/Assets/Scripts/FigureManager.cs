using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class FigureManager : MonoBehaviour {
    const int ROTATIONTYPES = 4;
    const int FigureSIZE = 4;

    string figurePath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + "Assets//Resources//Figures.txt";

    List<int[,,]> figuresArr = new List<int[,,]>();

    void Awake()
    {
        var text = File.ReadAllText(figurePath);
        var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        int[,,] figure = new int[ROTATIONTYPES, FigureSIZE, FigureSIZE];
        int rotation = 0;
        if (lines.Length % 16 != 0) Debug.Log("Wrong Input Figures");

        for (int line = 0; line < lines.Length; line++)
        {
            var isCube = lines[line].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int col = 0; col < isCube.Length; col++)
            {
                figure[rotation, line % FigureSIZE, col] = int.Parse(isCube[col]);
            }
            if (line % FigureSIZE == 3) rotation++;
            if (line % 16 == 15)
            {
                figuresArr.Add(figure);
                figure = new int[ROTATIONTYPES, FigureSIZE, FigureSIZE];
                rotation = 0;
            }
        }
    }
    
    public int[,,] GetRandomFigure()
    {
        int rand = UnityEngine.Random.Range(0, figuresArr.Count);
        int[,,] randomFigure = figuresArr[rand];
        return randomFigure;
    }
}
