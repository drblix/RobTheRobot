using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Robot : MonoBehaviour
{
    // lessons i've learned .-. :
    // don't destroy the object you originally called the coroutine from :)

    #region Constants

    private const int HALLWAY_LENGTH = 12;

    #endregion

    #region Variables

    private NavMeshAgent agent;
    private Rigidbody robotRb;

    private Transform ballHolster;

    [SerializeField]
    private Transform[] boxes;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    private Vector3 startingPos;
    private Quaternion startingRot;

    [SerializeField] [Range(1f, 5f)]
    private float droppingDistance;

    private bool canSpawnBall = true;
    public bool CanSpawnBall { get { return canSpawnBall; } }

    private bool usePathfinding = false;
    public bool UsePathfinding { get { return usePathfinding; } }

    private int deliveredBalls = 0;
    
    #endregion

    private void Awake()
    {
        robotRb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        ballHolster = transform.GetChild(0);

        startingPos = transform.position;
        startingRot = transform.rotation;
    }

    public IEnumerator BeginTask(GameObject ball)
    {
        canSpawnBall = false;

        Ball ballClass = ball.GetComponent<Ball>();
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        SphereCollider ballCollider = ball.GetComponent<SphereCollider>();

        agent.stoppingDistance = 1.5f;
        agent.SetDestination(ball.transform.position);

        yield return new WaitForSeconds(0.1f); // gives a second so agent can calculate remaining distance
        while (!CompletedCurrentPath()) { yield return null; }

        // rob has reached the ball at this point
        // operation to pick up ball
        PickupBall(ballClass, ballRb, ballCollider);

        yield return new WaitForSeconds(1f);

        agent.stoppingDistance = 1f;
        agent.SetDestination(ballClass.ChosenBox.position);

        yield return new WaitForSeconds(0.1f);
        while (!CompletedCurrentPath()) { yield return null; }

        // rob has now reached the box to deposit the ball in
        // places the ball a set distance above the box and "drops it in"
        DropBall(ballClass, ballRb, ballCollider);

        yield return new WaitForSeconds(1f);

        agent.stoppingDistance = 0.25f;
        agent.SetDestination(startingPos);

        yield return new WaitForSeconds(0.1f);
        while (!CompletedCurrentPath()) { yield return null; }

        transform.position = startingPos;
        transform.rotation = startingRot;

        canSpawnBall = true;
    }

    
    public IEnumerator PerformTranslateInstructions(GameObject ball)
    {
        canSpawnBall = false;

        robotRb.constraints = RigidbodyConstraints.FreezeAll;
        agent.enabled = false;
        transform.position = startingPos;
        transform.rotation = startingRot;

        Ball ballClass = ball.GetComponent<Ball>();
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        SphereCollider ballCollider = ball.GetComponent<SphereCollider>();

        // doing '-1' so rob doesn't clip inside the ball
        for (int i = 0; i < (HALLWAY_LENGTH - 1); i++)
        {
            transform.Translate(Vector3.forward, Space.Self);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        PickupBall(ballClass, ballRb, ballCollider);

        yield return new WaitForSeconds(1f);

        transform.Translate(Vector3.forward, Space.Self);
        transform.Rotate(Vector3.up * -90f);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < HALLWAY_LENGTH; i++)
        {
            transform.Translate(Vector3.forward, Space.Self);
            yield return new WaitForSeconds(0.5f);
        }

        transform.Rotate(Vector3.up * -90f);

        int distance = ballClass.ChosenBox.name switch
        {
            "RedBox" => 4,
            "GreenBox" => 8,
            "BlueBox" => 12,
            _ => throw new System.Exception("Invalid box name"), // default if no previous case was selected
        };

        for (int i = 0; i < distance; i++)
        {
            transform.Translate(Vector3.forward, Space.Self);
            yield return new WaitForSeconds(0.5f);
        }

        transform.Rotate(Vector3.up * 90f);

        yield return new WaitForSeconds(1f);

        DropBall(ballClass, ballRb, ballCollider);
        // ball is dropped at this moment

        yield return new WaitForSeconds(1f);

        transform.Rotate(Vector3.up * -90f);

        for (int i = 0; i < (HALLWAY_LENGTH - distance); i++)
        {
            transform.Translate(Vector3.forward, Space.Self);
            yield return new WaitForSeconds(0.5f);
        }

        transform.Rotate(Vector3.up * -90f);

        for (int i = 0; i < HALLWAY_LENGTH; i++)
        {
            transform.Translate(Vector3.forward, Space.Self);
            yield return new WaitForSeconds(0.5f);
        }

        transform.Rotate(Vector3.up * -90f);
        // reset back to starting position

        robotRb.constraints = RigidbodyConstraints.None;
        agent.enabled = true;
        canSpawnBall = true;
    }

    private void PickupBall(Ball ball, Rigidbody ballRb, SphereCollider ballCollider)
    {
        ballRb.constraints = RigidbodyConstraints.FreezePosition;
        ballCollider.enabled = false;
        ball.transform.parent = ballHolster;
        ball.transform.localPosition = Vector3.zero;
    }

    private void DropBall(Ball ball, Rigidbody ballRb, SphereCollider ballCollider)
    {
        ballRb.constraints = RigidbodyConstraints.None;
        ballCollider.enabled = true;
        ball.transform.parent = null;
        ball.transform.localPosition = ball.ChosenBox.position + new Vector3(0, droppingDistance, 0);
        ball.GetComponent<AudioSource>().Play();

        deliveredBalls++;
        scoreText.SetText("<b>Successful ball deliveries:</b>\n" + deliveredBalls.ToString());
    }

    private bool CompletedCurrentPath()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }

    public void SetPathfinding(bool state)
    {
        usePathfinding = state;
    }
}
