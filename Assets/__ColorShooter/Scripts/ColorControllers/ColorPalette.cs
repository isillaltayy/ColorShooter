using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorControllers
{
    public class ColorPalette : MonoBehaviour
    {
        public List<Color> boxColors = new List<Color>();
        public Color _firstColor;

        [SerializeField] private int boxNumbers;
        [SerializeField] private float colorChangeScale;

        public static Action<List<Color>> OnColorPaletteSet;
        private void Start()
        {
            _firstColor = GetRandomColour32();
            SetColors();
        }
        private Color GetRandomColour32()
        {
            return new Color32(
                (byte)UnityEngine.Random.Range(10, 100), //Red
                (byte)UnityEngine.Random.Range(10, 100), //Green
                (byte)UnityEngine.Random.Range(10, 100), //Blue
                255 //Alpha (transparency)
            );
        }
        private void SetColors()
        {
            boxColors.Add(_firstColor);
            for (int i = 0; i < boxNumbers; i++)
            {
                boxColors.Add(new Color(
                    (_firstColor.r + colorChangeScale),
                    (_firstColor.g + colorChangeScale),
                    (_firstColor.b + colorChangeScale), 
                    1));
                
                colorChangeScale+= colorChangeScale;
            }
            OnColorPaletteSet?.Invoke(boxColors);
        }
    }
}
