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
            StartCoroutine ( this.DrawNodes(new BinaryNode[] { node }, this.Canvas ) );
        }

        IEnumerator DrawNodes(BinaryNode[] nodes, Transform parent)
        {
            foreach (var node in nodes)
            {
                var leaves = new List<BinaryNode>();

                foreach(var leaf in node.Leaves)
                {
                    if (leaf != null)
                    {
                        leaves.Add(leaf);
                    }
                }

//                if (leaves.Count == 0)
//                {
                var nodeTrans = Draw (node, parent, leaves.Count == 0);
//                }

                yield return new WaitForSeconds(this.DrawSpeed);

                if (leaves.Count != 0)
                {
                    yield return StartCoroutine( this.DrawNodes( leaves.ToArray(), nodeTrans ) );
                }
            }
        }

        Transform Draw(BinaryNode node, Transform parent, bool drawRoom = false)
        {
            var nodeObj = (GameObject) Instantiate(NodePrefab);
            nodeObj.transform.SetParent(parent);

            if (drawRoom)
            {
                var roomObj = (GameObject) Instantiate(NodePrefab);
                roomObj.transform.SetParent(nodeObj.transform);
                roomObj.GetComponent<Image>().color = Color.black;
                roomObj.name = "Room";
                var roomRectTrans = roomObj.transform as RectTransform;
                roomRectTrans.sizeDelta = new Vector2( node.Room.Width, node.Room.Height );
                roomRectTrans.anchoredPosition = new Vector2( node.Room.Pos.X, node.Room.Pos.Y);
            }

            var color = new Color( Random.Range(0.5f, 1f), Random.Range(0.25f,1f), Random.Range(0.25f, 0.75f));
            nodeObj.GetComponent<Image>().color = color;

            var rectTrans = nodeObj.transform as RectTransform;
            rectTrans.sizeDelta = new Vector2( node.Width, node.Height );
            rectTrans.anchoredPosition = new Vector2( node.Pos.X, node.Pos.Y);

            return nodeObj.transform;
        }
    }
}
