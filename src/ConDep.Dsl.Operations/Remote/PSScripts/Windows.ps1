Import-Module ServerManager;

function Set-ConDepWindowsFeatures {
	[CmdletBinding()]
	param (
		[string[]] $featuresToAdd = "", 
		[string[]] $featuresToRemove = "")

	if($featuresToAdd) {
		Write-Host "Trying to add Windows Features: $featuresToAdd"
		addMissingWindowsFeatures $featuresToAdd
	}
	
	if($featuresToRemove) {
		Write-Host "Trying to remove Windows Features: $featuresToRemove"
		removeExistingWindowsFeatures $featuresToRemove
	}
}

function Add-ConDepUserToLocalGroup {
	[CmdletBinding()]
	param (
		[Parameter(Mandatory=$true)] [string] $group, 
		[Parameter(Mandatory=$true)] [string] $user)
	try {
		$de = [ADSI]"WinNT://./$group,group"
		$members = $de.psbase.Invoke("Members") | Foreach-Object  {$_.GetType().InvokeMember("Name", 'GetProperty', $null, $_, $null) }

		if($members -contains $user) {
			Write-Verbose "Member $user allready exists in group $group"
		}
		else {
			$de.psbase.Invoke("Add",([ADSI]"WinNT://$user").path)
			Write-Host "Member $user added to $group group"
		}
	}
	catch {
		write-Error "Failed to add user $user to group $group. See exception for more details."
		throw
	}
}

function addMissingWindowsFeatures($featureList) {
	Write-Host "Checking for missing Windows features..."
	$missingFeatures = getMissingWindowsFeatures $featureList
	
	if($missingFeatures) {
		Write-Host "Adding missing Windows features: $missingFeatures"
		$installedFeatures = Add-WindowsFeature $missingFeatures 
		$installedFeatures.FeatureResult
	}
	else {
		Write-Host "All requested Windows features where already installed. Doing nothing."
	}
}

function removeExistingWindowsFeatures($featureList) {
	Write-Host "Checking for existing Windows features..."
	$existingFeatures = getExistingWindowsFeatures $featureList
	
	if($existingFeatures) {
		Write-Host "Removing existing Windows features: $existingFeatures"
		$removedFeatures = Remove-WindowsFeature $existingFeatures 
		$removedFeatures.FeatureResult
	}
	else {
		Write-Host "None of the requested Windows features to remove where installed. Doing nothing."
	}
}

function getMissingWindowsFeatures($featureList) {
	$features = @()
	foreach($feature in $featureList) {
		$result = Get-WindowsFeature $feature
		if(!$result) { throw "Feature '$feature' is not an available feature to install on the server. It's not on the Windows feature list." }
		if($result.Installed -eq $false) { $features += $result }		
	}
    #$features = Get-WindowsFeature $featureList
	#return ($features | where { $_.Installed -eq $false })
	return $features
}

function getExistingWindowsFeatures($featureList) {
    $features = @()
	foreach($feature in $featureList) {
		$result = Get-WindowsFeature $feature
		if(!$result) { throw "Feature '$feature' is not an available feature to install on the server. It's not on the Windows feature list." }
		if($result.Installed -eq $true) { $features += $result }		
	}
	#$features = Get-WindowsFeature $featureList
	#return ($features | where { $_.Installed -eq $true })
	return $features
}
