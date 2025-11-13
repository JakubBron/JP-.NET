module Lab6.Library.CustomTypes.String50

type T = String50 of string

let create (s: string) =
    if s <> null && s.Length <= 50 then
        Some (String50 s)
    else
        None

let apply f (String50 s) = f s

let value s = apply id s