using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class Controller : MonoBehaviour
	{
        public Transform StartBlock;        
		void Start ()
		{
            this.camera.orthographicSize = Field.FIELD_Y * Field.RENDER_BLOCK_SIZE / 2;
            this.camera.aspect = 1.0f;

            Vector3 c = new Vector3(-this.camera.orthographicSize * this.camera.aspect + Field.RENDER_BLOCK_SIZE / 2,
                this.camera.orthographicSize - Field.RENDER_BLOCK_SIZE / 2, 0);            
            StartBlock.position = c;
            StartBlock.gameObject.SetActive(false);
            this.camera.orthographicSize *= 1.2f;
		}


        void Update()
        {
            
        }
	}
}