$OriginalProgressPreference = $Global:ProgressPreference
$Global:ProgressPreference = 'SilentlyContinue'

if (!(Test-NetConnection).PingSucceeded)
{
    Write-Output "ERR:NO_INTERNET"
    exit 1
}

Write-Output (Invoke-RestMethod $args[0] | ConvertTo-Json)

$Global:ProgressPreference = $OriginalProgressPreference