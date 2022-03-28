using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Data.Catalog;

namespace Gradients
{
    [RequireComponent(typeof(SpriteRenderer))]

    public class GradientItemController : MonoBehaviour
    {
        static readonly int GradientProperty = Shader.PropertyToID("_Gradient");

        [SerializeField] GradientList gradientList;

        private SpriteRenderer spriteRenderer;
        private int gradientID;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.material = new Material(Shader.Find("Friendbase/GradientSprite"));
        }

        public void SetGradientColor(ColorCatalogItem colorCatalogItem)
        {
            gradientID = colorCatalogItem.IdItem;
            Texture2D texture2D = gradientList.GetTextureByName(colorCatalogItem.NamePrefab);
            if (texture2D!=null)
            {
                spriteRenderer.material.SetTexture(GradientProperty, texture2D);
            }
        }
    }
}