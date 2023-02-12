using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject undoButton;
    [SerializeField]
    private GameObject redoButton;
    [SerializeField]
    private GameObject restartButton;
    [SerializeField]
    private GameObject settingsButton;
    [SerializeField]
    private GameObject numMovesText;
    [SerializeField]
    private GameObject settingsScreen;
    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private InputField numColorsInput;
    [SerializeField]
    private InputField numEmptyInput;
    [SerializeField]
    private InputField heightInput;
    [SerializeField]
    private GameObject winScreen;
    [SerializeField]
    private GameObject finalMovesText;

    public void Restart()
    {
        FindObjectOfType<BottleManager>().Restart();
    }

    public void Undo()
    {
        FindObjectOfType<BottleManager>().Undo();
    }

    public void Redo()
    {
        FindObjectOfType<BottleManager>().Redo();
    }

    public void OpenGameSettings()
    {
        undoButton.SetActive(false);
        redoButton.SetActive(false);
        restartButton.SetActive(false);
        settingsButton.SetActive(false);
        numMovesText.SetActive(false);
        settingsScreen.SetActive(true);
        backButton.SetActive(true);
    }

    public void CloseGameSettings()
    {
        undoButton.SetActive(true);
        redoButton.SetActive(true);
        restartButton.SetActive(true);
        settingsButton.SetActive(true);
        numMovesText.SetActive(true);
        settingsScreen.SetActive(false);
        backButton.SetActive(false);
    }

    public void GenerateNewBottles()
    {
        int _height;
        int _numColors;
        int _numEmpty;

        //if (heightInput.text == "" || numColorsInput.text == "" || numEmptyInput.text == "")
        //{
        //    return;
        //}
        if(heightInput.text == "")
        {
            _height = 3;
        }
        else
        {
            _height = System.Convert.ToInt16(heightInput.text);
        }

        if(numColorsInput.text == "")
        {
            _numColors = 3;
        }
        else
        {
            _numColors = System.Convert.ToInt16(numColorsInput.text);
        }

        if (numEmptyInput.text == "")
        {
            _numEmpty = 1;
        }
        else
        {
            _numEmpty = System.Convert.ToInt16(numEmptyInput.text);
        }

        FindObjectOfType<BottleManager>().DestroyBottles();
        FindObjectOfType<GameManager>().CreateRandomBottles(Mathf.Clamp(_height, 3, 10), Mathf.Clamp(_numColors, 2, 15), Mathf.Clamp(_numEmpty, 1, 5));
        CloseGameSettings();
    }

    public void UpdateMoveCount(int _numMoves)
    {
        numMovesText.GetComponent<Text>().text = "Moves: " + _numMoves;
    }

    public void OpenWinScreen(int _numMoves)
    {
        undoButton.SetActive(false);
        redoButton.SetActive(false);
        restartButton.SetActive(false);
        settingsButton.SetActive(false);
        numMovesText.SetActive(false);
        settingsScreen.SetActive(false);
        backButton.SetActive(false);
        winScreen.SetActive(true);

        finalMovesText.GetComponent<Text>().text = "Moves: " + _numMoves;
    }

    public void WinToSettingsScreen()
    {
        settingsScreen.SetActive(true);
        backButton.SetActive(false);
        winScreen.SetActive(false);
    }

    public void WinToNewGame()
    {
        GenerateNewBottles();

        FindObjectOfType<BottleManager>().ResetMoves();

        winScreen.SetActive(false);
        undoButton.SetActive(true);
        redoButton.SetActive(true);
        restartButton.SetActive(true);
        settingsButton.SetActive(true);
        numMovesText.SetActive(true);
    }
}
