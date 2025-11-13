module Script

let y = 0;
// result is, in fact, > val y: int = 0

let data = [1.; 2.; 3.; 4.;]

// result is > val data: float list = [1.0; 2.0; 3.0; 4.0]

let sqr x = x*x
// result is > val sqr: x: int -> int, "int" assumed
// calling:
// > sqr 3;;
//  val it: int = 9


let sumOfSquaresI nums = 
    let mutable acc = 0
    for x in nums do
        acc <- acc + sqr x
    acc

// calling: sumOfSquaresI [1;2;3;4];;
// result: val it: int = 30
// calling: sumOfSquaresI [1.;2.;3.;4.;];;
// result: stdin(6,16): error FS0001: This expression was expected to have type 'int but here has type 'float' 
// How to solve? 1/ new definition 2/ setting type explicite: let sqr (x: float) ... 3/ openly type conversion (ugh!) 4/ inline

let inline sqrII x = x * x
let sumOfSquaresII nums = 
    let mutable acc = 0.0
    for x in nums do
        acc <- acc + sqrII x
    acc

// And now it works ;)

let rec sumOfSquaresF nums = 
    match nums with
    | [] -> 0
    | x::xs -> sqrII x + sumOfSquaresF xs
// rec = recursive, match and "|" -> like switch case, h::t -> splits list for head (first elem.) and tail (rest)

// calling: sumOfSquaresF [1;2;3;4];;
// result: val it: int = 30
// also works when compiling sumOfSquaresII and sumOfSquaresF at the same time

let sumOfSquares nums = 
    Seq.sum(Seq.map(fun x -> x*x) nums)

// functional programming, Seq.map applies function to each element of the sequence, Seq.sum sums up all elements of the sequence
// calling: sumOfSquares [1;2;3;4];;
// result: val it: int = 30

let sumOfSquaresClearer nums = 
    nums
    |> Seq.map(fun x -> x*x)
    |> Seq.sum
// |> means get args from upper line and pass it to the function on the right
// calling: sumOfSquaresClearer [1;2;3;4];;
// result: val it: int = 30

#r "nuget: FSharp.Collections.ParallelSeq"
open FSharp.Collections.ParallelSeq
let sumOfSquaresParallel nums = 
    nums
    |> PSeq.map(fun x -> x*x)
    |> PSeq.sum
// parallel processing
// calling: sumOfSquaresParallel [1;2;3;4];;
// result: val it: int = 30


open System.Net.Http
let ticker = "msft.us"
let dataStart = System.DateTime(2000, 1, 1).ToString("yyyyMMdd")
let dataEnd = System.DateTime(2023, 2, 10).ToString("yyyyMMdd")
let url = sprintf "https://stooq.com/q/d/l/?s=%s&d1=%s&d2=%s&i=d"ticker dataStart dataEnd
let client = new HttpClient()
let getDataAsync = async {
    let! csvResponse = Async.AwaitTask (client. GetStringAsync(url))
    return csvResponse
}
let csv = Async. RunSynchronously getDataAsync
// Example: using rich options of .NET
// calling: csv;;

let prices = 
    csv.Split([|'\r'; '\n'|], System.StringSplitOptions.RemoveEmptyEntries)
    |> Seq.skip 1
    |> Seq.map(fun line -> line.Split(','))
    |> Seq.filter(fun values -> values.Length = 6)
    |> Seq.map(fun values -> (values[0], values[4]))
    |> Seq.toArray
// Example: processing CSV data
// calling: prices;;

#r "nuget: XPlot.Plotly"
open XPlot.Plotly
Chart.Line(prices).Show()


open System.Net.Http
open XPlot.Plotly
let loadPrices (ticker: string) = 
    let stooqTicker = ticker.ToLower() + ".us"
    let dataStart = System.DateTime(2000, 1, 1).ToString("yyyyMMdd")
    let dataEnd = System.DateTime(2023, 2, 10).ToString("yyyyMMdd")
    let url = sprintf "https://stooq.com/q/d/l/?s=%s&d1=%s&d2=%s&i=d"stooqTicker dataStart dataEnd
    let client = new HttpClient()
    let getDataAsync = async {
        let! csvResponse = Async.AwaitTask (client. GetStringAsync(url))
        return csvResponse
    }
    let csv = Async.RunSynchronously getDataAsync
    let prices = 
        csv.Split([|'\r'; '\n'|], System.StringSplitOptions.RemoveEmptyEntries)
            |> Seq.skip 1
            |> Seq.map(fun line -> line.Split(','))
            |> Seq.filter(fun values -> values.Length = 6)
            |> Seq.map(fun values -> (values[0], values[4]))
            |> Seq.toArray
    prices


["MSFT"; "ORCL"; "EBAY"] |> Seq.iter (fun ticker ->
    let data = loadPrices ticker
    Chart.Line(data).Show()
)
// Example: wrapping functionality into a function and using it for multiple tickers

let loadPricesAsync (ticker: string) = async {
    let ticker = ticker.ToLower() + ".us"                                                                      
    let dataStart = System.DateTime(2000, 1, 1).ToString("yyyyMMdd")                                                  
    let dataEnd = System.DateTime(2023, 2, 10).ToString("yyyyMMdd")                                                   
    let url = sprintf "https://stooq.com/q/d/l/?s=%s&d1=%s&d2=%s&i=d" ticker dataStart dataEnd                 
    let client = new HttpClient()                                                                                                                                                                                                          
    let! csv = Async.AwaitTask (client.GetStringAsync(url))                                                              
    let prices =                                                                                               
        csv.Split([|'\r'; '\n'|], System.StringSplitOptions.RemoveEmptyEntries)
        |> PSeq.ofArray
        |> PSeq.skip 1                                                                                           
        |> PSeq.map (fun line -> line.Split(','))                                                                
        |> PSeq.filter (fun values -> values.Length = 6)                                                         
        |> PSeq.map (fun values -> (values.[0], values.[4]))                                                     
        |> PSeq.toArray                                                                                          
    return prices                                                                                                     
}
// Example: asynchronous function with parallel sequence processing
// let! -> wait until ends, return -> end async block

let requests = 
    [
        loadPricesAsync "MSFT"
        loadPricesAsync "ORCL"
    ]
let parallelRequests = Async.Parallel requests
let results = Async.RunSynchronously parallelRequests
results |> Array.iter (fun data ->
    Chart.Line(data).Show()    
)
// calling: requests;; 
// calling: results;;