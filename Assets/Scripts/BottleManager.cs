using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleManager : MonoBehaviour
{
    //list holds bottle states at each move, array holds each bottle
    //private List<GameObject[]> moves = new List<GameObject[]>();

    //For the "restart" thing
    private GameObject[] initialBottles;
    private Stack<Move> undoMoves = new Stack<Move>();
    private Stack<Move> redoMoves = new Stack<Move>();

    private GameObject[] bottles;
    private GameObject heldBottle;
    private int numColors = 99;
    private int numEmpty = 99;
    private int numCompleted = 0;

    private int numMoves = 0;


    //pos1 is top left bottle
    [SerializeField]
    private Transform pos1;
    //pos2 is bottom right bottle
    [SerializeField]
    private Transform pos2;

    //GAME MANAGER BUILDS THIS AND DOES THINGS



    //_bottles MUST ALREADY HAVE COMPLETED BOTTLES FED INTO IT
    //THIS FUNCTION MERELY PLACES THE BOTTLES IN A PHYSICAL PLACE AND TELLS THE BOTTLE MANAGER HOW MANY BOTTLES NEED TO BE FILLED
    public void PlaceBottles(int _numColors, int _numEmpty, GameObject[] _bottles)
    {
        numColors = _numColors;
        numEmpty = _numEmpty;
        numCompleted = 0;

        float _xDist = pos2.position.x - pos1.position.x;
        float _yDist = pos1.position.y - pos2.position.y;

        //_max[var]Spots are 1 less than expected to make the math work
        int _maxXSpots = 3;
        int _maxYSpots = 4;
        int _xSpot = 0;
        int _ySpot = 0;

        for (int i = 0; i < _bottles.Length; i++)
        {
            //                                                                              X MATH                                           Y MATH
            float _xFraction = ((float)(_xSpot) / (float)(_maxXSpots));
            float _yFraction = ((float)(_ySpot) / (float)(_maxYSpots));

            //Debug.Log(_bottles[i]);

            _bottles[i].transform.position = new Vector3(pos1.transform.position.x + (_xDist * _xFraction), pos1.transform.position.y - (_yDist * _yFraction), _bottles[i].transform.position.z);

            //MAKE LIQUIDS GO INTO THE CORRECT SPOT HERE
            _bottles[i].GetComponent<Bottle>().FixDumbLiquids();

            _xSpot++;
            if(_xSpot == _maxXSpots + 1)
            {
                _ySpot++;
                _xSpot = 0;
            }
        }


        for (int i = 0; i < _bottles.Length; i++)
        {
            _bottles[i].GetComponent<Bottle>().SetMovePos();
            _bottles[i].GetComponent<Bottle>().CheckCompleted();
        }

        //Who knows what this could be used for
        //Deleting bottles, apparently
        bottles = _bottles;

        SetInitialBottles();
    }

    public void RecieveBottleInfo(GameObject _clickedBottle)
    {
        if (_clickedBottle.GetComponent<Bottle>().GetIsCompleted() == false)
        {
            if (heldBottle == null)
            {
                if(_clickedBottle.GetComponent<Bottle>().GetTopLiquid().transform.childCount == 0)
                {
                    _clickedBottle.GetComponent<Bottle>().ShakeBottle();
                    return;
                }
                //Debug.Log("Picked up (" + _clickedBottle.GetComponent<Bottle>().PrintBottleContents() + ")");

                _clickedBottle.GetComponent<Bottle>().StopAllCoroutines();
                StartCoroutine(_clickedBottle.GetComponent<Bottle>().ShiftBottle(true));

                heldBottle = _clickedBottle;
            }
            else if (heldBottle == _clickedBottle)
            {
                ClearHeldBottle();
                return;
            }
            else
            {
                //Debug.Log("Glug glug!");
                Move move = heldBottle.GetComponent<Bottle>().TransferLiquid(_clickedBottle);
                if(move != null)
                {
                    undoMoves.Push(move);
                    redoMoves.Clear();
                }
                _clickedBottle.GetComponent<Bottle>().CheckCompleted();
            }
        }
        else
        {
            //Shake
            _clickedBottle.GetComponent<Bottle>().ShakeBottle();
        }
    }

    public void RemoveCompleteBottle()
    {
        numCompleted--;
    }

    public void CompleteBottle()
    {
        numCompleted++;
        if(numCompleted == numColors)
        {
            FindObjectOfType<PlayerUI>().OpenWinScreen(numMoves);
        }
    }

    public void ClearHeldBottle()
    {
        //Debug.Log("Put down (" + heldBottle.GetComponent<Bottle>().PrintBottleContents() + ")");
        if(heldBottle != null)
        {
            StartCoroutine(heldBottle.GetComponent<Bottle>().ShiftBottle(false));
            heldBottle = null;
        }
    }

    //public void SaveBottleStates()
    //{
    //    Debug.Log("saved!");
    //    moves.Add(bottles);
    //}

    /*
     * Delay the execution of the Undo() method to allow time for the
     * held bottle to return to its un-held position.
     */
    public IEnumerator InvokeUndoLater( Move m, float delay )
    {
      yield return new WaitForSeconds( delay );
      m.Undo();
      redoMoves.Push(m);
    }

    public void Undo()
    {
        if( undoMoves.Count > 0 )
        {
            Move m = undoMoves.Pop();
            ClearHeldBottle();
            StartCoroutine( InvokeUndoLater( m, m.Source.timeToMove ) );
        }
    }

    /*
     * Delay the execution of the Redo() method to allow time for the
     * held bottle to return to its un-held position.
     */
    public IEnumerator InvokeRedoLater( Move m, float delay )
    {
      yield return new WaitForSeconds( delay );
      m.Redo();
      undoMoves.Push(m);
    }

    public void Redo()
    {
        if( redoMoves.Count > 0 )
        {
            Move m = redoMoves.Pop();
            ClearHeldBottle();
            StartCoroutine( InvokeRedoLater( m, m.Source.timeToMove ) );
        }
    }

    public void SetInitialBottles()
    {
        initialBottles = new GameObject[numColors + numEmpty];
        for(int i = 0; i < bottles.Length; i++)
        {
            initialBottles[i] = Instantiate(bottles[i], new Vector3(-100, -100, 0), new Quaternion(0, 0, 0, 0));

            //Why do I need this???
            initialBottles[i].GetComponent<Bottle>().SetCurrentLiquids();
            initialBottles[i].GetComponent<Bottle>().SetBottleManager(this);
        }
    }

    public void Restart()
    {
        DestroyBottles();
        ResetMoves();
	undoMoves.Clear();
	redoMoves.Clear();
        PlaceBottles(numColors, numEmpty, initialBottles);
    }

    public void DestroyBottles()
    {
        foreach(GameObject _bottle in bottles)
        {
            Destroy(_bottle);
        }
    }

    public void DestroyInitalBottles()
    {
        if(initialBottles != null)
        {
            foreach (GameObject _bottle in initialBottles)
            {
                Destroy(_bottle);
            }
        }
    }

    public void IncrementMoves()
    {
        numMoves++;
        FindObjectOfType<PlayerUI>().UpdateMoveCount(numMoves);
    }

    public void ResetMoves()
    {
        numMoves = 0;
        FindObjectOfType<PlayerUI>().UpdateMoveCount(0);
    }
}
