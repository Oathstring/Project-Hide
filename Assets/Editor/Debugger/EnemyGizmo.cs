using UnityEditor;
using UnityEngine;

namespace Oathstring
{
    [CustomEditor(typeof(Enemy))]
    public class EnemyGizmo : Editor
    {
        private void OnSceneGUI()
        {
            Enemy enemyFov = (Enemy)target;
            Handles.color = Color.white;
            Handles.DrawWireArc(enemyFov.transform.position, Vector3.up, Vector3.forward, 360, enemyFov.Radius);

            Vector3 viewAngle01 = DirectionFromAngle(enemyFov.transform.eulerAngles.y, -enemyFov.Angle / 2);
            Vector3 viewAngle02 = DirectionFromAngle(enemyFov.transform.eulerAngles.y, enemyFov.Angle / 2);

            Handles.color = Color.yellow;
            Handles.DrawLine(enemyFov.transform.position, enemyFov.transform.position + viewAngle01 * enemyFov.Radius);
            Handles.DrawLine(enemyFov.transform.position, enemyFov.transform.position + viewAngle02 * enemyFov.Radius);

            if (enemyFov.CanSeePlayer)
            {
                Handles.color = Color.green;
                Handles.DrawLine(enemyFov.transform.position, enemyFov.Player.transform.position);
            }

            Enemy enemySensor = (Enemy)target;
            Handles.color = Color.cyan;
            Handles.DrawWireArc(enemySensor.transform.position, Vector3.up, Vector3.forward, 360, enemySensor.SensorRadius);

            /*Vector3 viewAngleCapture01 = DirectionFromAngle(enemySensor.transform.eulerAngles.y, -enemySensor.CaptureAngle / 2);
            Vector3 viewAngleCapture02 = DirectionFromAngle(enemySensor.transform.eulerAngles.y, enemySensor.CaptureAngle / 2);

            Handles.color = Color.blue;
            Handles.DrawLine(enemySensor.transform.position, enemySensor.transform.position + viewAngleCapture01 * enemySensor.SensorRadius);
            Handles.DrawLine(enemySensor.transform.position, enemySensor.transform.position + viewAngleCapture02 * enemySensor.SensorRadius);*/

            if (enemySensor.PlayerNearby)
            {
                Handles.color = Color.green;
                Handles.DrawLine(enemySensor.transform.position, enemySensor.Player.transform.position);
            }

            Enemy enemyCapture = (Enemy)target;
            Handles.color = Color.cyan;
            Handles.DrawWireArc(enemyCapture.transform.position, Vector3.up, Vector3.forward, 360, enemyCapture.CaptureRadius);

            Vector3 viewAngleCapture01 = DirectionFromAngle(enemyCapture.transform.eulerAngles.y, -enemyCapture.CaptureAngle / 2);
            Vector3 viewAngleCapture02 = DirectionFromAngle(enemyCapture.transform.eulerAngles.y, enemyCapture.CaptureAngle / 2);

            Handles.color = Color.blue;
            Handles.DrawLine(enemyCapture.transform.position, enemyCapture.transform.position + viewAngleCapture01 * enemyCapture.CaptureRadius);
            Handles.DrawLine(enemyCapture.transform.position, enemyCapture.transform.position + viewAngleCapture02 * enemyCapture.CaptureRadius);

            if (enemyCapture.PlayerCaptured)
            {
                Handles.color = Color.green;
                Handles.DrawLine(enemyCapture.transform.position, enemyCapture.Player.transform.position);
            }
        }

        private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}
