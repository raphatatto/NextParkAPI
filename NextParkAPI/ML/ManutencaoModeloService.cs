using Microsoft.ML;
using NextParkAPI.ML;

namespace NextParkAPI.ML
{
    public class ManutencaoModelService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly PredictionEngine<ManutencaoInput, ManutencaoPrediction> _engine;

        public ManutencaoModelService()
        {
            _mlContext = new MLContext();

            // Dados de treino fictícios (poderia ser CSV)
            var dados = new List<ManutencaoInput>
            {
                new() { Quilometragem = 10000, IdadeMotoMeses = 6, TemperaturaMotor = 75, JaFezManutencao = false },
                new() { Quilometragem = 40000, IdadeMotoMeses = 24, TemperaturaMotor = 95, JaFezManutencao = true },
                new() { Quilometragem = 15000, IdadeMotoMeses = 10, TemperaturaMotor = 85, JaFezManutencao = false },
                new() { Quilometragem = 30000, IdadeMotoMeses = 18, TemperaturaMotor = 100, JaFezManutencao = true },
            };

            var data = _mlContext.Data.LoadFromEnumerable(dados);

            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(ManutencaoInput.Quilometragem),
                    nameof(ManutencaoInput.IdadeMotoMeses),
                    nameof(ManutencaoInput.TemperaturaMotor))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(ManutencaoInput.JaFezManutencao)));

            _model = pipeline.Fit(data);
            _engine = _mlContext.Model.CreatePredictionEngine<ManutencaoInput, ManutencaoPrediction>(_model);
        }

        public ManutencaoPrediction Prever(ManutencaoInput input)
        {
            return _engine.Predict(input);
        }
    }
}
