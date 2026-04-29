using UnityEngine;
using GG.Infrastructure.Utils.Swipe;
using DG.Tweening;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private SwipeListener swipeListener;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private LayerMask wallsAndRoadsLayer;
    [SerializeField] private float stepDuration = 0.1f;
    private const float MAX_RAY_DISTANCE = 10f;
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
        if (canMove)
        {
            canMove = false;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, moveDirection, MAX_RAY_DISTANCE, wallsAndRoadsLayer.value);
            Vector3 targetPosition = transform.position;
            int steps = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.isTrigger) // Road Tile
                {

                }
                else // Wall Tile
                {
                    if (i == 0) // wall is near to Ball
                    {
                        canMove = true;
                        return;
                    }
                    steps = i;
                    targetPosition = hits[i-1].transform.position;
                    break;
                }
            }

            // move the Ball to targetPosition
            float moveDuration = stepDuration * steps;
            transform
            .DOMove(targetPosition,moveDuration)
            .SetEase(Ease.OutExpo)
            .OnComplete(()=> canMove=true);

        }
    }
}
