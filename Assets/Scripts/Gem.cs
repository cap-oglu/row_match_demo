using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour

{
    //[HideInInspector]
    public Vector2Int posIndex;
    //[HideInInspector]
    public Board board;

    private Vector2 firstTouchPositon;
    private Vector2 finalTouchPositon;

    private bool mousePressed;
    private float swipeAngle = 0;


    private Gem otherGem;

    public enum GemType {blue, green, red, yellow, stone};
    public int[] gemPoint = {200, 150, 100, 250, 0};
    public GemType type;

    public bool isMatched;

    
    private Vector2Int previousPos;

    public GameObject destroyEffect;

    public int scoreValue = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > .01f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            //board.allGems[posIndex.x, posIndex.y] = this;
        }

        if(mousePressed && Input.GetMouseButtonUp(0))
        {   
            mousePressed = false;
            if (board.currentState == Board.BoardState.move && board.roundManager.roundNumber > 0)
            {
                finalTouchPositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalcualateAngle();
            }
        }
    }

    public void SetupGem(Vector2Int pos, Board theBoard)
    {
        posIndex = pos;
        board = theBoard;
    }


    private void OnMouseDown()
    {
        if(board.currentState == Board.BoardState.move && board.roundManager.roundNumber > 0) { 
            firstTouchPositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       
            mousePressed = true;
        }
    }

    private void CalcualateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPositon.y - firstTouchPositon.y, finalTouchPositon.x - firstTouchPositon.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;
        
        if(Vector3.Distance(firstTouchPositon, finalTouchPositon) > .5f ) 
        {
            MovePieces();
        }
        
    }

    private void MovePieces()
    {
        previousPos = posIndex;
        
        if(type == GemType.stone)
        {
            return;
        }
        
        if(swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1) 
        { 
            otherGem = board.allGems[posIndex.x + 1, posIndex.y];
            if(otherGem.type == GemType.stone)
            {
                return;
            }

            otherGem.posIndex.x--;
            posIndex.x++;

            string temp = otherGem.name;
            otherGem.name = this.name;
            this.name = temp;
        }
        else if(swipeAngle < 135 && swipeAngle > 45 && posIndex.y < board.height - 1)
        {
            otherGem = board.allGems[posIndex.x, posIndex.y + 1];
            if (otherGem.type == GemType.stone)
            {
                return;
            }
            otherGem.posIndex.y--;
            posIndex.y++;

            string temp = otherGem.name;
            otherGem.name = this.name;
            this.name = temp;
        }
        else if (swipeAngle < -45 && swipeAngle > -135 && posIndex.y > 0)
        {
            otherGem = board.allGems[posIndex.x, posIndex.y - 1];
            if (otherGem.type == GemType.stone)
            {
                return;
            }
            otherGem.posIndex.y++;
            posIndex.y--;

            string temp = otherGem.name;
            otherGem.name = this.name;
            this.name = temp;
        }
        else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
        {
            otherGem = board.allGems[posIndex.x - 1, posIndex.y];
            if (otherGem.type == GemType.stone)
            {
                return;
            }
            otherGem.posIndex.x++;
            posIndex.x--;

            string temp = otherGem.name;
            otherGem.name = this.name;
            this.name = temp;
        }

        board.allGems[posIndex.x,posIndex.y] = this;
        board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

   
        StartCoroutine(CheckMoveCo());

    }
    
    public IEnumerator CheckMoveCo()
    {
        board.currentState = Board.BoardState.wait;

        yield return new WaitForSeconds(.5f);

        board.matchFind.FindAllMatches();

        if(otherGem != null)
        {
            if(!isMatched && !otherGem.isMatched)
            {
                //otherGem.posIndex = posIndex;
                //posIndex = previousPos;

                //board.allGems[posIndex.x, posIndex.y] = this;
                //board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;
                
                //yield return new WaitForSeconds(.5f);
                board.currentState = Board.BoardState.move;
                //round manager decrease
                board.roundManager.roundNumber--;
            }
            else
            {
                  board.DestroyMatches();
            }
        }
    }
}
