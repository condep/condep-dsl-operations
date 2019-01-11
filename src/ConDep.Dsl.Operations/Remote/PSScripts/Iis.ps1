﻿$webAdmModule = get-module -name "WebAdministration" -ListAvailable
if($webAdmModule) { $webAdmModule | import-module }

function New-ConDepIisWebSite {
	[CmdletBinding()]
	param (
		[Parameter(Mandatory=$true)] [string] $Name, 
		[Parameter(Mandatory=$true)] [int] $Id,
		[hashtable[]] $Bindings = @(),
		[string] $Path = "", 
		[string] $AppPool = "",
		[string] $LogPath)

    $webSite = GetWebSite $Id
	$defaultPath = "$env:SystemDrive\inetpub\$Name"
	$physicalPath = if($Path) { $Path } else { $defaultPath }

    Write-Debug "Physical path for WebSite is $physicalPath"

	if(!$webSite) {
        write-debug "WebSite $Name does not exist. Creating now..."

		if(!$Bindings) {
			$Bindings = @(@{protocol='http';bindingInformation=':80:';sslFlags=0})
		}
	

		CreateWebSiteDir $physicalPath
		$webSite = new-item -force IIS:\Sites\$Name -Id $Id -Bindings $bindings -PhysicalPath $physicalPath -ApplicationPool $AppPool

		$Bindings | Where-Object {$_.protocol -eq "https"} | AssociateCertificateWithBinding

        UpdateWebSiteLogPath $webSite $LogPath
	}
    else {
	    UpdateWebSiteName $Id $Name

		$webSite = GetWebSite $Id

	    UpdateWebSiteBindings $webSite $Bindings
	    UpdateWebSitePath $webSite $physicalPath
	    UpdateWebSiteAppPool $webSite $AppPool
	    UpdateWebSiteLogPath $webSite $LogPath
    }


	Write-Debug "Starting Web Site $webSite..."
	StartWebSite $webSite
	Write-Debug "Web Site $webSite started."
}

function UpdateWebSiteName($id, $name) {
	$webSite = GetWebSite $id

    if($webSite.name -ne $name) {
        Write-Debug "WebSite name $name differs from current $($webSite.name). Updating now..."
		$webSite | set-itemproperty -Name name -Value $name
    }
    else {
        Write-Debug "WebSite name $name is correct. Doing nothing."
    }
}

function UpdateWebSiteBindings($webSite, $bindings) {
    if(!$bindings) {
        Write-Debug "No bindings provided. Using default."
		$siteBindings = @(@{protocol='http';bindingInformation=':80:';sslFlags=0})
    }
    else {
    	$siteBindings = $bindings | foreach-object { @{protocol=$_.protocol;bindingInformation=$_.bindingInformation;sslFlags=$_.sslFlags}  } 
    }

    $name = $webSite.name

    Write-Debug "Updating WebSite bindings now..."
    Set-ItemProperty -Path "IIS:\Sites\$name" -Name Bindings -Value $siteBindings

    Write-Debug "Associating certificates with https bindings now..."
    $bindings | Where-Object {$_.protocol -eq "https"} | AssociateCertificateWithBinding
}

function UpdateWebSitePath($webSite, $path) {
    $name = $webSite.name

    $defaultPath = "$env:SystemDrive\inetpub\$name"
    $physicalPath = if($path) { $path } else { $defaultPath }

    CreateWebSiteDir $physicalPath

    if($webSite.physicalPath -ne $path) {
        Write-Debug "WebSite physical path differs. Setting path to $path"
		Set-ItemProperty -Path "IIS:\Sites\$name" -Name PhysicalPath -Value $path
    }
    else {
        Write-Debug "WebSite physical path $path is correct. Doing nothing."
    }
}

function UpdateWebSiteAppPool($webSite, $appPool) {
    if($webSite.applicationPool -ne $appPool) {
        Write-Debug "WebSite application pool has changed to $appPool. Updating now..."
		Set-ItemProperty -Path "IIS:\Sites\$name" -Name ApplicationPool -Value $appPool
    }   
    else {
        Write-Debug "WebSite application pool $appPool is correct. Doing nothing."
    }
}

function UpdateWebSiteLogPath($webSite, $logPath) {
    if($logPath){
        if(!(Test-Path -Path $logPath)) {
	        Write-Debug "Creating log directory $logPath"
		    New-Item -Path $LogPath -Type Directory
	    	}

        if($webSite.logFile) {
            if($webSite.logFile.directory -ne $logPath) {
                $name = $webSite.name
                Write-Debug "Setting log path to $logPath"
                Set-ItemProperty IIS:\Sites\$name -name logfile -value @{directory=$logPath}
            }
        }
    }

	Write-Debug "Starting Web Site $webSite..."
}

function AssociateCertificateWithBinding {
	 [CmdletBinding()]
	 param(
		[Parameter(Mandatory=$True,ValueFromPipeline=$True,ValueFromPipelinebyPropertyName=$True)] 
		[hashtable[]] $Bindings
	)
	PROCESS {
		foreach($Binding in $Bindings) {
			$bindingDetails = $Binding.bindingInformation.Split(":")
			$bindingIp = $bindingDetails[0]
			$Port = $bindingDetails[1]
			
			if(!$bindingIp) {
				Write-Host "No binding ip, setting ip to [0.0.0.0]. Port is [$port]."
				$bindingIp = "0.0.0.0"
			}

			Write-Debug "Associating binding $($bindingIp):$($Port)"

			$certsInStore = Get-ChildItem cert:\\LocalMachine\\MY
			$certFinder = new-object System.Security.Cryptography.X509Certificates.X509Certificate2Collection(,$certsInStore)
			$findResult = $certFinder.Find($Binding.FindType, $Binding.FindValue, $false)

			if(!$findResult) { 
				throw "No Certificate found when looking for [$($Binding.FindType)] with value [$($Binding.FindValue)] found."
			}

			if($findResult.Count -gt 1) {
				throw "Certificates with [$($Binding.FindValue)] returned more than 1 result."
			}

			$webSiteCert = $findResult | Select-Object -First 1
			Remove-Item -Path "IIS:\\SslBindings\$bindingIp!$port" -ErrorAction SilentlyContinue
			
			#netsh http add sslcert ipport=$bindingIp:$port certhash=d9744c1e37bd575b431fed64e4e52cae9faced46 appid={5C7D7048-6609-4298-A2ED-6EFB68A66FF3} certstorename=MY clientcertnegotiation=enable
			New-Item -Path "IIS:\\SslBindings\$bindingIp!$port" -Value (Get-Item cert:\LocalMachine\MY\$($webSiteCert.Thumbprint))
			Write-Debug "SSL binding for SSL cert $($webSiteCert.Thumbprint) created."
		}
	}
}

function New-ConDepAppPool {
	param (
		[Parameter(Mandatory=$true)] [string] $AppPool, 
		[hashtable] $AppPoolOptions
	)

	try {
		Remove-WebAppPool $AppPool -ErrorAction SilentlyContinue
	}
	catch { }

	$newAppPool = New-WebAppPool $AppPool -Force

	if($AppPoolOptions) {
		if($AppPoolOptions.Enable32Bit) { $newAppPool.enable32BitAppOnWin64 = $AppPoolOptions.Enable32Bit }
		if($AppPoolOptions.IdentityUsername) { 
			if($AppPoolOptions.IdentityUsername -eq 'NetworkService') {
				$newAppPool.processModel.identityType = 'NetworkService'
			}
			elseif($AppPoolOptions.IdentityUsername -eq 'LocalService') {
				$newAppPool.processModel.identityType = 'LocalService'
			}
			elseif($AppPoolOptions.IdentityUsername -eq 'LocalSystem') {
				$newAppPool.processModel.identityType = 'LocalSystem'
			}
			elseif($AppPoolOptions.IdentityUsername -eq 'ApplicationPoolIdentity') {
				$newAppPool.processModel.identityType = 'ApplicationPoolIdentity'
			}
			else
			{
				$newAppPool.processModel.identityType = 'SpecificUser'
				$newAppPool.processModel.username = $AppPoolOptions.IdentityUsername
				$newAppPool.processModel.password = $AppPoolOptions.IdentityPassword
			}
		}
		
		if($AppPoolOptions.IdleTimeoutInMinutes -ne $null) { $newAppPool.processModel.idleTimeout = [TimeSpan]::FromMinutes($AppPoolOptions.IdleTimeoutInMinutes) }
		if($AppPoolOptions.LoadUserProfile) { $newAppPool.processModel.loadUserProfile =  $AppPoolOptions.LoadUserProfile }
		if($AppPoolOptions.ManagedPipeline) { $newAppPool.managedPipelineMode = $AppPoolOptions.ManagedPipeline }
		if($AppPoolOptions.NetFrameworkVersion -eq "" -or $AppPoolOptions.NetFrameworkVersion) { $newAppPool.managedRuntimeVersion = $AppPoolOptions.NetFrameworkVersion }
		if($AppPoolOptions.RecycleTimeInMinutes -ne $null) { if($AppPoolOptions.RecycleTimeInMinutes -eq 0) { $newAppPool.recycling.periodicrestart.time = [TimeSpan]::Zero } else { $newAppPool.recycling.periodicrestart.time = [TimeSpan]::FromMinutes($AppPoolOptions.RecycleTimeInMinutes)} }
		if($AppPoolOptions.DisableOverlappedRecycle) { $newAppPool.recycling.disallowOverlappingRotation =  $AppPoolOptions.DisableOverlappedRecycle }
		if($AppPoolOptions.AlwaysOn) { $newAppPool.startMode = 1 }

		$newAppPool | set-item
	}
}

function New-ConDepWebApp {
	param (
		[Parameter(Mandatory=$true)] [string] $Name,
		[Parameter(Mandatory=$true)] [string] $WebSite,
		[string] $PhysicalPath,
		[string] $AppPool
	)
	
	$existingWebSite = Get-WebSite | where-object { $_.Name -eq $WebSite }
	
	if(!$existingWebSite) {
		throw "Web Site with name [$WebSite] not found!"
	}
	
	if(!$PhysicalPath) {
		$webSitePath = [System.Environment]::ExpandEnvironmentVariables($existingWebSite.physicalPath)
		$PhysicalPath = "$webSitePath\$Name"
		if(!(Test-Path -path ($PhysicalPath))) { 
			New-Item ($PhysicalPath) -type Directory 
		}
	}
	
	if(!$AppPool) {
		$AppPool = $existingWebSite.applicationPool
	}

	if(!(Test-Path -path $PhysicalPath)) { New-Item $PhysicalPath -type Directory }; 
	
	$webApps = Get-WebApplication -Site $existingWebSite.Name | % { $_.path.TrimStart("/") }
    if($webApps -contains $Name) {
        # Remove existing web application since new-webapplication does not set the provided apppool if the application already exists.
        Remove-WebApplication -Site $WebSite -Name $Name
    }
	New-WebApplication -Name $Name -Site $WebSite -PhysicalPath $PhysicalPath -ApplicationPool $AppPool -force;    
}

function New-ConDepIisHttpBinding {
	param (
		[Parameter(Mandatory=$true)] [string] $WebSiteName, 
		[string] $Port = "", 
		[string] $Ip = "", 
		[string] $HostHeader = ""
	)

	$cmd = GetNewWebBindingCommand $WebSiteName $Port $Ip $HostHeader $false
	Invoke-Expression $cmd

}

function New-ConDepIisHttpsBinding {
	param (
		[Parameter(Mandatory=$true)] [string] $WebSiteName, 
		[Parameter(Mandatory=$true)] [string] $SslCertFindType, 
		[Parameter(Mandatory=$true)] [string] $SslCertFindValue, 
		[string] $Port = "", 
		[string] $Ip = "", 
		[string] $HostHeader = "")

	$cmd = GetNewWebBindingCommand $WebSiteName $Port $Ip $HostHeader $true
	Invoke-Expression $cmd
	
	AssociateCertificateWithBinding $Port $SslCertFindType $SslCertFindValue $Ip
			Write-Debug "Associating binding $($bindingIp):$($Port)"

			Write-Debug "SSL binding for SSL cert $($webSiteCert.Thumbprint) created."
}

function GetNewWebBindingCommand ([string] $WebSiteName, [string] $Port = "", [string] $Ip = "", [string] $HostHeader = "", [bool] $Ssl) {
	if(!$Port -and !$Ip -and !$HostHeader) {
		throw "Either port, ip or host header must be specified to create a new binding in IIS."
	}

	if(!$Port) {
		$Port = "80"
	}

	$newWebBindingCmd = "New-WebBinding -force -Name $webSiteName -Port $port "
	
	if($Ssl) {
		$newWebBindingCmd += "-Protocol https "
	}
	
	if($Ip) {
		$newWebBindingCmd += "-IPAddress $ip "
	}
			
	if($hostHeader) {
		$newWebBindingCmd += "-HostHeader $HostHeader "
	}
		
	Write-Host "Command to execute: $newWebBindingCmd"
	return $newWebBindingCmd
}

function RemoveWebSite($webSiteId) {
	get-website | where-object { $_.ID -eq $webSiteId } | Remove-Website
}

function GetWebSite($webSiteId) {
	$site = get-item -Path "IIS:\Sites\*" | where-object { $_.ID -eq $webSiteId }
	return $site
}

function CreateWebSiteDir($dirPath) {
	if(!$dirPath) {
		return
	}
	
	if(!(Test-Path -path $dirPath)) {
		Write-Debug "Creating directory $dirPath"
		New-Item -Path $dirPath -Type Directory
	}
    else {
        Write-Debug "WebSite path $dirPath allready exist. Doing nothing."
    }
}

function StartWebSite($webSite) {
	if($webSite.State -eq 'Stopped') 
	{
		try {
			Write-Host "Web site is stopped. Trying to start..."
			$webSite.Start()
		}
		catch {
			throw "Unable to start web site."
		}
		Write-Host "Web site started"
	} 
}

function Set-ConDepIisMachineKeys ($validationKey, $decryptionKey, $validation) {

	$netfxFolders = @{ 
		'Framework 1.1 x86' = 'C:\WINDOWS\Microsoft.NET\Framework\v1.1.4322\CONFIG';
		'Framework 2.0 x86' = 'C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\CONFIG'; 
		'Framework 4.0 x86' = 'C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\CONFIG'; 
		'Framework 2.0 x64' = 'C:\WINDOWS\Microsoft.NET\Framework64\v2.0.50727\CONFIG'; 
		'Framework 4.0 x64' = 'C:\WINDOWS\Microsoft.NET\Framework64\v4.0.30319\CONFIG';
	}
	
    $netfxFolders.GetEnumerator() | Sort-Object Name | ForEach-Object { 
    
        $NetFxVersion = $_.Name
        $MachineConfig = $_.Value + '\machine.config'

        if (Test-Path -Path $MachineConfig -PathType Leaf) {
            $xmlMachineConfig = [xml](Get-Content -Path $MachineConfig)
            $SystemWeb = $xmlMachineConfig.get_DocumentElement().'system.web'

            if(!(($SystemWeb.psobject.properties | Select -Expand Name) -contains 'machineKey')) { 
				Write-Host "Missing machineKey element, creating..."
                $machineKey = $xmlMachineConfig.CreateElement('machineKey')
                [void]$SystemWeb.AppendChild($machineKey)
            }

			$machineKey = $SystemWeb.SelectSingleNode('machineKey')

			if((HasMachineKeyChanged $machineKey $validationKey $decryptionKey $validation)) {
	            Copy-Item -Path $MachineConfig -Destination ($MachineConfig -replace '\.config', ('_{0:yyyyMMddHHmm}.config' -f (Get-Date))) -Force
	
				$machineKey.SetAttribute('validationKey', $validationkey)
				$machineKey.SetAttribute('decryptionKey', $decryptionKey)
				$machineKey.SetAttribute('validation', $validation)
				$xmlMachineConfig.Save($MachineConfig)
				Write-Host "Machine key for $NetFxVersion configured."
			}
			else {
				write-Verbose "Machine key for $NetFxVersion is OK. Skipping"
			}
        } else {
            Write-Verbose "$NetFxVersion is not installed, no machine config to edit."
        }
    }
}

function HasMachineKeyChanged($machineKey, $validationKey, $decryptionKey, $validation) {
	if($machineKey.GetAttribute('validationKey') -eq $null) {
		return $true
	}

	if($machineKey.GetAttribute('decryptionKey') -eq $null) {
		return $true
	}

	if($machineKey.GetAttribute('validation') -eq $null) {
		return $true
	}

	if($machineKey.GetAttribute('validationKey') -ne $validationKey) {
		return $true
	}

	if($machineKey.GetAttribute('decryptionKey') -ne $decryptionKey) {
		return $true
	}

	if($machineKey.GetAttribute('validation') -ne $validation) {
		return $true
	}

	return $false
}
