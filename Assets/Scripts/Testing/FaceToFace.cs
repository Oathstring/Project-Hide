using UnityEngine;

namespace Oathstring
{
    public class FaceToFace : MonoBehaviour
    {
        [SerializeField] Rigidbody[] faces;

        // Awake is called when the script object is initialised
        private void Awake()
        {
            FaceToFacing();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                FaceToFacing();
            }
        }

        private void FaceToFacing()
        {
            Vector3 direction = faces[0].position - faces[1].position;
            direction.y = 0;

            Vector3 direction1 = faces[1].position - faces[0].position;
            direction1.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookTarget = Quaternion.LookRotation(-direction);
                faces[0].rotation = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);
            }

            if (direction1 != Vector3.zero)
            {
                Quaternion lookTarget = Quaternion.LookRotation(-direction1);
                faces[1].rotation = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);
            }
        }
    }
}
