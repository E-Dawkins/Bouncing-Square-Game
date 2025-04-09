using UnityEngine;
using System.Collections.Generic;

public class SimulationController : MonoBehaviour
{
    public SquareUIBuilder squareUIBuilder;
    public GameObject squarePrefab;

    private BouncingSquare selectedSquare;
    private List<BouncingSquare> squaresInLevel = new List<BouncingSquare>();
    private bool isStarted = false;
    private bool isPaused = false;

    private void Awake()
    {
        Object[] foundSquares = FindObjectsByType(typeof(BouncingSquare), FindObjectsSortMode.None);
        foreach (Object foundSquare in foundSquares)
        {
            BouncingSquare square = foundSquare as BouncingSquare;
            if (square)
            {
                squaresInLevel.Add(square);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleSquareSelect();
        }

        if (isStarted)
        {
            CheckForWinner();
        }
    }

    private void HandleSquareSelect()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        BouncingSquare square = hit.transform?.GetComponent<BouncingSquare>();
        if (square)
        {
            selectedSquare?.SetSelected(false);
            square?.SetSelected(true);

            squareUIBuilder?.ClearUI();
            square.CreateUI(squareUIBuilder);

            selectedSquare = square;
        }
    }

    private void CheckForWinner()
    {
        int aliveSquares = 0;
        foreach (BouncingSquare square in squaresInLevel)
        {
            if (square.IsAlive())
                aliveSquares++;
        }

        if (!(aliveSquares > 1))
        {
            Debug.Log(aliveSquares == 1 ? "Someone won :)" : "Draw :(");
            HandleSimStop();
        }
    }

    public void HandleSimStart()
    {
        if (isPaused || isStarted) return;

        foreach (BouncingSquare square in squaresInLevel)
        {
            square.ApplyModifiers();
        }

        selectedSquare?.SetSelected(false);
        squareUIBuilder?.ClearUI();

        selectedSquare = null;

        isStarted = true;
    }

    public void HandleSimPause()
    {
        Time.timeScale = (isPaused ? 1 : 0);
        isPaused = !isPaused;
    }

    public void HandleSimStop()
    {
        if (!isStarted) return;

        // 1. copy old square properties to new squares
        // 2. delete old squares
        for (int i = squaresInLevel.Count - 1; i >= 0; i--)
        {
            BouncingSquare oldSquare = squaresInLevel[i];
            BouncingSquare newSquare = Instantiate(squarePrefab)?.GetComponent<BouncingSquare>();

            newSquare.name = oldSquare.name;
            newSquare.CopyPropsFrom(oldSquare);

            squaresInLevel.RemoveAt(i);
            squaresInLevel.Add(newSquare);

            Destroy(oldSquare.gameObject);
        }

        isStarted = false;
        isPaused = false;
        Time.timeScale = 1;
    }
}
