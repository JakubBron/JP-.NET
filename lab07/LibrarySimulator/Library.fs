namespace LibrarySimulator

open System
open System.IO
open System.Net.Http
open System.Text.Json
open System.Text.Json.Serialization
open FSharp.SystemTextJson

[<Measure>] type PLN
[<Measure>] type EUR
[<Measure>] type USD


type ContactInfo =
    | PhoneOnly of phoneStr:string
    | AddressOnly of addressStr:string
    | PhoneAndAddress of phoneStr:string * addressStr:string

[<CLIMutable>]
type DaneContactowe = {
    Name: string
    Surname: string
    BirthDate: DateTime
    LibraryCardId: string
    Email: string option
    Contact: ContactInfo
}

[<RequireQualifiedAccess>]
type UserStatus =
    | Standard
    | Premium

[<CLIMutable>]
type User = {
    Id: string
    Description: DaneContactowe option
    DepositUSD: decimal<USD>
    JoinDate: DateTime
    Status: UserStatus
    PenaltiesPLN: decimal<PLN>
}


[<CLIMutable>]
type Loan = { ISBN: string; Returned: bool }

[<CLIMutable>]
type LoanHistory = { UserId: string; Loans: Loan list }

module Serialization =
    let private jsonOptions =
        let options = JsonSerializerOptions(WriteIndented = true)
        options.Converters.Add(JsonStringEnumConverter())
        options.Converters.Add(JsonFSharpConverter())     // F# options converter
        options

    let serialize<'a> (value: 'a) =
        JsonSerializer.Serialize(value, jsonOptions)

    let deserialize<'a> (json: string) =
        JsonSerializer.Deserialize<'a>(json, jsonOptions)

module Files =
    let usersFile = Path.Combine("data", "users.json")
    let loansDir = Path.Combine("data", "loans")
    let loanFile userId = Path.Combine(loansDir, sprintf "loans_%s.json" userId)

    let ensureDataDirs () =
        Directory.CreateDirectory("data") |> ignore
        Directory.CreateDirectory(loansDir) |> ignore


module SampleData =
    let private usersToKickstart : User list =
        let user1 = { Name = "Jola"; Surname = "Abacka"; BirthDate = DateTime(1984,10,2); LibraryCardId = "Id_1"; Email = Some "jola.abacka@example.com"; Contact = PhoneOnly "600700800" }
        let user2 = { Name = "Krzysztof"; Surname = "Bbacki"; BirthDate = DateTime(1973,3,14); LibraryCardId = "Id_2"; Email = None; Contact = AddressOnly "G. Narutowicza 11/12 80-233 Gdańsk" }
        let user3 = { Name = "Natalia"; Surname = "Cbacka"; BirthDate = DateTime(2010,4,29); LibraryCardId = "Id_3"; Email = Some "natalia@cbacka.pl"; Contact = PhoneAndAddress("800700600", "Legionów 27, Gdynia") }
        [
            { Id = "1"; Description = Some user1; DepositUSD = 40.0M<USD>; JoinDate = DateTime.UtcNow.AddDays(-200.); Status = UserStatus.Standard; PenaltiesPLN = 10.0M<PLN> }
            { Id = "2"; Description = Some user2; DepositUSD = 500.0M<USD>; JoinDate = DateTime.UtcNow.AddDays(-50.); Status = UserStatus.Premium; PenaltiesPLN = 20.0M<PLN> }
            { Id = "3"; Description = None; DepositUSD = 15.0M<USD>; JoinDate = DateTime.UtcNow.AddDays(-5.); Status = UserStatus.Premium; PenaltiesPLN = 0.0M<PLN> }
            { Id = "4"; Description = Some user3; DepositUSD = 9.80M<USD>; JoinDate = DateTime.UtcNow.AddDays(-365.); Status = UserStatus.Standard; PenaltiesPLN = 0.0M<PLN> }
        ]

    let private loansToKickstart : LoanHistory list =
        [
            { UserId = "1"; Loans = [ { ISBN = "978-2-12-345680-3"; Returned = true }; { ISBN = "978-83-7348-887-8"; Returned = false } ] }
            { UserId = "2"; Loans = [] }
            { UserId = "3"; Loans = [ { ISBN = "978-83-240-9278-9"; Returned = false }; { ISBN = "978-150-150-1500-4"; Returned = true }; ] }
            { UserId = "4"; Loans = [{ ISBN = "978-83-66575-35-6"; Returned = false }] }
        ]

    let makeConfigFileIfMissing () =
        Files.ensureDataDirs ()
        if not (File.Exists Files.usersFile) then
            let json = Serialization.serialize usersToKickstart
            File.WriteAllText(Files.usersFile, json)
        for loans in loansToKickstart do
            let path = Files.loanFile loans.UserId
            if not (File.Exists path) then
                let json = Serialization.serialize loans
                File.WriteAllText(path, json)

module Repo =
    let loadUsers () : User list =
        Files.ensureDataDirs ()
        if File.Exists Files.usersFile then
            let json = File.ReadAllText Files.usersFile
            Serialization.deserialize json |> Option.defaultValue []
        else []

    let saveUsers (users: User list) =
        Files.ensureDataDirs ()
        let json = Serialization.serialize users
        File.WriteAllText(Files.usersFile, json)

    let getLoanHistory (userId: string) : LoanHistory =
        Files.ensureDataDirs ()
        let path = Files.loanFile userId
        if File.Exists path then
            let json = File.ReadAllText path
            Serialization.deserialize json |> Option.defaultValue { UserId = userId; Loans = [] }
        else { UserId = userId; Loans = [] }

    let saveLoanHistory (history: LoanHistory) : unit =
        Files.ensureDataDirs ()
        let path = Files.loanFile history.UserId
        let json = Serialization.serialize history
        File.WriteAllText(path, json)

module Currency =
    let http = new HttpClient()

    type Rates = { PLNEUR: decimal; PLNUSD: decimal }

    let private tryParseDecimal (arg:string) =
        match Decimal.TryParse(arg, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture) with
        | true, value -> Some value
        | _ -> None

    let private getRatesFromOnline (symbol:string) =
        task {
            try
                let! response = http.GetStringAsync($"https://stooq.pl/q/l/?s={symbol}")
                let dataRaw = response.Split([|'\n';',';';';'\t';' '|], StringSplitOptions.RemoveEmptyEntries)
                let data = dataRaw |> Array.choose tryParseDecimal
                return data |> Array.tryLast
            with _ -> return None
        }

    let fetchRates () = task {
        let! eur = getRatesFromOnline "eurpln"
        let! usd = getRatesFromOnline "usdpln"
        match eur, usd with
        | Some e, Some u -> return Some { PLNEUR = e; PLNUSD = u }
        | _ -> return None
    }

    let private strip (x:decimal<'u>) : decimal = decimal x
    let private withPLN (v:decimal) : decimal<PLN> = LanguagePrimitives.DecimalWithMeasure v
    let private withEUR (v:decimal) : decimal<EUR> = LanguagePrimitives.DecimalWithMeasure v
    let private withUSD (v:decimal) : decimal<USD> = LanguagePrimitives.DecimalWithMeasure v

    let PLNtoEUR (rates:Rates) (x: decimal<PLN>) : decimal<EUR> = strip x / rates.PLNEUR |> withEUR
    let PLNtoUSD (rates:Rates) (x: decimal<PLN>) : decimal<USD> = strip x / rates.PLNUSD |> withUSD
    let USDtoPLN (rates:Rates) (x: decimal<USD>) : decimal<PLN> = strip x * rates.PLNUSD |> withPLN
    let EURtoPLN (rates:Rates) (x: decimal<EUR>) : decimal<PLN> = strip x * rates.PLNEUR |> withPLN

module Domain =
    let kickstartData () = SampleData.makeConfigFileIfMissing ()
    let loadUsers () = Repo.loadUsers ()

    let getLoanHistory (userId: string) : Loan list =
        let history = Repo.getLoanHistory userId
        history.Loans

    let isUserForMoreThan (days:int) (user: User) : bool =
        (DateTime.UtcNow - user.JoinDate).TotalDays > float days

    let addFine (amount: decimal<PLN>) (user: User) : User =
        { user with PenaltiesPLN = user.PenaltiesPLN + amount }

    type Library(usersInit: User list) =
        let mutable users = usersInit

        member _.GetUsers(userId:string) = users |> List.tryFind (fun p -> p.Id = userId)
        member _.GetAll() = users

        member _.Save() = Repo.saveUsers users

        member this.AddFine(userId:string, amount:decimal<PLN>) =
            users <- users |> List.map (fun p -> if p.Id = userId then addFine amount p else p)
            this.Save()

        member _.UpgradeIfConsitionsMet(userId:string, minLoans:int, minDays:int) =
            match users |> List.tryFind (fun p -> p.Id = userId) with
            | None -> false
            | Some p ->
                let loans = getLoanHistory userId |> List.length
                let longEnough = isUserForMoreThan minDays p
                if p.Status = UserStatus.Standard && loans > minLoans && longEnough then
                    users <- users |> List.map (fun x -> if x.Id = userId then { x with Status = UserStatus.Premium } else x)
                    true
                else false

        member _.TotalFinesPLN(userId:string) =
            users |> List.tryFind (fun p -> p.Id = userId) |> Option.map (fun p -> p.PenaltiesPLN)

        member _.DepositUSD(userId:string) =
            users |> List.tryFind (fun p -> p.Id = userId) |> Option.map (fun p -> p.DepositUSD)

        member this.FinesExceedDeposit(userId:string, rates: Currency.Rates) =
            match this.TotalFinesPLN userId, this.DepositUSD userId with
            | Some fines, Some deposit ->
                let depPln = Currency.USDtoPLN rates deposit
                fines > depPln
            | _ -> false

        member this.AddOrReplaceUser(user: User) =
            let exists = users |> List.exists (fun x -> x.Id = user.Id)
            users <-
                if exists then users |> List.map (fun x -> if x.Id = user.Id then user else x)
                else user :: users
            this.Save()

        member _.GetLoanHistory(userId:string) : Loan list =
            Repo.getLoanHistory userId |> fun h -> h.Loans

        member _.AddLoan(userId:string, isbn:string) =
            let h = Repo.getLoanHistory userId
            let updated = { h with Loans = h.Loans @ [ { ISBN = isbn; Returned = false } ] }
            Repo.saveLoanHistory updated

        member _.MarkReturned(userId:string, isbn:string) =
            let h = Repo.getLoanHistory userId
            let updatedLoans =
                let mutable marked = false
                h.Loans |> List.map (fun l ->
                    if not marked && l.ISBN = isbn && l.Returned = false then
                        marked <- true; { l with Returned = true }
                    else l)
            let updated = { h with Loans = updatedLoans }
            Repo.saveLoanHistory updated
