using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BSP
{
    public class BSPDrawer : MonoBehaviour 
    {
        public Transform Canvas;
        public GameObject NodePrefab;

        public float DrawSpeed = 1f;

        const float LAYER_DEPTH = 5;
        //todo takes binary tree and draws it on layers with different colors

        public void DrawNode(BinaryNode node)
        {
            StartCoroutine ( this.DrawNodes(new BinaryNode[] { node }, 0) ) ;
        }

        IEnumerator DrawNodes(BinaryNode[] nodes, int depth)
        {
            var leaves = new List<BinaryNode>( nodes.Length * 2 );

            foreach (var node in nodes)
            {
                foreach(var leaf in node.Leaves)
                {
                    if (leaf != null)
                    {
                        leaves.Add(leaf);
                    }
                }

                if (leaves.Count == 0)
                {
                    Draw (node, depth);
                }

                yield return new WaitForSeconds(this.DrawSpeed);
            }

            if (leaves.Count != 0)
            {
                yield return StartCoroutine( this.DrawNodes( leaves.ToArray(), depth + 1 ) );
            }
        }

        void Draw(BinaryNode node, int depth)
        {
            var nodeObj = (GameObject) Instantiate(NodePrefab);
            nodeObj.transform.SetParent(this.Canvas);

            var color = new Color( Random.Range(0.5f, 1f), Random.Range(0.25f,1f), Random.Range(0.25f, 0.75f));
            nodeObj.GetComponent<Image>().color = color;

            var rectTrans = nodeObj.transform as RectTransform;
            rectTrans.sizeDelta = new Vector2( node.Width, node.Height );
            rectTrans.anchoredPosition = new Vector2( node.Pos.X, node.Pos.Y);

            var pos = nodeObj.transform.position;
            nodeObj.transform.position = new Vector3(pos.x, pos.y, - LAYER_DEPTH * depth);
        }
    }
}
