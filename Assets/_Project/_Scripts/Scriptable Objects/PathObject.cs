using UnityEngine;

[CreateAssetMenu(fileName = "New Path", menuName = "Path")]
public class PathObject : ScriptableObject
{
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private Vector2 _endPosition;

    public Vector2 StartPosition { get { return _startPosition; } }
    public Vector2 EndPosition { get { return _endPosition; } }
}
