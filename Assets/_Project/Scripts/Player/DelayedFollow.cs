using System.Collections.Generic;
using UnityEngine;

public class DelayedFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float delayTime = 1.5f;
    [SerializeField] private float recordInterval = 0.02f;

    private readonly List<TransformRecord> records = new List<TransformRecord>();
    private float recordTimer;

    private struct TransformRecord
    {
        public float time;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformRecord(float time, Transform source)
        {
            this.time = time;
            position = source.position;
            rotation = source.rotation;
            scale = source.localScale;
        }
    }

    private void Start()
    {
        if (target == null)
        {
            return;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.localScale = target.localScale;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        RecordTarget();
        FollowDelayedTarget();
        RemoveOldRecords();
    }

    private void RecordTarget()
    {
        recordTimer += Time.deltaTime;

        if (recordTimer < recordInterval)
        {
            return;
        }

        recordTimer = 0f;
        records.Add(new TransformRecord(Time.time, target));
    }

    private void FollowDelayedTarget()
    {
        float targetTime = Time.time - delayTime;

        if (records.Count == 0 || records[0].time > targetTime)
        {
            return;
        }

        TransformRecord record = records[0];

        for (int i = 1; i < records.Count; i++)
        {
            if (records[i].time > targetTime)
            {
                record = records[i - 1];
                break;
            }

            record = records[i];
        }

        transform.position = record.position;
        transform.rotation = record.rotation;
        transform.localScale = record.scale;
    }

    private void RemoveOldRecords()
    {
        float oldestTime = Time.time - delayTime - 1f;

        while (records.Count > 0 && records[0].time < oldestTime)
        {
            records.RemoveAt(0);
        }
    }
}
