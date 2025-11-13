module Lab6.Library.CustomTypes.ContactInfo

type ContactInfo =
    | EmailOnly of EmailAddress.T
    | PostOnly of string
    | EmailAndPost of EmailAddress.T * string