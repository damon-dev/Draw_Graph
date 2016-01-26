namespace Graph.Algorithms.Actions
{
    class Complete : Algorithm
    {
        public override void Execute()
        {
            int n = MainWindow.VertexHandler.Verticies.Count;
            if (n <= 15)
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == j) continue;
                        MainWindow.EdgeHandler.CreateEdge(MainWindow.VertexHandler.Verticies[i], MainWindow.VertexHandler.Verticies[j]);
                    }
                }
            else
            {
                MainWindow.MessageTextBlock.Text = "Graph shall have maximum 15 verticies!";
            }
            base.Execute();
        }
    }
}
