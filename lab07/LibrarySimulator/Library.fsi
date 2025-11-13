namespace LibrarySimulator

open System

[<Measure>] type PLN
[<Measure>] type EUR
[<Measure>] type USD

type ContactInfo =
    | PhoneOnly of phoneStr:string
    | AddressOnly of addressStr:string
    | PhoneAndAddress of phoneStr:string * addressStr:string

type DaneContactowe = {
    Name: string
    Surname: string
    BirthDate: DateTime
    LibraryCardId: string
    Email: string option
    Contact: ContactInfo
}

type UserStatus =
    | Standard
    | Premium

type User = {
    Id: string
    Description: DaneContactowe option
    DepositUSD: decimal<USD>
    JoinDate: DateTime
    Status: UserStatus
    PenaltiesPLN: decimal<PLN>
}

type Loan = { ISBN: string; Returned: bool }

type LoanHistory = { UserId: string; Loans: Loan list }

module Currency =
    type Rates = { PLNEUR: decimal; PLNUSD: decimal }
    val fetchRates: unit -> System.Threading.Tasks.Task<Rates option>
    val PLNtoEUR: rates:Rates -> x:decimal<PLN> -> decimal<EUR>
    val PLNtoUSD: rates:Rates -> x:decimal<PLN> -> decimal<USD>
    val USDtoPLN: rates:Rates -> x:decimal<USD> -> decimal<PLN>
    val EURtoPLN: rates:Rates -> x:decimal<EUR> -> decimal<PLN>

module Domain =
    val kickstartData: unit -> unit
    val loadUsers: unit -> User list
    val getLoanHistory: userId:string -> Loan list
    val isUserForMoreThan: days:int -> user:User -> bool
    val addFine: amount:decimal<PLN> -> user:User -> User

    type Library =
        new: usersInit: User list -> Library
        member GetUsers: userId:string -> User option
        member GetAll: unit -> User list
        member Save: unit -> unit
        member AddFine: userId:string * amount:decimal<PLN> -> unit
        member UpgradeIfConsitionsMet: userId:string * minLoans:int * minDays:int -> bool
        member TotalFinesPLN: userId:string -> decimal<PLN> option
        member DepositUSD: userId:string -> decimal<USD> option
        member FinesExceedDeposit: userId:string * rates: Currency.Rates -> bool
        member AddOrReplaceUser: p:User -> unit
        member GetLoanHistory: userId:string -> Loan list
        member AddLoan: userId:string * isbn:string -> unit
        member MarkReturned: userId:string * isbn:string -> unit
