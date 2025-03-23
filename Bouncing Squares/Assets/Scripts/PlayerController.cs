using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BouncingSquare selectedSquare;
    public SquareUIBuilder squareUIBuilder;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

        Object[] bouncingSquares = FindObjectsByType(typeof(BouncingSquare), FindObjectsSortMode.None);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (BouncingSquare square in bouncingSquares)
            {
                square.ApplyModifiers();
            }
        }
    }
}
