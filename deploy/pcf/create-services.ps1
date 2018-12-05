Param(
    [bool]$fanOutProcessing = $false,
    [bool]$waitForCreation = $true
)


function CreateService {
    param([string]$serviceType, [string]$servicePlan, [string]$serviceName, [string]$createArgs)
    $commandArgs = 'create-service ' + $serviceType + ' ' + $servicePlan + ' '  + $serviceName + ' '  + $createArgs
    if ($fanOutProcessing){
        Start-Process -FilePath "cf.exe" -ArgumentList $commandArgs
    }
    else {
        Invoke-Expression "cf $commandArgs"
    }
}

Write-Host "Provisioning Config Server..."
# escaping this and passing it to the function gets weird, don't bother...
& cf create-service p-config-server standard eShopConfig -c '{\""git\"":{\""uri\"":\""https://github.com/SteeltoeOSS/eShopOnContainers-config\"",\""cloneOnStart\"":\""true\""}}'

Write-Host "Provisioning Eureka..."
CreateService p-service-registry standard eShopRegistry

Write-Host "Provisioning MongoDB..."
# CreateService  a9s-mongodb36 mongodb-single-nano eShopDocDb
CreateService mongodb-odb standalone_small eShopDocDb

Write-Host "Provisioning MySQL..."
CreateService p.mysql db-small eShopMySQL

Write-Host "Provisioning RabbitMQ..."
CreateService p-rabbitmq standard eShopMQ

Write-Host "Provisioning Redis..."
CreateService p-redis shared-vm eShopCache

Write-Host "Waiting a few seconds before polling for service status..."
Start-Sleep -Seconds 5
if ($waitForCreation) {
    $servicesToMonitor = "eShopConfig", "eShopRegistry", "eShopDocDb", "eShopMySQL", "eShopMQ", "eShopCache"
    $waitCounter = "."
    while ((& cf services | Out-String) -Match "in progress"){
        foreach ($_ in $servicesToMonitor){
            if ((& cf service $_ | Out-String) -Match "in progress"){
                Write-Host "Still waiting for $($_)$($waitCounter)"
            }
        }
        $waitCounter += "."
        Start-Sleep -Seconds 5
    }
}