using UnityEngine;

public class MenuCursorMover : MonoBehaviour
{
    private RectTransform rect;
    public float offset = 5f;
    public float currentOffset = 0f;
    public float speed = 8f;

    private bool movingToOffset;

    private Vector2 oldPosition;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        oldPosition = rect.position;
        movingToOffset = true;
    }

    private void LateUpdate()
    {
        if (movingToOffset)
        {
            //currentOffset = Mathf.Lerp(currentOffset, offset, Time.deltaTime * speed);
            currentOffset = Mathf.MoveTowards(currentOffset, offset, Time.deltaTime * speed);
        }
        else
        {
            //currentOffset = Mathf.Lerp(currentOffset, 0, Time.deltaTime * speed);
            currentOffset = Mathf.MoveTowards(currentOffset, 0, Time.deltaTime * speed);
        }


        rect.position = new Vector2(oldPosition.x + currentOffset, oldPosition.y);


        if (currentOffset > offset - 0.1f || currentOffset < 0.1f)
        {
            movingToOffset = !movingToOffset;
        }
    }

    public void SetCursorToNewPosition(RectTransform target)
    {
        oldPosition = target.position;
    }

    public void SetCursorToNewPosition(Vector3 position)
    {
        oldPosition = position;
    }
}

