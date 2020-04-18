using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCoroutine : MonoBehaviour
{
    private IEnumerator _enumerator;

    private readonly static string UNITY_COROUTINE = "Unity";
    private readonly static string MANUAL_COROUTINE = "Manual";
    void Start()
    {
    }

    private void FixedUpdate() {
        if (_enumerator != null) {
            if (_enumerator.Current is myWaitForSeconds) {
                var Current = _enumerator.Current as myWaitForSeconds;
                if (Time.time > Current.waitTime) {
                    if (!_enumerator.MoveNext()) {
                        _enumerator = null;
                    }
                }
            }
        }
    }

    IEnumerator CountTime(string mode)
    {
        Debug.Log($"{mode} Coroutine: start");
        for (int i = 1; i <= 5; i++) {
            if (mode == UNITY_COROUTINE) {
                yield return new WaitForSeconds(1f);
            } else if (mode == MANUAL_COROUTINE) {
                yield return new myWaitForSeconds(1f);
            }
            Debug.Log($"{i} s");
        }
    }
    private void OnGUI() {
        if (GUI.Button(new Rect(100, 100, 200, 100), "Unity Coroutine")) 
        {
            StartCoroutine( CountTime(UNITY_COROUTINE) );
        };
        if (GUI.Button(new Rect(100, 300, 200, 100), "Manual Coroutine")) 
        {
            _enumerator = CountTime( MANUAL_COROUTINE );
            _enumerator.MoveNext();
        };

    }
}

public class myWaitForSeconds
{
    public float waitTime;
    public myWaitForSeconds(float t)
    {
        waitTime = Time.time + t;
    }
}