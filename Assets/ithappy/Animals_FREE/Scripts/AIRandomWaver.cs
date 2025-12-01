using UnityEngine;

namespace ithappy.Animals_FREE
{
    [RequireComponent(typeof(CreatureMover))]
    public class AIRandomWaver : MonoBehaviour
    {
        [SerializeField] private Transform m_Area;
        [SerializeField] private float m_StopDistance = 1f;
        [SerializeField] private float m_WaitTimeMin = 1f;
        [SerializeField] private float m_WaitTimeMax = 4f;

        private CreatureMover m_Mover;
        private Transform[] m_Waypoints;
        private Transform m_CurrentTarget;
        private bool m_Waiting;
        private float m_WaitTimer;

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();

            if (m_Area != null)
            {
                m_Waypoints = new Transform[m_Area.childCount];
                for (int i = 0; i < m_Area.childCount; i++)
                    m_Waypoints[i] = m_Area.GetChild(i);

                PickRandomTarget();
            }
        }

        private void Update()
        {
            if (m_Area == null || m_Waypoints == null || m_Waypoints.Length == 0)
            {
                // kalau belum di-set, diam saja
                SendStopInput();
                return;
            }

            if (m_Waiting)
            {
                m_WaitTimer -= Time.deltaTime;
                if (m_WaitTimer <= 0)
                {
                    m_Waiting = false;
                    PickRandomTarget();
                }
                else
                {
                    SendStopInput();
                }

                return;
            }

            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            Vector3 toTarget = m_CurrentTarget.position - transform.position;
            toTarget.y = 0f;
            float dist = toTarget.magnitude;

            if (dist <= m_StopDistance)
            {
                m_Waiting = true;
                m_WaitTimer = Random.Range(m_WaitTimeMin, m_WaitTimeMax);
                SendStopInput();
                return;
            }

            Vector3 dir = toTarget.normalized;
            Vector3 look = transform.position + dir;

            m_Mover.SetInput(
                new Vector2(0f, 1f),   // maju
                look,                  // arah pandang
                false,                 // tidak lari
                false                  // tidak lompat
            );
        }

        private void SendStopInput()
        {
            m_Mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
        }

        private void PickRandomTarget()
        {
            m_CurrentTarget = m_Waypoints[Random.Range(0, m_Waypoints.Length)];
        }
    }
}
