using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    // Properties are methods that pretend to be a field
    // Unity doesn't serialize properties
    public float AverageFPS {get; private set; }
    public float HighestFPS {get; private set; }
    public float LowestFPS {get; private set; }
    [SerializeField] private int frameRange = 60;

    private int[] fpsBuffer;
    private int fpsBufferIndex;

    private void InitializeBuffer() {
        if (frameRange <= 0) {
            frameRange = 1;
        }
        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }

    private void Update() {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange) {
            InitializeBuffer();
        }
        UpdateBuffer();
        CalculateFPS();
        // FPS = 1f / Time.unscaledDeltaTime;
    }

    private void UpdateBuffer() {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange) {
            fpsBufferIndex = 0;
        }
    }

    private void CalculateFPS() {
        float sum = 0;
        float highest = 0;
        float lowest = float.MaxValue;
        for (int i = 0; i < frameRange; i++) {
            int fps = fpsBuffer[i];
            sum += fps;
            if (fps > highest) {
                highest = fps;
            }
            if (fps < lowest) {
                lowest = fps;
            }
        }
        AverageFPS = sum / frameRange;
        HighestFPS = highest;
        LowestFPS = lowest;
    }

}
