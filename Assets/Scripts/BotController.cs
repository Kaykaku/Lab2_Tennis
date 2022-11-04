using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public GameObject ball;
    public float followSpeed;
    public float xRange=8f;
    public float zMaxPos = 12f;
    public float ZMinPos = 0.5f;
    public float maxForce = 12;
    public float minForce = 7;
    public bool isFreeMovement = false;

    // Update is called once per frame
    void Update()
    {
        if (ball == null) return;
        FollowBall();
    }

    //Bot will move according to the ball position
    void FollowBall()
    {
        if (ball.transform.position.z < 0) return;
        Vector3 wantedPosition = new Vector3(ball.transform.position.x, transform.position.y, transform.position.z);
        if (wantedPosition[2] > zMaxPos) wantedPosition[2] = zMaxPos;
        else if (wantedPosition[2] < ZMinPos) wantedPosition[2] = ZMinPos;
        if (wantedPosition[0] > xRange) wantedPosition[0] = xRange;
        else if (wantedPosition[0] < -xRange) wantedPosition[0] = -xRange;
        transform.position = Vector3.MoveTowards(transform.position, wantedPosition, Time.deltaTime * followSpeed+0.1f);
    }
    //Return the direction of the Bot's ball
    public Vector3 BotDirection()
    {
        Vector3 dir = (new Vector3(Random.Range(-xRange,xRange),1f,-12f) -transform.position).normalized;
        dir[1] = 1;
        return dir;
    }

    //Returns BOT's hitting power
    public float BotHitForce()
    {
        return Random.Range(minForce,maxForce);
    }

    //Set the parameters of the BOT according to the difficulty
    public void SetBotDifficulty(int difficulty)
    {
        xRange += difficulty;
        minForce -= difficulty;
        maxForce += difficulty;
        isFreeMovement = difficulty == 0;
        followSpeed -= difficulty;
    }
}
