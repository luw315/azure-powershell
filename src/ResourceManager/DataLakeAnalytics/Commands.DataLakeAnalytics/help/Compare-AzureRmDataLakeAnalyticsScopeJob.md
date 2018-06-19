---
external help file: Microsoft.Azure.Commands.DataLakeAnalytics.dll-Help.xml
Module Name: AzureRM.DataLakeAnalytics
online version: https://docs.microsoft.com/en-us/powershell/module/azurerm.datalakeanalytics/compare-azureatalakeanalyticsscopejob
schema: 2.0.0
---

# Compare-AzureRmDataLakeAnalyticsScopeJob

## SYNOPSIS
Compare jobs
## SYNTAX

```
Compare-AzureRmDataLakeAnalyticsScopeJob [-Account] <String> [-ResourceGroup] <String> [-JobId1] <Guid>
 [-JobId2] <Guid> [-DefaultProfile <IAzureContextContainer>]
```

## DESCRIPTION
The **Compare-AzureRmDataLakeAnalyticsScopeJob** cmdlet compare jobs
## EXAMPLES

### Example 1
```
PS C:\>Compare-AdlScopeJob  -Account "cosmosveadla" -ResourceGroup "rg-rollandeastus2"  -JobId1 "0dda6c59-7d86-4afa-b177-f6d14a3fcfdb" -JobId2 "0dda6c59-7d86-4afa-b177-f6d14a3fcfdc"
```

## PARAMETERS

### -Account
Name of the Data Lake Analytics account name under which want to retrieve the job information.

```yaml
Type: String
Parameter Sets: (All)
Aliases: AccountName

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DefaultProfile
The credentials, account, tenant, and subscription used for communication with azure.

```yaml
Type: IAzureContextContainer
Parameter Sets: (All)
Aliases: AzureRmContext, AzureCredential

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -JobId1
ID of the specific job to return job information for.

```yaml
Type: Guid
Parameter Sets: (All)
Aliases: 

Required: True
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -JobId2
ID of the specific job to return job information for.

```yaml
Type: Guid
Parameter Sets: (All)
Aliases: 

Required: True
Position: 3
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -ResourceGroup
ResourceGroup of Data Lake Analytics account under which the job will be submitted.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

## INPUTS

### System.String
System.Guid


## OUTPUTS

### System.String


## NOTES

## RELATED LINKS

