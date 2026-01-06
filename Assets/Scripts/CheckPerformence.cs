using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPerformence : MonoBehaviour
{
    public List<int> winCount = new List<int> { 0, 0, 0, 0 };
    public List<int> drawCount = new List<int> { 0, 0, 0, 0 };
    public List<int> loseCount = new List<int> { 0, 0, 0, 0 };
    public List<float> scoreBoard = new List<float>();
    public float totalScore;
    public float totalCount;
    public void UpdateCount(int type, int index)
    {
        switch (type)
        {
            case 0:
                winCount[index]++;
                break;
            case 1:
                drawCount[index]++;
                break;
            case 2:
                loseCount[index]++;
                break;
            default:
                break;
        }
    }
    void Update()
    {
        float wintotal = 0;
        float drawtotal = 0;
        float losetotal = 0;
        for (int i = 0; i < 4; i++)
        {
            wintotal += winCount[i];
            drawtotal += drawCount[i];
            losetotal += loseCount[i];
        }
        totalScore = wintotal / (wintotal + drawtotal + losetotal);
        totalCount = wintotal + drawtotal + losetotal;
        totalScore *= 100;
        if (totalCount >= 1000)
        {
            scoreBoard.Add(totalScore);
            totalScore = 0;
            totalCount = 0;
            winCount = new List<int> { 0, 0, 0, 0 };
            drawCount = new List<int> { 0, 0, 0, 0 };
            loseCount = new List<int> { 0, 0, 0, 0 };
        }
    }
}
