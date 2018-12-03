Param(
    [string]$os = "ubuntu.18.04-x64",
    [bool]$fanOutProcessing = $false,
    [bool]$includeAPIs = $true,
    [bool]$includeGateways = $true,
    [bool]$includeWebUIs = $true,
    [bool]$includeMisc = $true
)

$windows = ""
if ($os -match 'win') {
    $windows = "-windows"
}
$framework = "netcoreapp2.1"

function OutputPathOrDefault{
    param([string]$outputPath)
    if (!$outputPath){
        $outputPath = "bin/debug/$framework/$os/publish"
    }
    return $outputPath
}

function Publish {
    param([string]$outputPath)
    $op = OutputPathOrDefault $outputPath
    Write-Host "dotnet publish -f $framework -r $os -o $op"
    & dotnet publish -f $framework -r $os -o $outputPath
}
function Push {
    param([string]$outputPath, [string]$appName, [bool]$openNewWindow = $false, [string]$pushArgs)
    $op = OutputPathOrDefault $outputPath
    $commandArgs = 'push '+$appName+' -f manifest'+$windows+'.yml -p '+$op+' '+$pushArgs
    Write-Host "cf $commandArgs"
     if ($openNewWindow) {
         Write-Host "^ running that in a new window..."
         Start-Process -FilePath "cf.exe" -ArgumentList $commandArgs
     }
     else {
        Invoke-Expression "cf $commandArgs"
     }
}

function Publish-Push {
    param([string]$outputPath)
    $op = OutputPathOrDefault $outputPath
    Write-Host "Publish-Push from $pwd to $op"
    Publish $op
    Push $op -openNewWindow $fanOutProcessing
}

Write-Host "Publishing for and deploying to $os"

If ((Test-Path -Path "src/Services")) {
    & dotnet clean eShopOnContainers-ServicesAndWebApps.sln
    Set-Location src

        if ($includeAPIs) {
            Write-Host "Publishing and deploying all .API projects..."
            Set-Location Services
                $ApiList = "Basket Catalog Identity Locations Marketing Ordering Payment"
                ForEach ($_ in $ApiList.Split(' ')) {
                    If ((Test-Path -Path "$_/$_.API")) {
                        Set-Location $_/$_.API
                        Publish-Push
                        Set-Location ../../
                    }
                }
            Set-Location ../
        }

        if ($includeGateways) {
            Write-Host "Publishing and deploying all API gateways and aggregators..."
            Set-Location ApiGateways
                # publish base gateway
                Remove-Item publish -Recurse
                Set-Location ApiGw-Base
                Publish "../publish"
                Set-Location ../
                mkdir publish/configuration

                # put config in the right place and push configured instances
                ForEach ($_ in "Mobile Web".Split(' ')) {
                    # gateways
                    ForEach ($__ in "Marketing Shopping".Split(' ')) {
                        If ((Test-Path -Path "$_.Bff.$__/apigw")) {
                            Write-Host "Copy-Item ""$_.Bff.$__/apigw/configuration.json"" ""publish/configuration/configuration.json"" -Force"
                            Copy-Item "$_.Bff.$__/apigw/configuration.https.json" "publish/configuration/configuration.json" -Force
                            Push "publish" "eshop.$($_)$($__)ApiGw" $false "--var appName=$($_)$($__)apigw"
                        }
                    }
                    # aggregators
                    If ((Test-Path -Path "$_.Bff.Shopping/aggregator")) {
                        Set-Location "$_.Bff.Shopping/aggregator"
                        Publish-Push
                        Set-Location ../../
                    }
                }
            Set-Location ../
        }

        if ($includeWebUIs) {
            Write-Host "Deploying Web UI apps..."
            # deploy webmvc
            Set-Location Web
                ForEach ($_ in "WebMVC WebStatus WebSPA".Split(' ')) {
                    If ((Test-Path -Path $_)) {
                        Set-Location $_
                        Publish-Push
                        Set-Location ../
                    }
                }
            Set-Location ../
        }

        if ($includeMisc) {
            # deploy odds & ends
            Write-Host "Publishing and deploying remaining services..."
            Set-Location "Services\Ordering"
                ForEach ($_ in "BackgroundTasks SignalrHub".Split(' ')) {
                    If ((Test-Path -Path "Ordering.$_")) {
                        Set-Location "Ordering.$_"
                        Publish-Push
                        Set-Location ../
                    }
                }
            Set-Location ../../
        }

    Set-Location ../
}
Else {
    Write-Host "This script is intended to be run from the root of the eShopOnContainers repository"
}