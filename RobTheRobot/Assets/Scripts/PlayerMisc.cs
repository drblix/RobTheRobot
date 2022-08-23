using UnityEngine;

public class PlayerMisc : MonoBehaviour
{
    private Robot robot;
    private MovableCamera movableCam;

    [SerializeField]
    private Transform ballSpawn;

    [SerializeField]
    private Camera robCam;

    [SerializeField]
    private GameObject ballPrefab;

    private void Awake()
    {
        robot = FindObjectOfType<Robot>();
        movableCam = FindObjectOfType<MovableCamera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Camera thisCam = GetComponent<Camera>();

            thisCam.GetComponent<AudioListener>().enabled = !thisCam.GetComponent<AudioListener>().enabled;
            thisCam.enabled = !thisCam.enabled;

            robCam.GetComponent<AudioListener>().enabled = !robCam.GetComponent<AudioListener>().enabled;
            robCam.enabled = !robCam.enabled;

            movableCam.canMove = !movableCam.canMove;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (FindObjectOfType<Ball>() || !robot.canSpawnBall) { return; }

            GameObject newBall = Instantiate(ballPrefab);

            newBall.transform.position = ballSpawn.position;
        }
    }
}
