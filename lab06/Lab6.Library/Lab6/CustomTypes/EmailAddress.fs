module Lab6.Library.CustomTypes.EmailAddress

type T = EmailAddress of string with
    interface WrappedString.IWrappedString with
        member this.Value = let (EmailAddress s) = this in s

let create =
    let canonicalize = WrappedString.singleLineTrimmed
    let isValid s =
        (WrappedString.lengthValidator 100 s) &&
        System.Text.RegularExpressions.Regex.IsMatch(s, @"^\S+@\S+\.\S+$")

    WrappedString.create canonicalize isValid EmailAddress

let createWithCont success failure =
    let canonicalize = WrappedString.singleLineTrimmed
    let isValid success failure s =
        if WrappedString.lengthValidator 100 s && System.Text.RegularExpressions.Regex.IsMatch(s, @"^\S+@\S+\.\S+$")
        then
            success (EmailAddress s)
            true
        else
            failure "Email address must contain an @ sign and be shorter than 100 signs"
            false

    WrappedString.create canonicalize (isValid success failure) EmailAddress

let convert s = WrappedString.apply create s