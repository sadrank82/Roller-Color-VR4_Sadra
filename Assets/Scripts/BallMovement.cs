using UnityEngine;
using GG.Infrastructure.Utils.Swipe;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private SwipeListener swipeListener;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private LayerMask wallsAndRoadsLayer;
    [SerializeField] private float stepDuration = 0.1f;
    private const float MAX_RAY_DISTANCE = 10f;

    public UnityAction<List<RoadTile>,float> onMoveStart;
    private bool canMove = true;


    private Vector3 moveDirection;

    private void Start()
    {
        transform.position = levelManager.defaultBallRoadTile.position;

        swipeListener.OnSwipe.AddListener(swipe =>
        {
            switch (swipe)
            {
                case "Right":
                    moveDirection = Vector3.right;
                    break;
                case "Left":
                    moveDirection = Vector3.left;
                    break;
                case "Up":
                    moveDirection = Vector3.forward;
                    break;
                case "Down":
                    moveDirection = Vector3.back;
                    break;
            }
            MoveBall();
        });
    }
      private void MoveBall()
{
    if (!canMove)
        return;

    canMove = false;

    RaycastHit[] hits = Physics.RaycastAll(
        transform.position,
        moveDirection,
        MAX_RAY_DISTANCE,
        wallsAndRoadsLayer
    );

   // Sort by distance
    System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

    Vector3 targetPosition = transform.position;
    int steps = 0;

    List<RoadTile> pathRoadTiles = new List<RoadTile>();

    for (int i = 0; i < hits.Length; i++)
    {
        if (hits[i].collider.isTrigger)
        {
            RoadTile roadTile = hits[i].transform.GetComponent<RoadTile>();

            if (roadTile != null)
            {
                pathRoadTiles.Add(roadTile);
                targetPosition = roadTile.transform.position;
                steps++;
            }
        }
        else
        {
            break;
        }
    }

  // If there was no way
    if (steps == 0)
    {
        canMove = true;
        return;
    }

    float moveDuration = stepDuration * steps;

    transform.DOMove(targetPosition, moveDuration)
        .SetEase(Ease.OutExpo)
        .OnComplete(() => canMove = true);

    onMoveStart?.Invoke(pathRoadTiles, moveDuration);
}
}
