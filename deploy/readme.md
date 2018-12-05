# eShop Deployment

## Deploying to Pivotal Cloud Foundry

This fork has manifest files to facilitate deployment to both Linux and Windows, PowerShell scripts are provided.

### PCF Pre-requisites

1. [CF CLI Installed](https://docs.cloudfoundry.org/cf-cli/install-go-cli.html)
1. Cloud Foundry environment available, with the following tiles:
    1. MongoDB
    1. MySql
    1. RabbitMQ
    1. Redis
    1. Spring Cloud Services

Login to your Cloud Foundry environment with [cf login](https://docs.cloudfoundry.org/cf-cli/getting-started.html#login), selecting or creating the appropriate org/space.

### Included Scripts

Four PowerShell scripts have been included to help with service provisioning, app deployment and environment teardown. All scripts are expected to be executed from the root of the repository.

1. [create-services.ps1](./pcf/create-services.ps1) - creates all the resources used by the applications, optionally in parallel, optionally waiting for provisioning to complete before returning
1. [publish-deploy-apps.ps1](./pcf/publish-deploy-apps.ps1) - cleans the solution, navigates to each project to execute `dotnet publish` and `cf push`, optionally in parallel.
   1. Supports parameters for deploying subsets of projects in the solution
1. [complete-deployment.ps1](./pcf/complete-deployment.ps1) - calls both create-services and publish-deploy-apps, tracks time of the operations
1. [remove-all.ps1](./pcf/remove-all.ps1) - removes all apps and services, optionally in parallel

An OS parameter is available for both `complete-deployment` and `publish-deploy-apps`. This parameter should be a valid runtime for `dotnet` commands, as it is passed to `dotnet publish`. This value will also determine which manifest file is used - using a value that stars with `win` results in `manifest-windows.yml` files being used for app deployments. The `manifest-windows.yml` files all set the stack to `windows2016`. The default value is `ubuntu.18.04-x64`, which results in `manifest.yml` being used for deployments, which sets `cflinuxfs3` as the stack.

## Deploying Resources On Azure

### Pre-requisites

1. [Azure CLI 2.0 Installed](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
2. Azure subscription created

Login into your azure subscription by typing `az login` (note that you maybe need to use `az account set` to set the subscription to use). Refer to [this article](https://docs.microsoft.com/en-us/cli/azure/authenticate-azure-cli) for more details

### Deploying using CLI

### Deploying Virtual machines to host the services

1. [Deploying a Linux VM to run single-server development environment using docker-machine (**Recommended for development environments**)](az/vms/docker-machine.md)
2. [Deploying a Linux VM or Windows Server 2016 to run a single-server development environment using ARM template (**Recommended for creating testing environments**)](az/vms/plain-vm.md)

Using `docker-machine` is the recommended way to create a VM with docker installed. But it is limited to Linux based VMs.

### Deploying Azure resources used by the services

1. [Deploying SQL Server and databases](az/sql/readme.md)
2. [Deploying Azure Service Bus](az/servicebus/readme.md)
3. [Deploying Redis Cache](az/redis/readme.md)
4. [Deploying Cosmosdb](az/cosmos/readme.md)
5. [Deploying Catalog Storage](az/storage/catalog/readme.md)
6. [Deploying Marketing Storage](az/storage/marketing/readme.md)
7. [Deploying Marketing Azure functions](az/azurefunctions/readme.md)
