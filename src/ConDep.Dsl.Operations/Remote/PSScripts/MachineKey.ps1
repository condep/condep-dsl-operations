function Set-MachineKeys ($validationKey, $decryptionKey, $validation) {

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

            Copy-Item -Path $MachineConfig -Destination ($MachineConfig -replace '\.config', ('_{0:yyyyMMddHHmm}.config' -f (Get-Date))) -Force

            $xmlMachineConfig = [xml](Get-Content -Path $MachineConfig)
            $SystemWeb = $xmlMachineConfig.get_DocumentElement().'system.web'

            if(!(($SystemWeb.psobject.properties | Select -Expand Name) -contains 'machineKey')) { 
				Write-Verbose "Missing machineKey element, creating..."
                $machineKey = $xmlMachineConfig.CreateElement('machineKey')
                [void]$SystemWeb.AppendChild($machineKey)
            }

            $SystemWeb.SelectSingleNode('machineKey').SetAttribute('validationKey', $validationkey)
            $SystemWeb.SelectSingleNode('machineKey').SetAttribute('decryptionKey', $decryptionKey)
            $SystemWeb.SelectSingleNode('machineKey').SetAttribute('validation', $validation)
            $xmlMachineConfig.Save($MachineConfig)

			Write-Host "Machine key for $NetFxVersion configured."

        } else {
            Write-Verbose "$NetFxVersion is not installed, no machine config to edit."
        }
    }
}