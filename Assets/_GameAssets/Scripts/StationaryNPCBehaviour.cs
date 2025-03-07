using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class StationaryNPCBehaviour : MonoBehaviour
{
    [SerializeField] Transform TalkingPoint;

    public Vector3 GetTalkingPoint()
    {
        return TalkingPoint.position;
    }
    
}