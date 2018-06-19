---
external help file: Microsoft.Azure.Commands.DataLakeAnalytics.dll-Help.xml
Module Name: AzureRM.DataLakeAnalytics
online version: https://docs.microsoft.com/en-us/powershell/module/azurerm.datalakeanalytics/export-azureatalakescopejobdetails
schema: 2.0.0
---

# Export-AzureRmDataLakeScopeJobDetails

## SYNOPSIS
Export job
## SYNTAX

```
Export-AzureRmDataLakeScopeJobDetails [-Account] <String> [-JobId] <Guid> [-TargetFolder] <String>
 [-DefaultProfile <IAzureContextContainer>] [-WhatIf] [-Confirm]
```

## DESCRIPTION
The **Export-AzureRmDataLakeScopeJobDetails** cmdlet Export job
## EXAMPLES

### Example 1
```
PS C:\>Export-AdlScopeJob -Account "cosmosveadla" -JobId "0dda6c59-7d86-4afa-b177-f6d14a3fcfdb" -TargetFolder “d:\demo\result”
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

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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

### -JobId
ID of the specific job to return job information for.

```yaml
Type: Guid
Parameter Sets: (All)
Aliases: 

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -TargetFolder
TargetFolder.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: True
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

### System.String
System.Guid


## OUTPUTS

### System.String


## NOTES

## RELATED LINKS

