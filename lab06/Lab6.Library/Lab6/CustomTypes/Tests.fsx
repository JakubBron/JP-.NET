module Tests

open Lab6.Library.CustomTypes

let validString50 = String50.create "F# Programming"
let invalidString50 = String50.create (String.replicate 51 "x")

do match invalidString50 with
    | Some sbyte -> printfn "valid"
    | None -> printfn "invalid"

do validString50
    |> Option.map String50.value
    |> Option.map (fun s -> s.ToUpper())
    |> Option.iter (printfn "%s")

do validString50
    |> Option.map String50.value
    |> Option.map (fun s -> s.ToUpper())
    |> Option.map (printfn "%s")
    |> ignore

do match validString50 with
    | Some s -> printfn "%s" ((String50.value s).ToUpper())
    | None -> ()

do 
    let result =
        match validString50 with
        | Some s -> (String50.value s).ToUpper()
        | None -> ""
    printfn "%s" result


let s50 =  WrappedString.string50 "testing" |> Option.get
let bad = WrappedString.string50 null
let s100 = WrappedString.string100 "testing" |> Option.get

do
    printfn "s50 is %A" s50
    printfn "bad is %A" bad
    printfn "s100 is %A" s100
    printfn "s50 is equal to s100 using module equals? %b" (WrappedString.equals s50 s100)
    printfn "s50 is equal to s100 using Object.Equals? %b" (s50.Equals s100)

let address1 = EmailAddress.create "john.doe@company.net"
let address2 = EmailAddress.create "invalid-email"

do
    printfn "address1: %A" address1
    printfn "address2: %A" address2

let success (EmailAddress.EmailAddress s) = printfn "success creating email %s" s
let failure msg = printfn "error creating email: %s" msg
let createEmailAddress = EmailAddress.createWithCont success failure

let address3 = createEmailAddress "not-an-email"
printfn "address3: %A" address3
let address4 = createEmailAddress "jane.smith@domain.org"
printfn "address4: %A" address4

let contact1 = 
    match EmailAddress.create "admin@website.io" with
    | Some email -> ContactInfo.EmailOnly email
    | None -> ContactInfo.PostOnly "No email provided"
printfn "contact1: %A" contact1
let contact2 = 
    match EmailAddress.create "invalid.email.format" with
    | Some email -> ContactInfo.EmailAndPost (email, "789 Oak Avenue")
    | None -> ContactInfo.PostOnly "789 Oak Avenue"
printfn "contact2: %A" contact2
let contact3 = 
    match EmailAddress.create "support@service.com" with
    | Some email -> ContactInfo.EmailAndPost (email, "321 Pine Street")
    | None -> ContactInfo.PostOnly "321 Pine Street"
printfn "contact3: %A" contact3