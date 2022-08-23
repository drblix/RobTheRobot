using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Variables

    private Robot robot;
    private MeshRenderer meshRenderer;

    private List<Transform> boxes = new();

    private Transform chosenBox;
    public Transform ChosenBox { get { return chosenBox; } }

    private readonly string[] possibleColors =
    {
        "Red",
        "Green",
        "Blue"
    };

    #endregion

    private void Awake()
    {
        robot = FindObjectOfType<Robot>();
        meshRenderer = GetComponent<MeshRenderer>();

        Setup();
    }

    private void Setup()
    {
        Transform boxesParent = GameObject.FindGameObjectWithTag("BoxesParent").transform;

        foreach (Transform child in boxesParent)
        {
            boxes.Add(child);
        }

        string chosenColor = possibleColors[Random.Range(0, possibleColors.Length)];

        chosenBox = GetBoxFromColor(chosenColor);

        if (!chosenBox)
        {
            throw new System.Exception("Fetched box is null");
        }

        meshRenderer.material.color = GetColorFromStr(chosenColor);

        StartCoroutine(robot.BeginTask(this));
    }

    public IEnumerator Completion()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }



    #region UtilityFunctions

    private Transform GetBoxFromColor(string clr)
    {
        foreach (Transform box in boxes)
        {
            if (box.name.Contains(clr))
            {
                return box;
            }
        }

        return null;
    }

    private Color GetColorFromStr(string color)
    {
        switch (color)
        {
            case "Red":
                return Color.red;
            case "Green":
                return Color.green;
            case "Blue":
                return Color.blue;
            default:
                throw new System.Exception("Invalid color introduced");
        }
    }

    #endregion
}
