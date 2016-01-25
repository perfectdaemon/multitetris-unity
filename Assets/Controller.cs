using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class Controller : MonoBehaviour
    {
        public Transform StartBlock;
        void Start()
        {
            this.GetComponent<Camera>().orthographicSize = Field.FIELD_Y * Field.RENDER_BLOCK_SIZE / 2;
            this.GetComponent<Camera>().aspect = 1.0f;

            Vector3 c = new Vector3(-this.GetComponent<Camera>().orthographicSize * this.GetComponent<Camera>().aspect + Field.RENDER_BLOCK_SIZE / 2,
                this.GetComponent<Camera>().orthographicSize - Field.RENDER_BLOCK_SIZE / 2, 0);
            StartBlock.position = c;
            StartBlock.gameObject.SetActive(false);
            this.GetComponent<Camera>().orthographicSize *= 1.2f;
        }


        void Update()
        {

        }
    }
}