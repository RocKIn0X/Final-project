using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GaugeArea : MonoBehaviour
{
    public UI_GaugeObject hungerGauge;
    public UI_GaugeObject tirenessGauge;
    public UI_GaugeObject emotionGauge;
    
    public void SetGauge (float hunger, float tireness, float emotion)
    {
        hungerGauge.SetGaugePercent(hunger);
        tirenessGauge.SetGaugePercent(tireness);
        emotionGauge.SetGaugePercent(emotion);
    }
}
