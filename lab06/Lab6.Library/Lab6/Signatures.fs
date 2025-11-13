module Signatures

module Signatures =
    type Adder = int -> int
    type AdderGenerator = int -> Adder
    let a: AdderGenerator = fun x -> (fun y -> x + y)
    // let b:AdderGenerator = fun (x:float) -> (fun y -> x + y)
    let b:AdderGenerator = fun (x:int) -> (fun y -> x + y)
    let c = fun (x:float) -> (fun y -> x + y)