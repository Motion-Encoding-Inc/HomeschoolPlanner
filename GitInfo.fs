module ScholarsForge.Git
let builtOn =
    let fi = System.Reflection.Assembly.GetExecutingAssembly().Location |> System.IO.FileInfo
    let lastWriteTime = fi.LastWriteTime
    System.DateTimeOffset(lastWriteTime)
let builtBy = "patrick.simpson@revshare.com [Patrick Simpson]" 
let commit = "3b156a18" 
