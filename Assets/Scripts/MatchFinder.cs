using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    public List<Gem> currentMatches = new List<Gem>();

    private void Awake()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {   
        currentMatches.Clear(); //video 22. 7:00


        for(int i = 0; i < board.height; i++)
        {
            Gem currentGem = board.allGems[0, i];

            if (currentGem != null) { 
                if(currentGem.type == Gem.GemType.stone)
                {
                    continue;
                }
                int counter = 0;
                for(int j = 1; j < board.width; j++)
                {
                    if (board.allGems[j,i] != null)
                    {
                        if ((board.allGems[j,i].type != currentGem.type))
                        {
                            break;
                        }
                        else
                        {
                            counter++;
                        }
                    }
                }
                if(counter == board.width - 1)
                {
                    for(int k = 0; k < board.width; k++)
                    {
                        board.allGems[k,i].isMatched = true;
                        currentMatches.Add(board.allGems[k,i]);
                    }
                }
                

                
            }
        }
        /*for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            { 
                Gem currentGem = board.allGems[i,j];
                if (currentGem != null)
                {
                    if(i > 0 && i < board.width - 1)
                    {
                        Gem leftGem = board.allGems[i - 1,j];
                        Gem rightGem = board.allGems[i + 1, j];

                        if(leftGem != null && rightGem != null)
                        {
                            if(leftGem.type == currentGem.type && rightGem.type == currentGem.type)
                            {
                                currentGem.isMatched = true;
                                leftGem.isMatched = true;
                                rightGem.isMatched = true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(leftGem);
                                currentMatches.Add(rightGem);


                            }
                        }

                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        Gem aboveGem = board.allGems[i, j + 1];
                        Gem belowGem = board.allGems[i, j - 1];

                        if (aboveGem != null && belowGem != null)
                        {
                            if (aboveGem.type == currentGem.type && belowGem.type == currentGem.type)
                            {
                                currentGem.isMatched = true;
                                aboveGem.isMatched = true;
                                belowGem.isMatched = true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(aboveGem);
                                currentMatches.Add(belowGem);
                            }
                        }

                    }
                }
            }
        }*/

        if(currentMatches.Count > 0)
        {
            currentMatches = currentMatches.Distinct().ToList();
        }
    }
}
