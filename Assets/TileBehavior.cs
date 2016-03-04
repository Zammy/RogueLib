using UnityEngine;
using System.Collections;

namespace RogueLib
{
    public class TileBehavior : MonoBehaviour 
    {
        public Tile Tile
        {
            get;
            set;
        }

        public Point Pos
        {
            get;
            set;
        }

        public float  LightLevel = 1f;

        void Start()
        {
            StartCoroutine( this.DoLight() );
        }

        IEnumerator DoLight()
        {
            SpriteRenderer sprite = null;
            Color color = new Color(255, 55, 0);
            if (this.Tile.Type == TileType.Wall)
            {
                sprite = transform.FindChild("Background").GetComponent<SpriteRenderer>();
            }
            else if (this.Tile.Type == TileType.Ground)
            {
                sprite = transform.FindChild("Static").GetComponent<SpriteRenderer>();
                color = Color.white;
            }

            float h;
            float s;
            float v;
            ColorManipulator.ColorToHSV(color, out h, out s, out v);

            var wait = new WaitForSeconds(0.25f);
            while (true)
            {
                v = Mathf.Lerp(0, 1, this.LightLevel);
                
                sprite.color = ColorManipulator.ColorFromHSV(h, s, v);

                yield return wait;
            }
        }
    }

}