using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float wayPointGizmosRadius = 0.3f;
        [SerializeField] Color color = Color.magenta;
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            for(int i=0; i< transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWayPoint(i), wayPointGizmosRadius);
                Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
            }

        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount) return 0;
            return i + 1;
        }

        public Vector3 GetWayPoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
