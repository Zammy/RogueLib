﻿using UnityEngine;
using System.Collections;

namespace RogueLib
{
    public class LightSource : MonoBehaviour 
    {
        public int Range = 10;

        public float Flicker = 0.02f;

    	// Use this for initialization
    	void Start () 
        {
            StartCoroutine( this.LightSurroundings() );
    	}

        IEnumerator LightSurroundings()
        {
            var thisTileBhv = this.GetComponent<TileBehavior>();
            Point lightPos = thisTileBhv.Pos;
            var wait = new WaitForSeconds(0.25f);

            while (true)
            {
                thisTileBhv.LightLevel = 1f;

                for (int range = 1; range <= this.Range; range++)
                {
                    TileBehavior[] tilesInRange = LevelDrawer.Instance.TilesAroundInRange( lightPos, range );
                    foreach(var tileBhv in tilesInRange)
                    {
                        float lightLevel = ( 1f - (float)range / (float)this.Range );
                        lightLevel = Random.Range(lightLevel - this.Flicker, lightLevel + this.Flicker);
                        tileBhv.LightLevel = Mathf.Max(0f, Mathf.Min(1f, lightLevel ) );
                    }
                }

                yield return wait;
            }
        }
    }
}
