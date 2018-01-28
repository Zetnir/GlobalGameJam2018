using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {
    public Rigidbody ballRB;

    float velocityDiminition = 0.1f;
    public float power = 1f;
    public int powerStack = 1;
    public int localpower_;
    float attackDuration = 0.5f;
    public int indexPlayer = 0;
    public int zonePosition;

    public bool BombStart = false;
    public float BombDuration = 60f;
    public float currentBombTime = 0f;
    public GameManager gameManager;

    // Use this for initialization
    void Start() {
        ballRB = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update() {
        if (ballRB.velocity.x != 0 && ballRB.velocity.z != 0)
        {
            ballRB.velocity = Vector3.Lerp(ballRB.velocity, Vector3.zero, velocityDiminition * Time.deltaTime);
            power = ballRB.velocity.magnitude;
        }

        if (power > 20)
        {
            powerStack = 2;
        }
        if (power > 50)
        {
            powerStack = 3;
        }
        if (power > 80)
        {
            powerStack = 4;
        }
        else if (power < 20)
        {
            powerStack = 1;
        }

        power = Mathf.Clamp(power, 0, 120);

        if( Time.time - currentBombTime > BombDuration)
        {
            BombStart = true;
            gameManager.BombExplode(zonePosition);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player" && (collision.collider.GetComponent<PlayerController>().numController != indexPlayer && !collision.collider.GetComponent<PlayerController>().isBonus))
        {
            print(collision.collider.name);
            collision.collider.GetComponent<PlayerController>().Stun(powerStack);
        }

        if(collision.collider.GetComponent<Rigidbody>())
            collision.collider.GetComponent<Rigidbody>().velocity = Vector3.zero;

    }

    public void BallAttack(Vector3 direction, int playerIndex)
    {
        localpower_ = powerStack;
        indexPlayer = playerIndex;
        ballRB.velocity = Vector3.zero;
        StartCoroutine(moveBall(direction));
    }

    IEnumerator moveBall(Vector3 direction)
    {
        yield return new WaitForSeconds(attackDuration);
        ballRB.AddForce(direction * localpower_, ForceMode.Impulse);
    }
}
