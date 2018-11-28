Param(
    [bool]$waitForCreation = $true
)

Write-Host "Provisioning Config Server..."
& cf create-service p-config-server standard eShopConfig -c '{\""git\"":{\""uri\"":\""https://github.com/SteeltoeOSS/eShopOnContainers-config\"",\""cloneOnStart\"":\""true\""}}'

Write-Host "Provisioning Eureka..."
& cf create-service p-service-registry standard eShopRegistry

Write-Host "Provisioning MongoDB..."
#& cf create-service a9s-mongodb36 mongodb-single-nano eShopDocDb
& cf create-service mongodb-odb standalone_small eShopDocDb

Write-Host "Provisioning MySQL..."
& cf create-service p.mysql db-small eShopMySQL

Write-Host "Provisioning RabbitMQ..."
& cf create-service p-rabbitmq standard eShopMQ

Write-Host "Provisioning Redis..."
& cf create-service p-redis shared-vm eShopCache

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