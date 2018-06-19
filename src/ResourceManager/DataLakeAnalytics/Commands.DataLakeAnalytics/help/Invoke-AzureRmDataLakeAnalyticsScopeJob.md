---
external help file: Microsoft.Azure.Commands.DataLakeAnalytics.dll-Help.xml
Module Name: AzureRM.DataLakeAnalytics
online version: https://docs.microsoft.com/en-us/powershell/module/azurerm.datalakeanalytics/invoke-azureatalakeanalyticsscopejob
schema: 2.0.0
---

# Invoke-AzureRmDataLakeAnalyticsScopeJob

## SYNOPSIS
Invoke job
## SYNTAX

```
Invoke-AzureRmDataLakeAnalyticsScopeJob [[-StorageAccount] <String>] [-ScriptPath] <String>
 [-InputPath <String[]>] [-OutputPath <String>] [-ScopePath <String[]>] [-Parameter <Hashtable>]
 [-ScopeWorkingDir <String>] [-Option <String>] [-ScopeSdkPath <String>] [-DefaultProfile <IAzureContextContainer>]
```

## DESCRIPTION
The **Invoke-AzureRmDataLakeAnalyticsScopeJob** cmdlet Invoke job
## EXAMPLES

### Example 1
```
PS C:\>Invoke-AdlScopeJob  -ScriptPath "d:\data\2.script"  -parameter @{"input" = "/ScopeOnAdl/10M.txt"; "output" = "/ScopeOnAdl/10M_sorted.ss"} -ScopePath @('d:\demo\data\')  -option compile
```

compile Scope job

### Example 2
```
PS C:\>Invoke-AdlScopeJob  -ScriptPath "d:\data\2.script"  -parameter @{"input" = "/ScopeOnAdl/10M.txt"; "output" = "/ScopeOnAdl/10M_sorted.ss"} -ScopePath @('d:\demo\data\')  -option run
```

run Scope job

### Example 3
```
PS C:\>Invoke-AdlScopeJob  -ScriptPath "d:\data\2.script"  -parameter @{"input" = "/ScopeOnAdl/10M.txt"; "output" = "/ScopeOnAdl/10M_sorted.ss"} -ScopePath @('d:\demo\data\')  -option buildclusterplan
```

buildclusterplan Scope job

## PARAMETERS

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

### -Option
The scope Option

```yaml
Type: String
Parameter Sets: (All)
Aliases: 
Accepted values: Run, Compile, buildclusterplan

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Parameter
Specify parameter that you want to pass to SCOPE script.

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

### -InputPath
If the path given in the script is an absolute path, it is used and no further processing is needed. If the path is relative, the system will do probing as described above using the search path for input streams. The input stream search path is configurable, but the default value is: $(SCRIPT_DIR);$(SCOPE_DIR).

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -OutputPath
If the path given in the script is an absolute path, it is used and no further processing is needed. If the path is relative, the system will combine the relate path with the system Output Stream Path. The output stream path is a configurable value with a default of $(SCOPE_DIR).

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ScopePath
The ScopePath for this job

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ScopeSdkPath
The scope Sdk path

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ScopeWorkingDir
Specify root directory for script cache folders and temporary folders for script execution.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ScriptPath
Path to the script file to run.

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

### -StorageAccount
The default Data Lake storage account for this script

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

## INPUTS

### System.String
System.String[]
System.Collections.Hashtable


## OUTPUTS

### Microsoft.Azure.Management.DataLake.Analytics.Models.JobInformation


## NOTES

## RELATED LINKS

