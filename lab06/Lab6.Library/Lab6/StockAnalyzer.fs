//module StockAnalyzer

namespace Lab6

open Stock

type public StockAnalyzer (lprices, days) =
    let prices =
        lprices
        |> Seq.map snd
        |> Seq.take days

    static member public GetAnalyzer(ticker, days) =
        new StockAnalyzer(loadPrices ticker, days)

    static member public GetAnalyzers(tickers, days) =
        tickers
        |> Seq.map loadPrices
        |> Seq.map (fun prices -> new StockAnalyzer(prices, days))

    static member public GetAnalyzersParallel(tickers, days) =
        tickers
        |> Seq.map loadPricesAsync
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Seq.map (fun prices -> 
            // Convert (string * string) array to (string * float) array
            prices 
            |> Array.map (fun (date, priceStr) -> (date, float priceStr))
            |> fun convertedPrices -> new StockAnalyzer(convertedPrices, days)
        )

    static member public GetAnalyzersParallelAsync(tickers, days) = async {
        let operations = 
            tickers |> Seq.map loadPricesAsync
        let parallelOperation = Async.Parallel operations
        let! allPrices = parallelOperation
        let analyzers =
            allPrices
            |> Array.map (fun prices -> 
                // Convert (string * string) array to (string * float) array
                prices 
                |> Array.map (fun (date, priceStr) -> (date, float priceStr))
                |> fun convertedPrices -> new StockAnalyzer(convertedPrices, days)
            )
        return analyzers
    }

    static member public GetAnalyzersParallelTask(tickers, days) =
        StockAnalyzer.GetAnalyzersParallelAsync(tickers, days)
        |> Async.StartAsTask

    member s.Return =
        let lastPrice = prices |> Seq.item 0
        let startPrice = prices |> Seq.item (days - 1)
        lastPrice / startPrice - 1.0

    member s.StdDev =
        let logRets =
            prices
            |> Seq.pairwise
            |> Seq.map (fun (x, y) -> log y / x)
        let mean = logRets |> Seq.average
        let sqr x = x * x
        let var = logRets |> Seq.averageBy (fun r -> sqr (r - mean))
        sqrt var