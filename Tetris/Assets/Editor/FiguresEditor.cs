using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

public class FiguresEditor : EditorWindow {

    static TextAsset pref;
    static string file_path = "";

    const int rotationCount = 4;
    const int rowCount = 4;
    const int colloumnCount = 4;

    string defaultFigurePath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + "Assets//Resources//DefaultFigures.txt";

    int[,,] figurePartsArr = new int[rotationCount, rowCount, colloumnCount];
    Color[,,] battonColor = new Color[rotationCount, rowCount, colloumnCount];

    [MenuItem("Game Data/Figures Editor")]
    static public void OpenFiguresEditorWindow()
    {
        GetWindow<FiguresEditor>(false, "Figures Editor", true);
    }

    string GetAssetPath()
    {
        return EditorPrefs.GetString("figures_path", string.Empty);
    }

    void SaveAssetPath(string path)
    {
        EditorPrefs.SetString("figures_path", path);
    }

    void OnGUI()
    {
        if (string.IsNullOrEmpty(file_path))
        {
            file_path = GetAssetPath();
        }

        if (pref == null && !string.IsNullOrEmpty(file_path))
        {
            var go = AssetDatabase.LoadAssetAtPath(file_path, typeof(TextAsset)) as TextAsset;

            if (go != null) { pref = go; }
        }

        pref = EditorGUI.ObjectField(new Rect(3, 3, position.width - 6 , 20), "Figures Editor", pref, typeof (TextAsset) , false) as TextAsset;

        GUILayout.Space(26f);

        if (pref != null)
        {
            file_path = AssetDatabase.GetAssetPath(pref);
            SaveAssetPath(file_path);
        }

        GUILayout.BeginHorizontal();
        // очищаем поле ввода новой фигуры
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear Fild", GUILayout.Width(126f)))
        {
            for (int rotation = 0; rotation < rotationCount; rotation++)
            {
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < colloumnCount; col++)
                    {
                        figurePartsArr[rotation, row, col] = 0;
                        battonColor[rotation, row, col] = Color.gray;
                    }
                }
            }
        }
        //сохраняем новую фигруру в фаил
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add Figure", GUILayout.Width(126f)))
        {
            if (figureCorrect())
            {
                var text = new StringBuilder();

                for (int rotation = 0; rotation < rotationCount; rotation++)
                {
                    for (int row = 0; row < rowCount; row++)
                    {
                        for (int col = 0; col < colloumnCount; col++)
                        {
                            text.Append(figurePartsArr[rotation, row, col].ToString());
                            if (col != colloumnCount - 1) { text.Append("|"); }
                        }
                        text.Append("\r\n");
                    }
                    text.Append("\r\n");
                }

                File.AppendAllText(Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + file_path, text.ToString());
                AssetDatabase.Refresh();
                pref = null;
            }
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Load default", GUILayout.Width(126f)))
        {
            {
                File.Copy(defaultFigurePath, Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + file_path, true);
                AssetDatabase.Refresh();
                pref = null;
            }
        }


        GUILayout.EndHorizontal();

        GUILayout.Space(26f);
        // рисуем кнопки для создания поля ввода новой фигуры
        GUILayout.BeginHorizontal(GUILayout.MinWidth(10f));
        for (int rotation = 0; rotation < rotationCount; rotation++)
        {
            GUILayout.Space(26f);
            GUILayout.BeginVertical();
            for (int row = 0; row < rowCount; row++)
            {
                GUILayout.BeginHorizontal();
                for (int col = 0; col < colloumnCount; col++)
                {
                    if (battonColor[rotation, row, col] == Color.clear)
                    {
                        GUI.backgroundColor = Color.gray;
                    }
                    else { GUI.backgroundColor = battonColor[rotation, row, col]; }

                    if (GUILayout.Button(figurePartsArr[rotation,row,col].ToString(), GUILayout.Width(26f)))
                    {
                        figurePartsArr[rotation, row, col] = (figurePartsArr[rotation, row, col] + 1) % 2;
                        if (figurePartsArr[rotation, row, col] == 0) { battonColor[rotation, row, col] = Color.gray; }
                        else { battonColor[rotation, row, col] = Color.green; }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
    // фигуры должны состоять из одинакового числа блоков во всех состояниях поворота
    bool figureCorrect()
    {
        int[] partSum = new int[rotationCount];
        for (int rotation = 0; rotation < rotationCount; rotation++)
        {
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colloumnCount; col++)
                {
                    if (figurePartsArr[rotation, row, col] == 1) { partSum[rotation]++; }
                }
            }
            if (partSum[rotation] != partSum[0] || partSum[rotation] == 0) { return false; }
        }
        return true;
    }
}
