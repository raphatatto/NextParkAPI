using Microsoft.ML.Data;

namespace NextParkAPI.ML
{
    public class ManutencaoPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool NecessitaManutencao { get; set; }

        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
