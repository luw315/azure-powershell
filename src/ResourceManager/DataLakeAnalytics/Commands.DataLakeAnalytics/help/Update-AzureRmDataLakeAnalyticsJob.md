---
external help file: Microsoft.Azure.Commands.DataLakeAnalytics.dll-Help.xml
Module Name: AzureRM.DataLakeAnalytics
online version: https://docs.microsoft.com/en-us/powershell/module/azurerm.datalakeanalytics/update-azureatalakeanalyticsjob
schema: 2.0.0
---

# Update-AzureRmDataLakeAnalyticsJob

## SYNOPSIS
Updates a job.

## SYNTAX

```
Update-AzureRmDataLakeAnalyticsJob [-Account] <String> [-JobId] <Guid> [-DegreeOfParallelism <Int32>]
 [-Priority <Int32>] [-Tag <Hashtable>] [-DefaultProfile <IAzureContextContainer>] [-WhatIf] [-Confirm]
```

## DESCRIPTION
The **Update-AzureRmDataLakeAnalyticsJob** cmdlet updates an Azure Data Lake Analytics job.

## EXAMPLES

### Example 1
```
PS C:\>Update-AzureRmDataLakeAnalyticsJob -Account "ContosoAdlAccount" -JobId "a0a78d72-3fa8-4564-9b18-6becb3fda48a" -DegreeOfParallelism 32 -Priority 2 -Tag @{ "Second" = "Data" ; "Name" = "Contoso" }
```

This command updates the DegreeOfParallelism, Priority, and Tag of a job.

## PARAMETERS

### -Account
Name of the Data Lake Analytics account name under which want to update the job.

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

### -DegreeOfParallelism
The degree of parallelism to use for this job.
Typically, a higher degree of parallelism dedicated to a script results in faster script execution time.
At least DegreeOfParallelism, Priority, or Tag must be specified.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -JobId
ID of the specific job to update.

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

### -Priority
The priority for this job with a range from 1 to 1000, where 1000 is the lowest priority and 1 is the highest.
At least DegreeOfParallelism, Priority, or Tag must be specified.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Tag
A string,string dictionary of tag associated with this job.
At least DegreeOfParallelism, Priority, or Tag must be specified.

```yaml
Type: Hashtable
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
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
System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
System.Collections.Hashtable


## OUTPUTS

### Microsoft.Azure.Management.DataLake.Analytics.Models.JobInformation


## NOTES

## RELATED LINKS

