#r "nuget: XPlot.Plotly"
#r "nuget: FSharp.Collections.ParallelSeq"

open System
open System.Net.Http
open XPlot.Plotly
open FSharp.Collections.ParallelSeq

let loadPrices(ticker: string) =
    let ticker = ticker.ToLower() + ".us"
    let dataStart = DateTime(2000, 1, 1).ToString("yyyyMMdd")
    let dataEnd = DateTime(2023, 2, 10).ToString("yyyyMMdd")
    let url = sprintf "https://stooq.com/q/d/l/?s=%s&d1=%s&d2=%s&i=d" ticker dataStart dataEnd
    let client = new HttpClient()
    let getDataAsync = async {
        let! csvResponse = Async.AwaitTask(client.GetStringAsync(url))
        return csvResponse
    }
    let csv = Async.RunSynchronously getDataAsync
    let prices =
        csv.Split([|'\r'; '\n'|], StringSplitOptions.RemoveEmptyEntries)
        |> Seq.skip 1
        |> Seq.map (fun line -> line.Split(','))
        |> Seq.filter (fun values -> values.Length = 6)
        |> Seq.map (fun values -> (values.[0], Double.Parse(values.[4])))
        |> Seq.toArray
    prices

["MSFT"; "ORCL"; "EBAY"] |> Seq.iter (fun ticker ->
    let data = loadPrices ticker
    Chart.Line(data).Show()
)

let loadPricesAsync(ticker: string) = async {
    let ticker = ticker.ToLower() + ".us"                                                                      
    let dataStart = DateTime(2000, 1, 1).ToString("yyyyMMdd")                                                  
    let dataEnd = DateTime(2023, 2, 10).ToString("yyyyMMdd")                                                   
    let url = sprintf "https://stooq.com/q/d/l/?s=%s&d1=%s&d2=%s&i=d" ticker dataStart dataEnd                 
    let client = new HttpClient()                                                                                                                                                                                                          
    let! csv = Async.AwaitTask (client.GetStringAsync(url))                                                              
    let prices =                                                                                               
        csv.Split([|'\r'; '\n'|], StringSplitOptions.RemoveEmptyEntries)
        |> PSeq.ofArray
        |> PSeq.skip 1                                                                                           
        |> PSeq.map (fun line -> line.Split(','))                                                                
        |> PSeq.filter (fun values -> values.Length = 6)                                                         
        |> PSeq.map (fun values -> (values.[0], values.[4]))                                                     
        |> PSeq.toArray                                                                                          
    return prices                                                                                                     
}
let requests = [
    loadPricesAsync "MSFT"
    loadPricesAsync "ORCL"
]
let parallelRequests = Async.Parallel requests
let results = Async.RunSynchronously parallelRequests
results |> Array.iter (fun data ->
    Chart.Line(data).Show()    
)