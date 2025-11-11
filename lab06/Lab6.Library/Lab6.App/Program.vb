Imports System
Imports System.Threading.Tasks
Imports Lab6.StockAnalyzer

Module Program
    Sub Main(args As String())
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        Dim tickers = {"MSFT", "ORCL", "EBAY"}
        Const days = 30

        Console.WriteLine("Pobieranie danych i analiza asynchroniczna...")
        Dim analyzers = Await GetAnalyzersParallelTask(tickers, days)

        For i = 0 To tickers.Length - 1
            Dim a = analyzers(i)
            Dim ret = a.Return
            Dim sd = a.StdDev
            Console.WriteLine($"{tickers(i)} -> Stopa zwrotu: {ret:P2}, Odchylenie standardowe: {sd:P4}")
        Next
    End Function
End Module