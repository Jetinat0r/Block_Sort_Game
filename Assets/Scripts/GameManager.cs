using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject bottlePrefab;
    [SerializeField]
    private GameObject[] liquidPrefabs;

    //DEBUG VARIABLES
    [SerializeField]
    private int DEBUGHEIGHT;
    [SerializeField]
    private int DEBUGCOLORS;
    [SerializeField]
    private int DEBUGEMPTY;

    public void SetBottlePrefab( GameObject bottlePrefab )
    {
        this.bottlePrefab = bottlePrefab;
    }

    public GameObject GetBottlePrefab()
    {
        return bottlePrefab;
    }

    public void SetLiquidPrefabs( GameObject[] liquidPrefabs )
    {
        this.liquidPrefabs = liquidPrefabs;
    }

    public GameObject[] GetLiquidPrefabs()
    {
        return liquidPrefabs;
    }

    private void Awake()
    {
        //GameObject[] _objs = GameObject.FindGameObjectsWithTag("Game Manager");

        //if(_objs.Length > 1)
        //{
        //    Destroy(this.gameObject);
        //}

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateRandomBottles(DEBUGHEIGHT, DEBUGCOLORS, DEBUGEMPTY);
    }


    public void DEBUGSTART()
    {
        CreateRandomBottles(DEBUGHEIGHT, DEBUGCOLORS, DEBUGEMPTY);
    }

    //_numColors = number of bottles that start with colors too
    public void CreateRandomBottles(int _height, int _numColors, int _numEmpty)
    {
        FindObjectOfType<BottleManager>().DestroyInitalBottles();
        FindObjectOfType<BottleManager>().ResetMoves();
        GameObject[] _createdBottles = new GameObject[_numColors + _numEmpty];

        //For adding garbage liquids to
        //ArrayList[] _fakeBottles = new ArrayList[_numColors];
        //for(int i = 0; i < _fakeBottles.Length; i++)
        //{
        //    _fakeBottles[i] = new ArrayList();
        //}

        GameObject[][] _fakeFullBottles = new GameObject[_numColors][];
        for(int i = 0; i < _numColors; i++)
        {
            _fakeFullBottles[i] = new GameObject[_height];
        }

        //Fills fake bottles to the brim (no yankee)
        //i is the # color we are on
        //for(int i = 0; i < _numColors; i++)
        //{
        //    int _counter = 0;
        //    while(_counter < _height)
        //    {
        //        int _randomNumber = Random.Range(0, _numColors);
        //        //if(_fakeBottles[_randomNumber].Count < _height)
        //        //{
        //        //    _fakeBottles[i].Add(liquidPrefabs[i]);
        //        //    _counter--;
        //        //}
        //        for(int j = 0; j < _height; j++)
        //        {
        //            if(_fakeFullBottles[_randomNumber][j] == null)
        //            {
        //                _fakeFullBottles[_randomNumber][j] = liquidPrefabs[i];
        //                _counter++;
        //                break;
        //            }
        //        }
        //    }
        //}

        //Odd Attempt was me trying to move from using arrays and random numbers to arraylists that will never have a "bad" roll
        List<List<GameObject>> _oddAttempt = new List<List<GameObject>>();
        //i is the # color we are on
        for (int i = 0; i < _numColors; i++)
        {
            _oddAttempt.Add(new List<GameObject>());
        }

        int _fakeBottlesFilled = 0;

        for (int i = 0; i < _numColors; i++)
        {
            int _counter = 0;
            while (_counter < _height)
            {
                int _randomNumber = Random.Range(0, _oddAttempt.Count);
 
                _oddAttempt[_randomNumber].Add(liquidPrefabs[i]);
                _counter++;

                //Creates a full, randomized bottle
                //Only activates when a bottle is full
                if (_oddAttempt[_randomNumber].Count == _height)
                {
                    List<GameObject> _randomizerList = new List<GameObject>();

                    //copies _oddAttempt[_randomNumber] to _randomizerList
                    for (int j = 0; j < _height; j++)
                    {
                        _randomizerList.Add(_oddAttempt[_randomNumber][j]);
                    }

                    //_oddAttempt is set to a random part of _randomizerList, which removes what was just yoinked
                    //Essentially randomizes the order of the bottle once full
                    while (_randomizerList.Count > 0)
                    {
                        int _secondRandomNumber = Random.Range(0, _randomizerList.Count);
                        _oddAttempt[_randomNumber][_randomizerList.Count - 1] = _randomizerList[_secondRandomNumber];
                        _randomizerList.RemoveAt(_secondRandomNumber);
                    }

                    //fill _fakeFullBottles
                    for(int j = 0; j < _height; j++)
                    {
                        _fakeFullBottles[_fakeBottlesFilled][j] = _oddAttempt[_randomNumber][j];
                    }


                    _fakeBottlesFilled++;

                    _oddAttempt.RemoveAt(_randomNumber);
                }
            }
        }

        //Fills full bottles
        for (int i = 0; i < _numColors; i++)
        {
            _createdBottles[i] = Instantiate(bottlePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            _createdBottles[i].GetComponent<Bottle>().CreateBottle(_height, _fakeFullBottles[i], FindObjectOfType<BottleManager>());
            _createdBottles[i].GetComponent<Bottle>().StopAllCoroutines();
        }

        //Fills empty bottles
        GameObject[] _emptyBottle = new GameObject[_height];
        for (int i = 0; i < _height; i++)
        {
            _emptyBottle[i] = null;
        }

        for (int i = 0; i < _numEmpty; i++)
        {
            _createdBottles[i + _numColors] = Instantiate(bottlePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            _createdBottles[i + _numColors].GetComponent<Bottle>().CreateBottle(_height, _emptyBottle, FindObjectOfType<BottleManager>());
        }

        FindObjectOfType<BottleManager>().PlaceBottles(_numColors, _numEmpty, _createdBottles);
    }
}
