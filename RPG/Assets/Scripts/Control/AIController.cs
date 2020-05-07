using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Resources;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        #region Variables
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;


        Fighter _fighter;
        Health _health;
        GameObject _player;
        Mover _mover;


        LazyValue<Vector3> _guardPosition;
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int _currentWaypointIndex = 0;
        private ActionScheduler _actionScheduler;

        #endregion

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _player = GameObject.FindWithTag("Player");
            _guardPosition = new LazyValue<Vector3>(SetInitialPosition);
        }

        private Vector3 SetInitialPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead()) return;

            if (InAttackRange() && _fighter.CanAttack(_player))
                AttackBehaviour();
            else if (_timeSinceLastSawPlayer < suspicionTime)
                SuspicionBehaviour();
            else
                PatrolBehaviour();
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
        }


        #region Behaviours

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition.value;

            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if(_timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                _mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }

        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWayPoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);
        }
        #endregion


        private bool InAttackRange()
        {
            return Vector3.Distance(_player.transform.position, transform.position) < chaseDistance;
        }

        #region Gizmos
        //Called by Unity to draw gizmos
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.01f, 0f);
            Gizmos.DrawWireSphere(this.transform.position, chaseDistance);
        }
        #endregion
    }
}