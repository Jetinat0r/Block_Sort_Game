using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Bottle : MonoBehaviour
{
    private int height = 4;
    private GameObject[] currentLiquids;
    private bool bottleCompleted = false;
    private BottleManager bottleManager;
    [SerializeField]
    private GameObject bottomOfBottle;
    [SerializeField]
    private GameObject topOfBottle;
    [SerializeField]
    private GameObject bottleCapper;
    [SerializeField]
    private ParticleSystem confetti;
    [SerializeField]
    private Animator shaker;

    //Time for liquids to move
    [SerializeField]
    private float timeToShift = 0.2f;
    //Time for bottle to move
    [SerializeField]
    public float timeToMove = 0.2f;
    [SerializeField]
    private float distUp = 1f;
    private Vector3 initialPos;
    private Vector3 upPos;

    private float yHeightOfBottle = 10f;
    private float yHeightOfLiquid = 0.6715f;

    //0 IS THE BOTTOM OF THE BOTTLE

    public void CreateBottle(int _height, GameObject[] _startLiquids, BottleManager _bottleManager)
    {
        height = _height;

        yHeightOfBottle = topOfBottle.transform.position.y - bottomOfBottle.transform.position.y;
        yHeightOfLiquid = yHeightOfBottle / height;

        currentLiquids = new GameObject[height];
        CreateHeightMarkers();
        GameObject[] _physicalStartLiquids = CreateFirstLiquids(_startLiquids);
        AddLiquid(_physicalStartLiquids, _physicalStartLiquids.Length);
        bottleManager = _bottleManager;
    }

    public void SetBottomOfBottle( GameObject bottomOfBottle )
    {
        this.bottomOfBottle = bottomOfBottle;
    }

    public GameObject GetBottomOfBottle()
    {
        return bottomOfBottle;
    }

    public void SetTopOfBottle( GameObject topOfBottle )
    {
        this.topOfBottle = topOfBottle;
    }

    public GameObject GetTopOfBottle()
    {
        return topOfBottle;
    }

    public void SetBottleCapper( GameObject bottleCapper )
    {
        this.bottleCapper = bottleCapper;
    }

    public GameObject GetBottleCapper()
    {
        return bottleCapper;
    }

    public void SetConfetti( ParticleSystem confetti )
    {
        this.confetti = confetti;
    }

    public ParticleSystem GetConfetti()
    {
        return confetti;
    }

    public void SetShaker( Animator shaker )
    {
        this.shaker = shaker;
    }

    public Animator GetShaker()
    {
        return shaker;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        Debug.Log("Key Pressed");
    //        CheckCompleted();
    //    }
    //}

    public void CheckCompleted()
    {
        //Debug.Log(currentLiquids.Length + " " + height);

        //Change if from "currentLiquids.Length == height" JIC
        if (currentLiquids[0].transform.childCount != 0)
        {
            string _color = currentLiquids[0].transform.GetChild(0).GetComponent<Liquid>().GetColor();

            for(int i = 1; i < currentLiquids.Length; i++)
            {
                if(currentLiquids[i].transform.childCount == 0 || currentLiquids[i].transform.GetChild(0).GetComponent<Liquid>().GetColor() != _color)
                {
                    return;
                }
            }
            CompleteBottle();
        }
    }

    public GameObject GetTopLiquid()
    {
        GameObject _currentTop = null;
        for(int i = 0; i < height; i++)
        {
            if (currentLiquids[i].transform.childCount != 0 && currentLiquids[i].GetComponentInChildren<Liquid>().GetColor() != null)
            {
                _currentTop = currentLiquids[i];
            }
        }

        //If they are all empty, the bottom one is chosen
        if(_currentTop == null)
        {
            _currentTop = currentLiquids[0];
        }

        return _currentTop;
    }

    public int CheckNumOfTop(string _topColor)
    {
        int _numOfTop = 0;

        for (int i = height - 1; i >= 0; i--)
        {
            if(currentLiquids[i].transform.childCount == 0)
            {
                continue;
            }
            else if (currentLiquids[i].transform.GetChild(0).GetComponent<Liquid>().GetColor() == _topColor)
            {
                _numOfTop++;
            }
            else
            {
                return _numOfTop;
            }
        }

        return _numOfTop;
    }

    public int CheckCurrentEmpty()
    {
        int _empty = height;

        for(int i = 0; i < height; i++)
        {
            if (currentLiquids[i].transform.childCount != 0)
            {
                _empty--;
            }
        }

        return _empty;
    }


    //CALLED TOO EARLY FOR SETUP
    public void AddLiquid(GameObject[] _liquids, int _numToReplace)
    {
        int _numReplaced = 0;

        if(_numToReplace == 0)
        {
            return;
        }

        for(int i = 0; i < height; i++)
        {
            if(currentLiquids[i].transform.childCount == 0)
            {
                //currentLiquids[i].AddComponent(typeof(Liquid));
                //currentLiquids[i].GetComponent<Liquid>().SetColor(_liquid[i].GetComponent<Liquid>().GetColor());

                //currentLiquids[i] = _liquids[i];

                //currentLiquids[i].transform.SetParent()
                _liquids[_numReplaced].transform.SetParent(currentLiquids[i].transform);
                StartCoroutine(ShiftLiquid(_liquids[_numReplaced], _liquids[_numReplaced].transform.parent.position));
                //_liquids[_numReplaced].transform.position = _liquids[_numReplaced].transform.parent.position;

                _numReplaced++;

                //Debug.Log("Num to replace: " + _numToReplace + ", Num replaced: " + _numReplaced);

                if(_numToReplace == _numReplaced)
                {
                    return;
                }
            }
        }

        Debug.LogError("Something went wrong in Class: Bottle, Method: Add Liquid");
    }

    //public void RemoveLiquid(int _numToRemove)
    //{
    //    for(int i = height - 1; i >= 0; i--)
    //    {
    //        if(currentLiquids[i] != null && _numToRemove > 0)
    //        {
    //            currentLiquids[i].GetComponent<Liquid>().SetColor(null);
    //            _numToRemove--;
    //        }
    //        else
    //        {
    //            return;
    //        }
    //    }
    //}

    public Move TransferLiquid(GameObject _other)
    {
        return TransferLiquid(_other.GetComponent<Bottle>(), false, -1);
    }

    public Move TransferLiquid(Bottle _other, bool isUndo, int undoLiquidCount)
    {
        GameObject _liquidToTransfer = this.GetTopLiquid();

        //Checks if liquid transfer fails

        if (_other.GetTopLiquid().transform.childCount != 0)
        {
            if ( (! isUndo) && (_liquidToTransfer.transform.childCount == 0 || _other.GetTopLiquid().transform.GetChild(0).GetComponent<Liquid>().GetColor() != _liquidToTransfer.transform.GetChild(0).GetComponent<Liquid>().GetColor()))
            {
                ShakeBottle();
                return null;
            }
        }

        int _otherFree = _other.CheckCurrentEmpty();
        if(_otherFree == 0)
        {
            ShakeBottle();
            return null;
        }

        //Debug.Log("Other Free = " + _otherFree);

        int _numToTransfer = this.CheckNumOfTop(_liquidToTransfer.transform.GetChild(0).GetComponent<Liquid>().GetColor());

        if ( isUndo )
        {
            _numToTransfer = undoLiquidCount;
        }

        //Debug.Log("NumToTransfer before = " + _numToTransfer);

        if (_numToTransfer > _otherFree)
        {
            _numToTransfer = _otherFree;
        }

        //Debug.Log("NumToTransfer after = " + _numToTransfer);

        //Grabs the necessary liquids from the top
        GameObject[] _liquidsToTransfer = new GameObject[_numToTransfer];
        int _numNabbed = 0;
        for(int i = height - 1; i >= 0; i--)
        {
            if(_numNabbed != _numToTransfer)
            if (currentLiquids[i].transform.childCount == 0)
            {
                continue;
            }
            else if (isUndo || (currentLiquids[i].GetComponentInChildren<Liquid>().GetColor() == _liquidToTransfer.transform.GetChild(0).GetComponent<Liquid>().GetColor()))
            {
                _liquidsToTransfer[_numNabbed] = currentLiquids[i].transform.GetChild(0).gameObject;
                _numNabbed++;
            }
            else
            {
                continue;
            }
        }

        //Debug.Log("Here comes the add liquid");

        _other.AddLiquid(_liquidsToTransfer, _numToTransfer);

        if ( ! isUndo )
        {
            bottleManager.IncrementMoves();
        }

        bottleManager.ClearHeldBottle();
        //this.RemoveLiquid(_numToTransfer);

        //bottleManager.SaveBottleStates();
        return new Move( this, _other, _numToTransfer );
    }

    public void ResetCompleteBottle()
    {
        if ( bottleCompleted )
        {
            bottleCompleted = false;
            bottleCapper.SetActive(false);

            bottleManager.RemoveCompleteBottle();
        }
    }

    private void CompleteBottle()
    {
        bottleCompleted = true;
        bottleCapper.SetActive(true);

        if ( confetti != null )
	{
            confetti.Play();
	}

        bottleManager.CompleteBottle();

        //Debug.Log("Bottle Completed!!!");
    }

    //public string PrintBottleContents()
    //{
    //    string _contents = "";
    //    for(int i = 0; i < height; i++)
    //    {
    //        if(currentLiquids[i].transform.childCount != 0)
    //        {
    //            _contents += currentLiquids[i].transform.GetChild(0).GetComponent<Liquid>().GetColor();
                
    //        }
    //        else
    //        {
    //            _contents += "empty";
    //        }

    //        if (i != height - 1)
    //        {
    //            _contents += ", ";
    //        }
    //    }

    //    return _contents;
    //}

    public void CreateHeightMarkers()
    {
        for(int i = 0; i < height; i++)
        {
            GameObject _object = new GameObject("Liquid Spot " + i);
            currentLiquids[i] = _object;
            _object.transform.SetParent(bottomOfBottle.transform);
                                                                                          /*                                IMPORTANT                          */
            _object.transform.position = new Vector3 (bottomOfBottle.transform.position.x, bottomOfBottle.transform.position.y + (yHeightOfLiquid * (i + 0.5f)), bottomOfBottle.transform.position.z);
        }
    }

    public bool GetIsCompleted()
    {
        return bottleCompleted;
    }

    private GameObject[] CreateFirstLiquids(GameObject[] _liquids)
    {
        int _nonNull = 0;
        for(int i = 0; i < _liquids.Length; i++)
        {
            if (_liquids[i] != null)
            {
                _nonNull++;
            }
        }

        GameObject[] _physicalLiquids = new GameObject[_nonNull];

        for(int i = 0; i < _nonNull; i++)
        {
            GameObject _physicalLiquid = Instantiate(_liquids[i]);

            
            //SET SCALE OF LIQUID, COME BACK HERE LATER FOR X SCALE
                                                                                                      /*     IMPORTANT     */
            _physicalLiquid.transform.localScale = new Vector3(_physicalLiquid.transform.localScale.x, yHeightOfLiquid / 10, _physicalLiquid.transform.localScale.z);
            _physicalLiquid.name = _physicalLiquid.GetComponent<Liquid>().GetColor() + " Liquid";
            _physicalLiquids[i] = _physicalLiquid;
        }

        return _physicalLiquids;
    }

    public void ShakeBottle()
    {
        shaker.StopPlayback();
        shaker.Play("Shake");
    }

    private void OnMouseDown()
    {
        bottleManager.RecieveBottleInfo(this.gameObject);
    }

    public void SetMovePos()
    {
        initialPos = transform.position;
        upPos = new Vector3(transform.position.x, transform.position.y + distUp, transform.position.z);
    }

    public void FixDumbLiquids()
    {
        for(int i = 0; i < currentLiquids.Length; i++)
        {
            if(currentLiquids[i].transform.childCount != 0)
            {
                currentLiquids[i].transform.GetChild(0).transform.position = currentLiquids[i].transform.position;
            }
        }
    }

    public IEnumerator ShiftLiquid(GameObject _liquidInQuestion, Vector3 _newPosition)
    {
        Vector3 _startPos = _liquidInQuestion.transform.position;
        float _t = 0;

        while(_t < 1)
        {
            _t += Time.deltaTime / timeToShift;

            _liquidInQuestion.transform.position = Vector3.Lerp(_startPos, _newPosition, _t);
            yield return null;
        }
    }

    public IEnumerator ShiftBottle(bool _isDown)
    {
        Vector3 _startPos = transform.position;
        Vector3 _nextPos;
        float _t = 0;

        if(_isDown)
        {
            _nextPos = upPos;
        }
        else
        {
            _nextPos = initialPos;
        }

        while(_t < 1)
        {
            _t += Time.deltaTime / timeToMove;

            transform.position = Vector3.Lerp(_startPos, _nextPos, _t);
            yield return null;
        }
    }


    #region WACKY STUFF FOR RESTART
    //These actually break it fuck
    //public GameObject[] GetCurrentLiquids()
    //{
    //    return currentLiquids;
    //}

    public void SetCurrentLiquids()
    {
        height = transform.GetChild(1).transform.childCount;
        currentLiquids = new GameObject[height];
        for(int i = 0; i < height; i++)
        {
            currentLiquids[i] = transform.GetChild(1).transform.GetChild(i).gameObject;
        }
    }

    //Do i even need you???
    public void SetBottleManager(BottleManager _bottleManager)
    {
        bottleManager = _bottleManager;
    }
    #endregion
}
