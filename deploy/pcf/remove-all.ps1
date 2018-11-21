# service apps
foreach ($_ in "basket.api catalog.api identity.api locations.api marketing.api ordering.api ordering.backgroundtasks ordering.signalrhub payment.api".Split(' ')) {
    & cf delete "eshop.$_" -r -f
}

# ui apps
foreach ($_ in "mvc spa status".Split(' ')) {
    & cf delete "eshop.$_" -r -f
}

# aggregators
foreach ($_ in "mobileshoppingagg webshoppingagg".Split(' ')) {
    & cf delete "eshop.$_" -r -f
}

# gateways
foreach ($_ in "MobileMarketingApiGw MobileShoppingApiGw WebMarketingApiGw WebShoppingApiGw".Split(' ')) {
    & cf delete "eshop.$_" -r -f
}

# backing services
foreach ($_ in "eShopConfig eShopRegistry eShopDocDb eShopMySQL eShopMQ eShopCache".Split('')) {
    & cf delete-service $_ -f
}