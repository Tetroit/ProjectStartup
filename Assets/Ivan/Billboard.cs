using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        Transform target;
        private void OnEnable()
        {
            if (target == null)
                target = Camera.main.transform;
        }
        void Update()
        {
            if (target != null)
                transform.LookAt(transform.position + target.forward);
        }
    }
}
