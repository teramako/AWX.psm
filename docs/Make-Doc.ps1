<#
.SYNOPSIS
Create Documents with platyPS
#>
param(
    [string] $Locale = "en-US",
    [switch] $New
)
Import-Module platyPS
$moduleName = "AWX.psm"
$module = Get-Module $moduleName
if ($null -eq $module)
{
    # $ModulePath = Resolve-Path -RelativeBasePath $PSScriptRoot -Path ..\src\bin\Debug\net8.0\AWX.psm.psd1;
    $ModulePath = Resolve-Path -RelativeBasePath $PSScriptRoot -Path ..\out\AWX.psm
    Import-Module $ModulePath -Force
    $module = Get-Module $moduleName
}
$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
$OutputFolder = if ($Locale) {
    "$PSScriptRoot\$Locale\cmdlets"
} else {
    "$PSScriptRoot\cmdlets"
}

## See: https://github.com/Powershell/platyPS/issues/595
function Remove-CommonParameterFromMarkdown {
    <#
        .SYNOPSIS
            Remove a PlatyPS generated parameter block.

        .DESCRIPTION
            Removes parameter block for the provided parameter name from the markdown file provided.

    #>
    param(
        [Parameter(Mandatory)]
        [string[]]
        $Path,

        [Parameter(Mandatory = $false)]
        [string[]]
        $ParameterName = @('ProgressAction')
    )
    $ErrorActionPreference = 'Stop'
    foreach ($p in $Path) {
        $content = (Get-Content -Path $p -Raw).TrimEnd()
        $updateFile = $false
        foreach ($param in $ParameterName) {
            if (-not ($Param.StartsWith('-'))) {
                $param = "-$($param)"
            }
            # Remove the parameter block
            $pattern = "(?m)^### $param\r?\n[\S\s]*?(?=#{2,3}?)"
            $newContent = $content -replace $pattern, ''
            # Remove the parameter from the syntax block
            $pattern = " \[$param\s?.*?]"
            $newContent = $newContent -replace $pattern, ''
            if ($null -ne (Compare-Object -ReferenceObject $content -DifferenceObject $newContent)) {
                Write-Verbose "Added $param to $p"
                # Update file content
                $content = $newContent
                $updateFile = $true
            }
        }
        # Save file if content has changed
        if ($updateFile) {
            $newContent | Out-File -Encoding utf8 -FilePath $p
            Write-Verbose "Updated file: $p"
        }
    }
    return
}

function Add-MissingCommonParameterToMarkdown {
    param(
        [Parameter(Mandatory)]
        [string[]]
        $Path,

        [Parameter(Mandatory = $false)]
        [string[]]
        $ParameterName = @('ProgressAction')
    )
    $ErrorActionPreference = 'Stop'
    foreach ($p in $Path) {
        $content = (Get-Content -Path $p -Raw).TrimEnd()
        $updateFile = $false
        foreach ($NewParameter in $ParameterName) {
            if (-not ($NewParameter.StartsWith('-'))) {
                $NewParameter = "-$($NewParameter)"
            }
            $pattern = '(?m)^This cmdlet supports the common parameters:(.+?)\.'
            $replacement = {
                $Params = $_.Groups[1].Captures[0].ToString() -split ' '
                $CommonParameters = @()
                foreach ($CommonParameter in $Params) {
                    if ($CommonParameter.StartsWith('-')) {
                        if ($CommonParameter.EndsWith(',')) {
                            $CleanParam = $CommonParameter.Substring(0, $CommonParameter.Length -1)
                        } elseif ($p.EndsWith('.')) {
                            $CleanParam = $CommonParameter.Substring(0, $CommonParameter.Length -1)
                        } else{
                            $CleanParam = $CommonParameter
                        }
                        $CommonParameters += $CleanParam
                    }
                }
                if ($NewParameter -notin $CommonParameters) {
                    $CommonParameters += $NewParameter
                }
                $CommonParameters = ($CommonParameters | Sort-Object)
                $CommonParameters[-1] = "and $($CommonParameters[-1])."
                return "This cmdlet supports the common parameters: " + (($CommonParameters) -join ', ')
            }
            $newContent = $content -replace $pattern, $replacement
            if ($null -ne (Compare-Object -ReferenceObject $content -DifferenceObject $newContent)) {
                Write-Verbose "Added $NewParameter to $p"
                $updateFile = $true
                $content = $newContent
            }
        }
        # Save file if content has changed
        if ($updateFile) {
            $newContent | Out-File -Encoding utf8 -FilePath $p
            Write-Verbose "Updated file: $p"
        }
    }
    return
}

function Repair-PlatyPSMarkdown {
    param(
        [Parameter(Mandatory)]
        [string[]]
        $Path,

        [Parameter()]
        [string[]]
        $ParameterName = @('ProgressAction')
    )
    $ErrorActionPreference = 'Stop'
    $Parameters = @{
        Path = $Path
        ParameterName = $ParameterName
    }
    $null = Remove-CommonParameterFromMarkdown @Parameters
    $null = Add-MissingCommonParameterToMarkdown @Parameters
    return
}

if (-not (Test-Path -Path $OutputFolder -PathType Container)) {
    New-Item -Path $OutputFolder -ItemType Directory
    $New = $true
}

if ($New) {
    New-MarkdownAboutHelp -OutputFolder $OutputFolder -AboutName $module.Name
    $NewParams = @{
        Module = $moduleName;
        OutputFolder = $OutputFolder;
        AlphabeticParamsOrder = $false;
        ExcludeDontShow = $true;
        Encoding = $Utf8NoBomEncoding;
    }
    $resultFiles = New-MarkdownHelp @NewParams
} else {
    $UpdateParams = @{
        Path = $OutputFolder;
        RefreshModulePage = $true;
        AlphabeticParamsOrder = $false;
        ExcludeDontShow = $true;
        UpdateInputOutput = $false;
        Encoding = $Utf8NoBomEncoding;
    }
    $resultFiles = Update-MarkdownHelpModule @UpdateParams
}

if ($resultFiles.Count -gt 0) {
    Repair-PlatyPSMarkdown -Path $resultFiles
}


# $externalHelpDirPath = Join-Path $PSScriptRoot ..\out\$Locale
$externalHelpDirPath = Join-Path $module.ModuleBase $Locale
$externalHelpDir = if (-not (Test-Path -Path $externalHelpDirPath -PathType Container)) {
    New-Item -Path $externalHelpDirPath -ItemType Directory
} else {
    Get-Item -Path $externalHelpDirPath
}
$externalHelpParams = @{
    Path = $OutputFolder;
    OutputPath = $externalHelpDir;
    Encoding = $Utf8NoBomEncoding;
    Force = $true;
}
New-ExternalHelp @externalHelpParams
