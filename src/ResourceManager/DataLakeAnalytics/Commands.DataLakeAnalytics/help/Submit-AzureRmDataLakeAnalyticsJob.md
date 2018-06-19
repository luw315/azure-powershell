---
external help file: Microsoft.Azure.Commands.DataLakeAnalytics.dll-Help.xml
Module Name: AzureRM.DataLakeAnalytics
ms.assetid: 0DB9595A-6C8B-4F3F-A707-2DB41D7C7470
online version: https://docs.microsoft.com/en-us/powershell/module/azurerm.datalakeanalytics/submit-azureatalakeanalyticsjob
schema: 2.0.0
---

# Submit-AzureRmDataLakeAnalyticsJob

## SYNOPSIS
Submits a job.

## SYNTAX

### SubmitUSqlJobWithScriptPath
```
Submit-AzureRmDataLakeAnalyticsJob [-Account] <String> [-Name] <String> [-ScriptPath] <String>
 [[-Runtime] <String>] [[-CompileMode] <String>] [-CompileOnly] [[-AnalyticsUnits] <Int32>]
 [[-Priority] <Int32>] [-Type <String>] [-ScriptParameter <IDictionary>] [-DefaultProfile <IAzureContextContainer>]
 [<CommonParameters>]
```

### SubmitUSqlJob
```
Submit-AzureRmDataLakeAnalyticsJob [-Account] <String> [-Name] <String> [-Script] <String>
 [[-Runtime] <String>] [[-CompileMode] <String>] [-CompileOnly] [[-AnalyticsUnits] <Int32>]
 [[-Priority] <Int32>] [-ScriptParameter <IDictionary>] [-DefaultProfile <IAzureContextContainer>]
 [<CommonParameters>]
```

### SubmitUSqlJobWithScriptPathAndRecurrence
```
Submit-AzureRmDataLakeAnalyticsJob [-Account] <String> [-Name] <String> [-ScriptPath] <String>
 [[-Runtime] <String>] [[-CompileMode] <String>] [-CompileOnly] [[-AnalyticsUnits] <Int32>]
 [[-Priority] <Int32>] [-ScriptParameter <IDictionary>] -RecurrenceId <Guid> [-RecurrenceName <String>]
 [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

### SubmitUSqlJobWithRecurrence
```
Submit-AzureRmDataLakeAnalyticsJob [-Account] <String> [-Name] <String> [-Script] <String>
 [[-Runtime] <String>] [[-CompileMode] <String>] [-CompileOnly] [[-AnalyticsUnits] <Int32>]
 [[-Priority] <Int32>] [-ScriptParameter <IDictionary>] -RecurrenceId <Guid> [-RecurrenceName <String>]
 [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

### SubmitUSqlJobWithScriptPathAndPipeline
```
Submit-AzureRmDataLakeAnalyticsJob [-Account] <String> [-Name] <String> [-ScriptPath] <String>
 [[-Runtime] <String>] [[-CompileMode] <String>] [-CompileOnly] [[-AnalyticsUnits] <Int32>]
 [[-Priority] <Int32>] [-ScriptParameter <IDictionary>] -RecurrenceId <Guid> [-RecurrenceName <String>]
 -PipelineId <Guid> [-PipelineName <String>] [-PipelineUri <String>] [-RunId <Guid>]
 [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

### SubmitUSqlJobWithPipeline
```
Submit-AzureRmDataLakeAnalyticsJob [-Account] <String> [-Name] <String> [-Script] <String>
 [[-Runtime] <String>] [[-CompileMode] <String>] [-CompileOnly] [[-AnalyticsUnits] <Int32>]
 [[-Priority] <Int32>] [-ScriptParameter <IDictionary>] -RecurrenceId <Guid> [-RecurrenceName <String>]
 -PipelineId <Guid> [-PipelineName <String>] [-PipelineUri <String>] [-RunId <Guid>]
 [-DefaultProfile <IAzureContextContainer>] [<CommonParameters>]
```

### SubmitScopeJobWithScriptPath
```
Submit-AzureRmDataLakeAnalyticsJob [-Account] <String> [-Name] <String> [-ScriptPath] <String> [[-Runtime] <String>]
 [[-AnalyticsUnits] <Int32>] [[-Priority] <Int32>] [-Type <String>] -ResourceGroup <String>
 [-ScopePath <String[]>] [-Parameter <Hashtable>] [-ScopeWorkingDir <String>] [-NotifyEmail <String[]>]
 [-NebulaArgument <String>] [-DisableBonusToken] [-CustomProperty <Hashtable>] [-ScopeSdkPath <String>]
 [-DefaultProfile <IAzureContextContainer>] [-MaxUnavailability <Int32>] [<CommonParameters>]
```

## DESCRIPTION
The **Submit-AzureRmDataLakeAnalyticsJob** cmdlet submits an Azure Data Lake Analytics job.

## EXAMPLES

### Example 1: Submit a U-SQL job
```
PS C:\>Submit-AzureRmDataLakeAnalyticsJob -Account "ContosoAdlAccount" -Name "New Job" -ScriptPath $LocalScriptPath -AnalyticsUnits 32
```

This command submits a Data Lake Analytics U-SQL job.

### Example 2: Submit a U-SQL job with script parameters
```
PS C:\>$parameters = [ordered]@{}
$parameters["Department"] = "Sales"
$parameters["NumRecords"] = 1000
$parameters["StartDateTime"] = (Get-Date).AddDays(-14)
Submit-AzureRmDataLakeAnalyticsJob -Account "ContosoAdlAccount" -Name "New Job" -ScriptPath $LocalScriptPath -AnalyticsUnits 32 -ScriptParameter $parameters
```

U-SQL script parameters are prepended above the main script contents, e.g.:

DECLARE @Department string = "Sales";
DECLARE @NumRecords int = 1000;
DECLARE @StartDateTime DateTime = new DateTime(2017, 12, 6, 0, 0, 0, 0);

### Example 3: Submit a Scope job
```
PS C:\>Submit-AdlJob -Account "cosmosveadla" -ResourceGroup "rg-rollandeastus2" -ScriptPath "d:\demo\data\2.script" -DisableBonusToken -MaxUnavailability 70 -type scope -parameter @{"input" = "/ScopeOnAdl/100K.txt"; "output" = "/ScopeOnAdl/100K_sorted.ss"} -NotifyEmail "kazhou@microsoft.com" -name "100K" -runtime "soy__yarnpp__feconvergence_2" -Priority 1 -AnalyticsUnits 5
```

This command submits a Data Lake Analytics Scope job.

## PARAMETERS

### -Account
Name of Data Lake Analytics account under which the job will be submitted.

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

### -CompileMode
The type of compilation to be done on this job. 
Valid values: 

- Semantic (Only performs semantic checks and necessary sanity checks)
- Full (Full compilation)
- SingleBox (Full compilation performed locally)

```yaml
Type: String
Parameter Sets: SubmitUSqlJobWithScriptPath, SubmitUSqlJob, SubmitUSqlJobWithScriptPathAndRecurrence, SubmitUSqlJobWithRecurrence, SubmitUSqlJobWithScriptPathAndPipeline, SubmitUSqlJobWithPipeline
Aliases: 
Accepted values: Semantic, Full, SingleBox

Required: False
Position: 4
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -CompileOnly
Indicates that the submission should only build the job and not execute if set to true.

```yaml
Type: SwitchParameter
Parameter Sets: SubmitUSqlJobWithScriptPath, SubmitUSqlJob, SubmitUSqlJobWithScriptPathAndRecurrence, SubmitUSqlJobWithRecurrence, SubmitUSqlJobWithScriptPathAndPipeline, SubmitUSqlJobWithPipeline
Aliases: 

Required: False
Position: 5
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -CustomProperty
The CustomProperties for this job

```yaml
Type: Hashtable
Parameter Sets: SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DefaultProfile
The credentials, account, tenant, and subscription used for communication with azure

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

### -AnalyticsUnits
The analytics units to use for this job. Typically, more analytics units dedicated to a script results in faster script execution time.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases: DegreeOfParallelism

Required: False
Position: 6
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DisableBonusToken
Disable bonus tokens for execution

```yaml
Type: SwitchParameter
Parameter Sets: SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -MaxUnavailability
Specify the percentage to use for Maximum Unavailability of data.

```yaml
Type: Int32
Parameter Sets: SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Name
The friendly name of the job to submit.

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

### -NebulaArgument
The NebulaArguments for this job

```yaml
Type: String
Parameter Sets: SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -NotifyEmail
Specify user to be notified when job completes. The email list is comma-separated.

```yaml
Type: String[]
Parameter Sets: SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Parameter
Specify parameters that you want to pass to SCOPE script. 

```yaml
Type: Hashtable
Parameter Sets: SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PipelineId
An ID that indicates the submission of this job is a part of a set of recurring jobs and also associated with a job pipeline.

```yaml
Type: Guid
Parameter Sets: SubmitUSqlJobWithScriptPathAndPipeline, SubmitUSqlJobWithPipeline
Aliases: 

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PipelineName
An optional friendly name for the pipeline associated with this job.

```yaml
Type: String
Parameter Sets: SubmitUSqlJobWithScriptPathAndPipeline, SubmitUSqlJobWithPipeline
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PipelineUri
An optional uri that links to the originating service associated with this pipeline.

```yaml
Type: String
Parameter Sets: SubmitUSqlJobWithScriptPathAndPipeline, SubmitUSqlJobWithPipeline
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Priority
The priority of the job. If not specified, the priority is 1000. A lower number indicates a higher job priority.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases: 

Required: False
Position: 7
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -RecurrenceId
An ID that indicates the submission of this job is a part of a set of recurring jobs with the same recurrence ID.

```yaml
Type: Guid
Parameter Sets: SubmitUSqlJobWithScriptPathAndRecurrence, SubmitUSqlJobWithRecurrence, SubmitUSqlJobWithScriptPathAndPipeline, SubmitUSqlJobWithPipeline
Aliases: 

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -RecurrenceName
An optional friendly name for the recurrence correlation between jobs.

```yaml
Type: String
Parameter Sets: SubmitUSqlJobWithScriptPathAndRecurrence, SubmitUSqlJobWithRecurrence, SubmitUSqlJobWithScriptPathAndPipeline, SubmitUSqlJobWithPipeline
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ResourceGroup
ResourceGroup of Data Lake Analytics account under which the job will be submitted.

```yaml
Type: String
Parameter Sets: SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -RunId
An ID that identifies this specific run iteration of the pipeline.

```yaml
Type: Guid
Parameter Sets: SubmitUSqlJobWithScriptPathAndPipeline, SubmitUSqlJobWithPipeline
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Runtime
Optionally set the version of the runtime to use for the job. If left unset, the default runtime is used.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: 3
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ScopePath
If the path supplied for a RESOURCE, REFERENCE, or VIEW statement is absolute (such as c:\src\code\myscript\resource1.txt) then scope uses the provided path.
If the path is not absolute then Scope appends tries to find the path inside one the search paths. The default value for the search paths is: $(SCRIPT_DIR);$(CLUSTER_ROOT);$(SCOPE_DIR) 
$(SCRIPT_DIR) - The script directory. This is automatically set by the system.
$(SCOPE_DIR) - The directory where the Scope Editor application is located. 
$(CLUSTER_ROOT) - The path for the cluster root directory. This variable uses the URL for the cluster and the VC, such as the following: adl://sandbox-c08.azuredatalakestore.net/. 
 
Once a matching file name is found using this search, the search is terminated.
NOTES: $ is special in Powershell. Please Enclose $(SCRIPT_DIR), $(SCOPE_DIR) and $(CLUSTER_ROOT) in single-quotes instead of the double-quotes. Like: -ScopePath '$(CLUSTER_ROOT)',"C:\dataroot\"

```yaml
Type: String[]
Parameter Sets: SubmitScopeJobWithScriptPath
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
Parameter Sets: SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Script
Script to execute (written inline).

```yaml
Type: String
Parameter Sets: SubmitUSqlJob, SubmitUSqlJobWithRecurrence, SubmitUSqlJobWithPipeline
Aliases: 

Required: True
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -ScriptParameter
The script parameters for this job, as a dictionary of parameter names (string) to values (any combination of byte, sbyte, int, uint (or uint32), long, ulong (or uint64), float, double, decimal, short (or int16), ushort (or uint16), char, string, DateTime, bool, Guid, or byte[]).

```yaml
Type: IDictionary
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ScriptPath
Path to the script file to submit.

```yaml
Type: String
Parameter Sets: SubmitUSqlJobWithScriptPath, SubmitUSqlJobWithScriptPathAndRecurrence, SubmitUSqlJobWithScriptPathAndPipeline, SubmitScopeJobWithScriptPath
Aliases: 

Required: True
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Type
The type of job

```yaml
Type: String
Parameter Sets: SubmitUSqlJobWithScriptPath, SubmitScopeJobWithScriptPath
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### String
Parameter 'Script' accepts value of type 'String' from the pipeline

## OUTPUTS

### JobInformation
The initial job details for the submitted job.

## NOTES

## RELATED LINKS

[Get-AzureRmDataLakeAnalyticsJob](./Get-AzureRmDataLakeAnalyticsJob.md)

[Stop-AzureRmDataLakeAnalyticsJob](./Stop-AzureRmDataLakeAnalyticsJob.md)

[Wait-AzureRmDataLakeAnalyticsJob](./Wait-AzureRmDataLakeAnalyticsJob.md)


