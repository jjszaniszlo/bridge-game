using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BridgeExtendable : MonoBehaviour
{
    [Header("Bridge Extension Settings")]
    public Transform extensionPoint;
    public GameObject visualGameObject;

    private BridgeNode _head;
    private BridgeNode _tail;
    private int _segmentCount;

    /// <summary>
    /// Returns whether or not there are segments on this bridge root.
    /// </summary>
    /// <returns>true if it has segments, false otherwise</returns>
    public bool HasSegments()
    {
        return _segmentCount > 0;
    }
    
    /// <summary>
    /// Spawns the extension bridge and progresses the tail on the linked list.
    /// </summary>
    public void Extend()
    {
        var localSpawnPosition = new Vector3(0, 0, _segmentCount * 0.3f);
        var newBridgeSegment = Instantiate(visualGameObject, extensionPoint);
        newBridgeSegment.transform.localPosition = localSpawnPosition;
        _segmentCount++;
        
        var newBridgeSegmentNode = newBridgeSegment.GetComponent<BridgeNode>();

        if (_head == null)
        {
            _head = newBridgeSegmentNode;
            _tail = newBridgeSegmentNode;
        }
        else
        {
            _tail.next = newBridgeSegmentNode;
            _tail = newBridgeSegmentNode;
        }
    }

    /// <summary>
    /// Removes the furthest away bridge segment.
    /// </summary>
    public void RemoveLast()
    {
        if (_head == null || _tail == null) return;
        
        _segmentCount--;
        
        if (_head == _tail)
        {
            Destroy(_head.gameObject);
            _head = null;
            _tail = null;
        }
        else
        {
            var current = _head;
            while (current.next != _tail)
            {
                current = current.next;
            }
            Destroy(_tail.gameObject);
            _tail = current;
        }
    }
}
