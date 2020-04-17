using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(FPSCounter))]
public class FPSDisplay : MonoBehaviour
{
    [System.Serializable]
    private struct FPSColor {
        public Color color;
        public int minimumFPS;
    }
    [SerializeField]
    private FPSColor[] coloring;
    public TextMeshProUGUI highestFPSLabel;
    public TextMeshProUGUI averageFPSLabel;
    public TextMeshProUGUI lowestFPSLabel;

    private FPSCounter fPSCounter;
    private void Awake() {
        fPSCounter = GetComponent<FPSCounter>();
    }
    private void Update() {
        Display(highestFPSLabel, fPSCounter.HighestFPS);
        this.Display(averageFPSLabel, fPSCounter.AverageFPS);
        this.Display(lowestFPSLabel, fPSCounter.LowestFPS);
    }

    private void Display(TextMeshProUGUI label, float fps) {
        label.text = fps.ToString("f2");
        for (int i = 1; i < coloring.Length; i++) {
            if (fps >= coloring[i].minimumFPS) {
                label.color = coloring[i].color;
                break;
            }
        }

    }
}
