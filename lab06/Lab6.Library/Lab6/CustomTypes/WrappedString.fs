module Lab6.Library.CustomTypes.WrappedString

type IWrappedString =
    abstract Value: string

let create canonicalize isValid ctor (s: string) =
    if s = null then
        None
    else
        let s' = canonicalize s
        if isValid s' then
            Some (ctor s')
        else
            None

let apply f (s: IWrappedString) =
    s.Value |> f

let value (s: IWrappedString) =
    apply id s

let equals left right =
    (value left) = (value right)

let compareTo left right =
    (value left).CompareTo(value right)

let singleLineTrimmed s =
    System.Text.RegularExpressions.Regex.Replace(s, "\s", " ").Trim()

let lengthValidator maxLength (s: string) =
    s.Length <= maxLength

type String100 = String100 of string with
    interface IWrappedString with
        member this.Value = let (String100 s) = this in s

let string100 = create singleLineTrimmed (lengthValidator 100) String100

let convertToString100 s = apply string100 s

type String50 = String50 of string with
    interface IWrappedString with
        member this.Value = let (String50 s) = this in s

let string50 = create singleLineTrimmed (lengthValidator 50) String50

let convertToString50 s = apply string50 s