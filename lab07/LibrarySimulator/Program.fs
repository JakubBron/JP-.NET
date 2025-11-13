open System
open LibrarySimulator

let ConsoleRead userInput =
    printf "%s" userInput
    Console.ReadLine()

let ConsoleReadOptional userInput =
    let input = ConsoleRead (userInput + " (or press ENTER to skip): ")
    if String.IsNullOrWhiteSpace input then None else Some input

let ConsoleReadDateTime userInput =
    let input = ConsoleRead (userInput + " (use YYYY-MM-DD format or press ENTER to skip): ")
    if String.IsNullOrWhiteSpace input then DateTime.UtcNow
    else match DateTime.TryParse input with | true, date -> date | _ -> printfn "Date incorrect! Setting todays date..."; DateTime.UtcNow

let ConsoleReadFloat userInput =
    let input = ConsoleRead userInput
    match Decimal.TryParse input with | true, value -> Some value | _ -> printfn "Incorrect number!"; None

let printUsers (user: User) =
    match user.Description with
    | Some details -> printfn "[%s] %s %s | Email: %s | Status: %A | Penalties: %M PLN | Deposit: %M USD" user.Id details.Name details.Surname (defaultArg details.Email "none") user.Status (decimal user.PenaltiesPLN) (decimal user.DepositUSD)
    | None -> printfn "[%s] (no description) | Status: %A | Penalties: %M PLN | Deposit: %M USD" user.Id user.Status (decimal user.PenaltiesPLN) (decimal user.DepositUSD)

let printMenu () =
    printfn "\n=== Main menu ==="
    printfn "0. Show recent exchange rates"
    printfn "1. Users list"
    printfn "2. Show user info & borrows"
    printfn "3. Add penalty in PLN"
    printfn "4. Check if penalty exceeds deposit"
    printfn "5. Change user tier"
    printfn "6. Add/Update existing user"
    printfn "7. Make new borrow (ISBN)"
    printfn "8. Make a return"
    printfn "9. Save & exit"

[<EntryPoint>]
let main _ =
    Domain.kickstartData ()
    let users = Domain.loadUsers ()
    let library = Domain.Library users

    let mutable ratesOptional = Currency.fetchRates().Result
    match ratesOptional with
    | None -> printfn "Unable to fetch rates!"
    | Some r -> printfn "Rates: 1 EUR = %M PLN, 1 USD = %M PLN" r.PLNEUR r.PLNUSD

    let rec loop () =
        printMenu()
        match ConsoleRead "> " with
        | "0" ->
            ratesOptional <- Currency.fetchRates().Result
            match ratesOptional with
            | Some rates -> printfn "Recent rates: 1 EUR = %M PLN, 1 USD = %M PLN" rates.PLNEUR rates.PLNUSD
            | None -> printfn "No data (fetch error)."
            loop()
        | "1" ->
            library.GetAll() |> List.sortBy (fun user -> user.Id) |> List.iter printUsers
            loop()
        | "2" ->
            let userId = ConsoleRead "Enter user ID: "
            match library.GetUsers userId with
            | Some user ->
                printUsers user
                
                let loans = library.GetLoanHistory userId
                if loans.IsEmpty then printfn "No borrows."
                else loans |> List.iter (fun loan -> printfn "- ISBN: %s | Returned: %b" loan.ISBN loan.Returned)
            | None -> printfn "Not found"
            loop()
        | "3" ->
            let userId = ConsoleRead "Enter user ID: "
            match ConsoleReadFloat "Enter amount in PLN: " with
            | Some value -> library.AddFine(userId, (value * 1.0M<PLN>)); printfn "Penalty added successfully."
            | None -> ()
            loop()
        | "4" ->
            let userId = ConsoleRead "Enter user ID: "
            match ratesOptional with
            | Some rate ->
                let exceeds = library.FinesExceedDeposit(userId, rate)
                printfn (if exceeds then "Total penalties exceeds deposit!" else "All penalties are less than deposit. OK!")
            | None -> printfn "No exchange rates. Type 0 in main menu."
            loop()
        | "5" ->
            let userId = ConsoleRead "Enter user ID: "
            let loansStr = ConsoleRead "Needed borrows: "
            let daysStr = ConsoleRead "Needed membership duration (days): "
            match Int32.TryParse loansStr, Int32.TryParse daysStr with
            | (true, loan), (true, days) ->
                let readyForStatusChange = library.UpgradeIfConsitionsMet(userId, loan, days)
                printfn (if readyForStatusChange then "Changed status to 'Premium'" else "Does not qualify for upgrade.")
            | _ -> printfn "Incorrect data!"
            loop()
        | "6" ->
            let userId = ConsoleRead "Enter user ID: "
            let deposit = ConsoleReadFloat "Deposit (USD): "
            let joinDate = ConsoleReadDateTime "Joined"
            let statusStr = ConsoleRead "Status (Standard/Premium): "
            let status = if statusStr.Trim().ToLower() = "premium" then UserStatus.Premium else UserStatus.Standard
            
            let hasDescription = ConsoleRead "Add description? (y/n): "
            let userDescriptionOptional =
                if hasDescription.Trim().ToLower() = "y" then
                    let name = ConsoleRead "Name: "
                    let surname = ConsoleRead "Surname: "
                    let birthDateStr = ConsoleReadDateTime "Birth date:"
                    let cardNo = ConsoleRead "Library cardNo no.: "
                    let email = ConsoleReadOptional "Email:"
                    let phoneStr = ConsoleReadOptional "Phone:"
                    let addressStr = ConsoleReadOptional "Mail addressStr:"
                    let contact =
                        match phoneStr, addressStr with
                        | Some phone, Some address -> ContactInfo.PhoneAndAddress(phone, address)
                        | Some phone, None -> ContactInfo.PhoneOnly phone
                        | None, Some address -> ContactInfo.AddressOnly address
                        | None, None ->
                            printfn "Phone or mail address are needed!"; ContactInfo.PhoneOnly "none"
                    Some { Name = name; Surname = surname; BirthDate = birthDateStr; LibraryCardId = cardNo; Email = email; Contact = contact }
                else None
            match deposit with
            | Some deposit ->
                let newUser: User = { Id = userId; Description = userDescriptionOptional; DepositUSD = (deposit * 1.0M<USD>); JoinDate = joinDate; Status = status; PenaltiesPLN = 0.0M<PLN> }
                library.AddOrReplaceUser newUser
                printfn "User saved."
            | None -> ()
            loop()
        | "7" ->
            let userId = ConsoleRead "Enter user ID: "
            let isbn = ConsoleRead "Book ISBN: "
            library.AddLoan(userId, isbn)
            printfn "Borrowed sucessfully."
            loop()
        | "8" ->
            let userId = ConsoleRead "Enter user ID: "
            let isbn = ConsoleRead "ISBN of book to be returned: "
            library.MarkReturned(userId, isbn)
            printfn "Returned."
            loop()
        | "9" ->
            library.Save()
            0
        | _ ->
            printfn "Unknown. Enter 0-9."
            loop()
    loop()
