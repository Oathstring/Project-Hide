using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

namespace Oathstring
{
    public class Enemy : MonoBehaviour
    {
        private NavMeshAgent m_agent;
        private Animator m_animator;
        private Transform m_player;
        private Rigidbody m_rigidbody;

        private bool m_inCutscene = true;
        private readonly float m_sensorAngle = 360;

        private bool m_canSeePlayer;
        private bool m_playerNearby;
        private bool m_playerCaptured;
        private bool m_idle = false;
        private bool m_canPatrol;
        private float m_idleCD = 0;

        [Header("FOV Settings")]
        [SerializeField] float radius;
        [SerializeField][Range(0, 360)] float angle;
        [SerializeField] LayerMask targetMask;
        [SerializeField] LayerMask obstructionMask;

        [Header("Sensor Settings")]
        [SerializeField] float sensorRadius;

        [Header("Capture Settings")]
        [SerializeField] float captureRadius;
        [SerializeField][Range(0, 360)] float captureAngle;
        [SerializeField] PlayableDirector captureCutscene;

        [Header("Patrol Settings")]
        [SerializeField] float patrolRange = 10f;
        [SerializeField] float idleCDMax = 5;
        [SerializeField] float idleCDMin = 3;

        public Transform Player
        {
            get { return m_player; } 
        }

        public float Radius
        {
            get { return radius; }
        }

        public float Angle
        {
            get { return angle; } 
        }

        public bool CanSeePlayer
        {
            get { return m_canSeePlayer; } 
        }

        public float SensorRadius
        {
            get { return sensorRadius; }
        }

        public bool PlayerNearby
        {
            get { return m_playerNearby; }
        }

        public float CaptureRadius
        {
            get { return captureRadius; }
        }

        public float CaptureAngle
        {
            get { return captureAngle; }
        }

        public bool PlayerCaptured
        {
            get { return m_playerCaptured; }
        }

        public bool RandomPoint (Vector3 center, float range, out Vector3 result)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            if(NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRange, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }

        // Awake is called when the script object is initialised
        private void Awake()
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_animator = GetComponentInChildren<Animator>();
            m_rigidbody = GetComponent<Rigidbody>();

            StartCoroutine(VisionRoutine());
        }

        // Update is called once per frame
        private void Update()
        {
            if (!m_inCutscene)
            {
                if (m_canSeePlayer)
                {
                    m_agent.SetDestination(m_player.position);
                    m_animator.SetBool("Run", true);
                    m_idle = false;
                }

                else if (m_agent.remainingDistance == 0)
                {
                    m_animator.SetBool("Run", false);

                    if (m_canPatrol)
                    {
                        if (!m_idle)
                        {
                            m_idleCD = Random.Range(idleCDMin, idleCDMax);
                            m_idle = true;
                        }

                        else if (m_idleCD >= 0 && m_idle)
                        {
                            m_idleCD -= Time.deltaTime;
                        }

                        else if (m_idleCD < 0 && m_idle)
                        {
                            m_idleCD = 0;

                            if (RandomPoint(transform.position, patrolRange, out Vector3 point))
                            {
                                Debug.DrawRay(point, Vector3.up, Color.blue, 1f);
                                m_agent.SetDestination(point);
                                m_animator.SetBool("Run", true);
                                m_idle = false;
                            }
                        }
                    }
                }
            }

            m_canSeePlayer = m_playerNearby == true;

            if(m_playerCaptured) captureCutscene.Play(); // play capture scene

            //if (Input.GetKeyDown(KeyCode.J)) OnPlayerCaptured();
        }

        public void Enable()
        {
            m_inCutscene = false;
            m_agent.enabled = true;
        }

        public void Disable()
        {
            m_inCutscene = true;
            m_playerCaptured = false;

            if (m_agent.enabled)
            {
                m_agent.ResetPath();
                m_agent.enabled = false;
            }
        }

        public void OnPlayerCaptured()
        {
            if (Player && !m_agent.enabled)
            {
                Rigidbody playerRb = Player.GetComponentInParent<Rigidbody>();
                PlayerController playerController = Player.GetComponentInParent<PlayerController>();
                playerController.SetCapture(true);

                Vector3 direction = playerRb.position - m_rigidbody.position;
                direction.y = 0;

                Vector3 directionPlayer = m_rigidbody.position - playerRb.position;
                directionPlayer.y = 0;

                if(direction != Vector3.zero)
                {
                    Quaternion lookTarget = Quaternion.LookRotation(direction);
                    m_rigidbody.rotation = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);
                }

                if (directionPlayer != Vector3.zero)
                {
                    Quaternion lookTarget = Quaternion.LookRotation(directionPlayer);
                    playerRb.rotation = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);
                }
            }
        }

        public void OnEnemyAttack()
        {
            if (Player && !m_agent.enabled)
            {
                Rigidbody playerRb = Player.GetComponentInParent<Rigidbody>();
                //Collider playerCollider = Player.GetComponent<Collider>();

                //playerCollider.material = null;

                int torqueMultiplier = 15;
                playerRb.constraints = RigidbodyConstraints.None;
                playerRb.AddTorque(-playerRb.transform.forward * torqueMultiplier, ForceMode.Impulse); 
            }
        }

        private IEnumerator VisionRoutine()
        {
            WaitForSeconds wait = new (0.2f);

            while (true)
            {
                yield return wait;
                FieldOfViewCheck();
                SensorCheck();
                CaptureCheck();
            }
        }

        private void FieldOfViewCheck()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                m_player = target;
                PlayerController playerController = target.GetComponentInParent<PlayerController>();
                Vector3 directionToTarget = (m_player.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, m_player.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask) && !playerController.Hide)
                    {
                        m_canSeePlayer = true;
                        m_canPatrol = true;
                    }
                    else
                        m_canSeePlayer = false;
                }
                else
                    m_canSeePlayer = false;
            }
            else if (m_canSeePlayer)
                m_canSeePlayer = false;
        }

        private void SensorCheck()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, sensorRadius, targetMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                m_player = target;
                PlayerController playerController = target.GetComponentInParent<PlayerController>();
                Vector3 directionToTarget = (m_player.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < m_sensorAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, m_player.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask) && !playerController.Hide)
                        m_playerNearby = true;
                    else
                        m_playerNearby = false;
                }
                else
                    m_playerNearby = false;
            }
            else if (m_playerNearby)
                m_playerNearby = false;
        }

        private void CaptureCheck()
        {
            if (!m_inCutscene)
            {
                Collider[] rangeChecks = Physics.OverlapSphere(transform.position, captureRadius, targetMask);

                if (rangeChecks.Length != 0)
                {
                    Transform target = rangeChecks[0].transform;
                    m_player = target;
                    PlayerController playerController = target.GetComponentInParent<PlayerController>();
                    Vector3 directionToTarget = (m_player.position - transform.position).normalized;

                    if (Vector3.Angle(transform.forward, directionToTarget) < captureAngle / 2)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, m_player.position);

                        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask) && !playerController.Hide)
                            m_playerCaptured = true;
                    }
                }
            }
        }
    }
}
