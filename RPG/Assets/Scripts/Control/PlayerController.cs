using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Movement;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Resources;
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
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
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
            RaycastHit hit;
            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(hit.point, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
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