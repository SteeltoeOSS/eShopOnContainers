Param(
    [string]$os = "ubuntu.18.04-x64",
    $fanOutProcessing = $false
)
# time the whole operation
$TotalTime = New-Object -TypeName System.Diagnostics.Stopwatch
$TotalTime.Start()

# Create services
$ServiceTime = New-Object -TypeName System.Diagnostics.Stopwatch
$ServiceTime.Start()
& deploy/pcf/create-services.ps1 -fanOutProcessing $fanOutProcessing
$ServiceTime.Stop()
Write-Host "Time to create services:" $ServiceTime.Elapsed.ToString()

# publish and push apps
$AppTime = New-Object -TypeName System.Diagnostics.Stopwatch
$AppTime.Start()
& deploy/pcf/publish-deploy-apps.ps1 -os $os -fanOutProcessing $fanOutProcessing
$AppTime.Stop()
Write-Host "Time to publish and deploy apps:" $AppTime.Elapsed.ToString()

$TotalTime.Stop()
Write-Host "Total processing time:" $TotalTime.Elapsed.ToString()
