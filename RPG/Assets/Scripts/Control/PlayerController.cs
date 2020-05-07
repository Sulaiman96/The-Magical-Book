using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Movement;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Resources;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        private Mover _mover;

        [System.Serializable]
        struct CursorStyleMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorStyleMapping[] cursorStyleMappings = null;
        [SerializeField] private float maxNavMeshProjectionDistance = 3f;
        [SerializeField] private float maxNavPathLength = 20f;

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (var hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (var raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false; //There was no raycastable component
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }
        
        //This method will get all of the objects that were hit with the raycast and we will loop through each object in order to find the one we are looking for.
        //This is so that even if a tree is blocking our screen and we want to attack an enemy, the enemy should have high priority. 

        private void SetCursor(CursorType type)
        {
            CursorStyleMapping styleMapping = GetCursorStyleMapping(type);
            Cursor.SetCursor(styleMapping.texture, styleMapping.hotspot, CursorMode.Auto);
        }

        private CursorStyleMapping GetCursorStyleMapping(CursorType type)
        {
            foreach (CursorStyleMapping styleMapping in cursorStyleMappings)
            {
                if (styleMapping.type == type)
                {
                    return styleMapping;
                }
            }

            return cursorStyleMappings[0];
        }

        #region Movement
        private bool InteractWithMovement()
        {
            //RaycastHit hit;
            //bool hasHit = Physics.Raycast(GetMouseRay(), out hit)
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, 
                maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!hasCastToNavMesh) return false; //have not found a navmesh near the point which the cursor clicked on.

            target = navMeshHit.position;

            NavMeshPath path = new NavMeshPath();
            bool hasPath =  NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) return false;

            if (path.status != NavMeshPathStatus.PathComplete) return false; //if there is no complete path to the selected location, then return false.
            
            //Check if the length is too long, this allows me to give the control to the user to manually guide the player to the location.
            if (GetPathLength(path) > maxNavPathLength) return false;
            
            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float runningTotal = 0;
            //edge case check
            if (path.corners.Length < 2) return runningTotal;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                runningTotal += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            
            return runningTotal;
        }

        #endregion

        #region Mouse Ray
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        #endregion
    }

}