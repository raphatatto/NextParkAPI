using Microsoft.ML.Data;

namespace NextParkAPI.ML
{
    public class ManutencaoInput
    {
        [LoadColumn(0)]
        public float Quilometragem { get; set; }

        [LoadColumn(1)]
        public float IdadeMotoMeses { get; set; }

        [LoadColumn(2)]
        public float TemperaturaMotor { get; set; }

        [LoadColumn(3)]
        public bool JaFezManutencao { get; set; }
    }
}
