using System.Collections.Generic;
using UnityEngine;

namespace ColorControllers
{
    public class ColorBoxes : MonoBehaviour
    {
        private List<GameObject> _boxes;

        private void OnEnable()
        {
            ColorPalette.OnColorPaletteSet += OnColorPaletteSet;
        }
        private void OnColorPaletteSet(List<Color> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                _boxes[i].GetComponent<Renderer>().material.color = obj[i];
            }
        }
        private void OnDisable()
        {
            ColorPalette.OnColorPaletteSet -= OnColorPaletteSet;
        }
        void Start()
        {
            _boxes = new List<GameObject>();
            foreach (Transform child in transform)
            {
                _boxes.Add(child.gameObject);
            }
        }
 
    }
}
