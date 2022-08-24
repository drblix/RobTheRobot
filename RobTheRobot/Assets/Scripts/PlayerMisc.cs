using UnityEngine;
using TMPro;

public class PlayerMisc : MonoBehaviour
{
    private Robot robot;
    private MovableCamera movableCam;

    [SerializeField]
    private TextMeshProUGUI methodText;

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
            if (!robot.CanSpawnBall) { return; }

            foreach (Ball b in FindObjectsOfType<Ball>())
            {
                Destroy(b.gameObject);
            }

            GameObject newBall = Instantiate(ballPrefab);

            newBall.transform.position = ballSpawn.position;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!robot.CanSpawnBall) { return; }

            robot.SetPathfinding(!robot.UsePathfinding);

            if (robot.UsePathfinding)
            {
                methodText.SetText("M - change method\r\n<size=9>Current method = <b><u>pathfinding</u></b>");
            }
            else
            {
                methodText.SetText("M - change method\r\n<size=9>Current method = <b><u>manual translation</u></b>");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
