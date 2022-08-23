using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    #region Variables

    private NavMeshAgent agent;

    private Transform ballHolster;

    [SerializeField]
    private Transform[] boxes;

    private Vector3 startingPos;

    [SerializeField] [Range(1f, 5f)]
    private float droppingDistance;

    public bool canSpawnBall = false;

    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ballHolster = transform.GetChild(0);

        startingPos = transform.position;
    }

    public IEnumerator BeginTask(Ball ball)
    {
        canSpawnBall = false;

        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        SphereCollider ballCollider = ball.GetComponent<SphereCollider>();

        agent.SetDestination(ball.transform.position);

        yield return new WaitForSeconds(0.1f); // gives a second so agent can calculate remaining distance

        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        // rob has reached the ball at this point
        // operation to pick up ball
        ballRb.constraints = RigidbodyConstraints.FreezePosition;
        ballCollider.enabled = false;
        ball.transform.parent = ballHolster;
        ball.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(1f);

        agent.SetDestination(ball.ChosenBox.position);

        yield return new WaitForSeconds(0.1f); // gives a second so agent can calculate remaining distance
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        // rob has now reached the box to deposit the ball in
        // places the ball a set distance above the box and "drops it in"
        ballRb.constraints = RigidbodyConstraints.None;
        ballCollider.enabled = true;
        ball.transform.parent = null;
        ball.transform.localPosition = ball.ChosenBox.position + new Vector3(0, droppingDistance, 0);
        ball.GetComponent<AudioSource>().Play();
        StartCoroutine(ball.Completion());

        yield return new WaitForSeconds(1f);

        agent.SetDestination(startingPos);
        agent.stoppingDistance = 0.1f;

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        canSpawnBall = true;
    }
}
