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

    public const string HEIGHT_OF_BOTTLES_PREF    = "HeightOfBottles";
    public const int    HEIGHT_OF_BOTTLES_DEFAULT = 3;

    public const string NUM_OF_COLORS_PREF    = "NumOfColors";
    public const int    NUM_OF_COLORS_DEFAULT = 3;

    public const string NUM_EMPTY_BOTTLES_PREF    = "NumEmptyBottles";
    public const int    NUM_EMPTY_BOTTLES_DEFAULT = 1;

    public void LoadUserPrefs()
    {
        heightInput.text =
            PlayerPrefs.GetInt(HEIGHT_OF_BOTTLES_PREF,
                               HEIGHT_OF_BOTTLES_DEFAULT).ToString();
        numColorsInput.text =
            PlayerPrefs.GetInt(NUM_OF_COLORS_PREF,
                               NUM_OF_COLORS_DEFAULT).ToString();
        numEmptyInput.text =
            PlayerPrefs.GetInt(NUM_EMPTY_BOTTLES_PREF,
                               NUM_EMPTY_BOTTLES_DEFAULT).ToString();
    }

    public void StoreUserPrefs()
    {
        int _height = HEIGHT_OF_BOTTLES_DEFAULT;
        int _numColors = NUM_OF_COLORS_DEFAULT;
        int _numEmpty = NUM_EMPTY_BOTTLES_DEFAULT;

        if(heightInput.text != "")
        {
            _height = System.Convert.ToInt16(heightInput.text);
        }
        PlayerPrefs.SetInt(HEIGHT_OF_BOTTLES_PREF, _height);

        if(numColorsInput.text != "")
        {
            _numColors = System.Convert.ToInt16(numColorsInput.text);
        }
        PlayerPrefs.SetInt(NUM_OF_COLORS_PREF, _numColors);

        if(numEmptyInput.text != "")
        {
            _numEmpty = System.Convert.ToInt16(numEmptyInput.text);
        }
        PlayerPrefs.SetInt(NUM_EMPTY_BOTTLES_PREF, _numEmpty);
    }

    public void Awake()
    {
        // Called once at startup to restore the user's preferences.
        LoadUserPrefs();
    }

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

        // Capture the user's preferences when the dialog is withdrawn.
	StoreUserPrefs();
    }

    public void GenerateNewBottles()
    {
        int _height = HEIGHT_OF_BOTTLES_DEFAULT;
        int _numColors = NUM_OF_COLORS_DEFAULT;
        int _numEmpty = NUM_EMPTY_BOTTLES_DEFAULT;

        if(heightInput.text != "")
        {
            _height = System.Convert.ToInt16(heightInput.text);
        }

        if(numColorsInput.text != "")
        {
            _numColors = System.Convert.ToInt16(numColorsInput.text);
        }

        if (numEmptyInput.text != "")
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
