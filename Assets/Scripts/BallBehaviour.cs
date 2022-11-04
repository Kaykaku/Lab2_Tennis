using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallBehaviour : MonoBehaviour
{
    public float speed=1f;
    public float force=1f;
    public float time;
    public float radius=0.25f;
    public float bounceForce=1f;
    public float xRange=9f;
    public float zRange=12f;
    public bool isCollision =false;
    public int groundCollision =0;
    public bool isCheck =false;
    public bool isPlayerHit =false;
    private GameManager gameManager;
    private PlayerController playerController;
    private BotController botController;
    private CalculatorScore calculatorScore;
    public Vector3 direction;
    private Vector3 startPos;
    private Vector3 currentPos;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        direction = new Vector3(0, 2f, transform.position.z > 0 ? -4:4) ;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        botController = GameObject.Find("Bot").GetComponent<BotController>();
        calculatorScore = GetComponent<CalculatorScore>();
    }
    // Update is called once per frame
    void Update()
    {
        currentPos = PositionByTime(time * speed);
        time += Time.deltaTime;
        ProcessPosNotOverlap();
        CheckOutZone();

        if (isCollision)
        {
            ProcessBounce();
            return;
        }
        
        DetectCollision();
    }

    public Vector3 PositionByTime(float t)
    {
        return startPos + direction * t * force + 0.5f * Physics.gravity * t * t;
    }
    void DetectCollision()
    {
        Vector3 nextPos = PositionByTime((time + Time.deltaTime) * speed);
        float distance = Vector3.Distance(currentPos, nextPos) + radius;
        Ray ray = new Ray(transform.position, transform.TransformDirection((nextPos - transform.position)));
        isCollision = Physics.Raycast(ray, out hit, distance);
    }
    void ProcessBounce()
    {
        startPos = transform.position;

        if (hit.transform.CompareTag("Player"))
        {
            direction = playerController.PlayerDirection();
            force = playerController.PlayerHitForce();
            groundCollision = 0;
            isPlayerHit = true;
        }
        else if (hit.transform.CompareTag("Ground"))
        {
            groundCollision ++;
            direction = Vector3.Reflect(direction, Vector3.zero) * bounceForce;
        }
        else if (hit.transform.name.Equals("Bot"))
        {
            direction = botController.BotDirection();
            force = botController.BotHitForce();
            groundCollision = 0;
            isPlayerHit = false;
        }
        else
        {
            direction = Vector3.Reflect(direction, hit.normal) * bounceForce;
        }
        time = 0;
        isCollision = false;
    }
    void ProcessPosNotOverlap()
    {
        if (transform.position.Equals(currentPos)) return;
        if (currentPos.y < radius) currentPos[1] = radius;
        transform.position = currentPos;
    }

    void CheckOutZone()
    {
        if (isCheck) return;
        if( transform.position.z > zRange
            || transform.position.z < -zRange
            || transform.position.x > xRange
            || transform.position.x < -xRange || groundCollision>0)
        {
            isCheck = calculatorScore.CheckPoint(transform.position , isPlayerHit , groundCollision);
        }
    }
}
