using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class Controller : MonoBehaviour
	{		
		public Field field;	

		void Start ()
		{
            this.field = new Field();
		}


        void Update()
        {
            field.Update(Time.deltaTime);
        }
	}
}