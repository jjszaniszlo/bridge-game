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

    public void RemoveLast()
    {
        if (_head == null || _tail == null) return;
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
