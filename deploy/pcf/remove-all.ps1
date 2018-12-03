# service apps
foreach ($_ in "basket.api catalog.api identity.api locations.api marketing.api ordering.api ordering.backgroundtasks ordering.signalrhub payment.api".Split(' ')) {
    $commandArgs = "delete eshop.$_ -r -f"
    Start-Process -FilePath "cf.exe" -ArgumentList $commandArgs
}

# ui apps
foreach ($_ in "mvc spa status".Split(' ')) {
    $commandArgs = "delete eshop.$_ -r -f"
    Start-Process -FilePath "cf.exe" -ArgumentList $commandArgs
}

# aggregators
foreach ($_ in "mobileshoppingagg webshoppingagg".Split(' ')) {
    $commandArgs = "delete eshop.$_ -r -f"
    Start-Process -FilePath "cf.exe" -ArgumentList $commandArgs
}

# gateways
foreach ($_ in "MobileMarketingApiGw MobileShoppingApiGw WebMarketingApiGw WebShoppingApiGw".Split(' ')) {
    $commandArgs = "delete eshop.$_ -r -f"
    Start-Process -FilePath "cf.exe" -ArgumentList $commandArgs
}

# backing services
foreach ($_ in "eShopConfig eShopRegistry eShopDocDb eShopMySQL eShopMQ eShopCache".Split('')) {
   $commandArgs = "delete-service eshop.$_ -f"
   Start-Process -FilePath "cf.exe" -ArgumentList $commandArgs
}