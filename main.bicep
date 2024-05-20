@description('Name for the container group')
param name string = 'bid-service'

@minLength(2)
@maxLength(100)
param storageAccountName string = 'store${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location

@description('The behavior of Azure runtime if container has stopped.')
@allowed([
  'Always'
  'Never'
  'OnFailure'
])
param restartPolicy string = 'Always'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-04-01' = {
  name: 'bidbicep'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'Subnet-1'
        properties: {
          addressPrefix: '10.0.0.0/24'
        }
      }
      {
        name: 'Subnet-2'
        properties: {
          addressPrefix: '10.0.1.0/24'
        }
      }
    ]
  }
}


resource forsogstorage 'Microsoft.Storage/storageAccounts@2023-04-01' = {
  name: 'bicepbid'
  location: resourceGroup()
  sku: {
    name: 'bidserviceapi'
  }
  kind: 'bidkind'
}
