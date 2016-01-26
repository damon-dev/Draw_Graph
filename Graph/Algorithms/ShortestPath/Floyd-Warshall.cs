using System.Globalization;

namespace Graph.Algorithms.ShortestPath
{
    class FloydWarshall : Algorithm
    {
        private double[,] distances;

        public override void Initialize()
        {
            base.Initialize();
            distances = new double[NumberOfVerticies, NumberOfVerticies];
            for (int i = 0; i < NumberOfVerticies; ++i)
                for (int j = 0; j < NumberOfVerticies; ++j)
                {
                    distances[i, j] = (i == j)
                        ? 0
                        : MainWindow.VertexHandler.GetWeightBetween(MainWindow.VertexHandler.Verticies[i], MainWindow.VertexHandler.Verticies[j]);
                    if (!double.IsPositiveInfinity(distances[i, j]))
                        MainWindow.PathWindow.SetElement(MainWindow.VertexHandler.Verticies[i],
                            MainWindow.VertexHandler.Verticies[j],
                            distances[i, j].ToString(CultureInfo.InvariantCulture));
                }
        }

        public override void Execute()
        {
            Initialize();
            for (int k = 0; k < NumberOfVerticies; k++)
                for (int i = 0; i < NumberOfVerticies; i++)
                    for (int j = 0; j < NumberOfVerticies; j++)
                        if (distances[i, k] + distances[k, j] < distances[i, j])
                        {
                            distances[i, j] = distances[i, k] + distances[k, j];
                            MainWindow.PathWindow.SetElement(MainWindow.VertexHandler.Verticies[i],
                                MainWindow.VertexHandler.Verticies[j],
                                distances[i, j].ToString(CultureInfo.InvariantCulture));
                        }
            base.Execute();
        }
    }
}
