using UnityEngine;

[CreateAssetMenu(fileName = "New Path", menuName = "Path")]
public class PathObject : ScriptableObject
{
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private Vector2[] _destinations;

    public Vector2 StartPosition { get { return _startPosition; } }
    public Vector2 Destination(int i) 
    {
        if (i >= _destinations.Length) // returns only if called more than it should
            return Vector2.zero;
        return _destinations[i];
    }
}
