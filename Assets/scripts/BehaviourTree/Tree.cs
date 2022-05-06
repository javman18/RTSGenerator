using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    public abstract class Tree : SteerinBehaviours
    {
        // Start is called before the first frame update
        private BTNode root = null;
        void Start()
        {
            root = SetUpTree();
        }

        // Update is called once per frame
        void Update()
        {
            if (root != null)
            {
                root.Evaluate();
            }
        }

        protected abstract BTNode SetUpTree();
    }

